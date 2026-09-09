using PlateToolsAI.Models;
using PlateToolsAI.Readers;

namespace PlateToolsAI.AIEmployee
{
    public class AIEmployeeController
    {
        private readonly ICutlistReader _reader;

        public AIEmployeeController(ICutlistReader reader)
        {
            _reader = reader;
        }

        public CutlistJob ProcessJob(string filePath)
        {
            return _reader.Read(filePath);
        }
    }
}