using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Immutable;

namespace Phantonia.BranchingStory.Tests.StoriesAndNodes
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

            Assert.ThrowsException<InvalidOperationException>(story1.Progress);
        }

        [TestMethod]
        public void TestStoryWithActionNodes()
        {
            const string Action0 = "LikeJKRowlingTweet";
            const string Action1 = "YeetThem";

            ActionNode an0 = new(Action0);
            ActionNode an1 = new(Action1);

            an0 = an0 with { NextNode = an1 };

            Story story0 = new(an0);

            Assert.AreSame(an0, story0.CurrentNode);
            Assert.IsTrue(story0.CanProgressWithoutOption());

            Story story1 = story0.Progress();

            Assert.AreSame(an1, story1.CurrentNode);
            Assert.IsFalse(story1.CanProgressWithoutOption());

            Assert.ThrowsException<InvalidOperationException>(story1.Progress);
        }

        [TestMethod]
        public void TestStoryWithSwitch()
        {
            const string Option0 = "Get banana";
            const string Option1 = "Do not get banana";
            const string Text0 = "Potassium";
            const string Text1 = "Kris You Are Going To Get Sick";

            SwitchOptionNode so0 = new(id: 0, Option0);

            TextNode so0_tn = new(Text0);

            so0 = so0 with { NextNode = so0_tn };

            SwitchOptionNode so1 = new(id: 1, Option1);

            TextNode so1_tn = new(Text1);

            so1 = so1 with { NextNode = so1_tn };

            ImmutableDictionary<int, SwitchOptionNode> options = new[] { so0, so1 }.ToImmutableDictionary(o => o.Id);

            SwitchNode sn = new(options);

            Story story0 = new(sn);

            Assert.IsTrue(story0.CanProgressWithOption(0));
            Assert.IsTrue(story0.CanProgressWithOption(1));
            Assert.IsFalse(story0.CanProgressWithOption(2));
            Assert.IsFalse(story0.CanProgressWithOption(1337));

            Story story1_0 = story0.ProgressWithOption(0);
            Story story1_1 = story0.ProgressWithOption(1);

            Assert.IsTrue(story1_0.CurrentNode is TextNode { Text: Text0 });
            Assert.IsTrue(story1_1.CurrentNode is TextNode { Text: Text1 });

            Assert.IsFalse(story1_0.CanProgressWithoutOption());
            Assert.IsFalse(story1_1.CanProgressWithoutOption());

            Assert.ThrowsException<InvalidOperationException>(story1_0.Progress);
            Assert.ThrowsException<InvalidOperationException>(story1_1.Progress);
        }
    }
}