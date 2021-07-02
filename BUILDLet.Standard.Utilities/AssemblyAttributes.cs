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

using System.Reflection; // for Assembly Class
using System.Globalization; // for CultureInfo Class

namespace BUILDLet.Standard.Utilities
{
    /// <summary>
    /// アセンブリの属性、カスタム属性、および、特定のカルチャ ("ロケール") に関する情報を取得します。
    /// </summary>
    public class AssemblyAttributes
    {
        // ----------------------------------------------------------------------------------------------------
        // Private Variable(s)
        // ----------------------------------------------------------------------------------------------------

        private readonly Assembly assembly;


        // ----------------------------------------------------------------------------------------------------
        // Constructor(s)
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// <see cref="AssemblyAttributes"/> クラスの新しいインスタンスを初期化します。 
        /// </summary>
        /// <param name="assembly">
        /// 情報を取得するアセンブリを指定します。
        /// </param>
        /// <remarks>
        /// Version 1.6.1 より <c>null</c> を指定することはできなくなりました。
        /// </remarks>
        public AssemblyAttributes(Assembly assembly)
        {
            this.assembly = assembly;
        }


        // ----------------------------------------------------------------------------------------------------
        // Public, Protected Properties
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// アセンブリの簡易名 (<see cref="System.Reflection.AssemblyName.Name"/>) を取得します。
        /// </summary>
        public string Name => this.assembly.GetName().Name;

        /// <summary>
        /// アセンブリの完全名 (<see cref="System.Reflection.AssemblyName.FullName"/>) を取得します。
        /// </summary>
        public string FullName => this.assembly.GetName().FullName;

        /// <summary>
        /// アセンブリのバージョン (<see cref="System.Reflection.AssemblyName.Version"/>) を取得します。
        /// </summary>
        public Version Version => this.assembly.GetName().Version;

        /// <summary>
        /// アセンブリに関連付けられたカルチャ (<see cref="System.Reflection.AssemblyName.CultureInfo"/>) を取得します。
        /// </summary>
        public CultureInfo CultureInfo => this.assembly.GetName().CultureInfo;

        /// <summary>
        /// アセンブリに関連付けられたカルチャの名前 (<see cref="System.Reflection.AssemblyName.CultureName"/>) を取得します。
        /// </summary>
        public string CultureName => this.assembly.GetName().CultureName;


        /// <summary>
        /// アセンブリのタイトル (<see cref="System.Reflection.AssemblyTitleAttribute.Title"/>) を取得します。
        /// </summary>
        public string Title => (this.assembly.GetCustomAttribute(typeof(AssemblyTitleAttribute)) as AssemblyTitleAttribute)?.Title;

        /// <summary>
        /// アセンブリの説明 (<see cref="System.Reflection.AssemblyDescriptionAttribute.Description"/>) を取得します。
        /// </summary>
        public string Description => (this.assembly.GetCustomAttribute(typeof(AssemblyDescriptionAttribute)) as AssemblyDescriptionAttribute)?.Description;

        /// <summary>
        /// アセンブリの会社名に関するカスタム属性 (<see cref="System.Reflection.AssemblyCompanyAttribute.Company"/>) を取得します。
        /// </summary>
        public string Company => (this.assembly.GetCustomAttribute(typeof(AssemblyCompanyAttribute)) as AssemblyCompanyAttribute)?.Company;

        /// <summary>
        /// アセンブリの製品名に関するカスタム属性 (<see cref="System.Reflection.AssemblyProductAttribute.Product"/>) を取得します。
        /// </summary>
        public string Product => (this.assembly.GetCustomAttribute(typeof(AssemblyProductAttribute)) as AssemblyProductAttribute)?.Product;

        /// <summary>
        /// アセンブリの著作権に関するカスタム属性 (<see cref="System.Reflection.AssemblyCopyrightAttribute.Copyright"/>) を取得します。
        /// </summary>
        public string Copyright => (this.assembly.GetCustomAttribute(typeof(AssemblyCopyrightAttribute)) as AssemblyCopyrightAttribute)?.Copyright;

