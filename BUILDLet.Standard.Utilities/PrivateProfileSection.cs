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
using System.Linq;
using System.Diagnostics;             // for Debug Class
using System.IO;                      // for StreamReader Class
using System.Collections.ObjectModel; // for ReadOnlyDictionary Class

using BUILDLet.Standard.Utilities.Properties; // for Resources
using BUILDLet.Standard.Diagnostics;          // for DebugInfo Class

namespace BUILDLet.Standard.Utilities
{
    /// <summary>
    /// INI ファイル (初期化ファイル) のセクションを実装します。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="PrivateProfileSection"/> クラスは、内部に <see cref="PrivateProfileLine"/> クラスのコレクションを保持します。<br/>
    /// セクションの名前は、先頭 (インデックス番号 <c>0</c>) の <see cref="PrivateProfileLine"/> に格納されます。
    /// </para>
    /// <para>
    /// 名前のないセクションの場合の <see cref="PrivateProfileLine"/> コレクションの先頭は <c>null</c> です。
    /// </para>
    /// <para>
    /// 大文字と小文字は区別されません。
    /// </para>
    /// </remarks>
    public class PrivateProfileSection
    {
        // ----------------------------------------------------------------------------------------------------
        // Private Field(s)
        // ----------------------------------------------------------------------------------------------------

        // Inner Lines
        private readonly List<PrivateProfileLine> lines = new List<PrivateProfileLine>();

        // Inner Entries (Cash)
        private readonly Dictionary<string, string> entries = new Dictionary<string, string>(PrivateProfileSection.StringComparer);


        // ----------------------------------------------------------------------------------------------------
        // Constructor(s)
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// <see cref="PrivateProfileSection"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public PrivateProfileSection()
        {
#if false // DEBUG
            Debug.WriteLine("New Section", DebugInfo.ShortName);
#endif

            // NEW Entries
            this.Entries = new ReadOnlyDictionary<string, string>(this.entries);
        }


        /// <inheritdoc cref="PrivateProfile()"/>
        /// <param name="lines">
        /// このセクションに含まれる <see cref="PrivateProfileLine"/> の配列を指定します。
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="lines"/> に <c>null</c> が指定されました。
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="lines"/> の要素に不正な値が含まれています。
        /// </exception>
        public PrivateProfileSection(PrivateProfileLine[] lines) : this()
        {
            // Validation (Null Check):
            if (lines is null) { throw new ArgumentNullException(nameof(lines)); }

            // APPEND Lines
            for (int i = 0; i < lines.Length; i++)
            {
                // Validation (Null Check):
                if (lines[i] is null)
                {
                    throw new ArgumentException(Resources.PrivateProfileInvalidFormatErrorMessage, nameof(lines));
                }

                // Validation (Line Type):
                if (i == 0)
                {
                    // Validation (Line Type) for 1st line:
                    if (lines[0].LineType != PrivateProfileLineType.Section)
                    {
                        throw new ArgumentException(Resources.PrivateProfileInvalidLineTypeErrorMessage, nameof(lines));
                    }
                }
                else
                {
                    // Validation (Line Type) for other line:
                    if (lines[i].LineType != PrivateProfileLineType.Entry && lines[i].LineType != PrivateProfileLineType.Other)
                    {
                        throw new ArgumentException(Resources.PrivateProfileInvalidLineTypeErrorMessage, nameof(lines));
                    }
                }

                // APPEND Line
                this.Append(lines[i]);
            }
        }


        /// <inheritdoc cref="PrivateProfile()"/>
        /// <param name="content">
        /// INI ファイル (初期化ファイル) のセクション全体。
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="content"/> に <c>null</c> が指定されました。
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="content"/> の要素に <c>null</c> または不正な値が含まれます。
        /// </exception>
        public PrivateProfileSection(string[] content) : this()
        {
            // Validation (Null Check):
            if (content is null) { throw new ArgumentNullException(nameof(content)); }

            // APPEND Lines
            for (int i = 0; i < content.Length; i++)
            {
                // Validation (Null Check):
                if (content[i] is null)
                {
                    throw new ArgumentException(Resources.PrivateProfileInvalidFormatErrorMessage, nameof(content));
                }

                // NEW Line
                var line = new PrivateProfileLine(content[i]);

                // Validation (Line Type) & Null Section Insertion:
                if (i == 0)
                {
                    // for 1st content (Line)
                    // (No Validation)

                    // Check Line Type of 1st content (Line):
                    if (line.LineType != PrivateProfileLineType.Section)
                    {
                        // Insert Null Section as 1st element
                        this.Append(new PrivateProfileLine(null));
                    }
                }
                else
                {
                    // Validation (Line Type) for other content (Line):
                    if (line.LineType != PrivateProfileLineType.Entry && line.LineType != PrivateProfileLineType.Other)
                    {
                        throw new ArgumentException(Resources.PrivateProfileInvalidLineTypeErrorMessage, nameof(content));
                    }
                }

                // APPEND Line
                this.Append(line);
            }
        }


