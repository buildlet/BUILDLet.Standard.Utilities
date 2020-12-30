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
using System.IO;                      // for StreamReader Class
using System.Collections.ObjectModel; // for ReadOnlyDictionary Class

using BUILDLet.Standard.Utilities.Properties; // for Resources
using BUILDLet.Standard.Diagnostics;          // for DebuInfo class

namespace BUILDLet.Standard.Utilities
{
    /// <summary>
    /// INI ファイル (初期化ファイル) の簡易的な読み込みと書き込みをサポートします。
    /// </summary>
    /// <remarks>
    /// 行継続文字 (Line Continuetor) としてのバックスラッシュ (<c>\</c>) はサポートしていません。
    /// </remarks>
    public class PrivateProfile : IDisposable
    {
        // ----------------------------------------------------------------------------------------------------
        // Private Field(s)
        // ----------------------------------------------------------------------------------------------------

        // FileStream: If NOT null, File is open.
        private FileStream fileStream = null;

        // Sections
        private readonly Dictionary<string, PrivateProfileSection> sections = new Dictionary<string, PrivateProfileSection>(StringComparer.OrdinalIgnoreCase);


        // ----------------------------------------------------------------------------------------------------
        // Constructor(s)
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// <see cref="PrivateProfile"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public PrivateProfile()
        {
            // NEW Sections
            this.Sections = new ReadOnlyDictionary<string, PrivateProfileSection>(this.sections);
        }


        /// <summary>
        /// <see cref="PrivateProfile"/> クラスの新しいインスタンスを初期化して、指定された INI ファイルを読み取り専用で開きます。
        /// </summary>
        /// <param name="path">
        /// INI ファイルのパス
        /// </param>
        /// <param name="readOnly">
        /// INI ファイルを読み取り専用で開くときに <c>true</c> を指定します。
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// 既にファイルが開かれている状態で、書き込み用にファイルを開こうとしました。
        /// </exception>
        public PrivateProfile(string path, bool readOnly = true) : this() => this.Read(path, readOnly);


        /// <summary>
        /// <see cref="PrivateProfile"/> クラスの新しいインスタンスを初期化して、指定された INI ファイルのコンテンツを指定されたストリームから読み込みます。
        /// </summary>
        /// <param name="stream">
        /// コンテンツのストリーム。
        /// </param>
        public PrivateProfile(Stream stream) : this() => this.Read(stream);


        // ----------------------------------------------------------------------------------------------------
        // Static Properties
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// ファイルに値を書き込む際に、ファイル全体の改行コードがこのプロパティの値に置き換えられます。
        /// </summary>
        public static string LineBreakExpression { get; } = "\r\n";


        // ----------------------------------------------------------------------------------------------------
        // Public, Protected Properties
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// このオブジェクトに含まれる全てのセクションを <see cref="ReadOnlyDictionary{String, PrivateProfileSection}"/> として取得します。
        /// </summary>
        /// <remarks>
        /// <para>
        /// <note type="note">
        /// このプロパティによって、次のような形式でエントリの値を取得することができます。
        /// <para>
        /// <see cref="PrivateProfile"/><c>.</c><see cref="PrivateProfile.Sections"/><c>[&lt;section name&gt;].</c><see cref="PrivateProfileSection.Entries"/><c>[&lt;key&gt;]</c>
        /// </para>
        /// </note>
        /// </para>
        /// </remarks>
        public ReadOnlyDictionary<string, PrivateProfileSection> Sections { get; }

        /// <summary>
        /// 開いている INI ファイルの名前を表します。
        /// ファイルが開かれていないときは <c>null</c> を返します。
        /// </summary>
        public string FileName => this.fileStream?.Name;

        /// <summary>
        /// 現在のオブジェクトでファイルが開かれていることを示します。
        /// </summary>
        public bool IsOpen => this.fileStream != null;

        /// <summary>
        /// 現在のオブジェクトでファイルが読み込み専用で開かれていることを示します。
        /// </summary>
        public bool IsReadOnly { get; protected set; } = false;

        /// <summary>
        /// 現在のオブジェクトが更新されていることを示します。
        /// </summary>
        public bool IsUpdated { get; protected set; } = false;


        // ----------------------------------------------------------------------------------------------------
        // Public, Protected Indexer(s)
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// 指定されたセクションに含まれる全てのエントリ (キーと値の組み合わせ) を <see cref="Dictionary{String, String}"/> として取得します。
        /// </summary>
        /// <param name="section">
        /// セクションの名前
        /// </param>
        /// <remarks>
        /// <para>
        /// <note type="note">
        /// このプロパティによって、次のような形式でエントリの値を取得することができます。
        /// <para>
        /// <see cref="PrivateProfile"/><c>[&lt;section name&gt;][&lt;key&gt;]</c>
        /// </para>
        /// </note>
        /// </para>
        /// </remarks>
        public ReadOnlyDictionary<string, string> this[string section] => this.sections[section].Entries;


