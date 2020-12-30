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
using System.Linq;
using System.Diagnostics;              // for Debug
using System.Text.RegularExpressions;  // for RegEx
using System.ComponentModel.Design;
using System.Globalization;

using BUILDLet.Standard.Diagnostics;           // for DebugInfo
using BUILDLet.Standard.Utilities.Properties;  // for Resources

namespace BUILDLet.Standard.Utilities
{
    /// <summary>
    /// 簡易的な XPath の解析に必要なメソッドを提供します。
    /// </summary>
    public static class XPathSyntaxParser
    {
        /// <summary>
        /// XPath で表現されたロケーションパスをロケーションステップに分割します。
        /// </summary>
        /// <param name="xpath">
        /// ロケーションパス
        /// </param>
        /// <returns>
        /// ロケーションステップ
        /// </returns>
        public static string[] GetNodes(string xpath)
        {
            // Validation (Null Check)
            if (string.IsNullOrWhiteSpace(xpath)) { throw new ArgumentNullException(nameof(xpath)); }

            // GET Text of XPath as StringBuilder
            var xpath_text = new StringBuilder(xpath);

            // GET GUID
            var guid = Guid.NewGuid().ToString();

            // GET Quotation(s)
            var matches = Regex.Matches(xpath, @"('.*?')|("".*?"")", RegexOptions.IgnoreCase);

            // for Quotation(s)
            var originals = new List<string>();
            var tentatives = new List<string>();

            // for Quotation(s)
            for (int i = 0; i < matches.Count; i++)
            {
                // Store Original Quotation
                originals.Add(matches[i].Value);

                // Store Replaced Quotation
                tentatives.Add(guid + $"{i}");

                // Replace Quotation
                xpath_text.Replace(originals[i], tentatives[i]);
#if DEBUG
                Debug.WriteLine($"Replace [{i}] \"{originals[i]}\" -> \"{tentatives[i]}\"", DebugInfo.ShortName);
#endif
            }

            // GET Nodes to be returned
            var nodes = xpath_text.ToString().Split('/');

            // Restore Quotation(s)
            for (int i = 0; i < nodes.Length; i++)
            {
                for (int j = 0; j < matches.Count; j++)
                {
                    if (nodes[i].Contains(tentatives[j]))
                    {
                        // Restore Quotation
                        nodes[i] = nodes[i].Replace(tentatives[j], originals[j]);
#if DEBUG
                        Debug.WriteLine($"Restore [{j}] \"{tentatives[j]}\" -> \"{originals[j]}\"", DebugInfo.ShortName);
#endif
                    }
                }
            }

            // RETURN
            return nodes;
        }


        /// <summary>
        /// XPath で表現されたロケーションステップから述語 (Predicate) を取り出します。
        /// </summary>
        /// <param name="xpath">
        /// ロケーションステップ
        /// </param>
        /// <returns>
        /// 述語 (Predicate) のリスト
        /// </returns>
        public static string[] GetPredicates(string xpath)
        {
            // GET Match(es)
            var matches = Regex.Matches(XPathSyntaxParser.GetNodes(xpath)[0], @"\[.*?\]");

            // GET Predicate(s)
            var predicates = matches.Cast<Match>().ToList().ConvertAll(match => match.Value.Substring("[".Length, match.Length - "[]".Length));

            // RETURN
            return predicates.ToArray();
        }


        /// <summary>
        /// XPath で表現されたロケーションステップの述語 (Predicate) からインデックス番号を取り出します。
        /// </summary>
        /// <param name="xpath">
        /// ロケーションステップ
        /// </param>
        /// <returns>
        /// 指定されたロケーションステップの述語 (Predicate) にインデックス番号が含まれている場合に、そのインデックス番号を返します。
        /// インデックス番号が含まれていない場合は <c>-1</c> を返します。
        /// </returns>
        public static int GetIndex(string xpath)
        {
            // GET index(es)
            var indexes = from predicate
                          in XPathSyntaxParser.GetPredicates(xpath)
                          where int.TryParse(predicate, out _)
                          select int.Parse(predicate, CultureInfo.InvariantCulture);

            // RETURN
            return indexes.Any() ? indexes.First() : -1;
        }
    }
}