        // ----------------------------------------------------------------------------------------------------
        // Static Properties
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// キーの比較規則に使用する文字列比較操作を表します。
        /// </summary>
        public static StringComparer StringComparer { get; } = StringComparer.OrdinalIgnoreCase;


        // ----------------------------------------------------------------------------------------------------
        // Public, Protected Properties
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// このセクションの名前を取得または設定します。
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
        /// <exception cref="ArgumentNullException">
        /// <c>null</c> が指定されました。
        /// </exception>
        public string Name
        {
            get
            {
                // Validation:
                if (this.lines.Count == 0) { throw new InvalidOperationException(); }

                // RETURN
                return this.lines[0].SectionName;
            }

            set
            {
                // Validation (Null Check):
                if (value is null) { throw new ArgumentNullException(nameof(value)); }

                // // Validation (Format): -> White Space is allowed.
                // if (string.IsNullOrWhiteSpace(value)) { throw new FormatException(); }

                // Triming
                var trimed_name = value.Trim();

                // Change Section Name
                if (this.lines.Count == 0)
                {
                    // NEW Section Name
                    this.lines.Add(new PrivateProfileLine { SectionName = trimed_name });

#if DEBUG
                    Debug.WriteLine($"NEW Section Name: \"{this.Name}\"" + (string.IsNullOrEmpty(this.Name) ? " (Null Section)" : ""), DebugInfo.ShortName);
#endif
                }
                else
                {
#if DEBUG
                    Debug.Write($"UPDATE Section Name: \"{this.Name}\"" + (string.IsNullOrEmpty(this.Name) ? " (Null Section)" : ""), DebugInfo.ShortName);
#endif

                    // UPDATE Section Name
                    this.lines[0].SectionName = trimed_name;

#if DEBUG
                    Debug.WriteLine($" -> \"{this.Name}\"" + (string.IsNullOrEmpty(this.Name) ? " (Null Section)" : ""));
#endif
                }
            }
        }


        /// <summary>
        /// このセクションに含まれる全てのエントリーを <see cref="Dictionary{String, String}"/> として取得します。
        /// </summary>
        public ReadOnlyDictionary<string, string> Entries { get; protected set; }


        // ----------------------------------------------------------------------------------------------------
        // Public, Protected Method(s)
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// このセクションの RAW Lines を取得します。
        /// </summary>
        /// <returns>
        /// RAW Lines
        /// </returns>
        public string[] GetRawLines() => (from line in this.lines where line.RawLine != null select line.RawLine).ToArray();


        /// <summary>
        /// このセクションに新しい <see cref="PrivateProfileLine"/> クラスを追加します。
        /// </summary>
        /// <param name="line">
        /// 追加する <see cref="PrivateProfileLine"/> クラス。
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="line"/> に <c>null</c> が指定されました。
        /// </exception>
        public void Append(PrivateProfileLine line)
        {
            // Validation (Null Check):
            if (line is null) { throw new ArgumentNullException(nameof(line)); }

            // Other Validations:
            if (this.lines.Count == 0)
            {
                // for 1st Line

                // Validation (Line Type):
                if (line.LineType != PrivateProfileLineType.Section)
                {
                    throw new ArgumentException(Resources.PrivateProfileInvalidLineTypeErrorMessage, nameof(line));
                }
            }
            else
            {
                // for 2nd Line and after

                // Validation (Line Type):
                if (line.LineType != PrivateProfileLineType.Entry && line.LineType != PrivateProfileLineType.Other)
                {
                    throw new ArgumentException(Resources.PrivateProfileInvalidLineTypeErrorMessage, nameof(line));
                }

                // Validation (Key Existence):
                if ((line.LineType == PrivateProfileLineType.Entry) && this.entries.ContainsKey(line.Key))
                {
                    throw new ArgumentException(Resources.PrivateProfileKeyAlreadyExistsErrorMessage, nameof(line));
                }
            }

            // APPEND Line
            this.lines.Add(line);

            // UPDATE Entries (Cash)
            if (line.LineType == PrivateProfileLineType.Entry)
            {
                this.entries.Add(line.Key, line.Value);
            }

#if DEBUG
            Debug.Write(string.IsNullOrEmpty(this.Name) ? "Null Section" : $"Section \"{this.Name}\"", DebugInfo.ShortName);
            Debug.Write(": Append Line " + (line.RawLine is null ? "(null)" : $"\"{line.RawLine}\""));
            Debug.Write($" (LineType = {line.LineType}");

            switch (line.LineType)
            {
                case PrivateProfileLineType.NotInitialized:
                    throw new InvalidOperationException();

                case PrivateProfileLineType.Section:

                    // Section
                    Debug.WriteLine($", Section = \"{line.SectionName}\")");
                    break;

                case PrivateProfileLineType.Entry:

                    // Entry
                    Debug.WriteLine(", Key = {0}, Value = {1})", $"\"{line.Key}\"", line.Value is null ? "null" : $"\"{line.Value}\"");
                    break;

                case PrivateProfileLineType.Other:

                    // Other
                    Debug.WriteLine(")");
                    break;

                default:
                    throw new NotSupportedException();
            }
#endif
        }


