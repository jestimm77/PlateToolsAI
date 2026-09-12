namespace PlateToolsAI.Models
{
    public class CutlistPart
    {
        public string PieceMark { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public string Material { get; set; } = string.Empty;

        public string Thickness { get; set; } = string.Empty;

        public string Sequence { get; set; } = string.Empty;

        public string Lot { get; set; } = string.Empty;

        public string Machine { get; set; } = string.Empty;

        public string SuggestedMachine { get; set; } = string.Empty;

        public string ReviewNote { get; set; } = string.Empty;

        public bool HasManualMachineOverride { get; set; }
    }
}