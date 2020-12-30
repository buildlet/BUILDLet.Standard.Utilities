/***************************************************************************************************
The MIT License (MIT)

Copyright 2020 Daiki Sakamoto

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
associated documentation files (the "Software"), to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, publish, distribute, 
sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is 
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or 
substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT 
NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
***************************************************************************************************/
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BUILDLet.Standard.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

using BUILDLet.UnitTest.Utilities;  // for TestParameter
using System.Security.Authentication.ExtendedProtection;
using System.Diagnostics;

namespace BUILDLet.Standard.Utilities.Tests
{
    [TestClass]
    public class HtmlElementTests
    {
        // ----------------------------------------------------------------
        // Tests of RawText Property
        // ----------------------------------------------------------------

        [DataTestMethod]
        [DataRow(@"<P style=""font-weight: normal"">Hello, world.</P>")]
        [DataRow(@"<P style=""font-weight: normal"">
Hello, world.
</P>")]
        [DataRow(@"<P style=""font-weight: normal"">
Hello, 
world.
</P>")]
        public void RawTextTest(string text)
        {
            // ARRANGE
            var content = new HtmlElement(text);

            // ACT & ASSERT
            Assert.AreEqual(text, content.RawText);
        }


        // ----------------------------------------------------------------
        // Tests of Text Property
        // ----------------------------------------------------------------

        [TestMethod]
        // Input Text, Expected Text
        [DataRow(@"<P style=""font-weight: normal"">Hello, world.</P>", @"<P style=""font-weight: normal"">Hello, world.</P>")]
        [DataRow(@"<P style=""font-weight: normal"">
Hello, world.
</P>", @"<P style=""font-weight: normal"">Hello, world.</P>")]
        [DataRow(@"<P style=""font-weight: normal"">
Hello, 
world.
</P>", @"<P style=""font-weight: normal"">Hello, world.</P>")]
        public void TextTest(string input, string expected)
        {
            // ARRANGE
            var content = new HtmlElement(input);

            // ACT & ASSERT
            Assert.AreEqual(expected, content.Text);
        }


        // ----------------------------------------------------------------
        // Tests of GetTagName Method
        // ----------------------------------------------------------------

        [DataTestMethod]
        [DataRow(null, null)]
        [DataRow("<!DOCTYPE>", "!DOCTYPE")]
        [DataRow("<!DOCTYPE html>", "!DOCTYPE")]
        [DataRow("<test>", "test")]
        [DataRow("<test  >", "test")]
        [DataRow("<test/>", "test")]
        [DataRow("<test  />", "test")]
        [DataRow("</test>", "test")]
        [DataRow("</test  >", "test")]
        [DataRow("<html>Hello</html>", "html")]
        [DataRow("<html>Hello", "html")]
        [DataRow("<html  >Hello", "html")]
        [DataRow("<html lang=\"en\">Hello</html>", "html")]
        public void GetTagNameTest(string input, string expected)
        {
            Assert.AreEqual(expected, HtmlElement.GetTagName(input));
        }


        // ----------------------------------------------------------------
        // Tests of GetStartTag Method
        // ----------------------------------------------------------------

        [DataTestMethod]
        [DataRow("<!DOCTYPE>", "<!DOCTYPE>")]
        [DataRow("<!DOCTYPE html>", "<!DOCTYPE html>")]
        [DataRow("<test>", "<test>")]
        [DataRow("<test/>", "<test/>")]
        [DataRow("<test abc='xyz'/>", "<test abc='xyz'/>")]
        [DataRow("</test>", null)]
        [DataRow("<html>Hello</html>", "<html>")]
        [DataRow("<html>Hello", "<html>")]
        [DataRow("Hello</html>", null)]
        [DataRow("<html lang=\"en\">Hello</html>", "<html lang=\"en\">")]
        [DataRow("<html lang=\"en\">Hello", "<html lang=\"en\">")]
        public void GetStartTagTest(string input, string expected)
        {
            Assert.AreEqual(expected, HtmlElement.GetStartTag(input));
        }


        // ----------------------------------------------------------------
        // Tests of GetEndTag Method
        // ----------------------------------------------------------------

        [DataTestMethod]
        [DataRow("<!DOCTYPE>", null)]
        [DataRow("<!DOCTYPE html>", null)]
        [DataRow("<test>", null)]
        [DataRow("<test/>", null)]
        [DataRow("<test abc='xyz'/>", null)]
        [DataRow("</test>", "</test>")]
        [DataRow("<html>Hello</html>", "</html>")]
        [DataRow("<html>Hello", null)]
        [DataRow("Hello</html>", "</html>")]
        [DataRow("<html lang=\"en\">Hello</html>", "</html>")]
        [DataRow("<html lang=\"en\">Hello", null)]
        public void GetEndTagTest(string input, string expected)
        {
            Assert.AreEqual(expected, HtmlElement.GetEndTag(input));
        }


        // ----------------------------------------------------------------
        // Tests of GetAttributes Method
        // ----------------------------------------------------------------

        // TestParameter for GetAttributesMethodTest
        public class GetAttributesMethodTestParameter : TestParameter<string[][]>
        {
            public string StartTag;
            public string[][] Attributes;

            // ARRANGE: SET Expected
            public override void Arrange(out string[][] expected) => expected = this.Attributes;

            // ACT
            public override void Act(out string[][] actual)
            {
                // GET Actual
                var attributes = HtmlElement.GetAttributes(this.StartTag);

                // GET Attribute(s) Text
                string[][] attributes_texts = new string[attributes.Count][];
                int count = 0;
                foreach (var key in attributes.Keys)
                {
                    attributes_texts[count++] = new string[] { key, attributes[key] };
                }

                // SET Actual
                actual = attributes_texts;
            }

            // ASSERT
            public override void Assert<TItem>(TItem expected, TItem actual)
            {
                Console.WriteLine("");

                // Assert Attribute(s)
                GetAttributesMethodTestParameter.AssertAttributes(expected as string[][], actual as string[][]);
            }

            // Assert Attribute(s)
            public static void AssertAttributes(string[][] expected, string[][] actual)
            {
                // PRINT (Count)
                Console.WriteLine($"Expected Count = {expected.Length}");
                Console.WriteLine($"Actual Count = {actual.Length}");

                // ASSERT (Count)
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.Length, actual.Length);

                for (int i = 0; i < expected.Length; i++)
                {
                    // PRINT
                    Console.WriteLine($"Expected[{i}]:\t{expected[i][0]} = {expected[i][1]}");
                    Console.WriteLine($"Actual[{i}]:\t{actual[i][0]} = {actual[i][1]}");

                    // ASSERT
                    Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected[i][0], actual[i][0]);
                    Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected[i][1], actual[i][1]);
                }
            }
        }

        // Test Data for GetAttributesMethodTest
        public static IEnumerable<object[]> GetAttributesMethodTestData => new List<object[]>()
        {
            // object[]
            // {
            //    // Start Tag
            //    string,
            //
            //    // Attribute(s)
            //    Dictionary<string, string>() {}
            // }

            // 2) Double Quotation
            new object[]
            {
                // Start Tag
                "<HTML Attribute1=\"123\" Attribute2=\"ABC\">",

                // Attribute(s)
                new string[][]
                {
                    new string[] { "Attribute1", "\"123\"" },
                    new string[] { "Attribute2", "\"ABC\"" }
                }
            },

            // 3) Single Quotation
            new object[]
            {
                // Start Tag
                "<HTML Attribute1='123' Attribute2='ABC'>",

                // Attribute(s)
                new string[][]
                {
                    new string[] { "Attribute1", "'123'" },
                    new string[] { "Attribute2", "'ABC'" }
                }
            },

            // 4) No Quotation
            new object[]
            {
                // Start Tag
                "<HTML Attribute1=123 Attribute2=ABC>",

                // Attribute(s)
                new string[][]
                {
                    new string[] { "Attribute1", "123" },
                    new string[] { "Attribute2", "ABC" }
                }
            },

            // 5) Empty
            new object[]
            {
                // Start Tag
                "<HTML ABC XYZ>",

                // Attribute(s)
                new string[][]
                {
                    new string[] { "ABC", null },
                    new string[] { "XYZ", null }
                }
            },

            // 6) Mix
            new object[]
            {
                // Start Tag
                "<HTML Attribute1=\"123\" Attribute2='123' Attribute3=ABC XYZ>",

                // Attribute(s)
                new string[][]
                {
                    new string[] { "Attribute1", "\"123\"" },
                    new string[] { "Attribute2", "'123'" },
                    new string[] { "Attribute3", "ABC" },
                    new string[] { "XYZ", null }
                }
            }
        };

        [DataTestMethod]
        [DynamicData(nameof(GetAttributesMethodTestData))]
        public void GetAttributesMethodTest(string startTag, string[][] attributes)
        {
            // SET Parameter
            GetAttributesMethodTestParameter param = new GetAttributesMethodTestParameter
            {
                StartTag = startTag,
                Attributes = attributes,
            };

            // ASSERT
            param.Validate(autoEnumerable: false);
        }


        // ----------------------------------------------------------------
        // Tests of Constructor (Exception)
        // ----------------------------------------------------------------

        [TestMethod]
        [TestCategory("Exception")]
        [ExpectedException(typeof(ArgumentException))]
        public void HtmlElementStartTagEmptyTest()
        {
            // ACT & ASSERT
            _ = new HtmlElement("", new HtmlContentCollection("<title>Hello</title>"), "</head>");
        }


        [TestMethod]
        [TestCategory("Exception")]
        [ExpectedException(typeof(ArgumentException))]
        public void HtmlElementEndTagEmptyTest()
        {
            // ACT & ASSERT
            _ = new HtmlElement("<head>", new HtmlContentCollection("<title>Hello</title>"), "");
        }


        [TestMethod]
        [TestCategory("Exception")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HtmlElementContentsNullTest()
        {
            // ACT & ASSERT
            _ = new HtmlElement("<html>", null, "</html>");
        }


        // ----------------------------------------------------------------
        // Tests of Constructor
        // ----------------------------------------------------------------

        [DataTestMethod]
        // Input Text, Tag Name, Start Tag, End Tag, Attribute(s): always null, Content Text
        [DataRow("<HTML>Hello</HTML>", "HTML", "<HTML>", "</HTML>", null, "Hello")]
        [DataRow("<HTML >Hello</HTML >", "HTML", "<HTML >", "</HTML >", null, "Hello")]
        [DataRow("<HTML>", "HTML", "<HTML>", null, null, "")]
        [DataRow("<HTML >", "HTML", "<HTML >", null, null, "")]
        [DataRow("<HTML/>", "HTML", "<HTML/>", null, null, null)]
        [DataRow("<HTML />", "HTML", "<HTML />", null, null, null)]
        [DataRow("<HTML>Hello", "HTML", "<HTML>", null, null, "Hello")]
        [DataRow("<HTML >Hello", "HTML", "<HTML >", null, null, "Hello")]
        [DataRow("Hello</HTML>", "HTML", null, "</HTML>", null, "Hello")]
        [DataRow("Hello</HTML >", "HTML", null, "</HTML >", null, "Hello")]
        [DataRow("<!DOCTYPE>", "!DOCTYPE", "<!DOCTYPE>", null, null, "")]
        public void NewHtmlElementFromTextTest(string text, string tagName, string startTag, string endTag, string[][] _, string contentText)
        {
            // ACT
            var element = new HtmlElement(text);

            // ASSERT
            Assert.AreEqual(tagName, element.TagName);
            Assert.AreEqual(startTag, element.StartTag);
            Assert.AreEqual(endTag, element.EndTag);

            if (contentText is null)
            {
                Assert.IsNull(element.Contents);
            }
            else if (string.IsNullOrEmpty(contentText))
            {
                Assert.AreEqual(0, element.Contents.Count);
            }
            else
            {
                Assert.AreEqual(1, element.Contents.Count);
                Assert.AreEqual(HtmlContentType.HtmlText, element.Contents[0].Type);
                Assert.AreEqual(contentText, element.Contents.Text);
            }
        }


        [DataTestMethod]
        // Start Tag, Contents.Text, End Tag
        [DataRow("<title>", "Hello", "</title>", "title")]
        [DataRow("<head>", "<title>Hello</title>", "</head>", "head")]
        [DataRow("<p>", "Hello, world.", null, "p")]
        [DataRow("<br>", "", null, "br")]
        [DataRow("<br/>", null, null, "br")]
        public void NewHtmlElementFromContentsTest(string startTag, string contentsText, string endTag, string tagName)
        {
            // ARRANGE
            var contents = contentsText is null ? null : new HtmlContentCollection(contentsText);

            // for Debug
            Debug.WriteLine("");

            // ACT
            var element = new HtmlElement(startTag, contents, endTag);

            // ASSERT
            Assert.AreEqual(tagName, element.TagName);
            Assert.AreEqual(startTag, element.StartTag);
            Assert.AreEqual(endTag, element.EndTag);
            Assert.AreEqual(contentsText, element.Contents?.Text);
        }


        // ----------------------------------------------------------------
        // Tests of HtmlElement itself
        // ----------------------------------------------------------------

        // TestParameter for HtmlElement
        public class HtmlElementTestParameter : TestParameter<object[]>
        {
            public string Text;
            public string TagName;
            public string StartTag;
            public string EndTag;
            public string[][] Attributes;
            public string ContentText;

            // ARRANGE: SET Expected
            public override void Arrange(out object[] expected) =>
                expected = new object[] { this.TagName, this.StartTag, this.EndTag, this.Attributes, this.ContentText };

            // ACT
            public override void Act(out object[] actual)
            {
                // GET Actual
                var element = new HtmlElement(this.Text);

                // GET Attribute(s) Text
                string[][] attributes_texts = new string[element.Attributes.Count][];
                int count = 0;
                foreach (var key in element.Attributes.Keys)
                {
                    attributes_texts[count++] = new string[] { key, element.Attributes[key] };
                }

                // SET Actual
                actual = new object[] { element.TagName, element.StartTag, element.EndTag, attributes_texts, element.Contents.Text };
            }

            // ASSERT
            public override void Assert<TItem>(TItem expected, TItem actual)
            {
                var itemNames = new string[]
                {
                    "Tag Name",        // 0
                    "Start Tag",       // 1
                    "End Tag",         // 2
                    "Attribute(s)",    // 3
                    "Contents.Text"    // 4
                };

                Console.WriteLine("");

                for (int i = 0; i < itemNames.Length; i++)
                {
                    if (i == 3)
                    {
                        // ASSERT: Count of Attribute(s)
                        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(((expected as object[])[i] as string[][]).Length, ((actual as object[])[i] as string[][]).Length);

                        // PRINT
                        Console.WriteLine($"{itemNames[i]}:");

                        // Assert Attribute(s)
                        GetAttributesMethodTestParameter.AssertAttributes((expected as object[])[i] as string[][], (actual as object[])[i] as string[][]);
                    }
                    else
                    {
                        // PRINT
                        Console.WriteLine($"{itemNames[i]}: Expected\t= {(expected as object[])[i]}");
                        Console.WriteLine($"{itemNames[i]}: Actual\t= {(actual as object[])[i]}");

                        // ASSERT
                        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual((expected as object[])[i], (actual as object[])[i]);
                    }
                }
            }
        }

        // Test Data for HtmlElement
        public static IEnumerable<object[]> HtmlElementTestData => new List<object[]>()
        {
            // object[]
            // {
            //    // Text
            //    string,
            //
            //    // Tag Name
            //    string,
            //
            //    // Start Tag
            //    string,
            //
            //    // End Tag
            //    string,
            //
            //    // Attribute(s)
            //    Dictionary<string, string>() {}
            //
            //    // Content Text
            //    string,
            // }

            // 2)
            new object[]
            {
                // Text
                "<HTML attribute1=\"abc\" attribute2=\"xyz\">Hello, world.</HTML>",

                // Tag Name
                "HTML",

                // Start Tag
                "<HTML attribute1=\"abc\" attribute2=\"xyz\">",

                // End Tag
                "</HTML>",

                // Attribute(s)
                new string[][]
                {
                    new string[] { "attribute1", "\"abc\"" },
                    new string[] { "attribute2", "\"xyz\"" }
                },

                // Content Text
                "Hello, world."
            },

            // 3)
            new object[]
            {
                // Text
                "<HTML attribute1='abc' attribute2='xyz'>Hello, world.</HTML>",

                // Tag Name
                "HTML",

                // Start Tag
                "<HTML attribute1='abc' attribute2='xyz'>",

                // End Tag
                "</HTML>",

                // Attribute(s)
                new string[][]
                {
                    new string[] { "attribute1", "'abc'" },
                    new string[] { "attribute2", "'xyz'" }
                },

                // Content Text
                "Hello, world."
            },

            // 4)
            new object[]
            {
                // Text
                "<HTML attribute1=abc attribute2=xyz>Hello, world.</HTML>",

                // Tag Name
                "HTML",

                // Start Tag
                "<HTML attribute1=abc attribute2=xyz>",

                // End Tag
                "</HTML>",

                // Attribute(s)
                new string[][]
                {
                    new string[] { "attribute1", "abc" },
                    new string[] { "attribute2", "xyz" }
                },

                // Content Text
                "Hello, world."
            },

            // 5)
            new object[]
            {
                // Text
                "<HTML attribute1 attribute2>Hello, world.</HTML>",

                // Tag Name
                "HTML",

                // Start Tag
                "<HTML attribute1 attribute2>",

                // End Tag
                "</HTML>",

                // Attribute(s)
                new string[][]
                {
                    new string[] { "attribute1", null },
                    new string[] { "attribute2", null }
                },

                // Content Text
                "Hello, world."
            },

            // 6)
            new object[]
            {
                // Text
                "<HTML attribute1=\"abc\" attribute2='xyz' attribute3=123 attribute4>Hello, world.</HTML>",

                // Tag Name
                "HTML",

                // Start Tag
                "<HTML attribute1=\"abc\" attribute2='xyz' attribute3=123 attribute4>",

                // End Tag
                "</HTML>",

                // Attribute(s)
                new string[][]
                {
                    new string[] { "attribute1", "\"abc\"" },
                    new string[] { "attribute2", "'xyz'" },
                    new string[] { "attribute3", "123" },
                    new string[] { "attribute4", null }
                },

                // Content Text
                "Hello, world."
            }
        };

        [DataTestMethod]
        [DynamicData(nameof(HtmlElementTestData))]
        public void HtmlElementTest(string text, string tagName, string startTag, string endTag, string[][] attributes, string contentText)
        {
            // SET Parameter
            HtmlElementTestParameter param = new HtmlElementTestParameter
            {
                Text = text,
                TagName = tagName,
                StartTag = startTag,
                EndTag = endTag,
                Attributes = attributes,
                ContentText = contentText
            };

            // ASSERT
            param.Validate(autoEnumerable: false);
        }


        // ----------------------------------------------------------------
        // Tests of Indexer (Exception)
        // ----------------------------------------------------------------

        [TestMethod]
        [TestCategory("Exception")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void IndexerUnmatchTest()
        {
            // ARRANGE
            var element = new HtmlElement("<title>Hello</title>");

            // ACT & ASSERT
            _ = element["h1"];
        }


        [TestMethod]
        [TestCategory("Exception")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void IndexerEmptyTest()
        {
            // ARRANGE
            var element = new HtmlElement("<title>Hello</title>");

            // ACT & ASSERT
            _ = element[""];
        }


        [TestMethod]
        [TestCategory("Exception")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IndexerNullTest()
        {
            // ARRANGE
            var element = new HtmlElement("<title>Hello</title>");

            // ACT & ASSERT
            _ = element[null];
        }


        // ----------------------------------------------------------------
        // Tests of Indexer
        // ----------------------------------------------------------------

        [TestMethod]
        public void IndexerTest1()
        {
            // ARRANGE
            var text = "<head><title>Hello</title></head>";

            // ACT
            var element = new HtmlElement(text);

            // ASSERT
            Assert.AreEqual("Hello", element["title"].Contents.Text);
        }
    }
}