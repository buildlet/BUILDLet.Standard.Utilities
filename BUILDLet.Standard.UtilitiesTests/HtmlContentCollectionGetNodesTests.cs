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
using System.Diagnostics;  // for Debug

namespace BUILDLet.Standard.Utilities.Tests
{
    [TestClass]
    public class HtmlContentCollectionGetNodesTests
    {
        // ----------------------------------------------------------------
        // Tests of GetNode Method
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
</html>", "/html/head/title", "Hello")]
        [DataRow(@"<!DOCTYPE html>
<html>
    <head>
        <title>Hello</title>
    </head>
    <body>
        <p>Hello, world.
    </body>
</html>", "html/head/title", "Hello")]
        [DataRow(@"<!DOCTYPE html>
<html>
    <head>
        <title>Greeting</title>
    </head>
    <body>
        <table>
            <tr>
                <th>Image</th>
                <th>Greetnig</th>
            </tr>
            <tr>
                <td>Good <a href=""./morning.html"">Morning</a>.</td>
            </tr>
            <tr>
                <td>Good <a href=""./afternoon.html"">Afternoon</a>.</td>
            </tr>
            <tr>
                <td>Good <a href=""./evening.html"">Evening</a>.</td>
            </tr>
        </table>
    </body>
</html>", "/html/body/table/tr/td/a[@href=\"./afternoon.html\"]", "Afternoon")]
        public void GetNodeMethodTest(string input, string xpath, string text)
        {
            // ARRANGE: NEW Content(s)
            var contents = new HtmlContentCollection(input);

            // Output Blank
            Debug.WriteLine("");

            // ACT
            var nodes = contents.GetNodes(xpath) as List<HtmlContent>;

            // ASSERT
            Assert.AreEqual(text, nodes[0].Contents.Text);
        }
    }
}