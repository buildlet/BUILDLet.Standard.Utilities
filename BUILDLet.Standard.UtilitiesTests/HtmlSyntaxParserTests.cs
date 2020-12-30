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
using System.IO;

namespace BUILDLet.Standard.Utilities.Tests
{
    [TestClass]
    public class HtmlSyntaxParserTests
    {
        // ----------------------------------------------------------------
        // Tests of RemoveComment Method
        // ----------------------------------------------------------------

        [DataTestMethod]
        [DataRow("<html>Hello,<!-- This is comment. --> world.</html>", "<html>Hello, world.</html>")]
        [DataRow("<html>Hello, world.</html><!-- This is comment. -->", "<html>Hello, world.</html>")]
        [DataRow("<!-- This is comment. --><html>Hello, world.</html>", "<html>Hello, world.</html>")]
        [DataRow("<html>Hello,<!-- This is comment. --><!-- This is comment. --> world.</html>", "<html>Hello, world.</html>")]
        [DataRow(
@"<html>
    Hello,<!-- This is comment. --> world.
</html>",
@"<html>
    Hello, world.
</html>")]
        [DataRow(
@"<html>
    Hello, world.
    <!-- This is comment. -->
    Hello, world again.
</html>",
@"<html>
    Hello, world.
    
    Hello, world again.
</html>")]
        [DataRow(
@"<html>
    Hello, world.
    <!-- This is comment. -->
    <!-- This is comment. -->
    Hello, world again.
</html>",
@"<html>
    Hello, world.
    
    
    Hello, world again.
</html>")]
        [DataRow(
@"<html>
    Hello, world.
    <!-- This is comment. -->
    Hello, world again.
    <!-- This is comment. -->
</html>",
@"<html>
    Hello, world.
    
    Hello, world again.
    
</html>")]
        public void RemoveCommentMethodTest(string input, string expected)
        {
            Assert.AreEqual(expected, HtmlSyntaxParser.RemoveComment(input));
        }


        // ----------------------------------------------------------------
        // Tests of Parse Method
        // ----------------------------------------------------------------

        [TestMethod]
        public void ParseMethodTest()
        {
            // ARRANGE
            var text = @"<!DOCTYPE html>
<html>
    <head>
        <title>Hello</title>
    </head>
    <body>
        <p>Hello, world.
    </body>
</html>";

            // ACT
            var contents = HtmlSyntaxParser.Parse(text);

            // ASSERT
            Assert.AreEqual("Hello, world.", contents["html"]["body"]["p"].Contents.Text);
        }


        // ----------------------------------------------------------------
        // Tests of Read Method
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
</html>", "Hello, world.", null)]
        [DataRow(@"<!DOCTYPE html>
<html>
    <head>
        <title>Hello</title>
    </head>
    <body>
        <p>こんにちは
    </body>
</html>", "こんにちは", "shift_jis")]
        public void ReadMethodTest(string input, string expected, string encodingName)
        {
            // [NOTE]
            // Nuget Package "System.Text.Encoding.CodePages" is required for "shift_jis" encoding.
            // And, the following code is required.
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);


            // ARRANGE
            var encoding = string.IsNullOrEmpty(encodingName) ? Encoding.UTF8 : Encoding.GetEncoding(encodingName);
            var path = $@"./{nameof(HtmlSyntaxParser)}.{nameof(ReadMethodTest)} {encoding.EncodingName}.html";

            // ARRANGE (Create File)
            File.WriteAllText(path, input, encoding);

            // ACT
            var contents = encoding is null ? 
                HtmlSyntaxParser.Read(path) : 
                HtmlSyntaxParser.Read(path, encoding);

            // ASSERT
            Assert.AreEqual(expected, contents["html"]["body"]["p"].Contents.Text);
        }
    }
}