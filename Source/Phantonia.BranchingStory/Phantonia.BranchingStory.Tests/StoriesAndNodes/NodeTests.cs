using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Phantonia.BranchingStory.Tests.StoriesAndNodes
{
    [TestClass]
    public sealed class NodeTests
    {
        public NodeTests() { }

        [TestMethod]
        public void TestAttributes()
        {
            IHasAttributes attr0 = new TextNode(text: "Pottery not respected", new Dictionary<string, string>
            {
                ["char"] = "Queen"
            }.ToImmutableDictionary());

            IHasAttributes attr1 = new SwitchOptionNode(id: 0, new Dictionary<string, string>
            {
                ["type"] = "explicit"
            }.ToImmutableDictionary());

            Assert.IsTrue(attr0.Attributes.TryGetValue("char", out string? value)
                       && value == "Queen");
            Assert.IsFalse(attr0.Attributes.ContainsKey("type"));

            Assert.IsTrue(attr1.Attributes.TryGetValue("type", out value)
                       && value == "explicit");
        }
    }
}