﻿/***************************************************************************************************
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
using System.Text.RegularExpressions;
using BUILDLet.Standard.Diagnostics;           // for DebugInfo
using BUILDLet.Standard.Utilities.Properties;  // for Resources

namespace BUILDLet.Standard.Utilities
{
    /// <summary>
    /// HTML コメントを表します。
    /// </summary>
    public class HtmlComment : HtmlText
    {
        // ----------------------------------------------------------------------------------------------------
        // Private Field(s)
        // ----------------------------------------------------------------------------------------------------
        // (None)


        // ----------------------------------------------------------------------------------------------------
        // Constructor(s)
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// <see cref="HtmlWhiteSpace"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="text">
        /// このインスタンスに含まれるテキスト
        /// </param>
        public HtmlComment(string text) : base(text)
        {
            // Check Syntax
            var match = Regex.Match(text, "^<!--.*?-->", RegexOptions.Singleline);

            // Validation
            if (!match.Success)
            {
                // ERROR
                throw new ArgumentException(Resources.InvalidArgumentErrorMessage);
            }

            // Validation
            if (Regex.IsMatch(match.Value, "^<!--((>|->).*|.*(<!--|--!>).*|.*<!-)-->$", RegexOptions.Singleline))
            {
                // ERROR
                throw new FormatException(Resources.HtmlElementInvalidFormatErrorMessage);
            }
        }


        // ----------------------------------------------------------------------------------------------------
        // Public, Protected Properties
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// <inheritdoc cref="HtmlContent.Type"/>
        /// </summary>
        public override HtmlContentType Type => HtmlContentType.HtmlComment;


        /// <summary>
        /// 常に <see cref="string.Empty"/> を返します。
        /// </summary>
        public override string Text => string.Empty;


        // ----------------------------------------------------------------------------------------------------
        // Public, Protected Method(s)
        // ----------------------------------------------------------------------------------------------------
        // (None)
    }
}
