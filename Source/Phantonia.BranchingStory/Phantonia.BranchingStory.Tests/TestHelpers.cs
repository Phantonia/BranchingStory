using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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

        public static StoryTester TestStory(Story story)
            => new(story);

        public struct StoryTester
        {
            internal StoryTester(Story story)
            {
                this.story = story;
            }

            private readonly Story story;

            public void AssertIsDone()
            {
                Assert.IsFalse(story.CanProgressWithoutOption());

                Random rnd = new();

                for (int i = 0; i < 5; i++)
                {
                    unchecked
                    {
                        Assert.IsFalse(story.CanProgressWithOption(rnd.Next()));
                        Assert.IsFalse(story.CanProgressWithOption(-rnd.Next()));
                    }
                }

                Assert.ThrowsException<InvalidOperationException>(story.Progress);
            }

            public StoryTester TestWithOption<T>(int optionId, Func<T, bool>? assert = null)
                where T : StoryNode
            {
                Assert.IsTrue(story.CanProgressWithOption(optionId));

                Story newStory = story.ProgressWithOption(optionId);

                if (newStory.CurrentNode is T node)
                {
                    Assert.IsTrue(assert?.Invoke(node) ?? true);
                }
                else
                {
                    Assert.Fail();
                }

                return new StoryTester(newStory);
            }

            public StoryTester TestWithoutOption<T>(Func<T, bool>? assert = null)
            {
                Assert.IsTrue(story.CanProgressWithoutOption());

                Story newStory = story.Progress();

                if (newStory.CurrentNode is T node)
                {
                    Assert.IsTrue(assert?.Invoke(node) ?? true);
                }
                else
                {
                    Assert.Fail();
                }

                return new StoryTester(newStory);
            }
        }
    }
}