        // ----------------------------------------------------------------------------------------------------
        // Public, Protected Method(s)
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// INI ファイルを開きます。
        /// </summary>
        /// <param name="path">
        /// INI ファイルのパス
        /// </param>
        /// <param name="readOnly">
        /// INI ファイルを読み取り専用で開くときに <c>true</c> を指定します。
        /// 既定は <c>true</c> です。
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// 既にファイルが開かれている状態で、書き込み用にファイルを開こうとしました。
        /// </exception>
        public void Open(string path, bool readOnly = true)
        {
            // Validation: If the file is already open as READ-WRITE
            if (this.IsOpen && !this.IsReadOnly) { throw new InvalidOperationException(); }

            // Close all resources for re-open.
            this.Close();

            // SET READ-ONLY Flag
            this.IsReadOnly = readOnly;

            // Open File
            if (this.IsReadOnly)
            {
                // OPEN File as READ-ONLY
                this.fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            }
            else
            {
                // OPEN File as READ-WRITE
                this.fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            }
#if DEBUG
            Debug.WriteLine($"File Stream of file \"{this.FileName}\" was open as {(this.IsReadOnly ? "READ-ONLY" : "READ-WRITE")} mode.", DebugInfo.ShortName);
#endif

            // for IDisposable Support
            this.disposedValue = false;
        }


        /// <summary>
        /// このオブジェクトによって使用されているすべてのリソースを解放します。
        /// </summary>
        public void Close()
        {
            // Dispose File Stream
            ((IDisposable)this.fileStream)?.Dispose();
#if DEBUG
            Debug.WriteLine($"File Stream was closed.", DebugInfo.ShortName);
#endif

            // Clear sections
            this.sections.Clear();

            // Clear Flag(s)
            this.IsReadOnly = false;
            this.IsUpdated = false;
        }


        /// <summary>
        /// バッファ内のデータをファイルに書き込みます。
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// ファイルが開かれていないか、ファイルが読み込み専用で開かれています。
        /// </exception>
        public void Write()
        {
            // Validation:
            if (!this.IsOpen || this.IsReadOnly) { throw new InvalidOperationException(); }

            if (this.IsUpdated)
            {
                // Convert to Byte Array
                byte[] array = Encoding.UTF8.GetBytes(this.Export());

                // Set Length of File Stream to 0
                this.fileStream.SetLength(0);

                // Write to Stream
                this.fileStream.Write(array, 0, array.Length);

                // Flush File Stream
                this.fileStream.Flush();

#if DEBUG
                Debug.WriteLine($"File Stream of the file \"{this.FileName}\" was flushed.", DebugInfo.ShortName);
#endif

                // Clear Flag(s)
                this.IsUpdated = false;
            }
        }


        /// <summary>
        /// INI ファイル (初期化ファイル) を開いて、読み込みます。
        /// </summary>
        /// <param name="path">
        /// INI ファイルのパス
        /// </param>
        /// <param name="readOnly">
        /// INI ファイルを読み取り専用で開くときに <c>true</c> を指定します。
        /// </param>
        public void Read(string path, bool readOnly = true)
        {
            // Open
            this.Open(path, readOnly);

            // Read
            this.Read(this.fileStream);
        }


        /// <summary>
        /// INI ファイル (初期化ファイル) を読み込みます。
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// ファイルが開かれていません。
        /// </exception>
        public void Read()
        {
            // Validation:
            if (!this.IsOpen) { throw new InvalidOperationException(); }

            // Read
            this.Read(this.fileStream);
        }


