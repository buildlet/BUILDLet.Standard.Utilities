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

namespace BUILDLet.Standard.Utilities.Tests
{
    [TestClass]
    public class HtmlTextTests
    {
        // ----------------------------------------------------------------
        // Tests of RawText Property
        // ----------------------------------------------------------------

        [DataTestMethod]
        [DataRow(@"Hello, world.")]
        [DataRow(@"Hello, 
world.")]
        public void RawTextTest(string text)
        {
            // ARRANGE
            var content = new HtmlText(text);

            // ACT & ASSERT
            Assert.AreEqual(text, content.RawText);
        }


        // ----------------------------------------------------------------
        // Tests of Text Property
        // ----------------------------------------------------------------

        [DataTestMethod]
        [DataRow(@"Hello, world.", @"Hello, world.")]
        [DataRow(@"Hello, 
world.", @"Hello, world.")]
        [DataRow(@"Hello,
world.", @"Hello,world.")]
        [DataRow(@"Hello,
    world.", @"Hello, world.")]
        public void TextTest(string input, string expected)
        {
            // ARRANGE
            var content = new HtmlText(input);

            // ACT & ASSERT
            Assert.AreEqual(expected, content.Text);
        }


        // ----------------------------------------------------------------
        // Tests of Indexer
        // ----------------------------------------------------------------

        [TestMethod]
        public void IndexerUnmatchTest()
        {
            // ARRANGE
            var element = new HtmlText("Hello");

            // ACT & ASSERT
            Assert.AreEqual(null, element["Bye"]);
        }


        [TestMethod]
        public void IndexerEmptyTest()
        {
            // ARRANGE
            var element = new HtmlText("Hello");

            // ACT & ASSERT
            Assert.AreEqual(null, element[""]);
        }


        [TestMethod]
        public void IndexerNullTest()
        {
            // ARRANGE
            var element = new HtmlText("Hello");

            // ACT & ASSERT
            Assert.AreEqual(null, element[null]);
        }
    }
}
