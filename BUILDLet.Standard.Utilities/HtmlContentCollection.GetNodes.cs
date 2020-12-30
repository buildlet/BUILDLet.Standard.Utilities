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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;  // for ObservableCollection
using System.ComponentModel.Design;
using System.Diagnostics;  // for Debug
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;  // for RegEx

using BUILDLet.Standard.Diagnostics;           // for DebugInfo
using BUILDLet.Standard.Utilities.Properties;  // for Resources

namespace BUILDLet.Standard.Utilities
{
    public partial class HtmlContentCollection : List<HtmlContent>
    {
        /// <summary>
        /// このインスタンスから、指定されたノードのコンテンツ (<see cref="HtmlContent"/>) を取り出します。
        /// </summary>
        /// <param name="xpath">
        /// XPath 形式でロケーションパスを指定します。
        /// <note type="important">
        /// 完全な構文 (Unabbreviated Syntax) はサポートしていません。
        /// 省略構文 (Abbreviated Syntax) で指定してください。
        /// </note>
        /// <note type="important">
        /// 軸 (Axes) の指定はサポートしていません。
        /// </note>
        /// <note type="important">
        /// ノードテスト (Node Test) はノードの名前のみをサポートしています。
        /// </note>
        /// <note type="important">
        /// 述語 (Predicate) は <c>[n]</c> および <c>[@&lt;Attribute Name&gt;='&lt;Attribute Value&gt;']</c>
        /// または <c>[@&lt;Attribute Name&gt;="&lt;Attribute Value&gt;"]</c> の形式のみサポートしています。
        /// </note>
        /// </param>
        /// <returns>
        /// 指定されたノードのコンテンツ (<see cref="HtmlContent"/>) のリスト
        /// </returns>
        public IEnumerable<HtmlContent> GetNodes(string xpath)
        {
#if DEBUG
            Debug.WriteLine("[START]", DebugInfo.ShortName);
            Debug.WriteLine($"{nameof(xpath)} = \"{xpath}\"", DebugInfo.ShortName);
#endif

            // Validation (Null Check)
            if (string.IsNullOrWhiteSpace(xpath)) { throw new ArgumentNullException(nameof(xpath)); }

            // Check if target node is Root Node or not
            if (string.Compare(xpath, "/", StringComparison.OrdinalIgnoreCase) == 0 )
            {
                // Target node is Root Node;

#if DEBUG
                Debug.WriteLine("[END] RETURN this", DebugInfo.ShortName);
#endif
                // *************
                //  RETURN this
                // *************
                return this;
            }


            // Target node is NOT Root Node;

            // GET Nodes
            var nodes = XPathSyntaxParser.GetNodes(xpath);
#if DEBUG
            Debug.WriteLine($"{nameof(xpath)} -> {nameof(nodes)}[{nodes.Length}] {{\"{string.Join("\", \"", nodes)}\"}}", DebugInfo.ShortName);
#endif

            // GET Current Node & NEXT Node
            var current = string.IsNullOrEmpty(nodes[0]) ? nodes[1] : nodes[0];


            // UPDATE xpath for NEXT Node
            xpath = xpath.Substring((string.IsNullOrEmpty(nodes[0]) ? 1 : 0) + current.Length);

            // Validation (xpath for NEXT Node)
            if (string.Compare(xpath.Trim(), "/", StringComparison.OrdinalIgnoreCase) == 0)
            {
                // ERROR
                throw new ArgumentException(Resources.XPathNotSupportedExpressionErrorMessage, nameof(xpath));
            }


            // GET Tag Name
            var tag_name = current.Split('[')[0];

            // Validation
            if (string.IsNullOrWhiteSpace(tag_name))
            {
                // ERROR
                throw new ArgumentException(Resources.XPathNotSupportedExpressionErrorMessage, nameof(xpath));
            }

            // GET Content(s) as List<HtmlContent>
            List<HtmlContent> contents = this.FindAll(content => content.TagName == tag_name);


            // GET Index
            var index = XPathSyntaxParser.GetIndex(current);

            // Validation (Out of Range)
            if (index >= contents.Count)
            {
                // ERROR (Out of Range)
                throw new ArgumentOutOfRangeException(nameof(xpath), Resources.XPathNotSupportedExpressionErrorMessage);
            }

            // Check Index
            if (index >= 0 && index < this.Count)
            {
                // UPDATE Content(s)
                contents = new List<HtmlContent> { contents[index] };
            }


            // GET Predicate(s)
            var predicates = XPathSyntaxParser.GetPredicates(current);

            // Check Predicate(s)
            if (predicates.Length > 0)
            {
                // There are some Predicate(s);

                // Check Predicate(s)
                for (int i = 0; i < predicates.Length; i++)
                {
                    if (Regex.IsMatch(predicates[i], "^@.+=('.+'|\".+\")$"))
                    {
                        // Predicate is specifying attribute;

                        // GET Attribute Name & Value
                        var attrib_name = predicates[i].Substring("@".Length, predicates[i].IndexOf('=') - 1);
                        var attrib_value = predicates[i].Substring(predicates[i].IndexOf('=') + 1);

                        // UPDATE Content(s)
                        contents = contents.FindAll(content => string.Compare(content.Attributes[attrib_name], attrib_value, StringComparison.OrdinalIgnoreCase) == 0);
                    }
                    else
                    {
                        // ****************************************
                        //  Predicate is NOT supported expression.
                        // ****************************************

                        // ERROR
                        throw new ArgumentException(Resources.XPathNotSupportedExpressionErrorMessage, nameof(xpath));
                    }
                }
            }


            // Check NEXT Node
            if (string.IsNullOrEmpty(xpath))
            {
                // There is NO NEXT Node;

#if DEBUG
                Debug.WriteLine($"[END] RETURN {string.Join("/", contents.ConvertAll(content => content.Text))}", DebugInfo.ShortName);
#endif
                // ********
                //  RETURN
                // ********
                return contents;
            }


            // NEXT Node exists;

            // NEW Descendant Content(s)
            var descendants = new List<HtmlContent>();

            // GET Node for each Content(s)
            foreach (var content in contents)
            {
                // *********
                //  RECURSE
                // *********
                descendants.AddRange(content.Contents.GetNodes(xpath));
            }

#if DEBUG
            Debug.WriteLine($"[END] RETURN {string.Join("/", descendants.ConvertAll(content => content.Text))}", DebugInfo.ShortName);
#endif
            // ********
            //  RETURN
            // ********
            return descendants;
        }
    }
}
