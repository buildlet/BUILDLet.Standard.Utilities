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
using System.Diagnostics;  // for Debug
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions; // for Regex

using BUILDLet.Standard.Diagnostics;  // for DebugInfo

namespace BUILDLet.Standard.Utilities
{
    /// <summary>
    /// HTML 要素に含まれる HTML 要素ではないテキストを表します。
    /// </summary>
    public class HtmlText : HtmlContent
    {
        // ----------------------------------------------------------------------------------------------------
        // Private Field(s)
        // ----------------------------------------------------------------------------------------------------
        // (None)


        // ----------------------------------------------------------------------------------------------------
        // Constructor(s)
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// <see cref="HtmlText"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="text">
        /// このインスタンスに含まれるテキスト
        /// </param>
        public HtmlText(string text)
        {
            // SET Raw Text
            this.RawText = text;
        }


        // ----------------------------------------------------------------------------------------------------
        // Public, Protected Properties
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// <inheritdoc cref="HtmlContent.Type"/>
        /// </summary>
        public override HtmlContentType Type => HtmlContentType.HtmlText;


        /// <summary>
        /// <inheritdoc cref="HtmlContent.RawText"/>
        /// </summary>
        public override string RawText { get; }


        /// <summary>
        /// <inheritdoc cref="HtmlContent.Text"/>
        /// </summary>
        public override string Text =>
            Regex.Replace(Regex.Replace(this.RawText, @"^[\r\n]+[\t ]+|[\r\n]+[\t ]+$|[\r\n]+", "", RegexOptions.Singleline), @"[\t ]+", " ", RegexOptions.Singleline);


        /// <summary>
        /// 値は <c>null</c> です。
        /// </summary>
        public override string StartTag => null;


        /// <summary>
        /// 値は <c>null</c> です。
        /// </summary>
        public override string EndTag => null;


        /// <summary>
        /// 値は <c>null</c> です。
        /// </summary>
        public override string TagName => null;


        /// <summary>
        /// 値は <c>null</c> です。
        /// </summary>
        public override Dictionary<string, string> Attributes => null;


        /// <summary>
        /// 値は <c>null</c> です。
        /// </summary>
        public override HtmlContentCollection Contents => null;


        // ----------------------------------------------------------------------------------------------------
        // Public, Protected Method(s)
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// 何も実行しません。
        /// </summary>
        /// <param name="xpath">
        /// 破棄されます。
        /// </param>
        /// <returns>
        /// 常に <c>null</c> を返します。
        /// </returns>
        public override IEnumerable<HtmlContent> GetNodes(string xpath) => null;
    }
}
