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
    public class HtmlContentCollectionBaseClassTests
    {
        // ----------------------------------------------------------------
        // Tests of Insert Method
        // ----------------------------------------------------------------

        [TestMethod]
        public void InsertMethodTest()
        {
            var before_text = @"<table>
    <tr>
        <th>Item No.</th>
        <th>Value</th>
    </tr>
    <tr>
        <td>Item 1</td>
        <td>101</td>
    </tr>
    <tr>
        <td>Item 3</td>
        <td>103</td>
    </tr>
</table>";

            var item_text = @"<tr>
        <td>Item 2</td>
        <td>102</td>
    </tr>";

            var after_contents_text = @"
    <tr>
        <th>Item No.</th>
        <th>Value</th>
    </tr>
    <tr>
        <td>Item 1</td>
        <td>101</td>
    </tr><tr>
        <td>Item 2</td>
        <td>102</td>
    </tr>
    <tr>
        <td>Item 3</td>
        <td>103</td>
    </tr>
";
            var after_text = $"<table>{after_contents_text}</table>";

            // ARRANGE: NEW Content(s)
            var table = new HtmlElement(before_text);

            // ACT
            table.Contents.Insert(4, new HtmlElement(item_text));

            // Manual Update is NOT required.
            // table.UpdateRawText();

            // ASSERT
            Assert.AreEqual(after_contents_text, table.Contents.RawText);
            Assert.AreEqual(after_text, table.RawText);
        }


        // ----------------------------------------------------------------
        // Tests of RemoveAt Method
        // ----------------------------------------------------------------

        [TestMethod]
        public void RemoveAtMethodTest()
        {
            var before_contents_text = @"
    <tr>
        <th>Item No.</th>
        <th>Value</th>
    </tr>
    <tr>
        <td>Item 1</td>
        <td>101</td>
    </tr>
    <tr>
        <td>Item 2</td>
        <td>102</td>
    </tr>
    <tr>
        <td>Item 3</td>
        <td>103</td>
    </tr>
";

            var before_text = $@"<table>{before_contents_text}</table>";

            var after_contents_text = @"
    <tr>
        <th>Item No.</th>
        <th>Value</th>
    </tr>
    <tr>
        <td>Item 1</td>
        <td>101</td>
    </tr>
    
    <tr>
        <td>Item 3</td>
        <td>103</td>
    </tr>
";
            var after_text = $"<table>{after_contents_text}</table>";

            // ARRANGE: NEW Content(s)
            var table = new HtmlElement(before_text);

            // ACT
            table.Contents.RemoveAt(5);

            // Manual Update is NOT required.
            // table.UpdateRawText();

            // ASSERT
            Assert.AreEqual(after_contents_text, table.Contents.RawText);
            Assert.AreEqual(after_text, table.RawText);
        }


        // ----------------------------------------------------------------
        // Tests of Add Method
        // ----------------------------------------------------------------

        [TestMethod]
        public void AddMethodTest()
        {
            var before_content_text = @"
    <tr>
        <th>Item No.</th>
        <th>Value</th>
    </tr>
    <tr>
        <td>Item 1</td>
        <td>101</td>
    </tr>
    <tr>
        <td>Item 2</td>
        <td>102</td>
    </tr>
";
            var before_text = $@"<table>{before_content_text}</table>";

            var item_text = @"<tr>
        <td>Item 3</td>
        <td>103</td>
    </tr>";

            var after_contents_text = $"{before_content_text}{item_text}";
            var after_text = $"<table>{after_contents_text}</table>";

            // ARRANGE: NEW Content(s)
            var table = new HtmlElement(before_text);

            // ACT
            table.Contents.Add(new HtmlElement(item_text));

            // Manual Update is NOT required.
            // table.UpdateRawText();

            // ASSERT
            Assert.AreEqual(after_contents_text, table.Contents.RawText);
            Assert.AreEqual(after_text, table.RawText);
        }


        // ----------------------------------------------------------------
        // Tests of Clear Method
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
        public void ClearMethodTest(string text)
        {
            // ARRANGE: NEW Content(s)
            var contents = new HtmlContentCollection(text);

            // ACT
            contents.Clear();

            // Manual Update is NOT required.
            // contents.UpdateRawText();

            // ASSERT
            Assert.AreEqual("", contents.Text);
        }
    }
}