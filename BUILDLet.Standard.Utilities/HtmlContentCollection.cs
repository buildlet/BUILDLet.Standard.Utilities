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
    /// <summary>
    /// HTML コンテンツ (<see cref="HtmlContent"/>) のコレクションを実装します。
    /// </summary>
    /// <remarks>
    /// コレクションの要素は <see cref="HtmlElement"/> か <see cref="HtmlText"/> いずれかです。
    /// </remarks>
    public partial class HtmlContentCollection : List<HtmlContent>
    {
        // ----------------------------------------------------------------------------------------------------
        // Private Field(s)
        // ----------------------------------------------------------------------------------------------------

        // RegEx Option
        private const RegexOptions RegexOption = RegexOptions.IgnoreCase | RegexOptions.Singleline;


        // ----------------------------------------------------------------------------------------------------
        // Constructor(s)
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// <see cref="HtmlContentCollection"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        protected HtmlContentCollection() : base() { }


        /// <summary>
        /// 文字列から <see cref="HtmlContentCollection"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="text">
        /// ソースの文字列
        /// </param>
        public HtmlContentCollection(string text) : this()
        {
            // NEW input text
            var input = new StringBuilder(text);

            // for End Tag
            string endTag = null;

            // NEW Contents
            var contents = HtmlContentCollection.Parse(ref input, null, ref endTag);

            // Validation(s)
            if (input.Length != 0)
            {
                // ERROR
                throw new FormatException(Resources.HtmlElementInvalidFormatErrorMessage);
            }

            // ADD Content(s)
            this.AddRange(contents);
        }


        // ----------------------------------------------------------------------------------------------------
        // Public, Protected Properties
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// このインスタンスのソースの文字列
        /// </summary>
        public string RawText => string.Concat(this.ConvertAll(content => content.RawText));


        /// <summary>
        /// このインスタンスに含まれる全てのコンテンツの <see cref="HtmlContent.Text"/> を連結した文字列を取得します。
        /// </summary>
        public string Text => string.Concat(this.ConvertAll(content => content.Text));


        // ----------------------------------------------------------------------------------------------------
        // Public, Protected Indexer(s)
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// このインスタンスに含まれるコンテンツ (<see cref="HtmlContent"/>) の中から、
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
        public HtmlContent this[string name, int index = 0]
            => this.FindAll(content => string.Compare(content.TagName, name ?? throw new ArgumentNullException(nameof(name)), StringComparison.OrdinalIgnoreCase) == 0)?[index];


        // ----------------------------------------------------------------------------------------------------
        // Public, Protected Method(s)
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// このインスタンスを表す文字列を返します。
        /// </summary>
        /// <returns>
        /// このインスタンスの <see cref="HtmlContentCollection.Text"/>
        /// </returns>
        public override string ToString() => this.Text;


        // ----------------------------------------------------------------------------------------------------
        // Static Method(s)
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// 入力文字列を HTML として構文解析します。
        /// </summary>
        /// <param name="input">
        /// 入力文字列を指定します。<br/>
        /// また、入力文字列が最後まで構文解析されなかった場合は、このメソッドからの復帰時に、構文解析されていない文字列が格納されます。
        /// 入力文字列が最後まで構文解析された場合は <c>null</c> が設定されます。
        /// </param>
        /// <param name="startTag">
        /// 入力文字列よりも前に、対応する終了タグが出現していない開始タグ (閉じていない開始タグ) がある場合に、その開始タグの文字列を設定します。
        /// 通常は <c>null</c> を設定してください。
        /// </param>
        /// <param name="endTag">
        /// </param>
        /// <returns>
        /// 入力文字列を HTML として構文解析した結果を返します。
        /// <note type="important">
        /// このメソッドからの復帰時に、<paramref name="input"/> に <c>null</c> ではない文字列が格納されている場合は、入力文字列は最後まで構文解析されていません。
        /// その場合の構文解析されていない文字列は <paramref name="input"/> に格納されています。
        /// </note>
        /// </returns>
        /// <remarks>
        /// <note type="important">
        /// 対応する終了タグのない HTML 要素は、その種類にかかわらず、終了タグが省略されたものとして解釈します。
        /// </note>
        /// </remarks>
        private static HtmlContentCollection Parse(ref StringBuilder input, string startTag, ref string endTag)
        {
#if DEBUG
            Debug.WriteLine("[START]", DebugInfo.ShortName);
#endif

            // Validation (Null Check)
            if (input == null) { throw new ArgumentNullException(nameof(input)); }

            // Empty Validation is NOT reuqired because Empty input is allowed.
            // if (input.Length == 0) { throw new ArgumentException(Resources.InvalidArgumentErrorMessage, nameof(input)); }

            // NEW parsing Content(s)
            var contents = new HtmlContentCollection();

            // NEW RawTexts for parsing Content(s)
            var raw_texts = new StringBuilder();

            // for RegEx
            Match match;
            

            // Local Function:
            // Update parsing Content(s)
            void updateContents(string contentText, ref StringBuilder inputText, HtmlContentType type)
            {
                // Check WhiteSpace
                if (Regex.IsMatch(contentText, @"^[\t\r\n ]+$", RegexOption))
                {
                    // ADD WhiteSpace (NON-HTML)
                    contents.Add(new HtmlWhiteSpace(contentText));
                }
                else
                {
                    // ADD Content (if NOT Empty)

                    switch (type)
                    {
                        case HtmlContentType.HtmlElement:

                            // ADD Content (HTML)
                            contents.Add(new HtmlElement(contentText));
                            break;

                        case HtmlContentType.HtmlText:

                            // ADD Content (NON-HTML)
                            contents.Add(new HtmlText(contentText));
                            break;

                        case HtmlContentType.HtmlComment:

                            // ADD Comment (NON-HTML)
                            contents.Add(new HtmlComment(contentText));
                            break;

                        default:
                            break;
                    }
                }

                // UPDATE RawTexts for parsing Content(s)
                raw_texts.Append(contentText);

                // REMOVE content text (= Heading Tag) from current text
                inputText.Remove(0, contentText.Length);
            }


            // MAIN LOOP
            while (input.Length > 0)
            {
                // Check Comment
                if ((match = Regex.Match(input.ToString(), "^<!--.*?-->", RegexOption)).Success)
                {
                    // Validation
                    if (Regex.IsMatch(match.Value, "^<!--((>|->).*|.*(<!--|--!>).*|.*<!-)-->$", RegexOption))
                    {
                        // ERROR
                        throw new FormatException(Resources.HtmlElementInvalidFormatErrorMessage);
                    }

                    // UPDATE rawText and inputText (parsing Content(s) is NOT updated.)
                    updateContents(match.Value, ref input, HtmlContentType.HtmlComment);

#if DEBUG
                    Debug.WriteLine($"COMMENT: \"{match.Value}\" is removed.", DebugInfo.ShortName);
#endif
                }


                // Check !DOCTYPE
                if ((match = Regex.Match(input.ToString(), @"^<!DOCTYPE[\t\r\n ]+html([\t\r\n ]+(PUBLIC|SYSTEM)[\t\r\n ]*[^<>]*)?[\t\r\n ]*>", RegexOption)).Success)
                {
                    // !DOCTYPE is found;

                    // UPDATE parsing Content(s) (HTML)
                    updateContents(match.Value, ref input, HtmlContentType.HtmlElement);
                }


                // Check Heading Tag
                if (!(match = Regex.Match(input.ToString(), "^<[!/]?[^<>]+>", RegexOption)).Success)
                {
                    // Any Heading Tag does NOT exist;

                    // GET text before any tag
                    if ((match = Regex.Match(input.ToString(), "^[^<>]*<", RegexOption)).Success)
                    {
                        // There are some text before "<"

                        // UPDATE parsing Content(s) (NON-HTML)
                        updateContents(match.Value.Substring(0, match.Value.Length - 1), ref input, HtmlContentType.HtmlText);
                    }
                    else if (Regex.IsMatch(input.ToString(), "^[^<>]*$", RegexOption))
                    {
                        // There are NO any tags.

                        // UPDATE parsing Content(s) (NON-HTML)
                        updateContents(input.ToString(), ref input, HtmlContentType.HtmlText);
                    }
                    else
                    {
                        // ERROR
                        throw new FormatException(Resources.HtmlElementInvalidFormatErrorMessage);
                    }
                }
                else
                {
                    // Some Heading Tag exists;

                    // GET Heading Tag
                    var heading_tag = match.Value;

                    // GET Tag Name (from Heading Tag)
                    var tag_name = string.IsNullOrEmpty(heading_tag) ? null : HtmlElement.GetTagName(heading_tag);

                    // SET RegEx Pattern in Start Tag & End Tag
                    var pattern_in_startTag = $"!?{tag_name}+([\t\r\n ]+[0-9A-Za-z:.-]+(=[\"']?[^\"'=<>]*[\"']?)*)*[\t\r\n ]*";
                    var pattern_in_endTag = $"{tag_name}[\t\r\n ]*";

                    // Check Heading Tag
                    if (Regex.IsMatch(heading_tag, $"^<{pattern_in_startTag}/>$", RegexOption))
                    {
                        // Heading Tag is Self-Closing Start Tag;

                        // UPDATE parsing Content(s) (HTML)
                        updateContents(heading_tag, ref input, HtmlContentType.HtmlElement);
                    }
                    else if (Regex.IsMatch(heading_tag, $"^<{pattern_in_startTag}>$", RegexOption))
                    {
                        // Heading Tag is Start Tag, NOT Self-Closing;

                        // Check text following Heading Tag
                        if ((match = Regex.Match(input.ToString(), $"^<{pattern_in_startTag}>[^<>]*?</{pattern_in_endTag}>", RegexOption)).Success)
                        {
                            // Appropriate End Tag is neighbored;

                            // UPDATE parsing Content(s) (HTML)
                            updateContents(match.Value, ref input, HtmlContentType.HtmlElement);
                        }
                        else
                        {
                            // Appropriate End Tag is NOT neighbored;

                            // DO NOT ADD Content

                            // DO NOT UPDATE RawText for parsing Content(s)

                            // REMOVE content text (= Heading Tag) from current text
                            input.Remove(0, heading_tag.Length);

                            // **************************************************
                            //  RECURSE: ADD Content(s); to be Child or Siblings
                            // **************************************************
                            var subsequents = HtmlContentCollection.Parse(ref input, heading_tag, ref endTag);

                            // ADD subsequent Content(s) to parsing Content(s)
                            subsequents.ForEach(subsequent => contents.Add(subsequent));

                            // UPDATE RawTexts for parsing Content(s)
                            raw_texts.Append(subsequents.RawText);
                        }
                    }
                    else if ((match = Regex.Match(input.ToString(), $"^</{pattern_in_endTag}>", RegexOption)).Success)
                    {
                        // Heading Tag is End Tag;

                        // GET End Tag
                        endTag = match.Value;

                        // REMOVE content text (= End Tag) from current text
                        input.Remove(0, endTag.Length);
                    }
                    else
                    {
                        // ERROR
                        throw new FormatException(Resources.HtmlElementInvalidFormatErrorMessage);
                    }
                }


                // Check End Tag
                if (!string.IsNullOrEmpty(endTag))
                {
                    // Check Tag Name
                    if (string.Compare(HtmlElement.GetTagName(startTag), HtmlElement.GetTagName(endTag), StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        // Tag Name matches;

                        // NEW Child Content
                        var child = new HtmlElement(startTag, contents, endTag);

                        // RESET End Tag
                        endTag = null;

                        // ********
                        //  RETURN
                        // ********
                        return new HtmlContentCollection() { child };
                    }
                    else
                    {
                        // Tag Name Does NOT match;

                        // UPDATE RawTexts for parsing Content(s)
                        raw_texts.Insert(0, startTag);

                        // UPDATE or INSERT 1st Content to parsing Content(s)
                        var first_element_index = contents.FindIndex(content => content.Type == HtmlContentType.HtmlElement);
                        var first_text_index = contents.FindIndex(content => content.Type == HtmlContentType.HtmlText);
                        if ((contents.Count < 1) || (first_element_index > -1) && (first_element_index < first_text_index))
                        {
                            // INSERT Content (HTML)
                            contents.Insert(0, new HtmlElement(startTag));
                        }
                        else
                        {
                            // UPDATE Content from NON-HTML to HTML
                            contents[0] = new HtmlElement(startTag + contents[0].RawText);
                        }

                        // ********
                        //  RETURN
                        // ********
                        return contents;
                    }
                }
            }
            // MAIN LOOP


            // Validation
            if (!string.IsNullOrEmpty(startTag) || !string.IsNullOrEmpty(endTag))
            {
                // ERROR
                throw new FormatException(Resources.HtmlElementInvalidFormatErrorMessage);
            }

#if DEBUG
            Debug.WriteLine("[END]", DebugInfo.ShortName);
#endif
            // RETURN
            return contents;
        }
    }
}
