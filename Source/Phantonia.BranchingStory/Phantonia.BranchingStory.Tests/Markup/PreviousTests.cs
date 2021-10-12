using Microsoft.VisualStudio.TestTools.UnitTesting;
using Phantonia.BranchingStory.Markup;

namespace Phantonia.BranchingStory.Tests.Markup
{
    [TestClass]
    public sealed class PreviousTests
    {
        [TestMethod]
        public void TestLocalSwitchPrevious()
        {
            const string Code = @"
            <story>
                <switch name=""MySwitch"">
                    <opt id=""0"">
                        <text>Option 0</text>
                    </opt>

                    <opt id=""1"">
                        <text>Option 1</text>
                    </opt>
                </switch>

                <text>Something intermediate</text>

                <previous switch=""MySwitch"">
                    <opt id=""0"">
                        <text>Continue with option 0</text>
                    </opt>

                    <opt id=""1"">
                        <text>Continue with option 1</text>
                    </opt>
                </previous>
            </story>
            ";

            Interpreter intp = new(TestHelpers.StreamFromString(Code));

            InterpretationResult result = intp.Interpret();

            Story? story = result.Story;

            Assert.IsNotNull(story);

            // path 0
            TestHelpers.TestStory(story)
                       .TestWithOption<TextNode>(0, n => n.Text == "Option 0")
                       .TestWithoutOption<TextNode>(n => n.Text == "Something intermediate")
                       .TestWithoutOption<PreviousNode>()
                       .TestWithoutOption<TextNode>(n => n.Text == "Continue with option 0")
                       .AssertIsDone();

            // path 1
            TestHelpers.TestStory(story)
                       .TestWithOption<TextNode>(1, n => n.Text == "Option 1")
                       .TestWithoutOption<TextNode>(n => n.Text == "Something intermediate")
                       .TestWithoutOption<PreviousNode>()
                       .TestWithoutOption<TextNode>(n => n.Text == "Continue with option 1")
                       .AssertIsDone();
        }

        [TestMethod]
        public void TestIncompletePrevious()
        {
            const string Code = @"
            <story>
                <switch name=""MySwitch"">
                    <opt id=""0"">
                        <text>Option 0</text>
                    </opt>

                    <opt id=""1"">
                        <text>Option 1</text>
                    </opt>
                </switch>

                <text>Something intermediate</text>

                <previous switch=""MySwitch"">
                    <opt id=""0"">
                        <text>Continue with option 0</text>
                    </opt>
                </previous>
            </story>
            ";

            Interpreter intp = new(TestHelpers.StreamFromString(Code));

            InterpretationResult result = intp.Interpret();

            Story? story = result.Story;

            Assert.IsNotNull(story);

            // path 0
            TestHelpers.TestStory(story)
                       .TestWithOption<TextNode>(0, n => n.Text == "Option 0")
                       .TestWithoutOption<TextNode>(n => n.Text == "Something intermediate")
                       .TestWithoutOption<PreviousNode>()
                       .TestWithoutOption<TextNode>(n => n.Text == "Continue with option 0")
                       .AssertIsDone();

            // path 1
            TestHelpers.TestStory(story)
                       .TestWithOption<TextNode>(1, n => n.Text == "Option 1")
                       .TestWithoutOption<TextNode>(n => n.Text == "Something intermediate")
                       .TestWithoutOption<PreviousNode>()
                       .AssertIsDone();
        }
    }
}