        /// <summary>
        /// 指定されたキーに対応するエントリーの値を更新します。
        /// </summary>
        /// <param name="key">
        /// 検索キー。
        /// </param>
        /// <param name="value">
        /// 更新後の値。
        /// </param>
        /// <remarks>
        /// <para>
        /// <note type="note">
        /// <paramref name="key"/> および <paramref name="value"/> それぞれの先頭および末尾のホワイトスペースは取り除かれます。
        /// </note>
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> に <c>null</c> が指定されました。
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="key"/> が不正な文字を含んでいるか、空白 (ホワイトスペース) のみの文字列 あるいは 空文字 (<see cref="string.Empty"/>) が指定されました。
        /// または、<c>"="</c> を含む文字列が指定されました。
        /// </exception>
        /// <exception cref="KeyNotFoundException">
        /// 指定されたキーが見つかりません。
        /// </exception>
        public void Update(string key, string value)
        {
            // Validation (Null Check):
            if (key is null) { throw new ArgumentNullException(nameof(key)); }

            // Validation (Format):
            if (string.IsNullOrWhiteSpace(key) || key.Contains("="))
            {
                throw new ArgumentException(Resources.PrivateProfileInvalidFormatErrorMessage, nameof(key));
            }

            // Trim Key
            var trimed_key = key.Trim();

            // Get Index of KEY
            var index = this.IndexOf(trimed_key);

            // Validation (Key Existence):
            if (index < 0) { throw new KeyNotFoundException(); }

            // Trim Value
            var trimed_value = value?.Trim();

#if DEBUG
            Debug.Write(string.IsNullOrEmpty(this.Name) ? "Null Section" : $"Section \"{this.Name}\"", DebugInfo.ShortName);
            Debug.WriteLine(": Update Entry ({0}, {1}) -> ({2}, {3})",
                $"\"{this.lines[index].Key}\"", this.lines[index].Value is null ? "null" : $"\"{this.lines[index].Value}\"",
                $"\"{this.lines[index].Key}\"", trimed_value is null ? "null" : $"\"{trimed_value}\"");
#endif

            // UPDATE Line
            this.lines[index].Value = trimed_value;

            // UPDATE Cache
            this.entries[trimed_key] = trimed_value;
        }


        /// <summary>
        /// 指定されたキーに対応するエントリーの値を削除します。
        /// </summary>
        /// <param name="key">
        /// 削除するエントリーのキー。
        /// </param>
        /// <returns>
        /// <para>
        /// エントリーが見つかり、正常に削除された場合は <c>true</c>。
        /// それ以外の場合は <c>false</c>。
        /// </para>
        /// <para>
        /// <note type="note">
        /// このメソッドは、<paramref name="key"/> がエントリーに見つからなかった場合にも <c>false</c> を返します。
        /// </note>
        /// </para>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> に <c>null</c> が指定されました。
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="key"/> が不正な文字を含んでいるか、空白 (ホワイトスペース) のみの文字列 あるいは 空文字 (<see cref="string.Empty"/>) が指定されました。
        /// または、<c>"="</c> を含む文字列が指定されました。
        /// </exception>
        /// <exception cref="KeyNotFoundException">
        /// 指定されたキーが見つかりません。
        /// </exception>
        public bool Remove(string key)
        {
            // Validation (Null Check):
            if (key is null) { throw new ArgumentNullException(nameof(key)); }

            // Validation (Format):
            if (string.IsNullOrWhiteSpace(key) || key.Contains("="))
            {
                throw new ArgumentException(Resources.PrivateProfileInvalidFormatErrorMessage, nameof(key));
            }

            // Trim Key
            var trimed_key = key.Trim();

            // Get Index of KEY
            var index = this.IndexOf(trimed_key);

            // Validation (Key Existence):
            if (index < 0) { return false; }

#if DEBUG
            Debug.Write(string.IsNullOrEmpty(this.Name) ? "Null Section" : $"Section \"{this.Name}\"", DebugInfo.ShortName);
            Debug.WriteLine(": Remove Entry ({0}, {1})", $"\"{this.lines[index].Key}\"", this.lines[index].Value is null ? "null" : $"\"{this.lines[index].Value}\"");
#endif

            // REMOVE Line
            this.lines.RemoveAt(index);

            // UPDATE Cache
            if (!this.entries.Remove(trimed_key)) { throw new InvalidOperationException(); }

            // RETURN
            return true;
        }


