using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using PlateToolsAI.Models;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig;

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

        private static readonly Regex PieceMarkRegex =
            new Regex(@"\b[A-Za-z]{0,3}P\d+[A-Za-z0-9-]*\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex WhitespaceRegex =
            new Regex(@"\s+", RegexOptions.Compiled);

        public CutlistJob Read(string filePath)
        {
            const string defaultLot = "1";
            const string defaultSequence = "1";

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("A cut list PDF path is required.", nameof(filePath));
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The cut list PDF could not be found.", filePath);
            }

            var job = new CutlistJob
            {
                JobNumber = Path.GetFileNameWithoutExtension(filePath)
            };

            job.Lots.Add(defaultLot);
            job.Sequences.Add(defaultSequence);

            var textLines = ExtractTextLines(filePath);

            foreach (var assignment in ExtractAssignments(textLines))
            {
                job.MachineAssignments[assignment.Key] = assignment.Value;
            }

            foreach (var pieceMark in job.MachineAssignments.Keys)
            {
                job.Parts.Add(new CutlistPart
                {
                    PieceMark = pieceMark,
                    Lot = defaultLot,
                    Sequence = defaultSequence
                });
            }

            return job;
        }

        private static IEnumerable<string> ExtractTextLines(string filePath)
        {
            var lines = new List<string>();

            using (var document = PdfDocument.Open(filePath))
            {
                foreach (var page in document.GetPages())
                {
                    var pageLines = GroupWordsIntoLines(page.GetWords())
                        .Select(NormalizeWhitespace)
                        .Where(line => !string.IsNullOrWhiteSpace(line));

                    lines.AddRange(pageLines);
                }
            }

            return lines;
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

        private static Dictionary<string, string> ExtractAssignments(IEnumerable<string> textLines)
        {
            var assignments = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var lines = textLines.ToList();

            for (int index = 0; index < lines.Count; index++)
            {
                if (TryAddAssignments(lines[index], assignments))
                {
                    continue;
                }

                if (index + 1 < lines.Count)
                {
                    TryAddAssignments($"{lines[index]} {lines[index + 1]}", assignments);
                }
            }

            return assignments;
        }

        private static bool TryAddAssignments(string text, IDictionary<string, string> assignments)
        {
            var machine = FindMachine(text);
            var pieceMarks = PieceMarkRegex
                .Matches(text)
                .Cast<Match>()
                .Select(match => match.Value.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (string.IsNullOrEmpty(machine) || pieceMarks.Count == 0)
            {
                return false;
            }

            foreach (var pieceMark in pieceMarks)
            {
                assignments[pieceMark] = machine;
            }

            return true;
        }

        private static string FindMachine(string text)
        {
            var normalizedText = NormalizeWhitespace(text);

            return MachineOptions.FirstOrDefault(machine =>
                Regex.IsMatch(
                    normalizedText,
                    $@"\b{Regex.Escape(machine).Replace("\\ ", @"\s+")}\b",
                    RegexOptions.IgnoreCase
                )) ?? string.Empty;
        }

        private static string NormalizeWhitespace(string text)
        {
            return WhitespaceRegex.Replace(text ?? string.Empty, " ").Trim();
        }
    }
}