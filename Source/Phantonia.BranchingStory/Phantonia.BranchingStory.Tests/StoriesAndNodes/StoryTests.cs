using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
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

        [TestMethod]
        public void TestStoryWithLocalDecisions()
        {
            // Don't try this question at home kids!

            /*
            <text char="Liz">
                Hey, Mary, do you love me?
            </text>

            <switch name="MaryLovesLiz">
                <opt id="0">
                    <opttext>Yes</opttext>

                    <text char="Liz">I love you too!</text>
                </opt>

                <opt id="1">
                    <opttext>No</opttext>

                    <text char="Liz">Oh...</text>
                </opt>
            </switch>

            <action char="Liz">Sneeze</action>

            <pre switch="MaryLovesLiz">
                <opt id="0">
                    <action char="Liz">KissMary</action>
                </opt>
            </pre>

            <text char="Narrator">That's it. That's the story.</text>
            */

            // now I need to generate that graph procedurally fml
            // actually don't try to generate a bs graph procedurally at home either

            // Ignoring the attributes

            const string SwitchName = "MaryLovesLiz";

            const string ThatsIt = "That's it. That's the story.";
            TextNode tn_narr_thatsIt = new(ThatsIt);

            const string KissMary = "KissMary";
            ActionNode an_liz_kissMary = new(KissMary, nextNode: tn_narr_thatsIt);

            ImmutableDictionary<int, PreviousOptionNode> branches = new Dictionary<int, PreviousOptionNode>
            {
                [0] = new(id: 0, an_liz_kissMary)
            }.ToImmutableDictionary();
            PreviousNode pn_loveLiz = new(SwitchName, branches, elseNode: tn_narr_thatsIt);

            const string Sneeze = "Sneeze";
            ActionNode an_liz_sneeze = new(Sneeze, nextNode: pn_loveLiz);

            const string Oh = "Oh...";
            TextNode tn_liz_no_oh = new(Oh, nextNode: an_liz_sneeze);

            const int NoId = 1;
            const string No = "No";
            SwitchOptionNode so_no = new(NoId, No, nextNode: tn_liz_no_oh);

            const string LoveYouToo = "I love you too!";
            TextNode tn_liz_yes_loveYouToo = new(LoveYouToo, nextNode: an_liz_sneeze);

            // bs is not short for branching story but for bullshit

            const int YesId = 0;
            const string Yes = "Yes";
            SwitchOptionNode so_yes = new(YesId, Yes, nextNode: tn_liz_yes_loveYouToo);

            ImmutableDictionary<int, SwitchOptionNode> options = new[] { so_yes, so_no }.ToImmutableDictionary(o => o.Id);
            SwitchNode sn_loveLiz = new(options, SwitchName);

            const string MaryDoYouLoveMe = "Hey, Mary, do you love me?";
            TextNode tn_liz_maryLove = new(MaryDoYouLoveMe, nextNode: sn_loveLiz);

            // done but never again

            Story rootStory = new(tn_liz_maryLove);
            Assert.IsTrue(rootStory.CurrentNode is TextNode);

            Story story1 = rootStory.Progress();
            Assert.IsTrue(story1.CurrentNode is SwitchNode);

            // Yes path
            {
                Story story2 = story1.ProgressWithOption(YesId);
                Assert.IsTrue(story2.CurrentNode is TextNode { Text: LoveYouToo });

                Story story3 = story2.Progress();
                Assert.IsTrue(story3.CurrentNode is ActionNode { ActionName: Sneeze });

                Assert.IsTrue(story3.CanProgressWithoutOption());

                Story story4 = story3.Progress();
                Assert.IsTrue(story4.CurrentNode is PreviousNode);

                Assert.IsTrue(story4.CanProgressWithoutOption());

                Story story5 = story4.Progress();
                Assert.IsTrue(story5.CurrentNode is ActionNode { ActionName: KissMary });

                Assert.IsTrue(story4.CanProgressWithoutOption());

                Story story6 = story5.Progress();
                Assert.IsTrue(story6.CurrentNode is TextNode { Text: ThatsIt });
            }

            // No path
            {
                Story story2 = story1.ProgressWithOption(NoId);
                Assert.IsTrue(story2.CurrentNode is TextNode { Text: Oh });

                Story story3 = story2.Progress();
                Assert.IsTrue(story3.CurrentNode is ActionNode { ActionName: Sneeze });

                Assert.IsTrue(story3.CanProgressWithoutOption());

                Story story4 = story3.Progress();
                Assert.IsTrue(story4.CurrentNode is PreviousNode);

                Assert.IsTrue(story4.CanProgressWithoutOption());

                Story story5 = story4.Progress();
                Assert.IsTrue(story5.CurrentNode is TextNode { Text: ThatsIt });
            }
        }
    }
}