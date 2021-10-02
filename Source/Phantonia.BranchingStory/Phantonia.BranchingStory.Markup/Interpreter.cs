using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
            try
            {
                document.Load(xmlStream);
            }
            catch (XmlException ex)
            {
                // fatal error: cannot recover
                errors.Add(new Error(ErrorCode.XmlSyntaxError, MoreInformation: ex.Message));

                return new InterpretationResult { Errors = errors.ToImmutableArray() };
            }

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

            BodyInterpretationResult? result = InterpretBody(storyElement);

            errors.AddRange(result.Errors);

            Story? story = result.EntryPoint is null ? null : new(result.EntryPoint);

            return new InterpretationResult { Story = story, Errors = errors.ToImmutableArray() };
        }

        private BodyInterpretationResult InterpretBody(XmlElement bodyRootElement)
        {
            StoryNode? nextNode = null;

            foreach (XmlElement element in bodyRootElement.ChildNodes.Cast<XmlElement>().Reverse())
            {
                StoryNode? currentNode = MakeNode(element, nextNode);
                nextNode = currentNode;
            }

            Debug.Assert(nextNode is not null);

            // nextNode is the first node now
            return new(EntryPoint: nextNode, ExitPoints: ImmutableList<StoryNode>.Empty);
        }

        private static ImmutableDictionary<string, string> GetAttributes(XmlElement element)
        {
            return element.Attributes.Cast<XmlAttribute>().ToImmutableDictionary(a => a.Name, a => a.Value);
        }

        private StoryNode MakeNode(XmlElement element, StoryNode? nextNode)
        {
            return element.Name switch
            {
                Tags.TextTag => MakeTextNode(element, nextNode),
                Tags.ActionTag => MakeActionNode(element, nextNode),

                _ => throw new NotImplementedException(),
            };
        }

        private ActionNode MakeActionNode(XmlElement element, StoryNode? nextNode)
        {
            Debug.Assert(element.Name == Tags.ActionTag);

            var attributes = GetAttributes(element);

            if (element.ChildNodes.Count == 1 && element.ChildNodes[0] is XmlText xmlText)
            {
                return new ActionNode(xmlText.Value ?? "", nextNode, attributes);
            }

            if (!element.HasChildNodes)
            {
                return new ActionNode("", nextNode, attributes);
            }

            errors.Add(new Error(ErrorCode.TextElementWithChildren));

            // we can recover by just ignoring this but it might result in nonsense

            return new ActionNode(element.InnerText, nextNode, attributes);
        }

        private TextNode MakeTextNode(XmlElement element, StoryNode? nextNode)
        {
            Debug.Assert(element.Name == Tags.TextTag);

            var attributes = GetAttributes(element);

            if (element.ChildNodes.Count == 1 && element.ChildNodes[0] is XmlText xmlText)
            {
                return new TextNode(xmlText.Value ?? "", nextNode, attributes);
            }

            if (!element.HasChildNodes)
            {
                return new TextNode("", nextNode, attributes);
            }

            errors.Add(new Error(ErrorCode.TextElementWithChildren));

            // we can recover by just ignoring this but it might result in nonsense
            
            return new TextNode(element.InnerText, nextNode, attributes);
        }
    }
}