        /// <summary>
        /// INI ファイル (初期化ファイル) のコンテンツをストリームから読み込みます。
        /// </summary>
        /// <param name="stream">
        /// コンテンツのストリーム
        /// </param>
        public void Read(Stream stream)
        {
#if DEBUG
            Debug.WriteLine("Start reading Stream", DebugInfo.ShortName);
#endif
            using (var reader = new StreamReader(stream, Encoding.UTF8, false, 255, true))
            {
                // SET Position of BaseStream to 0
                reader.BaseStream.Position = 0;

                // Read 1st Line of Stream
                var current_line = reader.ReadLine();

                // MAIN LOOP: START
                while (current_line != null)
                {
                    // NEW & READ Section
                    PrivateProfileSection section = PrivateProfileSection.Read(reader, current_line, out string next_line);

                    // Validations:
                    if (string.IsNullOrWhiteSpace(section.Name))
                    {
                        // for Null Section:

                        // Validation: Null Section Check; Null Section is allowed only for 1st section (sections[0]).
                        if (this.sections.Count > 0)
                        {
                            throw new InvalidDataException(Resources.PrivateProfileNullSectionAlreadyExistsErrorMessage);
                        }
                    }
                    else
                    {
                        // for Normal Section:

                        // Validation: If the same Section Name already exists.
                        if (this.sections.ContainsKey(section.Name))
                        {
                            throw new InvalidDataException(Resources.PrivateProfileSectionNameAlreadyExistsErrorMessage);
                        }
                    }

                    // ADD Section
                    this.sections.Add(section.Name, section);

                    // SET Next Line
                    current_line = next_line;
                }
                // MAIN LOOP: END
            }
#if DEBUG
            Debug.WriteLine("End reading Stream", DebugInfo.ShortName);
#endif
        }


        /// <summary>
        /// 指定された名前のセクションが含まれるかどうかを判定します。
        /// </summary>
        /// <param name="section">
        /// 検索するセクションの名前
        /// </param>
        /// <returns>
        /// 指定された名前のセクションが含まれる場合に <c>true</c> を返します。
        /// </returns>
        /// <remarks>
        /// 名前のないセクションを調べる場合は <paramref name="section"/> に <see cref="string.Empty"/> (<c>""</c>) を指定します。
        /// </remarks>
        public bool Contains(string section) => this.sections.ContainsKey(section?.Trim());


        /// <summary>
        /// 指定された名前のセクションに、指定されたキーが含まれるかどうかを判定します。
        /// </summary>
        /// <param name="section">
        /// 検索するセクションの名前
        /// </param>
        /// <param name="key">
        /// 検索するキー
        /// </param>
        /// <returns>
        /// 指定された名前のセクションが存在し、かつ、そのセクションに指定されたキーが含まれる場合に <c>true</c> を返します。
        /// </returns>
        public bool Contains(string section, string key) =>
            this.sections.ContainsKey(section?.Trim()) && this.sections[section?.Trim()].Entries.ContainsKey(key?.Trim());


        /// <summary>
        /// INI ファイル (初期化ファイル) のコンテンツをインポートします。
        /// </summary>
        /// <param name="content">
        /// インポートする INI ファイル (初期化ファイル) のコンテンツ
        /// </param>
        public void Import(string content) => this.Read(new MemoryStream(Encoding.UTF8.GetBytes(content)));


        /// <summary>
        /// このオブジェクトに関連付けられる RAW Lines を文字列としてエクスポートします。
        /// </summary>
        /// <returns>
        /// INI ファイル (初期化ファイル) のコンテンツ
        /// </returns>
        /// <remarks>
        /// 改行コードは <see cref="PrivateProfile.LineBreakExpression"/> に置き換えられます。
        /// </remarks>
        public string Export() => string.Join(PrivateProfile.LineBreakExpression, this.GetRawLines()) + PrivateProfile.LineBreakExpression;


        /// <summary>
        /// このオブジェクトに関連付けられる RAW Lines を取得します。
        /// </summary>
        /// <returns>
        /// RAW Lines
        /// </returns>
        public string[] GetRawLines()
        {
            var raw_lines = new List<string>();

            foreach (var section_name in this.sections.Keys)
            {
                raw_lines.AddRange(this.sections[section_name].GetRawLines());
            }

            // RETURN
            return raw_lines.ToArray();
        }


        /// <summary>
        /// INI ファイル (初期化ファイル) から、指定したセクションとキーの組み合わせに対応する値を文字列として取得します。
        /// </summary>
        /// <param name="section">
        /// セクションの名前
        /// </param>
        /// <param name="key">
        /// エントリのキー
        /// </param>
        /// <returns>
        /// 指定したセクションとキーの組み合わせに対応する値の文字列を返します。<br/>
        /// 該当するセクションとキーの組み合わせが存在しない場合は <c>null</c> を返します。<br/>
        /// 該当するセクションとキーの組み合わせに値が存在しない場合は <see cref="System.String.Empty"/> を返します。
        /// </returns>
        public string GetValue(string section, string key) => this.sections[section].Entries[key];