        // ----------------------------------------------------------------------------------------------------
        // Static Method(s)
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// INI ファイル (初期化ファイル) からセクションを読み込みます。
        /// </summary>
        /// <param name="reader">
        /// このセクションを読み込む INI ファイルの <see cref="StreamReader"/> を指定します。
        /// </param>
        /// <param name="firstLine">
        /// このセクションの最初の 1 行を指定します。
        /// </param>
        /// <param name="nextLine">
        /// このセクションの次にセクションがある場合に、そのセクションの最初の 1 ラインが格納されます。
        /// このセクションが最後のセクションだった場合は <c>null</c> が格納されます。
        /// </param>
        /// <returns>
        /// 読み込みに成功した場合は、読み込んだセクション (<see cref="PrivateProfileSection"/>) を返します。
        /// 読み込みに失敗した場合は <c>null</c> を返します。
        /// </returns>
        public static PrivateProfileSection Read(TextReader reader, string firstLine, out string nextLine)
        {
            // Validation (Null Check):
            if (reader is null) { throw new ArgumentNullException(nameof(reader)); }
            if (firstLine is null) { throw new ArgumentNullException(nameof(firstLine)); }

            // Initialize value(s)
            nextLine = null;
            var raw_line = firstLine;

            // Initialize Section (NOT NEW)
            PrivateProfileSection section = null;

#if DEBUG
            Debug.WriteLine("Start reading the Section.", DebugInfo.ShortName);
#endif
            // MAIN LOOP: START
            while (raw_line != null) // instead of (!reader.EndOfStream)
            {
                // NEW Line
                var line = new PrivateProfileLine(raw_line);

                // Check only for 1st Line
                if (section is null)
                {
                    // NEW Section; because Section is NOT yet initialized.
                    section = new PrivateProfileSection();

                    // Check LineType (Null Section Check)
                    if (line.LineType != PrivateProfileLineType.Section)
                    {
                        // Insert Line for Null Section
                        section.Append(new PrivateProfileLine(null));

                        // Read NEXT Line
                        if ((raw_line = reader.ReadLine()) != null) { line = new PrivateProfileLine(raw_line); }
                    }
                }

                // ADD SECTION or ENTRY (or, Go to NEXT Section)
                switch (line.LineType)
                {
                    // SECTION Line:
                    case PrivateProfileLineType.Section:

                        // Check if Section Name is for Current or NEXT Section
                        if (section.lines.Count == 0)
                        {
                            // CURRENT Section:

                            // ADD SECTION
                            section.Append(line);
                            break;
                        }
                        else
                        {
                            // NEXT Section: Current Section has ended.
#if DEBUG
                            Debug.Write("End reading Section " + (section.Name is null ? "(null)" : $"\"{section.Name}\""), DebugInfo.ShortName);
                            Debug.WriteLine($", because NEXT Section \"{line.SectionName}\" was found.");
#endif
                            // SET 1st Line of NEXT Section
                            nextLine = raw_line;

                            // RETURN CURRENT Section
                            return section;
                        }

                    // ENTRY Line:
                    case PrivateProfileLineType.Entry:

                        // ADD ENTRY
                        section.Append(line);
                        break;

                    // ENTRY Line (Blank Line or Comment Line):
                    case PrivateProfileLineType.Other:

                        // ADD ENTRY
                        section.Append(line);
                        break;

                    default:
                        throw new NotSupportedException();
                }

                // Read NEXT Line
                raw_line = reader.ReadLine();
            }
            // MAIN LOOP: END
#if DEBUG
            Debug.WriteLine("End reading Section " + (section.Name is null ? "(null)" : $"\"{section.Name}\"") + ".", DebugInfo.ShortName);
#endif

            // RETURN
            return section;
        }


        // ----------------------------------------------------------------------------------------------------
        // Private Method(s)
        // ----------------------------------------------------------------------------------------------------

        // Get Index of KEY
        private int IndexOf(string key) => this.lines.FindIndex(line => (line?.LineType == PrivateProfileLineType.Entry) && (PrivateProfileSection.StringComparer.Compare(key.Trim(), line?.Key) == 0));
    }
}
