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
using System.Diagnostics;              // for Debug
using System.IO;                       // for TextReader
using System.Text;
using System.Text.RegularExpressions;  // for Regex

using BUILDLet.Standard.Diagnostics;           // for DebugInfo
using BUILDLet.Standard.Utilities.Properties;  // for Resources

namespace BUILDLet.Standard.Utilities
{
    /// <summary>
    /// 簡易的な HTML パーサーを実装します。
    /// </summary>
    public static class HtmlSyntaxParser
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
        // (None)


        // ----------------------------------------------------------------------------------------------------
        // Public, Protected Method(s)
        // ----------------------------------------------------------------------------------------------------
        // (None)


        // ----------------------------------------------------------------------------------------------------
        // Static Method(s)
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// 指定された文字列からコメントを取り除きます。
        /// </summary>
        /// <param name="text">
        /// 元の文字列
        /// </param>
        /// <returns>
        /// <paramref name="text"/> からコメントが取り除かれた文字列
        /// </returns>
        public static string RemoveComment(string text) => new Regex("<!--(.*?)-->", RegexOptions.Singleline).Replace(text, "");


        /// <summary>
        /// <inheritdoc cref="HtmlSyntaxParser.Read(string, Encoding)"/>
        /// </summary>
        /// <param name="path">
        /// <inheritdoc cref="HtmlSyntaxParser.Read(string, Encoding)"/>
        /// </param>
        /// <returns>
        /// <inheritdoc cref="HtmlSyntaxParser.Read(string, Encoding)"/>
        /// </returns>
        public static HtmlContentCollection Read(string path) => HtmlSyntaxParser.Read(path, Encoding.UTF8);


        /// <summary>
        /// 入力ファイルの文字列を HTML として構文解析します。
        /// </summary>
        /// <param name="path">
        /// 入力ファイルのパスを指定します。
        /// </param>
        /// <param name="encoding">
        /// 入力ファイルのエンコーディングを指定します。
        /// 既定のエンコーディングは <see cref="Encoding.UTF8"/> です。
        /// </param>
        /// <returns>
        /// 入力ファイルの文字列を HTML として構文解析した結果を HTML コンテンツのコレクションとして返します。
        /// </returns>
        public static HtmlContentCollection Read(string path, Encoding encoding) =>
            HtmlSyntaxParser.Parse(File.ReadAllText(path ?? throw new ArgumentNullException(nameof(path)), encoding ?? Encoding.UTF8));


        /// <summary>
        /// 入力文字列を HTML として構文解析します。
        /// </summary>
        /// <param name="text">
        /// 入力文字列
        /// </param>
        /// <returns>
        /// 入力文字列を HTML として構文解析した結果を HTML コンテンツのコレクションとして返します。
        /// </returns>
        public static HtmlContentCollection Parse(string text) => new HtmlContentCollection(text ?? throw new ArgumentNullException(nameof(text)));
    }
}