        /// <summary>
        /// INI ファイル (初期化ファイル) の指定したセクションとキーの組み合わせに対応する値を更新または追加します。
        /// </summary>
        /// <param name="section">
        /// セクションの名前
        /// </param>
        /// <param name="key">
        /// エントリのキー
        /// </param>
        /// <param name="value">
        /// 更新後の値
        /// </param>
        /// <returns>
        /// このメソッドは、この <see cref="PrivateProfile"/> オブジェクト自身を返します。
        /// </returns>
        /// <remarks>
        /// <para>
        /// 指定したセクションとキー、および、その値が存在する場合は、値を更新します。
        /// 指定したセクションとキーは存在して、値が存在しない場合は、値を追加します。
        /// 指定したセクションは存在して、キーが存在しない場合は、そのセクションの末尾にエントリ (キーと値) を追加します。
        /// 指定したセクションが存在しない場合は、ファイルの末尾に、そのセクションとエントリ (キーと値) を追加します。
        /// </para>
        /// <para>
        /// <note type="note">
        /// 当該エントリの末尾にコメントがある場合、そのコメントは削除されます。
        /// </note>
        /// </para>
        /// <para>
        /// <note type="warning">
        /// 既に名前のないセクションが存在している場合のみ、名前のないセクションのエントリを更新または追加をすることができます。
        /// 名前のないセクションが存在しない場合は、このメソッドによって、後から名前のないセクションを作成することはできません。
        /// </note>
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="section"/> あるいは <paramref name="key"/> に <c>null</c> が指定されました。
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="key"/> が不正です。
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// 既にファイルが開かれている状態で、書き込み用にファイルを開こうとしました。
        /// </exception>
        /// <exception cref="ArgumentException">
        /// 同一のキーを含む項目が既に追加されています。
        /// </exception>
        public PrivateProfile SetValue(string section, string key, string value)
        {
            // Validation (Null Check):
            if (section is null) { throw new ArgumentNullException(nameof(section)); }
            if (key is null) { throw new ArgumentNullException(nameof(key)); }

            // Validation (Format):
            if (string.IsNullOrWhiteSpace(key) || key.Contains("="))
            {
                throw new ArgumentException(Resources.PrivateProfileInvalidFormatErrorMessage, nameof(key));
            }

            // Triming
            var section_name = section.Trim();
            var trimed_key = key.Trim();
            var trimed_value = value?.Trim();

            // Section Existence Check:
            if (!this.sections.ContainsKey(section_name))
            {
                // Target Section is NOT found.

                // for Null Section:
                if (string.IsNullOrWhiteSpace(section_name)) { throw new InvalidOperationException(); }

                // ADD NEW Section
                this.sections.Add(section, new PrivateProfileSection { Name = section_name });
            }

            // GET Target Section
            var target_section = this.sections[section_name];

            // Entry Existence Check:
            if (target_section.Entries.ContainsKey(trimed_key))
            {
                // KEY is found.

                // Check VALUE
                if (string.CompareOrdinal(trimed_value, target_section.Entries[trimed_key]) != 0)
                {
                    // Different VALUE (VALUE will be changed.):

                    // UPDATE Line
                    target_section.Update(trimed_key, trimed_value);

                    // SET Update Flag
                    this.IsUpdated = true;
                }
                else
                {
                    // Same VALUE (VALUE will NOT be changed.):
                    // Do Nothing
                }
            }
            else
            {
                // KEY is NOT found.

                // APPEND NEW Line
                target_section.Append(new PrivateProfileLine { Key = trimed_key, Value = trimed_value });

                // SET Update Flag
                this.IsUpdated = true;
            }


            // RETURN this object
            return this;
        }


        /// <summary>
        /// INI ファイル (初期化ファイル) の指定したセクションとキーの組み合わせに対応する値を削除します。
        /// </summary>
        /// <param name="section">
        /// セクションの名前
        /// </param>
        /// <param name="key">
        /// エントリのキー
        /// </param>
        /// <returns>
        /// <para>
        /// エントリが見つかり、正常に削除された場合は <c>true</c> を返し、それ以外の場合は <c>false</c> を返します。
        /// </para>
        /// <para>
        /// <note type="note">
        /// このメソッドは、<paramref name="key"/> がエントリに見つからなかった場合にも <c>false</c> を返します。
        /// </note>
        /// </para>
        /// </returns>
        public bool Remove(string section, string key) => this.IsUpdated = this.sections[section].Remove(key);


        /// <summary>
        /// INI ファイル (初期化ファイル) の指定したセクションを削除します。
        /// </summary>
        /// <param name="section">
        /// セクションの名前
        /// </param>
        /// <returns>
        /// <para>
        /// セクションが見つかり、正常に削除された場合は <c>true</c> を返し、それ以外の場合は <c>false</c> を返します。
        /// </para>
        /// <para>
        /// <note type="note">
        /// このメソッドは、<paramref name="section"/> という名前のセクションが見つからなかった場合にも <c>false</c> を返します。
        /// </note>
        /// </para>
        /// </returns>
        public bool Remove(string section) => this.IsUpdated = this.sections.Remove(section);