        /// <summary>
        /// アセンブリの商標に関するカスタム属性 (<see cref="System.Reflection.AssemblyTrademarkAttribute.Trademark"/>) を取得します。
        /// </summary>
        public string Trademark => (this.assembly.GetCustomAttribute(typeof(AssemblyTrademarkAttribute)) as AssemblyTrademarkAttribute)?.Trademark;

        /// <summary>
        /// アセンブリの Win32 ファイルバージョン (<see cref="System.Reflection.AssemblyFileVersionAttribute.Version"/>) を取得します。
        /// </summary>
        public string FileVersion => (this.assembly.GetCustomAttribute(typeof(AssemblyFileVersionAttribute)) as AssemblyFileVersionAttribute)?.Version;


        /// <summary>
        /// アセンブリのカルチャ識別子 (<see cref="System.Globalization.CultureInfo.LCID"/>) を取得します。
        /// </summary>
        public int LCID => this.assembly.GetName().CultureInfo.LCID;

        /// <summary>
        /// アセンブリの完全にローカライズされたカルチャ名 (<see cref="System.Globalization.CultureInfo.DisplayName"/>) を取得します。
        /// </summary>
        public string DisplayName => this.assembly.GetName().CultureInfo.DisplayName;

        /// <summary>
        /// アセンブリのカルチャの表示設定である、言語、国/地域、およびオプションのスクリプトで構成されるカルチャ名 (<see cref="System.Globalization.CultureInfo.NativeName"/>) を取得します。
        /// </summary>
        public string NativeName => this.assembly.GetName().CultureInfo.NativeName;

        /// <summary>
        /// アセンブリの英語で表したカルチャ名 (<see cref="System.Globalization.CultureInfo.EnglishName"/>) を取得します。
        /// </summary>
        public string EnglishName => this.assembly.GetName().CultureInfo.EnglishName;


        // ----------------------------------------------------------------------------------------------------
        // Public, Protected Method(s)
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// アセンブリに関連付けられた各種情報を文字列として取得します。
        /// </summary>
        /// <returns>
        /// 指定したアセンブリに関連付けられた各種情報を文字列として取得します。
        /// </returns>
        /// <remarks>取得される文字列には次の情報が含まれます。
        /// <para>
        /// <see cref="System.Reflection.AssemblyName.FullName"/> (Name, Version, Culture, PublicKeyToken), 
        /// <see cref="System.Reflection.AssemblyTitleAttribute.Title"/>, <see cref="System.Reflection.AssemblyDescriptionAttribute.Description"/>, 
        /// <see cref="System.Reflection.AssemblyCompanyAttribute.Company"/>, <see cref="System.Reflection.AssemblyProductAttribute.Product"/>, 
        /// <see cref="System.Reflection.AssemblyCopyrightAttribute.Copyright"/>, <see cref="System.Reflection.AssemblyTrademarkAttribute.Trademark"/>, 
        /// <see cref="System.Reflection.AssemblyFileVersionAttribute.Version"/>
        /// </para>
        /// </remarks>
        public override string ToString() => assembly.GetName().FullName
            + string.Format(CultureInfo.CurrentCulture, ", Title={0}", this.Title is null ? "null" : $"\"{this.Title}\"")
            + string.Format(CultureInfo.CurrentCulture, ", Description={0}", this.Description is null ? "null" : $"\"{this.Description}\"")
            + string.Format(CultureInfo.CurrentCulture, ", Company={0}", this.Company is null ? "null" : $"\"{this.Company}\"")
            + string.Format(CultureInfo.CurrentCulture, ", Product={0}", this.Product is null ? "null" : $"\"{this.Product}\"")
            + string.Format(CultureInfo.CurrentCulture, ", Copyright={0}", this.Copyright is null ? "null" : $"\"{this.Copyright}\"")
            + string.Format(CultureInfo.CurrentCulture, ", Trademark={0}", this.Trademark is null ? "null" : $"\"{this.Trademark}\"")
            + string.Format(CultureInfo.CurrentCulture, ", FileVersion={0}", this.FileVersion is null ? "null" : $"\"{this.FileVersion}\"");
    }
}
