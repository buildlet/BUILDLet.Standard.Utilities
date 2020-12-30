/***************************************************************************************************
The MIT License (MIT)

Copyright 2019 Daiki Sakamoto

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
using System.Diagnostics;             // for Debug Class
using System.Text.RegularExpressions; // for Regex Class

using BUILDLet.Standard.Diagnostics;  // for DebugInfo Class

[assembly: System.Resources.NeutralResourcesLanguage("en-US")]
namespace BUILDLet.Standard.Utilities
{
    /// <summary>
    /// INI ファイル (初期化ファイル) の 1 ラインを実装します。
    /// </summary>
    public class PrivateProfileLine
    {
        // ----------------------------------------------------------------------------------------------------
        // Private Field(s)
        // ----------------------------------------------------------------------------------------------------

        private string section_name;
        private string key;
        private string value;


        // ----------------------------------------------------------------------------------------------------
        // Constructor(s)
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// <see cref="PrivateProfileLine"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks>
        /// <see cref="PrivateProfileLine.LineType"/> および <see cref="PrivateProfileLine.RawLine"/> には、
        /// それぞれ、既定の値 (<see cref="PrivateProfileLineType.NotInitialized"/> および <c>null</c>)
        /// が設定されます。
        /// </remarks>
        public PrivateProfileLine()
        {
#if false // DEBUG
            Debug.WriteLine("New Line", DebugInfo.ShortName);
#endif
        }


        /// <summary>
        /// <inheritdoc cref="PrivateProfileLine()"/>
        /// </summary>
        /// <param name="line">
        /// INI ファイルの 1 ラインを指定します。
        /// </param>
        /// <remarks>
        /// <paramref name="line"/> に <c>null</c> を指定すると、名前のないセクション行が作成され、
        /// <see cref="PrivateProfileLine.LineType"/>、<see cref="PrivateProfileLine.SectionName"/> および <see cref="PrivateProfileLine.RawLine"/> には、
        /// それぞれ、<see cref="PrivateProfileLineType.Section"/>、<see cref="string.Empty"/> および <c>null</c> が設定されます。
        /// </remarks>
        public PrivateProfileLine(string line) : this()
        {
            // Validation (Null Check):
            // (None)

            // SET RawLine
            this.RawLine = line?.Trim();

            // SET LineType, Section, Key and Value
            if (line is null)
            {
                // for Null Section
                this.LineType = PrivateProfileLineType.Section;
                this.section_name = string.Empty;
                this.key = null;
                this.value = null;
            }
            else
            {
                // for Normal Section

                // Initialize value(s)
                this.LineType = PrivateProfileLineType.Other;
                this.section_name = null;
                this.key = null;
                this.value = null;

                // Remove Comment & Triming
                string body = PrivateProfileLine.Trim(line);

                // Parse Line
                if (!string.IsNullOrWhiteSpace(body))
                {
                    // Check SECTION
                    if (Regex.IsMatch(body, @"^\[.+\]$"))
                    {
                        // SECTION Line:
                        // SECTION was found!

                        // SET SECTION Name
                        this.section_name = body.Substring(1, body.Length - 2).Trim();

                        // SET Line Type
                        this.LineType = PrivateProfileLineType.Section;
                    }
                    else if (Regex.IsMatch(body, @"^\[[ \t]*\]$"))
                    {
                        // Invalid SECTION Line:

                        // ERROR
                        throw new FormatException();
                    }
                    else
                    {
                        // NOT SECTION Line:

                        // Check KEY
                        if (body.Contains(Char.ToString('=')))
                        {
                            // KEY and VALUE:

                            // Validation: No KEY;
                            if (body[0] == '=') { throw new FormatException(); }

                            // SET KEY and VALUE
                            this.key = body.Substring(0, body.IndexOf('=')).Trim();
                            this.value = body.EndsWith("=", StringComparison.OrdinalIgnoreCase) ? string.Empty : body.Substring(body.IndexOf('=') + 1).Trim();
                        }
                        else
                        {
                            // KEY only:

                            // SET KEY and VALUE
                            this.key = body.Trim();
                            this.value = null;
                        }

                        // SET Line Type
                        this.LineType = PrivateProfileLineType.Entry;
                    }
                }
            }
#if DEBUG
            Debug.Write($"Line \"{line}\" (LineType = {this.LineType}", DebugInfo.ShortName);
            Debug.WriteLine(", Section = {0}, Key = {1}, Value = {2})",
                (this.section_name is null) ? "null" : $"\"{this.section_name}\"",
                (this.key is null) ? "null" : $"\"{this.key}\"",
                (this.value is null) ? "null" : $"\"{this.value}\"");
#endif
        }


        // ----------------------------------------------------------------------------------------------------
        // Static Properties
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// コメントの開始を示す記号を表します。
        /// </summary>
        /// <remarks>
        /// 文字は <c>';'</c> です。
        /// </remarks>
        public static char CommentIndicator { get; } = ';';


        // ----------------------------------------------------------------------------------------------------
        // Public, Protected Properties
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// ラインタイプ (<see cref="PrivateProfileLineType"/>) を取得します。
        /// </summary>
        /// <remarks>
        /// 既定の設定は <see cref="PrivateProfileLineType.NotInitialized"/> です。
        /// </remarks>
        public PrivateProfileLineType LineType { get; protected set; } = PrivateProfileLineType.NotInitialized;


        /// <summary>
        /// RAW Line
        /// </summary>
        public string RawLine { get; protected set; }


        /// <summary>
        /// セクションの名前を取得および設定します。
        /// </summary>
        /// <remarks>
        /// <c>null</c> を設定することはできません。
        /// 名前のないセクションのセクション名としてのみ、空文字 (<see cref="string.Empty"/>) を設定することができます。
        /// <para>
        /// <note type="note">
        /// 先頭および末尾のホワイトスペースは取り除かれます。
        /// </note>
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// <see cref="PrivateProfileLineType"/> が <see cref="PrivateProfileLineType.Section"/> ではありません。
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <c>null</c> が指定されました。
        /// </exception>
        public string SectionName
        {
            get
            {
                // Validation (Line Type):
                if (this.LineType != PrivateProfileLineType.Section) { throw new InvalidOperationException(); }

                // RETURN
                return this.section_name;
            }

            set
            {
                // Validation (LineType):
                if (this.LineType != PrivateProfileLineType.NotInitialized && this.LineType != PrivateProfileLineType.Section) { throw new InvalidOperationException(); }

                // Validation (Null Check):
                if (value is null) { throw new ArgumentNullException(nameof(value)); }

                // // Validation (Format): -> White Space is allowed.
                // if (string.IsNullOrWhiteSpace(value)) { throw new FormatException(); }

                // // Validation (Format) -> '[' and ']' is allowed.
                // if (value.Contains("[") || value.Contains("]")) { throw new FormatException(); }


                // UPDATE LineType
                this.LineType = PrivateProfileLineType.Section;

                // Triming
                var trimed_name = value.Trim();

                // UPDATE value
                this.section_name = trimed_name;

                // UPDATE Raw Line
                this.RawLine = string.IsNullOrWhiteSpace(trimed_name) ? null : $"[{trimed_name}]";
            }
        }


        /// <summary>
        /// エントリ (キーと値の組み合わせ) のキーを取得および設定します。
        /// </summary>
        /// <remarks>
        /// <c>null</c> および 空文字 (<see cref="string.Empty"/>) を指定することはできません。
        /// また、<c>"="</c> を含む文字列を指定することはできません。
        /// <para>
        /// <note type="note">
        /// 先頭および末尾のホワイトスペースは取り除かれます。
        /// </note>
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// <see cref="PrivateProfileLineType"/> が <see cref="PrivateProfileLineType.Entry"/> ではありません。
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <c>null</c> が指定されました。
        /// </exception>
        /// <exception cref="FormatException">
        /// 空白 (ホワイトスペース) のみの文字列 または 空文字 (<see cref="string.Empty"/>) が指定されました。
        /// あるいは <c>"="</c> を含む文字列が指定されました。
        /// </exception>
        public string Key
        {
            get
            {
                // Validation (Line Type):
                if (this.LineType != PrivateProfileLineType.Entry) { throw new InvalidOperationException(); }

                // RETURN
                return this.key;
            }

            set
            {
                // Validation (Line Type):
                if (this.LineType != PrivateProfileLineType.NotInitialized && this.LineType != PrivateProfileLineType.Entry) { throw new InvalidOperationException(); }

                // Validation (Null Check):
                if (value is null) { throw new ArgumentNullException(nameof(value)); }

                // Validation (Format):
                if (string.IsNullOrWhiteSpace(value) || value.Contains("=")) { throw new FormatException(); }


                // UPDATE LineType
                this.LineType = PrivateProfileLineType.Entry;

                // Triming
                var body = value.Trim();

                // UPDATE value
                this.key = body;

                // UPDATE Raw Line
                this.RawLine = body + (this.value is null ? "" : $"={this.value}");
            }
        }


        /// <summary>
        /// エントリ (キーと値の組み合わせ) のキーを取得および設定します。
        /// </summary>
        /// <remarks>
        /// <c>null</c> および 空文字 (<see cref="string.Empty"/>) を指定することができます。
        /// 空文字 (<see cref="string.Empty"/>) を指定すると、RAW Line (<see cref="PrivateProfileLine.RawLine"/>) の末尾
        /// (<see cref="PrivateProfileLine.Key"/> の後ろ) に <c>"="</c> が追加されます。
        /// <c>null</c> を指定すると、RAW Line の末尾に <c>"="</c> は追加されません。
        /// <para>
        /// <note type="note">
        /// 文字列の両端のホワイトスペースは取り除かれます。
        /// </note>
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// <see cref="PrivateProfileLineType"/> が <see cref="PrivateProfileLineType.Entry"/> ではありません。
        /// </exception>
        /// <exception cref="ArgumentException">
        /// 不正な文字を含んでいます。
        /// </exception>
        public string Value
        {
            get
            {
                // Validation (Line Type):
                if (this.LineType != PrivateProfileLineType.Entry) { throw new InvalidOperationException(); }

                // RETURN
                return this.value;
            }

            set
            {
                // Validation (Line Type):
                if (this.LineType != PrivateProfileLineType.NotInitialized && this.LineType != PrivateProfileLineType.Entry) { throw new InvalidOperationException(); }

                // Validation (Null Check):
                // (None)

                // // Validation (Format) -> '=' is allowed.
                // if ((value != null) && value.Contains("=")) { throw new FormatException(); }


                // UPDATE LineType
                this.LineType = PrivateProfileLineType.Entry;

                // Triming
                var body = value?.Trim();

                // UPDATE value
                this.value = body;

                // UPDATE Raw Line
                this.RawLine = this.key + (body is null ? "" : $"={body}");
            }
        }


        // ----------------------------------------------------------------------------------------------------
        // Public, Protected Method(s)
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// このインスタンスを表す文字列を返します。
        /// </summary>
        /// <returns>
        /// このインスタンスの <see cref="PrivateProfileLine.RawLine"/>。
        /// </returns>
        public override string ToString() => this.RawLine;


        // ----------------------------------------------------------------------------------------------------
        // Static Method(s)
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// 指定された文字列から、コメントを取り除いて、トリミングします。
        /// </summary>
        /// <param name="line">
        /// 元の文字列
        /// </param>
        /// <returns>
        /// コメントを取り除き、トリミングされた文字列
        /// </returns>
        /// <remarks>
        /// <paramref name="line"/> の中に <see cref="PrivateProfileLine.CommentIndicator"/> を検出すると、
        /// その文字を含むそれ以降の文字列が削除されます。
        /// その後、文字列の両端にあるホワイトスペース および タブ文字 (<c>'\t'</c>) が取り除かれます。
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="line"/> に <c>null</c> が指定されました。
        /// </exception>
        public static string Trim(string line)
        {
            // Validation (Null Check):
            if (line is null) { throw new ArgumentNullException(nameof(line)); }

            var index = line.IndexOf(PrivateProfileLine.CommentIndicator);
            var result = (index < 0 ? line : line.Substring(0, index)).Trim(' ', '\t');

#if DEBUG
            Debug.WriteLineIf(line != result, $"Trim Line \"{line}\" -> \"{result}\"", DebugInfo.ShortName);
#endif

            // RETURN
            return result;
        }
    }
}
