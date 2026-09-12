using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using PlateToolsAI.Models;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace PlateToolsAI.Readers
{
    public class PdfCutlistReader : ICutlistReader
    {
        private static readonly string[] MachineOptions =
        {
            "Shear",
            "FPB",
            "2500A",
            "2500B",
            "Burn Table",
            "ESAB-1",
            "ESAB-2",
            "ESAB-3",
            "T-Order",
            "Stock"
        };

        private static readonly string[] ThicknessPatterns =
        {
            "2 1/2",
            "1 1/2",
            "1 1/4",
            "1 1/8",
            "3/4",
            "5/8",
            "1/2",
            "3/8",
            "5/16",
            "1/4",
            "3/16",
            "1/8",
            "1"
        };

        private static readonly Regex PieceMarkRegex =
            new Regex(@"\b[A-Za-z]{0,3}\s*P\s*\d+[A-Za-z0-9-]*\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex QuantityPrefixRegex =
            new Regex(@"^[^\dIlL|]*(?<qty>\d+)\s*[_-]?\s*(?<rest>.+)$", RegexOptions.Compiled);

        private static readonly Regex OcrOnePrefixRegex =
            new Regex(@"^[^A-Za-z0-9]*(?<qty>[IlL|])\s*[_-]?\s*(?<rest>.+)$", RegexOptions.Compiled);

        private static readonly Regex SequenceRegex =
            new Regex(@"\bSequence\s*:\s*(?<value>[A-Za-z0-9.\-]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex LotRegex =
            new Regex(@"\bLot\s*:\s*(?<value>[A-Za-z0-9.\-]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex WhitespaceRegex =
            new Regex(@"\s+", RegexOptions.Compiled);

        public CutlistJob Read(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("A cut list path is required.", nameof(filePath));
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The cut list file could not be found.", filePath);
            }

            var job = new CutlistJob
            {
                JobNumber = Path.GetFileNameWithoutExtension(filePath)
            };

            var lines = ReadInputLines(filePath, job.ImportWarnings).ToList();
            var context = new ParseContext();

            foreach (var line in lines)
            {
                if (TryParseSequence(line, out var sequence))
                {
                    context.Sequence = sequence;
                }

                if (TryParseLot(line, out var lot))
                {
                    context.Lot = lot;
                }

                if (TryParseSectionHeader(line, out var material, out var thickness))
                {
                    context.Material = material;
                    context.Thickness = thickness;
                    continue;
                }

                if (IsIgnoredLine(line))
                {
                    continue;
                }

                if (TryParsePart(line, context, out var part))
                {
                    job.Parts.Add(part);
                }
            }

            PopulateAssignments(job, lines);
            PopulateLotsAndSequences(job, context);

            if (job.MachineAssignments.Count == 0 && job.AmbiguousMachineAssignments.Count == 0)
            {
                AddWarning(job, "No supported machine assignments were recognized. Review and route machines manually.");
            }

            if (job.Parts.Count == 0)
            {
                AddWarning(job, "No part rows were imported. Review the file format or OCR setup and try again.");
            }

            if (job.AmbiguousMachineAssignments.Count > 0)
            {
                AddWarning(
                    job,
                    $"{job.AmbiguousMachineAssignments.Count} piece mark assignment(s) were ambiguous and left for manual review."
                );
            }

            return job;
        }

        private static IEnumerable<string> ReadInputLines(string filePath, ICollection<string> warnings)
        {
            var extension = Path.GetExtension(filePath);

            if (string.Equals(extension, ".txt", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(extension, ".csv", StringComparison.OrdinalIgnoreCase))
            {
                return File.ReadLines(filePath)
                    .Select(NormalizeWhitespace)
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .ToList();
            }

            if (!string.Equals(extension, ".pdf", StringComparison.OrdinalIgnoreCase))
            {
                throw new NotSupportedException("Only PDF, TXT, and CSV cut list files are supported.");
            }

            var textLines = ExtractTextLinesFromPdf(filePath).ToList();
            if (HasEnoughPartCandidates(textLines))
            {
                return textLines;
            }

            var ocrLines = ExtractTextLinesWithOcr(filePath, warnings).ToList();
            if (HasEnoughPartCandidates(ocrLines))
            {
                AddWarning(warnings, "OCR fallback was used because the PDF text layer did not contain readable part rows.");
                return ocrLines;
            }

            return textLines;
        }

        private static IEnumerable<string> ExtractTextLinesFromPdf(string filePath)
        {
            var lines = new List<string>();

            using (var document = PdfDocument.Open(filePath))
            {
                foreach (var page in document.GetPages())
                {
                    lines.AddRange(
                        GroupWordsIntoLines(page.GetWords())
                            .Select(NormalizeWhitespace)
                            .Where(line => !string.IsNullOrWhiteSpace(line))
                    );
                }
            }

            return lines;
        }

        private static IEnumerable<string> ExtractTextLinesWithOcr(string filePath, ICollection<string> warnings)
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), "PlateToolsAI-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);

            try
            {
                var imagePrefix = Path.Combine(tempDirectory, "page");
                RunProcess("pdftoppm", $"-r 300 -png {Quote(filePath)} {Quote(imagePrefix)}");

                var imageFiles = Directory.GetFiles(tempDirectory, "page-*.png")
                    .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                var lines = new List<string>();
                foreach (var imageFile in imageFiles)
                {
                    var output = RunProcess("tesseract", $"{Quote(imageFile)} stdout --psm 6");
                    lines.AddRange(
                        output.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                            .Select(NormalizeWhitespace)
                            .Where(line => !string.IsNullOrWhiteSpace(line))
                    );
                }

                return lines;
            }
            catch (Exception ex)
            {
                AddWarning(
                    warnings,
                    $"OCR fallback is unavailable: {ex.Message}. Install pdftoppm and tesseract for scanned PDF cut lists."
                );

                return Array.Empty<string>();
            }
            finally
            {
                try
                {
                    if (Directory.Exists(tempDirectory))
                    {
                        Directory.Delete(tempDirectory, true);
                    }
                }
                catch
                {
                    // Best-effort cleanup only.
                }
            }
        }

        private static string RunProcess(string fileName, string arguments)
        {
            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                process.Start();

                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    throw new InvalidOperationException(
                        $"{fileName} exited with code {process.ExitCode}: {NormalizeWhitespace(error)}"
                    );
                }

                return output;
            }
        }

        private static IEnumerable<string> GroupWordsIntoLines(IEnumerable<Word> words)
        {
            const double lineTolerance = 2d;

            var orderedWords = words
                .OrderByDescending(word => word.BoundingBox.Bottom)
                .ThenBy(word => word.BoundingBox.Left)
                .ToList();

            var currentLine = new List<Word>();
            double? currentBottom = null;

            foreach (var word in orderedWords)
            {
                if (!currentBottom.HasValue || Math.Abs(word.BoundingBox.Bottom - currentBottom.Value) <= lineTolerance)
                {
                    currentLine.Add(word);
                    currentBottom ??= word.BoundingBox.Bottom;
                    continue;
                }

                yield return string.Join(" ", currentLine
                    .OrderBy(lineWord => lineWord.BoundingBox.Left)
                    .Select(lineWord => lineWord.Text));

                currentLine.Clear();
                currentLine.Add(word);
                currentBottom = word.BoundingBox.Bottom;
            }

            if (currentLine.Count > 0)
            {
                yield return string.Join(" ", currentLine
                    .OrderBy(lineWord => lineWord.BoundingBox.Left)
                    .Select(lineWord => lineWord.Text));
            }
        }

        private static void PopulateAssignments(CutlistJob job, IEnumerable<string> lines)
        {
            var detectedAssignments = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
            var normalizedLines = lines.ToList();

            for (var index = 0; index < normalizedLines.Count; index++)
            {
                CollectAssignments(detectedAssignments, normalizedLines[index]);
            }

            foreach (var entry in detectedAssignments)
            {
                var machines = entry.Value.OrderBy(machine => machine, StringComparer.OrdinalIgnoreCase).ToList();

                if (machines.Count == 1)
                {
                    job.MachineAssignments[entry.Key] = machines[0];
                    continue;
                }

                job.AmbiguousMachineAssignments[entry.Key] = machines;
            }

            foreach (var part in job.Parts)
            {
                if (job.MachineAssignments.TryGetValue(part.PieceMark, out var machine))
                {
                    part.SuggestedMachine = machine;
                    part.ReviewNote = string.Empty;
                }
                else if (job.AmbiguousMachineAssignments.TryGetValue(part.PieceMark, out var machines))
                {
                    part.ReviewNote = "Ambiguous: " + string.Join(" / ", machines);
                }
                else
                {
                    part.ReviewNote = "Unassigned";
                }
            }
        }

        private static void CollectAssignments(IDictionary<string, HashSet<string>> assignments, string line)
        {
            var machines = FindMachines(line).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (machines.Count != 1)
            {
                return;
            }

            var pieceMarks = PieceMarkRegex.Matches(line)
                .Cast<Match>()
                .Select(match => NormalizePieceMark(match.Value))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (pieceMarks.Count == 0)
            {
                return;
            }

            foreach (var pieceMark in pieceMarks)
            {
                if (!assignments.TryGetValue(pieceMark, out var values))
                {
                    values = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    assignments[pieceMark] = values;
                }

                values.Add(machines[0]);
            }
        }

        private static IEnumerable<string> FindMachines(string line)
        {
            var normalized = Regex.Replace((line ?? string.Empty).ToUpperInvariant(), @"[^A-Z0-9]+", " ").Trim();

            foreach (var machine in MachineOptions)
            {
                var pattern = Regex.Replace(machine.ToUpperInvariant(), @"[^A-Z0-9]+", " ").Trim();
                if (Regex.IsMatch(normalized, $@"\b{Regex.Escape(pattern).Replace("\\ ", @"\s+")}\b"))
                {
                    yield return machine;
                }
            }
        }

        private static bool TryParsePart(string line, ParseContext context, out CutlistPart part)
        {
            part = null;
            if (!TryGetQuantityAndRemainder(line, out var quantity, out var remainder))
            {
                return false;
            }

            if (!TryExtractPieceMark(remainder, out var pieceMark))
            {
                return false;
            }

            part = new CutlistPart
            {
                PieceMark = pieceMark,
                Quantity = quantity,
                Material = context.Material,
                Thickness = context.Thickness,
                Sequence = context.Sequence,
                Lot = context.Lot,
                ReviewNote = "Unassigned"
            };

            return true;
        }

        private static bool TryGetQuantityAndRemainder(string line, out int quantity, out string remainder)
        {
            quantity = 0;
            remainder = string.Empty;

            var normalized = NormalizeWhitespace(line);
            if (string.IsNullOrWhiteSpace(normalized))
            {
                return false;
            }

            var digitMatch = QuantityPrefixRegex.Match(normalized);
            if (digitMatch.Success &&
                int.TryParse(digitMatch.Groups["qty"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out quantity))
            {
                remainder = digitMatch.Groups["rest"].Value;
                return quantity > 0;
            }

            var ocrOneMatch = OcrOnePrefixRegex.Match(normalized);
            if (ocrOneMatch.Success)
            {
                quantity = 1;
                remainder = ocrOneMatch.Groups["rest"].Value;
                return true;
            }

            return false;
        }

        private static bool TryExtractPieceMark(string remainder, out string pieceMark)
        {
            pieceMark = string.Empty;

            var tokens = NormalizeWhitespace(remainder)
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            if (tokens.Count == 0)
            {
                return false;
            }

            var candidate = CleanPieceToken(tokens[0]);
            if (tokens.Count > 1 &&
                Regex.IsMatch(candidate, @"^[A-Za-z]{0,3}P\d+$", RegexOptions.IgnoreCase) &&
                Regex.IsMatch(tokens[1], @"^[A-Za-z][A-Za-z0-9-]{0,3}$", RegexOptions.IgnoreCase))
            {
                candidate += CleanPieceToken(tokens[1]);
            }

            if (!Regex.IsMatch(candidate, @"^[A-Za-z]{0,3}P\d+[A-Za-z0-9-]*$", RegexOptions.IgnoreCase))
            {
                return false;
            }

            pieceMark = NormalizePieceMark(candidate);
            return true;
        }

        private static string CleanPieceToken(string token)
        {
            return (token ?? string.Empty)
                .Trim()
                .Trim('|', ':', ';', ',', '.', '_', '-', '(', ')', '[', ']', '{', '}')
                .Replace("_", string.Empty)
                .Replace(" ", string.Empty)
                .Replace("'", string.Empty)
                .Replace("`", string.Empty);
        }

        private static string NormalizePieceMark(string pieceMark)
        {
            return CleanPieceToken(pieceMark).ToUpperInvariant();
        }

        private static bool TryParseSectionHeader(string line, out string material, out string thickness)
        {
            material = string.Empty;
            thickness = string.Empty;

            var normalized = NormalizeSectionText(line);
            if (!normalized.StartsWith("PL ", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var body = normalized.Substring(3).Trim();
            var upperBody = body.ToUpperInvariant();
            foreach (var pattern in ThicknessPatterns)
            {
                var search = pattern.ToUpperInvariant();
                var index = upperBody.LastIndexOf(search, StringComparison.Ordinal);
                if (index < 0)
                {
                    continue;
                }

                material = NormalizeMaterial(body.Substring(0, index));
                thickness = pattern;
                return !string.IsNullOrWhiteSpace(material);
            }

            return false;
        }

        private static string NormalizeSectionText(string line)
        {
            var normalized = NormalizeWhitespace(line)
                .Replace('_', ' ')
                .Replace("AS72", "A572")
                .Replace("AST2", "A572")
                .Replace("GRS0", "GR50")
                .Replace("GRSO", "GR50")
                .Replace("GR 5O", "GR 50")
                .Replace("W/4", "1/4")
                .Replace("S/16", "5/16")
                .Replace("11/8", "1 1/8")
                .Replace("11/4", "1 1/4")
                .Replace("11/2", "1 1/2")
                .Replace("21/2", "2 1/2")
                .Replace("A361", "A36 1")
                .Replace("A36W", "A36 1")
                .Replace("A36-", "A36 ");

            return NormalizeWhitespace(normalized);
        }

        private static string NormalizeMaterial(string materialText)
        {
            var material = NormalizeWhitespace(materialText)
                .Trim('-', '_');

            material = Regex.Replace(material, @"\bA36[0O]?\b.*$", "A36", RegexOptions.IgnoreCase);
            material = Regex.Replace(material, @"\bA572\b[\s-]*GR[\s-]*50\b.*$", "A572-GR 50", RegexOptions.IgnoreCase);
            material = Regex.Replace(material, @"\bA572\b[\s-]*GR[\s-]*65\b.*$", "A572-GR 65", RegexOptions.IgnoreCase);

            return material;
        }

        private static bool TryParseSequence(string line, out string sequence)
        {
            var match = SequenceRegex.Match(line ?? string.Empty);
            sequence = match.Success ? match.Groups["value"].Value : string.Empty;
            return match.Success;
        }

        private static bool TryParseLot(string line, out string lot)
        {
            var match = LotRegex.Match(line ?? string.Empty);
            lot = match.Success ? match.Groups["value"].Value : string.Empty;
            return match.Success;
        }

        private static void PopulateLotsAndSequences(CutlistJob job, ParseContext context)
        {
            var lots = job.Parts
                .Select(part => string.IsNullOrWhiteSpace(part.Lot) ? context.Lot : part.Lot)
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var sequences = job.Parts
                .Select(part => string.IsNullOrWhiteSpace(part.Sequence) ? context.Sequence : part.Sequence)
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            job.Lots.Clear();
            if (lots.Count > 0)
            {
                job.Lots.AddRange(lots);
            }

            job.Sequences.Clear();
            if (sequences.Count > 0)
            {
                job.Sequences.AddRange(sequences);
            }
        }

        private static bool HasEnoughPartCandidates(IEnumerable<string> lines)
        {
            var count = 0;

            foreach (var line in lines)
            {
                if (TryGetQuantityAndRemainder(line, out _, out var remainder) &&
                    TryExtractPieceMark(remainder, out _))
                {
                    count++;
                }
            }

            return count >= 3;
        }

        private static bool IsIgnoredLine(string line)
        {
            var normalized = NormalizeWhitespace(line).ToUpperInvariant();
            return normalized.StartsWith("STEELFAB", StringComparison.Ordinal) ||
                   normalized.StartsWith("JOB DESCRIPTION", StringComparison.Ordinal) ||
                   normalized.StartsWith("JOB DATE", StringComparison.Ordinal) ||
                   normalized.StartsWith("SOLD TO", StringComparison.Ordinal) ||
                   normalized.StartsWith("SHIP TO", StringComparison.Ordinal) ||
                   normalized.StartsWith("FILTER", StringComparison.Ordinal) ||
                   normalized.StartsWith("SHAPE", StringComparison.Ordinal) ||
                   normalized.StartsWith("QTY PIECE", StringComparison.Ordinal) ||
                   normalized.StartsWith("PIECE MARK", StringComparison.Ordinal) ||
                   normalized.StartsWith("PAGE ", StringComparison.Ordinal) ||
                   normalized.Contains("CUT LIST") ||
                   normalized.StartsWith("TOTAL", StringComparison.Ordinal) ||
                   normalized.EndsWith(" TOTAL", StringComparison.Ordinal) ||
                   normalized == "PL" ||
                   normalized.StartsWith("=====", StringComparison.Ordinal);
        }

        private static string NormalizeWhitespace(string text)
        {
            return WhitespaceRegex.Replace(text ?? string.Empty, " ").Trim();
        }

        private static string Quote(string value)
        {
            return "\"" + value + "\"";
        }

        private static void AddWarning(CutlistJob job, string warning)
        {
            AddWarning(job.ImportWarnings, warning);
        }

        private static void AddWarning(ICollection<string> warnings, string warning)
        {
            if (!warnings.Contains(warning))
            {
                warnings.Add(warning);
            }
        }

        private sealed class ParseContext
        {
            public string Material { get; set; } = string.Empty;

            public string Thickness { get; set; } = string.Empty;

            public string Sequence { get; set; } = string.Empty;

            public string Lot { get; set; } = string.Empty;
        }
    }
}