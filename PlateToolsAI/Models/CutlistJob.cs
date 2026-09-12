using System;
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

        public Dictionary<string, string> MachineAssignments { get; set; }
            = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public Dictionary<string, List<string>> AmbiguousMachineAssignments { get; set; }
            = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        public List<string> ImportWarnings { get; }
            = new List<string>();
    }
}