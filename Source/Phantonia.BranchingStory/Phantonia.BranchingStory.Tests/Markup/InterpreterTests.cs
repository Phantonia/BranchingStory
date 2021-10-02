using Microsoft.VisualStudio.TestTools.UnitTesting;
using Phantonia.BranchingStory.Markup;
using System;
using System.Diagnostics;

namespace Phantonia.BranchingStory.Tests.Markup
{
    [TestClass]
    public sealed class InterpreterTests
    {
        public InterpreterTests() { }

        [TestMethod]
        public void TestSimpleTextCommand()
        {
            const string Code = @"
            <story>
                <text>Hello world.</text>
            </story>";

            Interpreter intp = new(TestHelpers.StreamFromString(Code));
            InterpretationResult result = intp.Interpret();

            Assert.AreEqual(0, result.Errors.Length);
            Assert.IsTrue(result.Story?.CurrentNode is TextNode { Text: "Hello world." });
            Assert.IsFalse(result.Story!.CanProgressWithoutOption());
            Assert.ThrowsException<InvalidOperationException>(result.Story.Progress);
        }

        [TestMethod]
        public void TestAttributes()
        {
            const string Code = @"
            <story>
                <text char=""Me"" otherAttr=""blob"">Hello world.</text>
            </story>";

            Interpreter intp = new(TestHelpers.StreamFromString(Code));
            InterpretationResult result = intp.Interpret();

            Assert.AreEqual(0, result.Errors.Length);
            Assert.IsTrue(result.Story?.CurrentNode is TextNode { Text: "Hello world." });
            Assert.AreEqual("Me", result.Story!.CurrentNode.Attributes["char"]);
            Assert.AreEqual("blob", result.Story.CurrentNode.Attributes["otherAttr"]);
        }

        [TestMethod]
        public void TestMultipleCommands()
        {
            const string Code = @"
            <story>
                <text>Hello world.</text>
                <text>Other text</text>
            </story>";

            Interpreter intp = new(TestHelpers.StreamFromString(Code));
            InterpretationResult result = intp.Interpret();

            Assert.AreEqual(0, result.Errors.Length);

            Story? story = result.Story;

            Debug.Assert(story is not null);

            Assert.IsTrue(story.CurrentNode is TextNode { Text: "Hello world." });
            Assert.IsTrue(story.CanProgressWithoutOption());

            story = story.Progress();

            Assert.IsTrue(story.CurrentNode is TextNode { Text: "Other text" });
            Assert.IsFalse(story.CanProgressWithoutOption());
            Assert.ThrowsException<InvalidOperationException>(story.Progress);
        }

        [TestMethod]
        public void TestActionAndTextCommands()
        {
            const string Code = @"
            <story>
                <text>Hello world.</text>
                <action>MyAction</action>
            </story>";

            Interpreter intp = new(TestHelpers.StreamFromString(Code));
            InterpretationResult result = intp.Interpret();

            Assert.AreEqual(0, result.Errors.Length);

            Story? story = result.Story;

            Debug.Assert(story is not null);

            Assert.IsTrue(story.CurrentNode is TextNode { Text: "Hello world." });
            Assert.IsTrue(story.CanProgressWithoutOption());

            story = story.Progress();

            Assert.IsTrue(story.CurrentNode is ActionNode { ActionName: "MyAction" });
            Assert.IsFalse(story.CanProgressWithoutOption());
            Assert.ThrowsException<InvalidOperationException>(story.Progress);
        }
    }
}