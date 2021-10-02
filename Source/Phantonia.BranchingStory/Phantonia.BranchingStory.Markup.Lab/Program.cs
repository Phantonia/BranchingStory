using Phantonia.BranchingStory.Markup;
using System.IO;

const string Markup = @"
<story>
    <text>Hello world!</text>
</story>";

using MemoryStream stream = new();
StreamWriter streamWriter = new(stream);
streamWriter.Write(Markup);
streamWriter.Flush();
stream.Position = 0;

Interpreter intp = new(stream);
InterpretationResult result = intp.Interpret();

{ }