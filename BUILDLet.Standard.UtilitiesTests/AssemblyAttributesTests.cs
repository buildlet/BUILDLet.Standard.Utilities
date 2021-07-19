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
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Reflection;    // for Assembly
using System.Globalization; // for CultureInfo

using BUILDLet.UnitTest.Utilities; // for TestParameter Class

namespace BUILDLet.Standard.Utilities.Tests
{
    [TestClass]
    public class AssemblyAttributesTests
    {
        public static readonly string FileVersion = "1.6.4.0";

        public class ToStringTestParameter : TestParameter<string>
        {
            public string Name = "BUILDLet.Standard.UtilitiesTests";
            public string FullName = $"BUILDLet.Standard.UtilitiesTests, Version={AssemblyAttributesTests.FileVersion}, Culture=neutral, PublicKeyToken=null";
            public string Version = AssemblyAttributesTests.FileVersion;
            public CultureInfo CultureInfo = CultureInfo.InvariantCulture;
            public string CultureName = CultureInfo.InvariantCulture.Name;
            public string Title = "BUILDLet.Standard.UtilitiesTests";
            public string Description = "Unit Tests for BUILDLet.Standard.Utilities";
            public string Company = "BUILDLet";
            public string Product = "BUILDLet.Standard.UtilitiesTests";
            public string Copyright = "© 2019 Daiki Sakamoto";
            public string Trademark = null;
            public string FileVersion = AssemblyAttributesTests.FileVersion;
            public int LCID = 127;
            public string DisplayName = CultureInfo.InvariantCulture.DisplayName;
            public string NativeName = CultureInfo.InvariantCulture.NativeName;
            public string EnglishName = CultureInfo.InvariantCulture.EnglishName;

            public override void Arrange(out string expected)
            {
                expected = $"{this.FullName}"
                    + $", Title=\"{this.Title}\""
                    + $", Description=\"{this.Description}\""
                    + $", Company=\"{this.Company}\""
                    + $", Product=\"{this.Product}\""
                    + $", Copyright=\"{this.Copyright}\""
                    + $", Trademark=null"
                    + $", FileVersion=\"{this.FileVersion}\"";
            }

            public override void Act(out string actual)
            {
                actual = new AssemblyAttributes(Assembly.GetExecutingAssembly()).ToString();
            }
        }

        [TestMethod]
        public void ToStringTest()
        {
            // Parameter
            ToStringTestParameter param = new ToStringTestParameter()
            {
                Keyword = nameof(ToStringTest)
            };

            // ASSERT
            param.Validate();
        }
    }
}
