using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Phantonia.BranchingStory.Tests.StoryAndNodes
{
    [TestClass]
    public sealed class StoryTests
    {
        public StoryTests() { }

        [TestMethod]
        public void TestStoryWithTextNodes()
        {
            const string Text0 = "'Hello world!'";
            const string Text1 = "I'm something of a programmer myself!";

            TextNode tn0 = new(Text0);
            TextNode tn1 = new(Text1);

            tn0 = tn0 with { NextNode = tn1 };

            Story story0 = new(tn0);

            Assert.AreSame(tn0, story0.CurrentNode);

            if (story0.CurrentNode is not TextNode { Text: Text0, NextNode: TextNode tn0_next })
            {
                Assert.Fail();
                return;
            }

            Assert.AreSame(tn1, tn0_next);

            Assert.IsTrue(story0.CanProgressWithoutOption());
            Assert.IsFalse(story0.CanProgressWithOption(0));

            Story story1 = story0.Progress();

            Assert.AreSame(tn1, story1.CurrentNode);

            Assert.IsFalse(story1.CanProgressWithoutOption());
            Assert.IsFalse(story1.CanProgressWithOption(1729));

            Assert.ThrowsException<InvalidOperationException>(() => story1.Progress());
        }
    }
}