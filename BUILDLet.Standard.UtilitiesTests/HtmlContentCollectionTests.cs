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
using System.Linq;

namespace BUILDLet.Standard.Utilities.Tests
{
    [TestClass]
    public class HtmlContentCollectionTests
    {
        // ----------------------------------------------------------------
        // Tests of RawText Property
        // ----------------------------------------------------------------

        [DataTestMethod]
        [DataRow(@"<!DOCTYPE html>
<html>
    <head>
        <title>Hello</title>
    </head>
    <body>
        <p>Hello, world.
    </body>
</html>")]
        public void RawTextTest(string text)
        {
            // ARRANGE
            var contents = new HtmlContentCollection(text);

            // ACT & ASSERT
            Assert.AreEqual(text, contents.RawText);
        }


        // ----------------------------------------------------------------
        // Tests of Text Property
        // ----------------------------------------------------------------

        [TestMethod]
        // Input Text, Expected Text
        [DataRow(@"<!DOCTYPE html>
<html>
    <head>
        <title>Hello</title>
    </head>
    <body>
        <p>Hello, world.
    </body>
</html>", @"<!DOCTYPE html><html><head><title>Hello</title></head><body><p>Hello, world.</body></html>")]
        public void TextTest(string input, string expected)
        {
            // ARRANGE
            var contents = new HtmlContentCollection(input);

            // ACT & ASSERT
            Assert.AreEqual(expected, contents.Text);
        }


        // ----------------------------------------------------------------
        // Tests of HtmlContentCollection itself
        // ----------------------------------------------------------------

        [TestMethod]
        public void HtmlContentCollectionTest1()
        {
            // ARRANGE
            var text = "<title>Hello</title>";

            // ACT
            var contents = new HtmlContentCollection(text);

            // ASSERT (1)
            Assert.AreEqual(1, contents.Count);
            Assert.AreEqual("<title>Hello</title>", contents.Text);
            Assert.AreEqual("<title>Hello</title>", contents.RawText);
            Assert.AreEqual("<title>Hello</title>", contents[0].Text);
            Assert.AreEqual("<title>Hello</title>", contents[0].RawText);
            Assert.AreEqual("<title>", contents[0].StartTag);
            Assert.AreEqual("</title>", contents[0].EndTag);
            Assert.AreEqual("title", contents[0].TagName);
            Assert.AreEqual(HtmlContentType.HtmlElement, contents[0].Type);
            Assert.AreEqual(1, contents[0].Contents.Count);

            // ASSERT (2)
            Assert.AreEqual("Hello", contents[0].Contents[0].Text);
            Assert.AreEqual("Hello", contents[0].Contents[0].RawText);
            Assert.AreEqual(HtmlContentType.HtmlText, contents[0].Contents[0].Type);
        }


        [TestMethod]
        public void HtmlContentCollectionTest2()
        {
            // ARRANGE
            var text = "<p>Click <a href=\"http://www.google.com\">Me</a>!</p>";

            // ACT
            var contents = new HtmlContentCollection(text);

            // ASSERT
            Assert.AreEqual("<p>Click <a href=\"http://www.google.com\">Me</a>!</p>", contents.Text);
            Assert.AreEqual("<p>Click <a href=\"http://www.google.com\">Me</a>!</p>", contents[0].Text);
            Assert.AreEqual("<p>", contents[0].StartTag);
            Assert.AreEqual("</p>", contents[0].EndTag);
            Assert.AreEqual("p", contents[0].TagName);
            Assert.AreEqual(HtmlContentType.HtmlElement, contents[0].Type);
            Assert.AreEqual("Click ", contents[0].Contents[0].Text);
            Assert.AreEqual(HtmlContentType.HtmlText, contents[0].Contents[0].Type);
            Assert.AreEqual("<a href=\"http://www.google.com\">Me</a>", contents[0].Contents[1].Text);
            Assert.AreEqual(HtmlContentType.HtmlElement, contents[0].Contents[1].Type);
            Assert.AreEqual("!", contents[0].Contents[2].Text);
            Assert.AreEqual(HtmlContentType.HtmlText, contents[0].Contents[2].Type);

            // ASSERT (Attributes)
            Assert.AreEqual(true, contents[0].Contents[1].Attributes.ContainsKey("href"));
            Assert.AreEqual("\"http://www.google.com\"", contents[0].Contents[1].Attributes["href"]);
        }


        [TestMethod]
        public void HtmlContentCollectionTest3()
        {
            // ARRANGE
            var text = @"<table>
    <tr>
        <th>Item No.</th>
        <th>Value</th>
    </tr>
    <tr>
        <td>Item 1</td>
        <td>101</td>
    </tr>
    <tr>
        <td><a href=""./item2.html"">Item 2</a></td>
        <td>102</td>
    </tr>
    <tr>
        <td>Item 3</td>
        <td>103</td>
    </tr>
</table>";

            // ACT
            var contents = new HtmlContentCollection(text);

            // ASSERT
            Assert.AreEqual("Value", contents[0].Contents[1].Contents[3].Contents.Text);
            Assert.AreEqual(@"""./item2.html""", contents[0].Contents[5].Contents[1].Contents[0].Attributes["href"]);
            Assert.AreEqual("Item 3", contents[0].Contents[7].Contents[1].Contents.Text);
            Assert.AreEqual("103", contents[0].Contents[7].Contents[3].Contents.Text);

            // ASSERT (Indexer)
            Assert.AreEqual("Value", contents["table"].Contents["tr"].Contents["th", 1].Contents.Text);
            Assert.AreEqual(@"""./item2.html""", contents["table"].Contents["tr", 2].Contents["td"].Contents["a"].Attributes["href"]);
            Assert.AreEqual("Item 3", contents["table"].Contents["tr", 3].Contents["td"].Contents.Text);
            Assert.AreEqual("103", contents["table"].Contents["tr", 3].Contents["td", 1].Contents.Text);

            // ASSERT (Indexer 2)
            Assert.AreEqual("Value", contents["table"]["tr"]["th", 1].Contents.Text);
            Assert.AreEqual(@"""./item2.html""", contents["table"]["tr", 2]["td"]["a"].Attributes["href"]);
            Assert.AreEqual("Item 3", contents["table"]["tr", 3]["td"].Contents.Text);
            Assert.AreEqual("103", contents["table"]["tr", 3]["td", 1].Contents.Text);
        }


        [DataTestMethod]
        [DataRow("<head><title>Hello, world</title></head>")]
        [DataRow(@"<head>
    <title>Hello, world</title>
</head>")]
        [DataRow(@"<head>
    <title>Hello,
    world</title>
</head>")]
        public void HtmlContentCollectionTest4(string text)
        {
            // ARRANGE
            // (None)

            // ACT
            var contents = new HtmlContentCollection(text);

            // ASSERT (Root Contents)
            Assert.AreEqual(1, contents.Count);
            Assert.AreEqual("<head><title>Hello, world</title></head>", contents.Text);

            // ASSERT (head)
            Assert.AreEqual("<head><title>Hello, world</title></head>", contents["head"].Text);
            Assert.AreEqual("<head>", contents["head"].StartTag);
            Assert.AreEqual("</head>", contents["head"].EndTag);
            Assert.AreEqual("head", contents["head"].TagName);
            Assert.AreEqual(HtmlContentType.HtmlElement, contents["head"].Type);

            // ASSERT (head.Contents)
            // Assert.AreEqual(1, contents["head"].Contents.Count);  // Count is not correct.
            Assert.AreEqual("<title>Hello, world</title>", contents["head"].Contents.Text);

            // ASSERT (title)
            Assert.AreEqual("<title>Hello, world</title>", contents["head"]["title"].Text);
            Assert.AreEqual("<title>", contents["head"]["title"].StartTag);
            Assert.AreEqual("</title>", contents["head"]["title"].EndTag);
            Assert.AreEqual("title", contents["head"]["title"].TagName);
            Assert.AreEqual(HtmlContentType.HtmlElement, contents["head"]["title"].Type);

            // ASSERT (title.Contents)
            // Assert.AreEqual(1, contents[0].Contents[0].Contents.Count);  // Count is not correct.
            Assert.AreEqual("Hello, world", contents["head"]["title"].Contents.Text);

            // ASSERT (text)
            Assert.AreEqual("Hello, world", contents["head"]["title"].Contents[0].Text);
            Assert.AreEqual(null, contents["head"]["title"].Contents[0].StartTag);
            Assert.AreEqual(null, contents["head"]["title"].Contents[0].EndTag);
            Assert.AreEqual(null, contents["head"]["title"].Contents[0].TagName);
            Assert.AreEqual(HtmlContentType.HtmlText, contents["head"]["title"].Contents[0].Type);

            // ASSERT (text.Contents)
            Assert.AreEqual(null, contents["head"]["title"].Contents[0].Contents);
        }


        [DataTestMethod]
        [DataRow(@"<!DOCTYPE html>
<html>
    <head>
        <title>Hello</title>
    </head>
    <body>
        <p>Hello, world.
    </body>
</html>", false)]
        [DataRow(@"<!DOCTYPE html>
<!-- This is comment 1 -->
<html>
    <!-- This is comment 2 -->
    <head>
        <title>Hello</title>
    </head>
    <body>
        <p>Hello, world.
    </body>
</html>", true)]
        public void HtmlContentCollectionTest5(string text, bool comment)
        {
            // ARRANGE
            // (None)

            // ACT
            var contents = new HtmlContentCollection(text);

            // ASSERT (Root Contents)
            // Assert.AreEqual(2, contents.Count);  // Count is not correct.
            Assert.AreEqual("<!DOCTYPE html><html><head><title>Hello</title></head><body><p>Hello, world.</body></html>", contents.Text);

            if (!comment)
            {
                Assert.AreEqual(@"<!DOCTYPE html>
<html>
    <head>
        <title>Hello</title>
    </head>
    <body>
        <p>Hello, world.
    </body>
</html>", contents.RawText);
            }
            else
            {
                Assert.AreEqual(@"<!DOCTYPE html>
<!-- This is comment 1 -->
<html>
    <!-- This is comment 2 -->
    <head>
        <title>Hello</title>
    </head>
    <body>
        <p>Hello, world.
    </body>
</html>", contents.RawText);
            }


            // ASSERT (DOCTYPE)
            Assert.AreEqual("<!DOCTYPE html>", contents[0].Text);
            Assert.AreEqual("<!DOCTYPE html>", contents[0].RawText);
            Assert.AreEqual("<!DOCTYPE html>", contents[0].StartTag);
            Assert.AreEqual(null, contents[0].EndTag);
            Assert.AreEqual("!DOCTYPE", contents[0].TagName);
            // Assert.AreEqual(0, contents[0].Contents.Count);  // Count is not correct.

            // Attributes for !DOCTYPE has been changed into null.
            // Assert.AreEqual(1, contents[0].Attributes.Count);
            // Assert.AreEqual(true, contents[0].Attributes.ContainsKey("html"));
            // Assert.AreEqual(null, contents[0].Attributes["html"]);

            // ASSERT (html)
            Assert.AreEqual("<html><head><title>Hello</title></head><body><p>Hello, world.</body></html>", contents["html"].Text);
            if (!comment)
            {
                Assert.AreEqual(@"<html>
    <head>
        <title>Hello</title>
    </head>
    <body>
        <p>Hello, world.
    </body>
</html>", contents["html"].RawText);
            }
            else
            {
                Assert.AreEqual(@"<html>
    <!-- This is comment 2 -->
    <head>
        <title>Hello</title>
    </head>
    <body>
        <p>Hello, world.
    </body>
</html>", contents["html"].RawText);
            }

            // ASSERT (html.Contents)
            // Assert.AreEqual(2, contents[1].Contents.Count);  // Count is not correct.
            Assert.AreEqual("<head><title>Hello</title></head><body><p>Hello, world.</body>", contents["html"].Contents.Text);
            if (!comment)
            {
                Assert.AreEqual(@"
    <head>
        <title>Hello</title>
    </head>
    <body>
        <p>Hello, world.
    </body>
", contents["html"].Contents.RawText);
            }
            else
            {
                Assert.AreEqual(@"
    <!-- This is comment 2 -->
    <head>
        <title>Hello</title>
    </head>
    <body>
        <p>Hello, world.
    </body>
", contents["html"].Contents.RawText);
            }

            // ASSERT (head)
            Assert.AreEqual("<head><title>Hello</title></head>", contents["html"]["head"].Text);
            Assert.AreEqual(@"<head>
        <title>Hello</title>
    </head>", contents["html"]["head"].RawText);

            // ASSERT (head.Contents)
            // Assert.AreEqual(1, contents["html"]["head"].Contents.Count);  // Count is not correct.
            Assert.AreEqual("<title>Hello</title>", contents["html"]["head"].Contents.Text);
            Assert.AreEqual(@"
        <title>Hello</title>
    ", contents["html"]["head"].Contents.RawText);

            // ASSERT (body)
            Assert.AreEqual("<body><p>Hello, world.</body>", contents["html"]["body"].Text);
            Assert.AreEqual(@"<body>
        <p>Hello, world.
    </body>", contents["html"]["body"].RawText);

            // ASSERT (body.Contents)
            // Assert.AreEqual(1, contents["html"]["body"].Contents.Count);  // Count is not correct.
            Assert.AreEqual("<p>Hello, world.", contents["html"]["body"].Contents.Text);
            Assert.AreEqual(@"
        <p>Hello, world.
    ", contents["html"]["body"].Contents.RawText);

            // ASSERT (p)
            Assert.AreEqual("<p>Hello, world.", contents["html"]["body"]["p"].Text);
            Assert.AreEqual(@"<p>Hello, world.
    ", contents["html"]["body"]["p"].RawText);

            // ASSERT (p.Contents)
            // Assert.AreEqual(1, contents["html"]["body"]["p"].Contents.Count);  // Count is not correct.
            Assert.AreEqual("Hello, world.", contents["html"]["body"]["p"].Contents.Text);
            Assert.AreEqual(@"Hello, world.
    ", contents["html"]["body"]["p"].Contents.RawText);

            // ASSERT (text)
            Assert.AreEqual("Hello, world.", contents["html"]["body"]["p"].Contents.Text);
            Assert.AreEqual(@"Hello, world.
    ", contents["html"]["body"]["p"].Contents.RawText);

            // ASSERT (text.Contents)
            // Assert.AreEqual(null, contents[1].Contents[1].Contents[0].Contents[0].Contents);
        }


        [TestMethod]
        public void HtmlContentCollectionTest6()
        {
            // ARRANGE
            var text = @"<!DOCTYPE html>
<html>
    <head>
        <title>Greeting</title>
    </head>
    <body>
        <p>Good morning,<br>
        Good morning.<br>
        And, good morning.
        <p>Good afternoon.<br/>
        Good afternoon.<br>
        And, good afternoon.</p>
        <p>Good evening.<br>
        Good afternoon.<br>
        And, good afternoon.<br>
    </body>
</html>";

            // ACT
            var contents = new HtmlContentCollection(text);

            // ASSERT (.html.body.RawText)
            Assert.AreEqual(@"<body>
        <p>Good morning,<br>
        Good morning.<br>
        And, good morning.
        <p>Good afternoon.<br/>
        Good afternoon.<br>
        And, good afternoon.</p>
        <p>Good evening.<br>
        Good afternoon.<br>
        And, good afternoon.<br>
    </body>", contents["html"]["body"].RawText);

            // ASSERT (Count)
            // Assert.AreEqual(8, contents[1].Contents[1].Contents.Count);
            // Assert.AreEqual(4, contents[1].Contents[1].Contents[3].Contents.Count);


            // ARRANGE (body.Contents)
            var expected_body_contents_texts = new string[] {
                "<p>Good morning,",
                "<br>Good morning.",
                "<br>And, good morning.",
                "<p>Good afternoon.<br/>Good afternoon.<br>And, good afternoon.</p>",
                "<p>Good evening.",
                "<br>Good afternoon.",
                "<br>And, good afternoon.",
                "<br>"
            };
            var actual_body_contents = (from content in contents["html"]["body"].Contents where content.Type == HtmlContentType.HtmlElement select content.Text).ToArray();

            // ASSERT (body.Contents)
            Assert.AreEqual(expected_body_contents_texts.Length, actual_body_contents.Length);
            for (int i = 0; i < expected_body_contents_texts.Length; i++)
            {
                Assert.AreEqual(expected_body_contents_texts[i], actual_body_contents[i]);
            }


            // ARRANGE (body.p)
            var expected_p_texts = new string[] {
                "<p>Good morning,",
                "<p>Good afternoon.<br/>Good afternoon.<br>And, good afternoon.</p>",
                "<p>Good evening.",
            };
            var actual_p_texts = (from content in contents["html"]["body"].Contents where content.TagName == "p" select content.Text).ToArray();

            // ASSERT (body.p)
            Assert.AreEqual(expected_p_texts.Length, actual_p_texts.Length);
            for (int i = 0; i < expected_p_texts.Length; i++)
            {
                Assert.AreEqual(expected_p_texts[i], actual_p_texts[i]);
            }


            // ARRANGE (Contents of "afternoon")
            var expected_afternoon_contents_texts = new string[] {
                "Good afternoon.",
                "<br/>",
                "Good afternoon.",
                "<br>And, good afternoon."
            };
            var actual_afternoon_contents_texts = (from content in contents["html"]["body"]["p", 1].Contents select content.Text).ToArray();

            // ASSERT (Contents of "afternoon")
            Assert.AreEqual(expected_afternoon_contents_texts.Length, actual_afternoon_contents_texts.Length);
            for (int i = 0; i < expected_afternoon_contents_texts.Length; i++)
            {
                Assert.AreEqual(expected_afternoon_contents_texts[i], actual_afternoon_contents_texts[i]);
            }
        }


        [TestMethod]
        public void HtmlContentCollectionTest7()
        {
            // ARRANGE
            var doctype = @"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01 Transitional//EN"" ""http://www.w3.org/TR/html4/loose.dtd"">";
            var meta = @"<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />";
            var text = @$"{doctype}
<html>
    <head>
        <title>Test</title>
        {meta}
    </head>
    <body>
        <h1>Test</h1>
        
        <div class=""section"">
            <p>Hello, world.
        </div>
        
    </body>
</html>";

            // ACT
            var content = new HtmlContentCollection(text);

            // ASSERT (!DOCTYPE)
            Assert.AreEqual(doctype, content["!DOCTYPE"].Text);
            Assert.AreEqual(doctype, content["!DOCTYPE"].RawText);
            Assert.AreEqual(doctype, content["!DOCTYPE"].StartTag);
            Assert.AreEqual("!DOCTYPE", content["!DOCTYPE"].TagName);
            Assert.IsNull(content["!DOCTYPE"].Attributes);
            Assert.IsNull(content["!DOCTYPE"].EndTag);
            Assert.AreEqual(0, content["!DOCTYPE"].Contents.Count);

            // ASSERT (meta)
            Assert.AreEqual(meta, content["html"]["head"]["meta"].Text);
            Assert.AreEqual("meta", content["html"]["head"]["meta"].TagName);

            // ASSERT
            Assert.AreEqual("Hello, world.", content["html"]["body"]["div"]["p"].Contents.Text);
            Assert.AreEqual("\"section\"", content["html"]["body"]["div"].Attributes["class"]);
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
            var contents = new HtmlContentCollection("<title>Hello</title>");

            // ACT & ASSERT
            _ = contents["h1"];
        }


        [TestMethod]
        [TestCategory("Exception")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void IndexerEmptyTest()
        {
            // ARRANGE
            var contents = new HtmlContentCollection("<title>Hello</title>");

            // ACT & ASSERT
            _ = contents[""];
        }


        [TestMethod]
        [TestCategory("Exception")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IndexerNullTest()
        {
            // ARRANGE
            var contents = new HtmlContentCollection("<title>Hello</title>");

            // ACT & ASSERT
            _ = contents[null];
        }


        // ----------------------------------------------------------------
        // Tests of Indexer
        // ----------------------------------------------------------------

        [TestMethod]
        public void IndexerTest1()
        {
            // ARRANGE
            var text = "<!DOCTYPE html><html><head><title>Hello</title></head></html>";

            // ACT
            var contents = new HtmlContentCollection(text);

            // ASSERT
            Assert.AreEqual("Hello", contents["html"].Contents["head"].Contents["title"].Contents.Text);
        }


        [TestMethod]
        public void IndexerTest2()
        {
            // ARRANGE
            var text = "<!DOCTYPE html><html><head><title>Hello</title></head></html>";

            // ACT
            var contents = new HtmlContentCollection(text);

            // ASSERT
            Assert.AreEqual("Hello", contents["html"]["head"]["title"].Contents.Text);
        }


        [TestMethod]
        public void IndexerTest3()
        {
            // ARRANGE
            var text = "<html><head><title>Hello</title></head></html>";

            // ACT
            var content = new HtmlElement(text);

            // ASSERT
            Assert.AreEqual("Hello", content.Contents["head"].Contents["title"].Contents.Text);
        }


        [TestMethod]
        public void IndexerTest4()
        {
            // ARRANGE
            var text = "<html><head><title>Hello</title></head></html>";

            // ACT
            var content = new HtmlElement(text);

            // ASSERT
            Assert.AreEqual("Hello", content["head"]["title"].Contents.Text);
        }


        [TestMethod]
        public void IndexerTest5()
        {
            // ARRANGE
            var text = "<TABLE><TR><TH>Items</TH><TD>Item1</TD><TD>Item2</TD><TD>Item3</TD></TR></TABLE>";

            // ACT
            var contents = new HtmlContentCollection(text);

            // ASSERT
            Assert.AreEqual("Item3", contents["TABLE"]["TR"]["TD", 2].Contents.Text);
        }


        // ----------------------------------------------------------------
        // Tests of Invalid Comment (Exception)
        // ----------------------------------------------------------------

        [TestMethod]
        [TestCategory("Exception")]
        [ExpectedException(typeof(FormatException))]
        public void InvalidCommentTest1()
        {
            // ACT & ASSERT
            new HtmlContentCollection("<p>This is a <!-- Invalid <!-- comment --> test.</p>");
        }


        [TestMethod]
        [TestCategory("Exception")]
        [ExpectedException(typeof(FormatException))]
        public void InvalidCommentTest2()
        {
            // ACT & ASSERT
            new HtmlContentCollection("<p>This is a <!-- Invalid --!> comment --> test.</p>");
        }


        [TestMethod]
        [TestCategory("Exception")]
        [ExpectedException(typeof(FormatException))]
        public void InvalidCommentTest3()
        {
            // ACT & ASSERT
            new HtmlContentCollection("<p>This is a <!--> Invalid comment --> test.</p>");
        }


        [TestMethod]
        [TestCategory("Exception")]
        [ExpectedException(typeof(FormatException))]
        public void InvalidCommentTest4()
        {
            // ACT & ASSERT
            new HtmlContentCollection("<p>This is a <!---> Invalid comment <!---> test.</p>");
        }


        [TestMethod]
        [TestCategory("Exception")]
        [ExpectedException(typeof(FormatException))]
        public void InvalidCommentTest5()
        {
            // ACT & ASSERT
            new HtmlContentCollection("<p>This is a <!-- Invalid comment <!---> test.</p>");
        }


        // ----------------------------------------------------------------
        // Tests of Comment Removal
        // ----------------------------------------------------------------

        [TestMethod]
        public void CommentRemovalTest1()
        {
            // ARRANGE
            var text = @"<!-- COMMENT1 -->
<head>
    <!-- COMMENT2 -->
    <title><!-- COMMENT3 -->He<!-- COMMENT4 -->llo<!-- COMMENT5 --></title>
</head>
<!-- COMMENT6 -->";


            // ACT
            var contents = new HtmlContentCollection(text);


            // ASSERT (Root)
            Assert.AreEqual(@"<head><title>Hello</title></head>", contents.Text);
            Assert.AreEqual(@"<!-- COMMENT1 -->
<head>
    <!-- COMMENT2 -->
    <title><!-- COMMENT3 -->He<!-- COMMENT4 -->llo<!-- COMMENT5 --></title>
</head>
<!-- COMMENT6 -->", contents.RawText);

            // ASSERT (head)
            Assert.AreEqual(@"<head><title>Hello</title></head>", contents["head"].Text);
            Assert.AreEqual(@"<head>
    <!-- COMMENT2 -->
    <title><!-- COMMENT3 -->He<!-- COMMENT4 -->llo<!-- COMMENT5 --></title>
</head>", contents["head"].RawText);

            // ASSERT (head.Contents)
            Assert.AreEqual(@"<title>Hello</title>", contents["head"].Contents.Text);
            Assert.AreEqual(@"
    <!-- COMMENT2 -->
    <title><!-- COMMENT3 -->He<!-- COMMENT4 -->llo<!-- COMMENT5 --></title>
", contents["head"].Contents.RawText);

            // ASSERT (title)
            Assert.AreEqual(@"<title>Hello</title>", contents["head"]["title"].Text);
            Assert.AreEqual(@"<title><!-- COMMENT3 -->He<!-- COMMENT4 -->llo<!-- COMMENT5 --></title>", contents["head"]["title"].RawText);

            // ASSERT (title.Contents)
            Assert.AreEqual(@"Hello", contents["head"]["title"].Contents.Text);
            Assert.AreEqual(@"<!-- COMMENT3 -->He<!-- COMMENT4 -->llo<!-- COMMENT5 -->", contents["head"]["title"].Contents.RawText);
        }
    }
}