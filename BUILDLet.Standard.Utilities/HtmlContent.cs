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
using System.ComponentModel;  // for INotifyPropertyChanged
using System.Diagnostics;     // for Debug
using System.IO;              // for TextReader
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

using System.Text.RegularExpressions;  // for Regex
using System.Threading;
using BUILDLet.Standard.Diagnostics;  // for DebugInfo
using BUILDLet.Standard.Utilities.Properties;  // for Resources

namespace BUILDLet.Standard.Utilities
{
    /// <summary>
    /// HTML コンテンツを表します。
    /// </summary>
    public abstract class HtmlContent
    {
        // ----------------------------------------------------------------------------------------------------
        // Private Field(s)
        // ----------------------------------------------------------------------------------------------------
        // (None)


        // ----------------------------------------------------------------------------------------------------
        // Constructor(s)
        // ----------------------------------------------------------------------------------------------------
        // (None)


        // ----------------------------------------------------------------------------------------------------
        // Public, Protected Properties
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// このインスタンスの型を示します。
        /// </summary>
        public abstract HtmlContentType Type { get; }


        /// <summary>
        /// このインスタンスのソースの文字列
        /// </summary>
        public abstract string RawText { get; }


        /// <summary>
        /// このインスタンスの整形された文字列
        /// </summary>
        public abstract string Text { get; }


        /// <summary>
        /// このインスタンスが HTML 要素 (<see cref="HtmlElement"/>) だった場合に、この HTML 要素の開始タグを表します。
        /// このインスタンスが HTML 要素ではなかった場合 (<see cref="HtmlText"/> だった場合) の値は <c>null</c> です。
        /// </summary>
        public abstract string StartTag { get; }


        /// <summary>
        /// このインスタンスが HTML 要素 (<see cref="HtmlElement"/>) だった場合に、この HTML 要素の終了タグを表します。
        /// このインスタンスが HTML 要素ではなかった場合 (<see cref="HtmlText"/> だった場合) の値は <c>null</c> です。
        /// </summary>
        public abstract string EndTag { get; }


        /// <summary>
        /// このインスタンスが HTML 要素 (<see cref="HtmlElement"/>) だった場合に、この HTML 要素のタグ名を表します。
        /// このインスタンスが HTML 要素ではなかった場合 (<see cref="HtmlText"/> だった場合) の値は <c>null</c> です。
        /// </summary>
        public abstract string TagName { get; }


        /// <summary>
        /// このインスタンスが HTML 要素 (<see cref="HtmlElement"/>) だった場合に、この HTML 要素の属性を表します。
        /// このインスタンスが HTML 要素ではなかった場合 (<see cref="HtmlText"/> だった場合) の値は <c>null</c> です。
        /// </summary>
        public abstract Dictionary<string, string> Attributes { get; }


        /// <summary>
        /// このインスタンスが HTML 要素 (<see cref="HtmlElement"/>) だった場合に、この HTML 要素に含まれるコンテンツのコレクションを表します。
        /// このインスタンスが HTML 要素ではなかった場合 (<see cref="HtmlText"/> だった場合) の値は <c>null</c> です。
        /// </summary>
        public abstract HtmlContentCollection Contents { get; }


        // ----------------------------------------------------------------------------------------------------
        // Public, Protected Indexer(s)
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// このインスタンスが内包しているコンテンツのコレクション (<see cref="HtmlContent.Contents"/>) の中から、
        /// 指定したタグ名 (<see cref="HtmlContent.TagName"/>) に対応するコンテンツを取得します。
        /// </summary>
        /// <param name="name">
        /// 取得したいコンテンツのタグ名 (<see cref="HtmlContent.TagName"/>) を指定します。
        /// </param>
        /// <param name="index">
        /// 指定されたノードに、同じタグ名のコンテンツが複数ある場合に、インデックスを指定します。
        /// 既定の値は <c>0</c> です。
        /// </param>
        /// <returns>
        /// 指定されたタグ名およびインデックスに該当するコンテンツ
        /// </returns>
        public HtmlContent this[string name, int index = 0] => this.Contents?[name ?? throw new ArgumentNullException(nameof(name)), index];


        // ----------------------------------------------------------------------------------------------------
        // Public, Protected Method(s)
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// このインスタンスを表す文字列を返します。
        /// </summary>
        /// <returns>
        /// このインスタンスの <see cref="HtmlContent.Text"/>
        /// </returns>
        public override string ToString() => this.Text;


        /// <summary>
        /// <inheritdoc cref="HtmlContentCollection.GetNodes(string)"/>
        /// </summary>
        /// <param name="xpath">
        /// <inheritdoc cref="HtmlContentCollection.GetNodes(string)"/>
        /// </param>
        /// <returns>
        /// <inheritdoc cref="HtmlContentCollection.GetNodes(string)"/>
        /// </returns>
        public abstract IEnumerable<HtmlContent> GetNodes(string xpath);
    }
}
