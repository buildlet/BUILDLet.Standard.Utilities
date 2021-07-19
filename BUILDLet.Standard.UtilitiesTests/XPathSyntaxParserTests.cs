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
    public class XPathSyntaxParserTests
    {
        // ----------------------------------------------------------------
        // Tests of GetNodes Method
        // ----------------------------------------------------------------

        // TestData for GetNodesMethodTest
        public static IEnumerable<object[]> GetNodesMethodTestData => new List<object[]>()
        {
            // object[]
            // {
            //    // Input
            //    string,
            //
            //    // Expected
            //    string[]
            // }

            // 1)
            new object[]
            {
                // Input
                "/html/head/title",
            
                // Expected
                new string[] { "", "html", "head", "title" }
            },

            // 2)
            new object[]
            {
                // Input
                "html/head/title",
            
                // Expected
                new string[] { "html", "head", "title" }
            },

            // 3)
            new object[]
            {
                // Input
                "/HTML/FRAMESET[@rows=\"100,*\"]",
            
                // Expected
                new string[] { "", "HTML", "FRAMESET[@rows=\"100,*\"]" }
            },

            // 4)
            new object[]
            {
                // Input
                "/html/body/div[@class=\"100\"]/p/span[@class=\"200\"]",
            
                // Expected
                new string[] { "", "html", "body", "div[@class=\"100\"]", "p", "span[@class=\"200\"]" }
            }
        };

        [DataTestMethod]
        [DynamicData(nameof(GetNodesMethodTestData))]
        public void GetNodesMethodTest(string input, string[] expected)
        {
            // ARRANGE
            // (None)

            // ACT
            var nodes = XPathSyntaxParser.GetNodes(input);

            // ASSERT (Length)
            Assert.AreEqual(expected.Length, nodes.Length);

            // ASSERT
            for (int i = 0; i < nodes.Length; i++)
            {
                Assert.AreEqual(expected[i], nodes[i]);
            }
        }


        // ----------------------------------------------------------------
        // Tests of GetPredicates Method
        // ----------------------------------------------------------------

        // TestData for GetPredicatesMethodTest
        public static IEnumerable<object[]> GetPredicatesMethodTestData => new List<object[]>()
        {
            // object[]
            // {
            //    // Input
            //    string,
            //
            //    // Expected
            //    string[]
            // }

            // 1)
            new object[]
            {
                // Input
                "FRAMESET[@rows=\"100,*\"]",
            
                // Expected
                new string[] { "@rows=\"100,*\"" }
            },

            // 2)
            new object[]
            {
                // Input
                "div[@class=\"100\"][@class=\"200\"]",
            
                // Expected
                new string[] { "@class=\"100\"", "@class=\"200\"" }
            },

            // 3)
            new object[]
            {
                // Input
                "/html/head/title",
            
                // Expected
                Array.Empty<string>()
            }

        };

        [DataTestMethod]
        [DynamicData(nameof(GetPredicatesMethodTestData))]
        public void GetPredicatesMethodTest(string input, string[] expected)
        {
            // ARRANGE
            // (None)

            // ACT
            var predicates = XPathSyntaxParser.GetPredicates(input);

            // ASSERT (Length)
            Assert.AreEqual(expected.Length, predicates.Length);

            // ASSERT
            for (int i = 0; i < predicates.Length; i++)
            {
                Assert.AreEqual(expected[i], predicates[i]);
            }
        }


        // ----------------------------------------------------------------
        // Tests of GetIndex Method
        // ----------------------------------------------------------------

        [DataTestMethod]
        [DataRow("tr[2]", 2)]
        [DataRow("div[@class1=\"100\"][2][@class2=\"200\"]", 2)]
        [DataRow("html", -1)]
        public void GetIndexMethodTest(string input, int expected)
        {
            // ARRANGE
            // (None)

            // ACT
            var index = XPathSyntaxParser.GetIndex(input);

            // ASSERT
            Assert.AreEqual(expected, index);
        }
    }
}