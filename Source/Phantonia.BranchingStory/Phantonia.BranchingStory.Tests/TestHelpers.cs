using System.IO;

namespace Phantonia.BranchingStory.Tests
{
    internal static class TestHelpers
    {
        public static Stream StreamFromString(string str)
        {
            MemoryStream stream = new();
            StreamWriter writer = new(stream);
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
