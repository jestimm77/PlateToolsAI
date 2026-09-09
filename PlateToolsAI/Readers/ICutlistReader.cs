using PlateToolsAI.Models;

namespace PlateToolsAI.Readers
{
    public interface ICutlistReader
    {
        CutlistJob Read(string filePath);
    }
}