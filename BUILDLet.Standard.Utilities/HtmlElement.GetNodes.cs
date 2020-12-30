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
using System.Collections.Generic;
using System.Text;

using System.Collections.ObjectModel;  // for ReadOnlyDictionary
using System.Text.RegularExpressions;  // for Regex
using System.Diagnostics;              // for Debug

using BUILDLet.Standard.Utilities.Properties;  // for Resources
using BUILDLet.Standard.Diagnostics;           // for DebugInfo
using System.IO;

namespace BUILDLet.Standard.Utilities
{
    public partial class HtmlElement : HtmlContent
    {
        // ----------------------------------------------------------------------------------------------------
        // Public, Protected Method(s)
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// <inheritdoc cref="HtmlContentCollection.GetNodes(string)"/>
        /// </summary>
        /// <param name="xpath">
        /// <inheritdoc cref="HtmlContentCollection.GetNodes(string)"/>
        /// </param>
        /// <returns>
        /// <inheritdoc cref="HtmlContentCollection.GetNodes(string)"/>
        /// </returns>
        public override IEnumerable<HtmlContent> GetNodes(string xpath)
        {
#if DEBUG
            Debug.WriteLine("[START]", DebugInfo.ShortName);
            Debug.WriteLine($"{nameof(xpath)} = \"{xpath}\"", DebugInfo.ShortName);
#endif

            // Validation (Null Check)
            if (string.IsNullOrWhiteSpace(xpath)) { throw new ArgumentNullException(nameof(xpath)); }

            // Check if target node is Root Node or not
            if (string.Compare(xpath, "/", StringComparison.OrdinalIgnoreCase) == 0)
            {
                // Target node is Root Node;

                // *************
                //  RETURN this
                // *************
                return new List<HtmlContent>() { this };
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


            // Validation (Tag Name)
            if (this.TagName != current.Split('[')[0])
            {
#if DEBUG
                Debug.WriteLine("[END] RETURN Empty", DebugInfo.ShortName);
#endif
                // **************
                //  RETURN Empty
                // **************
                return new List<HtmlContent>();
            }


            // Validation (Check if index is specified for element.)
            if (XPathSyntaxParser.GetIndex(current) > -1)
            {
                // ERROR
                throw new ArgumentException(Resources.XPathNotSupportedExpressionErrorMessage, nameof(xpath));
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
                        var attrib_name = predicates[i].Substring("@".Length, predicates[i].IndexOf('='));
                        var attrib_value = predicates[i].Substring(predicates[i].IndexOf('=') + 1);

                        // Validation: Attribute Check
                        if (string.Compare(this.Attributes[attrib_name], attrib_value, StringComparison.OrdinalIgnoreCase) != 0)
                        {
#if DEBUG
                            Debug.WriteLine("[END] RETURN Empty", DebugInfo.ShortName);
#endif
                            // **************
                            //  RETURN Empty
                            // **************
                            return new List<HtmlContent>();
                        }
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
                Debug.WriteLine("[END] RETURN this", DebugInfo.ShortName);
#endif
                // *************
                //  RETURN this
                // *************
                return new List<HtmlContent>() { this };
            }


            // NEXT Node exists;

#if DEBUG
            Debug.WriteLine($"[END] RETURN this.Contents.GetNode(\"{xpath}\")", DebugInfo.ShortName);
#endif
            // ********
            //  RETURN
            // ********
            return this.Contents.GetNodes(xpath);
        }
    }
}
