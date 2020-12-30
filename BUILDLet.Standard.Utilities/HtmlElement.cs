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
    /// <summary>
    /// HTML 要素を表します。
    /// </summary>
    public partial class HtmlElement : HtmlContent
    {
        // ----------------------------------------------------------------------------------------------------
        // Private Field(s)
        // ----------------------------------------------------------------------------------------------------
        // (None)


        // ----------------------------------------------------------------------------------------------------
        // Constructor(s)
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// <see cref="HtmlElement"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="text">
        /// この HTML 要素全体の文字列
        /// </param>
        public HtmlElement(string text)
        {
#if DEBUG
            Debug.WriteLine("[START]", DebugInfo.ShortName);
#endif

            // Validation (Null Check)
            if (text is null) { throw new ArgumentNullException(nameof(text)); }


            // GET Start Tag
            this.StartTag = HtmlElement.GetStartTag(text);
#if DEBUG
            Debug.WriteLine("Start Tag = " + (this.StartTag is null ? "(null)" : $"\"{this.StartTag}\""), DebugInfo.ShortName);
#endif


            // GET Tag Name, End Tag and Attribute(s)
            if (string.IsNullOrEmpty(this.StartTag))
            {
                // Start Tag is missing.

                // GET Attribute (null)
                this.Attributes = null;

                // GET End Tag (or null)
                this.EndTag = HtmlElement.GetEndTag(text);

                // GET Tag Name (from End Tag) (or null)
                this.TagName = HtmlElement.GetTagName(this.EndTag);
#if DEBUG
                Debug.WriteLine($"Tag Name = " + (this.TagName is null ? "(null)" : $"\"{this.TagName}\""), DebugInfo.ShortName);
#endif
            }
            else
            {
                // Start Tag exists.

                // GET Tag Name (from Start Tag)
                this.TagName = HtmlElement.GetTagName(this.StartTag);
#if DEBUG
                Debug.WriteLine($"Tag Name = " + (this.TagName is null ? "(null)" : $"\"{this.TagName}\""), DebugInfo.ShortName);
#endif

                // GET Attribute(s)
                this.Attributes = HtmlElement.GetAttributes(this.StartTag);

                // GET End Tag
                this.EndTag = HtmlElement.GetEndTag(text);

                // Validation (Tag Name in End Tag)
                if (!string.IsNullOrEmpty(this.EndTag) && string.Compare(HtmlElement.GetTagName(this.EndTag), this.TagName, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    // ERROR
                    throw new FormatException(Resources.HtmlElementInvalidFormatErrorMessage);
                }
            }


            // Text for Content(s) to be parsed
            var content_text = text?.Substring(
                string.IsNullOrEmpty(this.StartTag) ? 0 : this.StartTag.Length,
                text.Length - (string.IsNullOrEmpty(this.StartTag) ? 0 : this.StartTag.Length) - (string.IsNullOrEmpty(this.EndTag) ? 0 : this.EndTag.Length));

            // GET Content(s)
            if (!string.IsNullOrEmpty(this.StartTag) && Regex.IsMatch(this.StartTag, "/>$", RegexOptions.Singleline))
            {
                // Self-Closing:

                // Validation
                if (!string.IsNullOrEmpty(this.EndTag) || content_text.Length > 0)
                {
                    // ERROR
                    throw new FormatException(Resources.HtmlElementInvalidFormatErrorMessage);
                }

                // Content is null
                this.Contents = null;
            }
            else
            {
                // ********************
                // GET HTML Content(s)
                // ********************
                this.Contents = new HtmlContentCollection(content_text);
            }

#if DEBUG
            Debug.WriteLine("End Tag = " + (this.EndTag is null ? "(null)" : $"\"{this.EndTag}\""), DebugInfo.ShortName);
            Debug.WriteLine("[END]", DebugInfo.ShortName);
#endif
        }


        /// <summary>
        /// <inheritdoc cref="HtmlElement.HtmlElement(string)"/>
        /// </summary>
        /// <param name="startTag">
        /// このインスタンスの開始タグを指定します。
        /// <note type="implement">
        /// 省略する場合は、空文字 (<c>""</c>, <see cref="string.Empty"/>) ではなく <c>null</c> を指定します。
        /// </note>
        /// </param>
        /// <param name="contents">
        /// このインスタンスが内包するコンテンツ (<see cref="HtmlContentCollection"/>) を指定します。
        /// <note type="important">
        /// 開始タグが <c>/></c> で終了している場合のみ <c>null</c> を指定できます。
        /// その他の場合で、コンテンツがない場合は、長さ <c>0</c> の <see cref="HtmlContentCollection"/> を指定してください。
        /// </note>
        /// </param>
        /// <param name="endTag">
        /// このインスタンスの終了タグを指定します。
        /// <note type="implement">
        /// 省略する場合は、空文字 (<c>""</c>, <see cref="string.Empty"/>) ではなく <c>null</c> を指定します。
        /// </note>
        /// </param>
        public HtmlElement(string startTag, HtmlContentCollection contents, string endTag)
        {
#if DEBUG
            Debug.WriteLine("[START]", DebugInfo.ShortName);
#endif

            // Validation (Null Check)
            if (string.IsNullOrWhiteSpace(startTag) && string.IsNullOrWhiteSpace(endTag))
            {
                throw new ArgumentNullException(nameof(startTag));
            }

            // Validation (Empty Check)
            if (startTag != null && startTag.Length == 0) { throw new ArgumentException(Resources.InvalidArgumentErrorMessage, nameof(startTag)); }
            if (endTag != null && endTag.Length == 0) { throw new ArgumentException(Resources.InvalidArgumentErrorMessage, nameof(endTag)); }

            // Validation (Self-closing Start Tag Check)
            if (Regex.IsMatch(startTag, "/>$", RegexOptions.Singleline))
            {
                // Self-closing:

                if (!string.IsNullOrWhiteSpace(endTag))
                {
                    throw new ArgumentException(Resources.InvalidArgumentErrorMessage, nameof(endTag));
                }
                else if (contents != null)
                {
                    throw new ArgumentException(Resources.InvalidArgumentErrorMessage, nameof(contents));
                }
            }
            else
            {
                // NOT Self-closing:

                if (contents is null)
                {
                    throw new ArgumentNullException(nameof(contents));
                }
            }

            // Validation (Tag Name in Start Tag and End Tag)
            if (!string.IsNullOrEmpty(startTag) && !string.IsNullOrEmpty(endTag)
                && string.Compare(HtmlElement.GetTagName(startTag), HtmlElement.GetTagName(endTag), StringComparison.OrdinalIgnoreCase) != 0)
            {
                // ERROR
                throw new FormatException(Resources.HtmlElementInvalidFormatErrorMessage);
            }

            // SET value(s)
            this.StartTag = startTag;
            this.EndTag = endTag;
            this.TagName = !string.IsNullOrEmpty(startTag) ? HtmlElement.GetTagName(startTag) : HtmlElement.GetTagName(endTag);
            this.Attributes = HtmlElement.GetAttributes(startTag);
            this.Contents = contents;

#if DEBUG
            Debug.WriteLine($"Start Tag = " + (this.StartTag is null ? "(null)" : $"\"{this.StartTag}\""), DebugInfo.ShortName);
            Debug.WriteLine($"Contents.Text = " + (this.Contents is null || this.Contents.Text is null ? "(null)" : $"\"{this.Contents.Text}\""), DebugInfo.ShortName);
            Debug.WriteLine($"End Tag = " + (this.EndTag is null ? "(null)" : $"\"{this.EndTag}\""), DebugInfo.ShortName);
            Debug.WriteLine("[END]", DebugInfo.ShortName);
#endif
        }


        // ----------------------------------------------------------------------------------------------------
        // Public, Protected Properties
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// <inheritdoc cref="HtmlContent.Type"/>
        /// </summary>
        public override HtmlContentType Type => HtmlContentType.HtmlElement;


        /// <summary>
        /// <inheritdoc cref="HtmlContent.RawText"/>
        /// </summary>
        public override string RawText => (this.StartTag ?? "") + this.Contents?.RawText + (this.EndTag ?? "");


        /// <summary>
        /// <inheritdoc cref="HtmlContent.Text"/>
        /// </summary>
        public override string Text => (this.StartTag ?? "") + this.Contents?.Text + (this.EndTag ?? "");


        /// <summary>
        /// この HTML 要素の開始タグを取得します。
        /// 開始タグが省略されている場合の値は <c>null</c> です。
        /// </summary>
        public override string StartTag { get; }


        /// <summary>
        /// この HTML 要素の終了タグを取得します。
        /// </summary>
        /// <remarks>
        /// 終了タグが省略されている場合の値は <c>null</c> です。
        /// </remarks>
        public override string EndTag { get; }


        /// <summary>
        /// この HTML 要素のタグ名を取得します。
        /// </summary>
        public override string TagName { get; }


        /// <summary>
        /// この HTML 要素の属性を取得します。
        /// </summary>
        /// <remarks>
        /// <para>
        /// <note type="note">
        /// 属性の値には、シングルクォーテーション (<c>'</c>) またはダブルクォーテーション (<c>"</c>) が含まれます。
        /// </note>
        /// </para>
        /// </remarks>
        public override Dictionary<string, string> Attributes { get; }


        /// <summary>
        /// この HTML 要素が内包しているコンテンツのコレクションを取得します。
        /// </summary>
        /// <remarks>
        /// 開始タグが <c>/></c> で終了している場合のみ <c>null</c> が設定されます。
        /// 開始タグが省略されている場合 (<see cref="HtmlElement.StartTag"/> が <c>null</c> の場合) や
        /// 終了タグが省略されている場合 (<see cref="HtmlElement.EndTag"/> が <c>null</c> の場合) は、
        /// 長さ (<see cref="List{HtmlContent}.Count"/>) が <c>0</c> の <see cref="HtmlContentCollection"/> が設定されます。
        /// </remarks>
        public override HtmlContentCollection Contents { get; }


        // ----------------------------------------------------------------------------------------------------
        // Public, Protected Method(s)
        // ----------------------------------------------------------------------------------------------------
        // (None)


        // ----------------------------------------------------------------------------------------------------
        // Static Method(s)
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// HTML 要素の開始タグまたは終了タグからタグ名を取得します。
        /// </summary>
        /// <param name="tag">
        /// HTML 要素の開始タグまたは終了タグ
        /// </param>
        /// <returns>
        /// HTML 要素のタグ名
        /// <para>
        /// <note type="note">
        /// <c>&lt;!DOCTYPE&lt;</c> タグの場合、<c>!</c> までが含まれた文字列 (<c>!DOCTYPE</c>) を返します。
        /// </note>
        /// </para>
        /// <para>
        /// <note type="note">
        /// <paramref name="tag"/> に <c>null</c> が指定された場合や、タグ名が取得できなかった場合は <c>nul</c> を返します。
        /// </note>
        /// </para>
        /// </returns>
        /// <remarks>
        /// <para>
        /// タグの開始文字 (<c>'&lt;'</c>) とタグ名が正しく配置されていれば、その後の文字列は任意です。
        /// </para>
        /// </remarks>
        public static string GetTagName(string tag)
        {
            // GET
            var results = tag?.Split(new char[] { '<', '/', '\t', '\r', '\n', ' ', '>' }, StringSplitOptions.RemoveEmptyEntries);

            // RETURN
            return (results is null || results.Length == 0) ? null : results[0];
        }


        /// <summary>
        /// HTML 要素全体の文字列から HTML 要素の開始タグを取得します。
        /// </summary>
        /// <param name="text">
        /// HTML 要素全体の文字列
        /// </param>
        /// <returns>
        /// HTML 要素の開始タグ
        /// </returns>
        /// <remarks>
        /// <para>
        /// 開始タグが取得できなかった場合の値は <c>null</c> です。
        /// </para>
        /// <para>
        /// <note type="note">
        /// <paramref name="text"/> に開始タグの文字列を指定して、同じ文字列が返ってくるかどうかを確認することで、開始タグの文字列を検証することができます。
        /// </note>
        /// </para>
        /// </remarks>
        public static string GetStartTag(string text)
        {
            // Check !DOCTYPE
            var match = Regex.Match(text, @"^<!DOCTYPE[\t\r\n ]+html([\t\r\n ]+(PUBLIC|SYSTEM)[\t\r\n ]*[^<>]*)?[\t\r\n ]*>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            if (match.Success)
            {
                // !DOCTYPE is found;

                // RETURN
                return match.Value;
            }
            else
            {
                // !DOCTYPE is NOT found;

                // GET
                match = Regex.Match(text, "^<!?[0-9A-Za-z:.-]+([\t\r\n ]+[0-9A-Za-z:.-]+(=[\"']?[^\"'=<>]*[\"']?)*)*[\t\r\n ]*/?>", RegexOptions.Singleline);

                // RETURN
                return string.IsNullOrEmpty(match.Value) ? null : match.Value;
            }
        }


        /// <summary>
        /// HTML 要素全体の文字列から HTML 要素の終了タグを取得します。
        /// </summary>
        /// <param name="text">
        /// HTML 要素全体の文字列
        /// </param>
        /// <returns>
        /// HTML 要素の終了タグ
        /// </returns>
        /// <remarks>
        /// <para>
        /// 終了タグが取得できなかった場合の値は <c>null</c> です。
        /// </para>
        /// <para>
        /// <note type="note">
        /// <paramref name="text"/> に終了タグの文字列を指定して、同じ文字列が返ってくるかどうかを確認することで、終了タグの文字列を検証することができます。
        /// </note>
        /// </para>
        /// </remarks>
        public static string GetEndTag(string text)
        {
            // GET
            var match = Regex.Match(text, "</[0-9A-Za-z:.-]+[\t\r\n ]*>$", RegexOptions.Singleline).Value;

            // RETURN
            return string.IsNullOrEmpty(match) ? null : match;
        }


        /// <summary>
        /// HTML 要素の開始タグから属性を取得します。
        /// </summary>
        /// <param name="startTag">
        /// HTML 要素の開始タグ
        /// </param>
        /// <returns>
        /// HTML 要素の開始タグから取得した属性
        /// <para>
        /// <note type="note">
        /// 属性の値には、シングルクォーテーション (<c>'</c>) またはダブルクォーテーション (<c>"</c>) が含まれます。
        /// </note>
        /// </para>
        /// </returns>
        /// <remarks>
        /// <c>!DOCTYPE</c> タグが指定された場合は <c>null</c> を返します。
        /// </remarks>
        public static Dictionary<string, string> GetAttributes(string startTag)
        {
            // Validation (Null Check)
            if (startTag is null) { throw new ArgumentNullException(nameof(startTag)); }

            // Validation (Start Tag)
            if (HtmlElement.GetStartTag(startTag) != startTag)
            {
                throw new FormatException(Resources.HtmlElementInvalidFormatErrorMessage);
            }


            // GET Tag Name
            var tag_name = HtmlElement.GetTagName(startTag);

            // Check !DOCTYPE
            if (string.Compare(tag_name, "!DOCTYPE", true) == 0)
            {
                // RETURN null
                return null;
            }

            // GET Text of Attribute(s)
            // Ex.) "ABC='123' XYZ='456' xxxxxx", "ABC XYZ xxxxxx", "ABC"
            var attributes_text = startTag.Substring("<".Length + tag_name.Length).TrimStart('\t', '\r', '\n', ' ').TrimEnd('\t', '\r', '\n', ' ', '/', '>');

            if (attributes_text.Length == 0)
            {
                // RETURN null
                return null;
            }
            else
            {
                // NEW attributes
                var attributes = new Dictionary<string, string>();

                // for Getting Attribute(s)
                while (attributes_text.Length > 0)
                {
                    // GET Attribute Name
                    var attrib_name = attributes_text.Split('=', '\t', '\r', '\n', ' ')[0];

                    // UPDATE Attribute Text
                    // Ex.) "ABC='123' XYZ='456' xxxxxx" -> "='123' XYZ='456' xxxxxx"
                    //      or
                    // Ex.) "ABC XYZ xxxxxx" -> "XYZ xxxxxx",
                    //      "ABC" -> ""
                    attributes_text = attributes_text.Substring(attrib_name.Length).TrimStart('\t', '\r', '\n', ' ');

                    // Check type of Attribute(s)
                    if (attributes_text.Length == 0 || attributes_text[0] != '=')
                    {
                        // Empty attribute syntax:
                        // Ex.) <input disabled xxxxxx xxxxxx>

                        // ADD Attribute (Empty attribute syntax)
                        attributes.Add(attrib_name, null);
                    }
                    else
                    {
                        // Unquoted Attribute Value Syntax, Single-Quoted Attribute Value Syntax or Double-Quoted Attribute Value Syntax:
                        //
                        // Ex.) <input value=yes xxxxxx xxxxxx>, <input type='checkbox' xxxxxx xxxxxx> or <input name="be evil" xxxxxx xxxxxx>
                        //                  ^^^^                            ^^^^^^^^^^^                              ^^^^^^^^^^

                        // UPDATE Attribute Text
                        // Ex.) "='123' XYZ='456' xxxxxx" -> "'123' XYZ='456' xxxxxx"
                        attributes_text = attributes_text.TrimStart('=', '\t', '\r', '\n', ' ');

                        // GET Quotation Character (as string)
                        var quote = (attributes_text[0] == '\'' || attributes_text[0] == '"') ? attributes_text[0].ToString() : null;

                        // for Attribute Value
                        string attrib_value;

                        // Check Quotation
                        if (quote == null)
                        {
                            // Unquoted Attribute Value Syntax:
                            //
                            // Ex.) <input value=yes xxxxxx xxxxxx>
                            //                   ^^^

                            // GET Attribute Value
                            attrib_value = attributes_text.Split('\t', '\r', '\n', ' ')[0];

                            // UPDATE Attribute Text
                            // Ex.) "123 XYZ=456 xxxxxx" -> "XYZ=456 xxxxxx"
                            attributes_text = attributes_text.Substring(attrib_value.Length).TrimStart('\t', '\r', '\n', ' ');
                        }
                        else
                        {
                            // Single-Quoted Attribute Value Syntax or Double-Quoted Attribute Value Syntax:
                            //
                            // Ex.) <input type='checkbox' xxxxxx xxxxxx> or <input name="be evil" xxxxxx xxxxxx>
                            //                  ^^^^^^^^^^                               ^^^^^^^^^

                            // GET index of closing quotation (2nd quotation)
                            var endIndex = attributes_text.Substring(1).IndexOf(quote, StringComparison.OrdinalIgnoreCase) + 1;

                            // GET Attribute Value (including Quotation)
                            attrib_value = attributes_text.Substring(0, endIndex + 1);

                            // UPDATE Attribute Text
                            // Ex.) "'123' XYZ='456' xxxxxx" -> "XYZ='456' xxxxxx"
                            attributes_text = attributes_text.Substring(endIndex + 1).TrimStart('\t', '\r', '\n', ' ');
                        }

                        // ADD Attribute
                        attributes.Add(attrib_name, attrib_value);
                    }
                }
#if DEBUG
                var count = 0;
                foreach (var attribute in attributes)
                {
                    Debug.WriteLine($"Attributes[{count++}]: {attribute.Key} = " + (attribute.Value is null ? "(null)" : attribute.Value), DebugInfo.ShortName);
                }
#endif

                // RETURN
                return attributes;
            }
        }
    }
}
