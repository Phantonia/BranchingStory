using Microsoft.VisualStudio.TestTools.UnitTesting;
using Phantonia.BranchingStory.Markup;
using System;

namespace Phantonia.BranchingStory.Tests.Markup
{
    [TestClass]
    public sealed class SwitchTests
    {
        public SwitchTests() { }

        [TestMethod]
        public void TestSimpleSwitch()
        {
            const string Code = @"
            <story>
                <switch>
                    <opt id=""0"">
                        <text>Option 0</text>
                    </opt>

                    <opt id=""1"">
                        <text>Option 1</text>
                    </opt>
                </switch>
            </story>
            ";

            Interpreter intp = new(TestHelpers.StreamFromString(Code));

            InterpretationResult result = intp.Interpret();

            Story? story = result.Story;

            Assert.IsNotNull(story);

            Assert.IsTrue(story.CurrentNode is SwitchNode { IsGlobal: false, Name: null, Options: { Count: 2 } });

            Assert.IsTrue(story.CanProgressWithOption(0));
            Assert.IsTrue(story.CanProgressWithOption(1));
            Assert.IsFalse(story.CanProgressWithOption(23));
            Assert.IsFalse(story.CanProgressWithoutOption());

            // path 0
            {
                Story story1 = story.ProgressWithOption(0);

                Assert.IsTrue(story1.CurrentNode is TextNode { Text: "Option 0" });

                Assert.IsFalse(story1.CanProgressWithoutOption());
            }

            // path 1
            {
                Story story1 = story.ProgressWithOption(1);

                Assert.IsTrue(story1.CurrentNode is TextNode { Text: "Option 1" });

                Assert.IsFalse(story1.CanProgressWithoutOption());
            }
        }

        [TestMethod]
        public void TestSwitchWithFollowingLogic()
        {
            const string Code = @"
            <story>
                <switch>
                    <opt id=""0"">
                        <text>Option 0</text>
                    </opt>

                    <opt id=""1"">
                        <text>Option 1</text>
                    </opt>
                </switch>

                <action>MyAction</action>
            </story>
            ";

            Interpreter intp = new(TestHelpers.StreamFromString(Code));

            InterpretationResult result = intp.Interpret();

            Story? story = result.Story;

            Assert.IsNotNull(story);

            Assert.IsTrue(story.CurrentNode is SwitchNode { IsGlobal: false, Name: null, Options: { Count: 2 } });

            Assert.IsTrue(story.CanProgressWithOption(0));
            Assert.IsTrue(story.CanProgressWithOption(1));
            Assert.IsFalse(story.CanProgressWithOption(23));
            Assert.IsFalse(story.CanProgressWithoutOption());

            TestOption(0);
            TestOption(1);

            void TestOption(int optionId)
            {
                Story story1 = story.ProgressWithOption(optionId);

                Assert.IsTrue(story1.CurrentNode is TextNode { Text: var text } && text == "Option " + optionId);

                Assert.IsTrue(story1.CanProgressWithoutOption());

                story1 = story1.Progress();

                Assert.IsTrue(story1.CurrentNode is ActionNode { ActionName: "MyAction" });

                Assert.IsFalse(story1.CanProgressWithoutOption());
            }
        }

        [TestMethod]
        public void TestSwitchWithMoreComplicatedBranches()
        {
            const string TheBeginning = "The beginning";
            const string Option0 = "Option 0";
            const string MoreText = "More text";
            const string Option1 = "Option 1";
            const string AnAction = "AnAction";
            const string MyAction = "MyAction";

            string code = $@"
            <story>
                <text>{TheBeginning}</text>
                
                <switch>
                    <opt id=""0"">
                        <text>{Option0}</text>
                        <text>{MoreText}</text>
                    </opt>

                    <opt id=""1"">
                        <text>{Option1}</text>
                        <action>{AnAction}</action>
                    </opt>
                </switch>

                <action>{MyAction}</action>
            </story>
            ";

            Interpreter intp = new(TestHelpers.StreamFromString(code));

            InterpretationResult result = intp.Interpret();

            Story? story = result.Story;

            Assert.IsNotNull(story);

            Assert.IsTrue(story.CurrentNode is TextNode { Text: TheBeginning });

            story = story.Progress();

            Assert.IsTrue(story.CurrentNode is SwitchNode { IsGlobal: false, Name: null, Options: { Count: 2 } });

            // option 0
            {
                Story story1 = story.ProgressWithOption(0);
                story1 = TestNextNode(story1, n => n is TextNode { Text: Option0 }, expectsToProgress: true)!;
                story1 = TestNextNode(story1, n => n is TextNode { Text: MoreText }, expectsToProgress: true)!;
                _ = TestNextNode(story1, n => n is ActionNode { ActionName: MyAction }, expectsToProgress: false);
            }

            // option 1
            {
                Story story1 = story.ProgressWithOption(1);
                story1 = TestNextNode(story1, n => n is TextNode { Text: Option1 }, expectsToProgress: true)!;
                story1 = TestNextNode(story1, n => n is ActionNode { ActionName: AnAction }, expectsToProgress: true)!;
                _ = TestNextNode(story1, n => n is ActionNode { ActionName: MyAction }, expectsToProgress: false);
            }

            static Story? TestNextNode(Story story, Func<StoryNode, bool> predicate, bool expectsToProgress)
            {
                Assert.IsTrue(predicate(story.CurrentNode));

                Assert.AreEqual(expectsToProgress, story.CanProgressWithoutOption());

                if (expectsToProgress)
                {
                    return story.Progress();
                }

                Assert.ThrowsException<InvalidOperationException>(story.Progress);

                return null;
            }
        }

        [TestMethod]
        public void TestNestedSwitch()
        {
            const string Code = @"
            <story>
                <switch>
                    <opt id=""0"">
                        <switch>
                            <opt id=""2"">
                                <text>Option 0_2</text>
                            </opt>

                            <opt id=""3"">
                                <text>Option 0_3</text>
                            </opt>
                        </switch>
                    </opt>

                    <opt id=""1"">
                        <text>Option 1</text>
                    </opt>
                </switch>
            </story>
            ";

            Interpreter intp = new(TestHelpers.StreamFromString(Code));

            InterpretationResult result = intp.Interpret();

            Story? story = result.Story;

            Assert.IsNotNull(story);

            Assert.IsTrue(story.CurrentNode is SwitchNode { IsGlobal: false, Name: null, Options: { Count: 2 } });

            // option 0
            {
                Story story1 = story.ProgressWithOption(0);

                Assert.IsTrue(story1.CurrentNode is SwitchNode { IsGlobal: false, Name: null, Options: { Count: 2 } });

                // option 0_2
                {
                    Story story2 = story1.ProgressWithOption(2);

                    Assert.IsTrue(story2.CurrentNode is TextNode { Text: "Option 0_2" });
                    Assert.IsFalse(story2.CanProgressWithoutOption());
                }

                // option 0_3
                {
                    Story story2 = story1.ProgressWithOption(3);

                    Assert.IsTrue(story2.CurrentNode is TextNode { Text: "Option 0_3" });
                    Assert.IsFalse(story2.CanProgressWithoutOption());
                }

                Assert.IsFalse(story1.CanProgressWithOption(0));
                Assert.IsFalse(story1.CanProgressWithOption(1));
            }

            // option 1
            {
                Story story1 = story.ProgressWithOption(1);

                Assert.IsTrue(story1.CurrentNode is TextNode { Text: "Option 1" });
                Assert.IsFalse(story1.CanProgressWithoutOption());
            }
        }
    }
}