        // ----------------------------------------------------------------------------------------------------
        // Static Method(s)
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// INI ファイル (初期化ファイル) から、指定したセクションとキーの組み合わせに対応する値を文字列として取得します。
        /// </summary>
        /// <param name="path">
        /// INI ファイルのパス
        /// </param>
        /// <param name="section">
        /// セクションの名前
        /// </param>
        /// <param name="key">
        /// エントリのキー
        /// </param>
        /// <returns>
        /// 指定したセクションとキーの組み合わせに対応する値の文字列を返します。
        /// 該当するセクションとキーの組み合わせが存在しない場合は <c>null</c> を返します。
        /// 該当するセクションとキーの組み合わせに値が存在しない場合は <see cref="System.String.Empty"/> を返します。
        /// </returns>
        /// <remarks>
        /// このメソッドは呼び出されるたびにファイルを開きます。
        /// 連続的に値を取得する場合は、このメソッドではなく、
        /// <see cref="PrivateProfile.GetValue(string, string)" autoUpgrade="true"/> メソッドを使用してください。
        /// </remarks>
        public static string GetPrivateProfile(string path, string section, string key)
        {
            using (var profile = new PrivateProfile(path))
            {
                return profile.GetValue(section, key);
            }
        }


        /// <summary>
        /// INI ファイル (初期化ファイル) の指定したセクションとキーの組み合わせに対応する値を更新または追加します。
        /// </summary>
        /// <param name="path">
        /// INI ファイルのパス
        /// </param>
        /// <param name="section">
        /// セクションの名前
        /// </param>
        /// <param name="key">
        /// エントリのキー
        /// </param>
        /// <param name="value">
        /// 更新後の値
        /// </param>
        /// <remarks>
        /// <para>
        /// このメソッドは呼び出されるたびにファイルを開きます。
        /// 連続的に値を取得する場合は、このメソッドではなく、
        /// <see cref="PrivateProfile.SetValue(string, string, string)" autoUpgrade="true"/> メソッドを使用してください。
        /// </para>
        /// <para>
        /// 指定したセクションとキー、および、その値が存在する場合は、値を更新します。
        /// 指定したセクションとキーは存在して、値が存在しない場合は、値を追加します。
        /// 指定したセクションは存在して、キーが存在しない場合は、そのセクションの末尾にエントリ (キーと値) を追加します。
        /// 指定したセクションが存在しない場合は、ファイルの末尾に、そのセクションとエントリ (キーと値) を追加します。
        /// </para>
        /// <para>
        /// <note type="note">
        /// 当該エントリの末尾にコメントがある場合、そのコメントは削除されます。
        /// </note>
        /// </para>
        /// <para>
        /// <note type="warning">
        /// 既に名前のないセクションが存在している場合のみ、名前のないセクションのエントリを更新または追加をすることができます。
        /// 名前のないセクションが存在しない場合は、このメソッドによって、後から名前のないセクションを作成することはできません。
        /// </note>
        /// </para>
        /// </remarks>
        public static void SetPrivateProfile(string path, string section, string key, string value)
        {
            using (var profile = new PrivateProfile(path, false))
            {
                profile.SetValue(section, key, value).Write();
            }
        }


        // ----------------------------------------------------------------------------------------------------
        // IDisposable Support
        // ----------------------------------------------------------------------------------------------------
        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        /// <summary>
        /// アンマネージ リソースの解放またはリセットに関連付けられているアプリケーション定義のタスクを実行します。
        /// </summary>
        /// <param name="disposing">
        /// メソッドの呼び出し元が <see cref="IDisposable.Dispose"/> メソッドか (値は <c>true</c>)、それともファイナライザーか (値は <c>false</c>) を示します。
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージ状態を破棄します (マネージ オブジェクト)。
                    this.Close();
                }

                // TODO: アンマネージ リソース (アンマネージ オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
                // TODO: 大きなフィールドを null に設定します。

                disposedValue = true;
            }
        }

        // TODO: 上の Dispose(bool disposing) にアンマネージ リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
        // ~PrivateProfile()
        // {
        //   // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
        //   Dispose(false);
        // }

        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        /// <summary>
        /// アンマネージ リソースの解放またはリセットに関連付けられているアプリケーション定義のタスクを実行します。
        /// </summary>
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(true);
            // TODO: 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
            // # CA1816: Dispose メソッドは、SuppressFinalize を呼び出す必要があります
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
