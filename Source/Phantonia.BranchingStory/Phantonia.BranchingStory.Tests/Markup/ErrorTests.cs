using Microsoft.VisualStudio.TestTools.UnitTesting;
using Phantonia.BranchingStory.Markup;
using System.IO;

namespace Phantonia.BranchingStory.Tests.Markup
{
    [TestClass]
    public sealed class ErrorTests
    {
        public ErrorTests() { }

        [TestMethod]
        public void TestSyntaxErrors()
        {
            Test(@"
            <story>
                <text>
            </story>");

            Test(@"
            print('Suddenly python')
            ");

            Test(@"
            <story>
                <text>XYZ</text
            <story>");

            Test(@"
            [story]
                [text]ABC[/text]
            [/story]");

            Test("");

            Test(@"
            <story>
                <text/>
            </story>
            <anotherElmt/>");
            
            Test(@"
            <story>
                <text></action>
            </story>");

            static void Test(string xml)
            {
                Stream xmlStream = TestHelpers.StreamFromString(xml);
                Interpreter intp = new(xmlStream);

                InterpretationResult result = intp.Interpret();

                Assert.IsNull(result.Story);
                Assert.AreEqual(1, result.Errors.Length);

                Assert.AreEqual(ErrorCode.XmlSyntaxError, result.Errors[0].ErrorCode);
            }
        }
    }
}