using System.Collections.Generic;

namespace PlateToolsAI.Models
{
    public class CutlistJob
    {
        public string JobNumber { get; set; } = string.Empty;

        public List<string> Lots { get; set; }
            = new List<string>();

        public List<string> Sequences { get; set; }
            = new List<string>();

        public List<CutlistPart> Parts { get; set; }
            = new List<CutlistPart>();
    }
}