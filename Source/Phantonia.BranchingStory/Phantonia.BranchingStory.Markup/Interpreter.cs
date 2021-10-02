using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Xml;

namespace Phantonia.BranchingStory.Markup
{
    public sealed class Interpreter
    {
        public Interpreter(Stream xmlStream)
        {
            document = new XmlDocument();
            this.xmlStream = xmlStream;
        }

        private readonly XmlDocument document;
        private readonly List<Error> errors = new();
        private readonly Stream xmlStream;

        public InterpretationResult Interpret()
        {
            document.Load(xmlStream);

            XmlElement? storyElement = document.DocumentElement;

            if (storyElement is null)
            {
                return new InterpretationResult() { Errors = errors.ToImmutableArray() };
            }

            if (storyElement.Name != Tags.StoryTag)
            {
                errors.Add(new Error(ErrorCode.RootElementNotStory));

                // we can recover from this by just treating the non-story root element as the story element
            }

            BodyInterpreter bodyInterpreter = new(storyElement);
            BodyInterpretationResult? result = bodyInterpreter.Interpret();

            errors.AddRange(result.Errors);

            Story? story = result.EntryPoint is null ? null : new(result.EntryPoint);

            return new InterpretationResult { Story = story, Errors = errors.ToImmutableArray() };
        }
    }
}
