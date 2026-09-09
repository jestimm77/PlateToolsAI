using PlateToolsAI.Models;

namespace PlateToolsAI.Readers
{
    public class PdfCutlistReader : ICutlistReader
    {
        public CutlistJob Read(string filePath)
        {
            var job = new CutlistJob();

            job.JobNumber = "34778";

            job.Lots.Add("1");

            job.Sequences.Add("1");

            job.Parts.Add(new CutlistPart
            {
                PieceMark = "SP1",
                Quantity = 36,
                Material = "A36",
                Thickness = "3/8",
                Sequence = "1",
                Lot = "1",
                Machine = "FPB"
            });

            job.Parts.Add(new CutlistPart
            {
                PieceMark = "tp1",
                Quantity = 20,
                Material = "A36",
                Thickness = "3/4",
                Sequence = "1",
                Lot = "1",
                Machine = "2500A"
            });

            job.Parts.Add(new CutlistPart
            {
                PieceMark = "bp1",
                Quantity = 20,
                Material = "A36",
                Thickness = "1",
                Sequence = "1",
                Lot = "1",
                Machine = "2500A"
            });

            return job;
        }
    }
}