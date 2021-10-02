using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Xml;

namespace Phantonia.BranchingStory.Markup
{
    internal sealed class BodyInterpreter
    {
        public BodyInterpreter(XmlElement bodyRootElement)
        {
            this.bodyRootElement = bodyRootElement;
        }

        private readonly XmlElement bodyRootElement;
        private List<Error>? errors;

        public BodyInterpretationResult Interpret()
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

        private StoryNode MakeNode(XmlElement element, StoryNode? nextNode)
        {
            return element.Name switch
            {
                Tags.TextTag => MakeTextNode(element, nextNode),
                _ => throw new NotImplementedException(),
            };
        }

        private TextNode MakeTextNode(XmlElement element, StoryNode? nextNode)
        {
            Debug.Assert(element.Name == Tags.TextTag);

            var attributes = element.Attributes.Cast<XmlAttribute>().ToImmutableDictionary(a => a.Name, a => a.Value);

            if (element.HasChildNodes)
            {
                errors ??= new List<Error>();
                errors.Add(new Error(ErrorCode.TextElementWithChildren));

                // we can recover by just ignoring this
            }

            return new TextNode(element.InnerText, nextNode, attributes);
        }
    }
}
