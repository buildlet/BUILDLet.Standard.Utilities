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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

using BUILDLet.UnitTest.Utilities; // for TestParameter Class

namespace BUILDLet.Standard.Utilities.Tests
{
    [TestClass]
    public class PrivateProfileLineTests
    {
        // ----------------------------------------------------------------
        // Not Initialized Exception Tests
        // ----------------------------------------------------------------

        [TestMethod]
        [TestCategory("Exception")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SectionNameNotInitializedExceptionTest() => _ = new PrivateProfileLine().SectionName;

        [TestMethod]
        [TestCategory("Exception")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void KeyNotInitializedExceptionTest() => _ = new PrivateProfileLine().Key;

        [TestMethod]
        [TestCategory("Exception")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ValueNotInitializedExceptionTest() => _ = new PrivateProfileLine().Value;


        // ----------------------------------------------------------------
        // Format Exception Tests
        // ----------------------------------------------------------------

        [DataTestMethod]
        [TestCategory("Exception")]
        [ExpectedException(typeof(FormatException))]
        [DataRow("[]")]
        [DataRow("=a")]
        public void InvalidLineFormatExceptionTest(string line) => PrivateProfileLine.Parse(line, out _, out _, out _);


        // ----------------------------------------------------------------
        // Base Type of TestParameter
        // ----------------------------------------------------------------

        // Base Type of TestParameter for PrivateProfileLine Class Tests
        public abstract class LineTestParameter : TestParameter<object[]>
        {
            public PrivateProfileLineType LineType;
            public string RawLine;
            public string SectionName;
            public string Key;
            public string Value;

            // ARRANGE: SET Expected
            public override void Arrange(out object[] expected) =>
                expected = new object[] { this.LineType, this.RawLine, this.SectionName, this.Key, this.Value };
        }


        // ----------------------------------------------------------------
        // Tests of New Line by Constructor w/o Parameter
        // ----------------------------------------------------------------

        // TestParameter for NewLineTest
        public class NewLineTestParameter : LineTestParameter
        {
            // ACT
            public override void Act(out object[] actual)
            {
                // Prepare PrivateProfileLine
                PrivateProfileLine line;

                switch (this.LineType)
                {
                    case PrivateProfileLineType.NotInitialized:
                        throw new NotSupportedException();

                    case PrivateProfileLineType.Section:
                        line = new PrivateProfileLine() { SectionName = this.SectionName };
                        actual = new object[] { line.LineType, line.RawLine, line.SectionName, null, null };
                        break;

                    case PrivateProfileLineType.Entry:
                        line = new PrivateProfileLine() { Key = this.Key, Value = this.Value };
                        actual = new object[] { line.LineType, line.RawLine, null, line.Key, line.Value };
                        break;

                    case PrivateProfileLineType.Other:
                        throw new NotSupportedException();

                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        [DataTestMethod]
        // Common Test Case for NewLineTest, ParseMethodTest and NewLineFromRawLineTest.
        [DataRow(PrivateProfileLineType.Section, "[SECTION]", "SECTION", null, null)]
        [DataRow(PrivateProfileLineType.Section, "[Valid[Section]", "Valid[Section", null, null)]
        [DataRow(PrivateProfileLineType.Section, "[Valid]Section]", "Valid]Section", null, null)]
        [DataRow(PrivateProfileLineType.Section, "[Valid[Valid]Section]", "Valid[Valid]Section", null, null)]
        [DataRow(PrivateProfileLineType.Section, "[Section=Valid]", "Section=Valid", null, null)]
        [DataRow(PrivateProfileLineType.Entry, "KEY=VALUE", null, "KEY", "VALUE")]
        [DataRow(PrivateProfileLineType.Entry, "KEY", null, "KEY", null)]
        [DataRow(PrivateProfileLineType.Entry, "KEY=", null, "KEY", "")]
        [DataRow(PrivateProfileLineType.Entry, "KEY=VALUE=Valid", null, "KEY", "VALUE=Valid")]
        //
        // Common Test Case only for NewLineTest and NewLineFromRawLineTest.
        [DataRow(PrivateProfileLineType.Section, null, "", null, null, DisplayName = "Null Section")]
        public void NewLineTest(PrivateProfileLineType type, string line, string section, string key, string value)
        {
            // SET Parameter
            NewLineTestParameter param = new NewLineTestParameter
            {
                LineType = type,
                RawLine = line,
                SectionName = section,
                Key = key,
                Value = value
            };

            // ASSERT
            param.Validate();
        }


        // ----------------------------------------------------------------
        // Invalid Operation (LineType) Exception Tests
        // ----------------------------------------------------------------

        [TestMethod]
        [TestCategory("Exception")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SectionNameInvalidOperationExceptionTest() => _ = new PrivateProfileLine() { Key = "KEY", Value = "VALUE" }.SectionName;

        [TestMethod]
        [TestCategory("Exception")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void KeyInvalidOperationExceptionTest() => _ = new PrivateProfileLine() { SectionName = "SECTION" }.Key;

        [TestMethod]
        [TestCategory("Exception")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ValueInvalidOperationExceptionTest() => _ = new PrivateProfileLine() { SectionName = "SECTION" }.Value;


        // ----------------------------------------------------------------
        // Tests of Trim Method
        // ----------------------------------------------------------------

        // TestParameter for TrimMethodTest
        public class TrimMethodTestParameter : TestParameter<string>
        {
            public string Before;
            public string After;

            // ARRANGE: SET Expected
            public override void Arrange(out string expected) => expected = this.After;

            // ACT: GET Actual
            public override void Act(out string actual) => actual = PrivateProfileLine.Trim(this.Before);
        }

        [DataTestMethod]
        [DataRow("TEXT", "TEXT")]
        [DataRow("This is TEXT", "This is TEXT")]
        [DataRow("This is TEXT\t\t", "This is TEXT")]
        [DataRow("TEXT:\tThis is TEXT\t\t", "TEXT:\tThis is TEXT")]
        [DataRow("TEXT ", "TEXT")]
        [DataRow(" TEXT ", "TEXT")]
        [DataRow("TEXT    ", "TEXT")]
        [DataRow("TEXT\t", "TEXT")]
        [DataRow("\tTEXT\t", "TEXT")]
        [DataRow("TEXT\t\t", "TEXT")]
        [DataRow("TEXT;Comment", "TEXT")]
        [DataRow("TEXT ;Comment", "TEXT")]
        [DataRow("TEXT\t;Comment", "TEXT")]
        [DataRow("", "")]
        [DataRow(";Comment", "")]
        [DataRow(" ;Comment", "")]
        [DataRow("  ;Comment", "")]
        [DataRow("\t;Comment", "")]
        [DataRow("\t\t;Comment", "")]
        [DataRow(";\tThis is Comment\t\t", "")]
        public void TrimMethodTest(string before, string after)
        {
            // SET Parameter
            TrimMethodTestParameter param = new TrimMethodTestParameter { Before = before, After = after };

            // ASSERT
            param.Validate();
        }


        // ----------------------------------------------------------------
        // Tests of Parse Method
        // ----------------------------------------------------------------

        // TestParameter for ParseMethodTest
        public class ParseMethodTestParameter : LineTestParameter
        {
            // ACT
            public override void Act(out object[] actual)
            {
                // GET Actual
                var type = PrivateProfileLine.Parse(this.RawLine, out var section, out var key, out var value);

                // SET Actual
                actual = new object[] { type, this.RawLine, section, key, value };
            }
        }

        [DataTestMethod]
        // Common Test Case for NewLineTest, ParseMethodTest and NewLineFromRawLineTest.
        [DataRow(PrivateProfileLineType.Section, "[SECTION]", "SECTION", null, null)]
        [DataRow(PrivateProfileLineType.Section, "[Valid[Section]", "Valid[Section", null, null)]
        [DataRow(PrivateProfileLineType.Section, "[Valid]Section]", "Valid]Section", null, null)]
        [DataRow(PrivateProfileLineType.Section, "[Valid[Valid]Section]", "Valid[Valid]Section", null, null)]
        [DataRow(PrivateProfileLineType.Section, "[Section=Valid]", "Section=Valid", null, null)]
        [DataRow(PrivateProfileLineType.Entry, "KEY=VALUE", null, "KEY", "VALUE")]
        [DataRow(PrivateProfileLineType.Entry, "KEY", null, "KEY", null)]
        [DataRow(PrivateProfileLineType.Entry, "KEY=", null, "KEY", "")]
        [DataRow(PrivateProfileLineType.Entry, "KEY=VALUE=Valid", null, "KEY", "VALUE=Valid")]
        //
        // Common Test Case only for ParseMethodTest and NewLineFromRawLineTest.
        [DataRow(PrivateProfileLineType.Other, "", null, null, null, DisplayName = "String.Empty")]
        [DataRow(PrivateProfileLineType.Other, ";This is comment line.", null, null, null, DisplayName = "Comment Line")]
        [DataRow(PrivateProfileLineType.Section, "[SECTION] ;Comment", "SECTION", null, null, DisplayName = "Section Line with Comment")]
        [DataRow(PrivateProfileLineType.Entry, "KEY=VALUE", null, "KEY", "VALUE", DisplayName = "Entry Line with Comment")]
        public void ParseMethodTest(PrivateProfileLineType type, string line, string section, string key, string value)
        {
            // SET Parameter
            ParseMethodTestParameter param = new ParseMethodTestParameter
            {
                LineType = type,
                RawLine = line,
                SectionName = section,
                Key = key,
                Value = value
            };

            // ASSERT
            param.Validate();
        }


        // ----------------------------------------------------------------
        // Tests of New Line from Raw Lines
        // ----------------------------------------------------------------

        // TestParameter for NewLineFromRawLineTest
        public class NewLineFromRawLineTestParameter : LineTestParameter
        {
            // ACT
            public override void Act(out object[] actual)
            {
                var line = new PrivateProfileLine(this.RawLine);

                actual = this.LineType switch
                {
                    PrivateProfileLineType.NotInitialized => throw new NotSupportedException(),
                    PrivateProfileLineType.Section => new object[] { line.LineType, line.RawLine, line.SectionName, null, null },
                    PrivateProfileLineType.Entry => new object[] { line.LineType, line.RawLine, null, line.Key, line.Value },
                    PrivateProfileLineType.Other => new object[] { line.LineType, line.RawLine, null, null, null },
                    _ => throw new InvalidOperationException(),
                };
            }
        }

        [DataTestMethod]
        // Common Test Case for NewLineTest, ParseMethodTest and NewLineFromRawLineTest.
        [DataRow(PrivateProfileLineType.Section, "[SECTION]", "SECTION", null, null)]
        [DataRow(PrivateProfileLineType.Section, "[Valid[Section]", "Valid[Section", null, null)]
        [DataRow(PrivateProfileLineType.Section, "[Valid]Section]", "Valid]Section", null, null)]
        [DataRow(PrivateProfileLineType.Section, "[Valid[Valid]Section]", "Valid[Valid]Section", null, null)]
        [DataRow(PrivateProfileLineType.Section, "[Section=Valid]", "Section=Valid", null, null)]
        [DataRow(PrivateProfileLineType.Entry, "KEY=VALUE", null, "KEY", "VALUE")]
        [DataRow(PrivateProfileLineType.Entry, "KEY", null, "KEY", null)]
        [DataRow(PrivateProfileLineType.Entry, "KEY=", null, "KEY", "")]
        [DataRow(PrivateProfileLineType.Entry, "KEY=VALUE=Valid", null, "KEY", "VALUE=Valid")]
        //
        // Common Test Case only for NewLineTest and NewLineFromRawLineTest.
        [DataRow(PrivateProfileLineType.Section, null, "", null, null, DisplayName = "Null Section")]
        //
        // Common Test Case only for ParseMethodTest and NewLineFromRawLineTest.
        [DataRow(PrivateProfileLineType.Other, "", null, null, null, DisplayName = "Blank Line")]
        [DataRow(PrivateProfileLineType.Other, ";This is comment line.", null, null, null, DisplayName = "Comment Line")]
        [DataRow(PrivateProfileLineType.Section, "[SECTION] ;Comment", "SECTION", null, null, DisplayName = "Section Line with Comment")]
        [DataRow(PrivateProfileLineType.Entry, "KEY=VALUE", null, "KEY", "VALUE", DisplayName = "Entry Line with Comment")]
        public void NewLineFromRawLineTest(PrivateProfileLineType type, string line, string section, string key, string value)
        {
            // SET Parameter
            NewLineFromRawLineTestParameter param = new NewLineFromRawLineTestParameter
            {
                LineType = type,
                RawLine = line,
                SectionName = section,
                Key = key,
                Value = value
            };

            // ASSERT
            param.Validate();
        }


        // ----------------------------------------------------------------
        // Tests of SectionName, Key and Value Properties
        // ----------------------------------------------------------------

        // Test Target: Section, Key or Value
        public enum SectionKeyValuePropertiesChangeTestTarget
        {
            SectionName,
            Key,
            Value
        }

        // Base Type of TestParameter for SectionNamePropertyChangeTest, KeyPropertyChangeTest and ValuePropertyChangeTest
        public abstract class SectionKeyValuePropertiesChangeTestParameter : LineTestParameter
        {
            // Additional value
            public string BeforeChange;

            // GET Actual
            public object[] GetActual(SectionKeyValuePropertiesChangeTestTarget target)
            {
                // Prepare Line
                PrivateProfileLine line;

                switch (target)
                {
                    case SectionKeyValuePropertiesChangeTestTarget.SectionName:

                        // New Line for SectionNamePropertyChangeTest
                        line = new PrivateProfileLine() { SectionName = this.BeforeChange };

                        // ACT: Change value
                        line.SectionName = this.SectionName;

                        // RETURN
                        return new object[] { line.LineType, line.RawLine, line.SectionName, null, null };

                    case SectionKeyValuePropertiesChangeTestTarget.Key:

                        // New Line for KeyPropertyChangeTest
                        line = new PrivateProfileLine() { Key = this.BeforeChange, Value = this.Value };

                        // ACT: Change KEY
                        line.Key = this.Key;

                        // RETURN
                        return new object[] { line.LineType, line.RawLine, null, line.Key, line.Value };

                    case SectionKeyValuePropertiesChangeTestTarget.Value:

                        // New Line for ValuePropertyChangeTest
                        line = new PrivateProfileLine() { Key = this.Key, Value = this.BeforeChange };

                        // ACT: Change VALUE
                        line.Value = this.Value;

                        // RETURN
                        return new object[] { line.LineType, line.RawLine, null, line.Key, line.Value };

                    default:
                        throw new InternalTestFailureException();
                }
            }
        }


        // ----------------------------------------------------------------
        // Tests of SectionName Property Change
        // ----------------------------------------------------------------

        // TestParameter for SectionNamePropertyChangeTest
        public class SectionNamePropertyChangeTestParameter : SectionKeyValuePropertiesChangeTestParameter
        {
            // ACT: GET Actual
            public override void Act(out object[] actual) => actual = this.GetActual(SectionKeyValuePropertiesChangeTestTarget.SectionName);
        }

        [DataTestMethod]
        [DataRow(PrivateProfileLineType.Section, "[AfterSection]", "AfterSection", null, null, "BeforeSection")]
        [DataRow(PrivateProfileLineType.Section, "[AfterSection]", "AfterSection", null, null, "BeforeSection ")]
        [DataRow(PrivateProfileLineType.Section, "[AfterSection]", "AfterSection", null, null, "BeforeSection  ")]
        [DataRow(PrivateProfileLineType.Section, "[AfterSection]", "AfterSection", null, null, "BeforeSection\t")]
        [DataRow(PrivateProfileLineType.Section, "[AfterSection]", "AfterSection", null, null, " BeforeSection ")]
        [DataRow(PrivateProfileLineType.Section, null, "", null, null, "BeforeSection", DisplayName = "Change into Null Section")]
        [DataRow(PrivateProfileLineType.Section, "[AfterSection]", "AfterSection", null, null, "", DisplayName = "Change from Null Section")]
        public void SectionNamePropertyChangeTest(PrivateProfileLineType type, string line, string section, string key, string value, string beforeSection)
        {
            // SET Parameter
            SectionNamePropertyChangeTestParameter param = new SectionNamePropertyChangeTestParameter
            {
                LineType = type,
                RawLine = line,
                SectionName = section,
                Key = key,
                Value = value,
                BeforeChange = beforeSection
            };

            // ASSERT
            param.Validate();
        }


        // ----------------------------------------------------------------
        // Tests of Key Property Change
        // ----------------------------------------------------------------

        // TestParameter for KeyPropertyChangeTest
        public class KeyPropertyChangeTestParameter : SectionKeyValuePropertiesChangeTestParameter
        {
            // ACT: GET Actual
            public override void Act(out object[] actual) => actual = this.GetActual(SectionKeyValuePropertiesChangeTestTarget.Key);
        }

        [DataTestMethod]
        [DataRow(PrivateProfileLineType.Entry, "AfterKey=VALUE", null, "AfterKey", "VALUE", "BeforeKey")]
        [DataRow(PrivateProfileLineType.Entry, "AfterKey=VALUE", null, "AfterKey", "VALUE", "BeforeKey ")]
        [DataRow(PrivateProfileLineType.Entry, "AfterKey=VALUE", null, "AfterKey", "VALUE", " BeforeKey ")]
        public void KeyPropertyChangeTest(PrivateProfileLineType type, string line, string section, string key, string value, string beforeKey)
        {
            // SET Parameter
            KeyPropertyChangeTestParameter param = new KeyPropertyChangeTestParameter
            {
                LineType = type,
                RawLine = line,
                SectionName = section,
                Key = key,
                Value = value,
                BeforeChange = beforeKey
            };

            // ASSERT
            param.Validate();
        }


        // ----------------------------------------------------------------
        // Tests of Value Property
        // ----------------------------------------------------------------

        // TestParameter for ValuePropertyChangeTest
        public class ValuePropertyChangeTestParameter : SectionKeyValuePropertiesChangeTestParameter
        {
            // ACT: GET Actual
            public override void Act(out object[] actual) => actual = this.GetActual(SectionKeyValuePropertiesChangeTestTarget.Value);
        }

        [DataTestMethod]
        [DataRow(PrivateProfileLineType.Entry, "KEY=AfterValue", null, "KEY", "AfterValue", "BeforeValue")]
        [DataRow(PrivateProfileLineType.Entry, "KEY=AfterValue", null, "KEY", "AfterValue", "BeforeValue ")]
        [DataRow(PrivateProfileLineType.Entry, "KEY=AfterValue", null, "KEY", "AfterValue", " BeforeValue ")]
        public void ValuePropertyChangeTest(PrivateProfileLineType type, string line, string section, string key, string value, string beforeValue)
        {
            // SET Parameter
            ValuePropertyChangeTestParameter param = new ValuePropertyChangeTestParameter
            {
                LineType = type,
                RawLine = line,
                SectionName = section,
                Key = key,
                Value = value,
                BeforeChange = beforeValue
            };

            // ASSERT
            param.Validate();
        }
    }
}
