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

namespace BUILDLet.Standard.Utilities
{
    /// <summary>
    /// <see cref="HtmlContent"/> の型を表します。
    /// </summary>
    public enum HtmlContentType
    {
        /// <summary>
        /// HTML 要素を表します。
        /// </summary>
        HtmlElement,

        /// <summary>
        /// HTML 要素に含まれる HTML 要素ではないテキストを表します。
        /// </summary>
        HtmlText,

        /// <summary>
        /// HTML コメントを表します。
        /// </summary>
        /// <remarks>
        /// コンテンツのタイプが <see cref="HtmlContentType.HtmlComment"/> であるコンテンツの
        /// <see cref="HtmlContent.RawText"/> は <see cref="HtmlContent.Text"/> に含まれません。
        /// </remarks>
        HtmlComment,

        /// <summary>
        /// HTML 要素と HTML 要素の間などにあるホワイトスペースを表します。
        /// </summary>
        /// <remarks>
        /// コンテンツのタイプが <see cref="HtmlContentType.HtmlWhiteSpace"/> であるコンテンツの
        /// <see cref="HtmlContent.RawText"/> は <see cref="HtmlContent.Text"/> に含まれません。
        /// </remarks>
        HtmlWhiteSpace
    }
}
