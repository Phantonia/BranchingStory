﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using AttributeList = System.Collections.Immutable.ImmutableDictionary<string, string>;

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

            BodyInterpretationResult? result = InterpretBody(storyElement, nextNode: null);

            errors.AddRange(result.Errors);

            Story? story = result.EntryPoint is null ? null : new(result.EntryPoint);

            return new InterpretationResult { Story = story, Errors = errors.ToImmutableArray() };
        }

        private BodyInterpretationResult InterpretBody(XmlElement bodyRootElement, StoryNode? nextNode, int childStartIndex = 0)
        {
            StoryNode? followingNode = nextNode;

            foreach (XmlElement element in bodyRootElement.ChildNodes.Cast<XmlElement>()
                                                                     .Reverse()
                                                                     .Skip(childStartIndex))
            {
                StoryNode? currentNode = MakeNode(element, followingNode);
                followingNode = currentNode;
            }

            Debug.Assert(followingNode is not null);

            // nextNode is the first node now
            return new(EntryPoint: followingNode, ExitPoints: ImmutableList<StoryNode>.Empty);
        }

        private static AttributeList GetAttributes(XmlElement element)
        {
            return element.Attributes.Cast<XmlAttribute>().ToImmutableDictionary(a => a.Name, a => a.Value);
        }

        private StoryNode MakeNode(XmlElement element, StoryNode? nextNode)
        {
            return element.Name switch
            {
                Tags.TextTag => MakeTextNode(element, nextNode),
                Tags.ActionTag => MakeActionNode(element, nextNode),
                Tags.SwitchTag => MakeSwitchNode(element, nextNode),

                _ => throw new NotImplementedException(),
            };
        }

        private ActionNode MakeActionNode(XmlElement element, StoryNode? nextNode)
        {
            Debug.Assert(element.Name == Tags.ActionTag);

            AttributeList attributes = GetAttributes(element);

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

            AttributeList attributes = GetAttributes(element);

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

        private SwitchNode MakeSwitchNode(XmlElement element, StoryNode? nextNode)
        {
            Debug.Assert(element.Name == Tags.SwitchTag);

            AttributeList attributes = GetAttributes(element);

            bool isGlobal;

            if (attributes.TryGetValue(Tags.GlobalAttribute, out string? globalValue))
            {
                switch (globalValue)
                {
                    case Tags.True:
                        isGlobal = true;
                        break;
                    case Tags.False:
                        isGlobal = false;
                        break;
                    default:
                        errors.Add(new Error(ErrorCode.BooleanAttributeNotTrueOrFalse));
                        isGlobal = false;
                        break;
                }

                attributes = attributes.Remove(Tags.GlobalAttribute);
            }
            else
            {
                isGlobal = false;
            }

            if (attributes.TryGetValue(Tags.NameAttribute, out string? name))
            {
                attributes = attributes.Remove(Tags.NameAttribute);
            }
            else
            {
                name = null;
            }

            List<SwitchOptionNode> options = new();

            foreach (XmlElement child in element.ChildNodes)
            {
                if (child.Name != Tags.OptTag)
                {
                    errors.Add(new Error(ErrorCode.SwitchChildNotOpt));

                    // we recover by ignoring this child
                    continue;
                }

                SwitchOptionNode option = MakeSwitchOptionNode(child, nextNode);
                options.Add(option);
            }

            ImmutableDictionary<int, SwitchOptionNode> optionsDict = options.ToImmutableDictionary(o => o.Id);

            return new SwitchNode(optionsDict, name, isGlobal, attributes);
        }

        private SwitchOptionNode MakeSwitchOptionNode(XmlElement optElement, StoryNode? nextNode)
        {
            Debug.Assert(optElement.Name == Tags.OptTag);

            AttributeList optAttributes = GetAttributes(optElement);

            if (!optAttributes.TryGetValue(Tags.IdAttribute, out string? value) || !int.TryParse(value, out int id))
            {
                errors.Add(new Error(ErrorCode.OptWithoutId));

                // we can recover by ignoring the issue

                id = -1;
            }
            else
            {
                optAttributes = optAttributes.Remove(Tags.IdAttribute);
            }

            int startIndex = 0;
            string nodeText = "";

            if (optElement.ChildNodes.Count >= 1 && optElement.ChildNodes[0]?.Name == Tags.OptTextTag)
            {
                startIndex = 1;

                XmlElement? optTextElement = optElement.ChildNodes[0] as XmlElement;

                Debug.Assert(optTextElement is not null);

                //AttributeList optTextAttributes = GetAttributes(optTextElement);

                if (optTextElement.ChildNodes.Count == 1 && optTextElement.ChildNodes[0] is XmlText xmlText)
                {
                    nodeText = xmlText.Value ?? "";
                }
                else if (!optTextElement.HasChildNodes)
                {
                    nodeText = "";
                }
                else
                {
                    errors.Add(new Error(ErrorCode.TextElementWithChildren));
                    nodeText = optTextElement.InnerText;
                }
            }

            BodyInterpretationResult result = InterpretBody(optElement, nextNode, startIndex);

            return new SwitchOptionNode(id, nodeText, result.EntryPoint, optAttributes);
        }
    }
}