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
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO; // for MemoryStream

using BUILDLet.UnitTest.Utilities; // for TestParameter Class

namespace BUILDLet.Standard.Utilities.Tests
{
    [TestClass]
    public class PrivateProfileSectionTests
    {
        // ----------------------------------------------------------------
        // Base Type of TestParameter
        // ----------------------------------------------------------------

        // Base Type of TestParameter for PrivateProfileSection Class Tests
        public abstract class SectionTestParameter : TestParameter<object[]>
        {
            public string Name = null;
            public string[][] Entries = null;
            public string[] RawLines = null;

            // GET Expected
            protected object[] GetExpected() => new object[] { this.Name, this.Entries, this.RawLines };

            // GET Object[] from Section
            protected object[] GetTestData(PrivateProfileSection section)
            {
                // GET Entries
                string[][] entries = new string[section.Entries.Count][];

                // Copy to Array
                var i = 0;
                foreach (var key in section.Entries.Keys)
                {
                    entries[i++] = new string[] { key, section.Entries[key] };
                }

                // RETURN
                return new object[]
                {
                    section.Name,
                    Entries = entries,
                    RawLines = section.GetRawLines()
                };
            }

            // ASSERT
            public override void Assert<TItem>(TItem expected, TItem actual)
            {
                string expected_Name = (expected as object[])[0] as string;
                string[][] expected_Entries = (expected as object[])[1] as string[][];
                string[] expected_RawLines = (expected as object[])[2] as string[];

                string actual_Name = (actual as object[])[0] as string;
                string[][] actual_Entries = (actual as object[])[1] as string[][];
                string[] actual_RawLines = (actual as object[])[2] as string[];


                // Print Section Name
                Console.WriteLine($"Name: Expected\t= \"{expected_Name}\"");
                Console.WriteLine($"Name: Actual\t= \"{actual_Name}\"");

                // ASSERT Section Name
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected_Name, actual_Name);


                // Print Number of Entries and Raw Lines
                Console.WriteLine($"Number of Entries: Expected = {expected_Entries.Length}, Actual = {actual_Entries.Length}");
                Console.WriteLine($"Number of Raw Lines: Expected = {expected_RawLines.Length}, Actual = {actual_RawLines.Length}");

                // for internal validation
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected_RawLines.Length, expected_Entries.Length + 1);

                // ASSERT Number of Entries and Raw Lines
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected_Entries.Length, actual_Entries.Length);
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected_RawLines.Length, actual_RawLines.Length);


                // for Entries
                for (int i = 0; i < expected_Entries.Length; i++)
                {
                    // // Print Numbers in Entry
                    // Console.WriteLine($"Numbers in Entry[{i}]: Expected = {expected_Entries[i].Length}, Actual = {actual_Entries[i].Length}");

                    // // ASSERT Numbers in Entry
                    // Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected_Entries[i].Length, actual_Entries[i].Length);

                    // Print Entry (KEY and VALUE)
                    Console.WriteLine($"Entries[{i}]: Expected\t(Key, Value) = (\"{expected_Entries[i][0]}\", \"{expected_Entries[i][1]}\")");
                    Console.WriteLine($"Entries[{i}]: Actual\t(Key, Value) = (\"{actual_Entries[i][0]}\", \"{actual_Entries[i][1]}\")");

                    // ASSERT Entry
                    Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected_Entries[i][0], actual_Entries[i][0]);
                    Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected_Entries[i][1], actual_Entries[i][1]);
                }


                // for Raw Lines
                for (int i = 0; i < expected_RawLines.Length; i++)
                {
                    // Print Raw Line
                    Console.WriteLine($"Raw Lines[{i}]: Expected\t= \"{expected_RawLines[i]}\"");
                    Console.WriteLine($"Raw Lines[{i}]: Actual\t= \"{actual_RawLines[i]}\"");

                    // ASSERT Raw Line
                    Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected_RawLines[i], actual_RawLines[i]);
                }
            }
        }


        // ----------------------------------------------------------------
        // Utilities
        // ----------------------------------------------------------------

        // Print Section
        public static void PrintSection(PrivateProfileSection section)
        {
            Console.WriteLine("Section Name = " + (section.Name is null ? "(null)" : $"\"{section.Name}\""));
            var i = 0;
            foreach (var key in section.Entries.Keys)
            {
                Console.WriteLine("Entries[{0}] = ({1}, {2})", i++, key, (section.Entries[key] is null ? "null" : $"\"{section.Entries[key]}\""));
            }
        }


        // ----------------------------------------------------------------
        // Tests of New Section
        // ----------------------------------------------------------------

        // TestParameter for NewSectionFromLinesTest
        public class NewSectionFromRawLinesTestParameter : SectionTestParameter
        {
            // ARRANGE: SET Expected
            public override void Arrange(out object[] expected) => expected = this.GetExpected();

            // ACT
            public override void Act(out object[] actual)
            {
                // ACT: NEW Section
                var section = new PrivateProfileSection(this.RawLines);

                // GET Actual
                actual = this.GetTestData(section);
            }
        }

        // Test Data for New Section; NewSectionFromRawLinesTest and NewSectionFromPrivateProfileLinesTest
        public static IEnumerable<object[]> NewSectionTestData => new List<object[]>()
        {
            // 1)
            // (None)

            // 2)
            new object[]
            {
                // Section Name
                "SECTION",

                // Entries
                new string[][]
                {
                    new string[] { "KEY", "VALUE" }
                },

                // Raw Lines
                new string[]
                {
                    "[SECTION]",
                    "KEY=VALUE"
                }
            },

            // 3)
            new object[]
            {
                // Section Name
                "SECTION",

                // Entries
                new string[][]
                {
                    new string[] { "KEY1", "VALUE1" },
                    new string[] { "KEY2", "VALUE2" }
                },

                // Raw Lines
                new string[]
                {
                    "[SECTION]",
                    "KEY1=VALUE1",
                    "KEY2=VALUE2"
                }
            }
        };

        [DataTestMethod]
        [DynamicData(nameof(NewSectionTestData))]
        public void NewSectionFromRawLinesTest(string name, string[][]entries, string[] lines)
        {
            // SET Parameter
            NewSectionFromRawLinesTestParameter param = new NewSectionFromRawLinesTestParameter
            {
                Name = name,
                RawLines = lines,
                Entries = entries
            };

            // ASSERT
            param.Validate(autoEnumerable: false);
        }


        // TestParameter for NewSectionFromPrivateProfileLinesTest
        public class NewSectionFromPrivateProfileLinesTestParameter : SectionTestParameter
        {
            // ARRANGE: SET Expected
            public override void Arrange(out object[] expected) => expected = this.GetExpected();

            // ACT
            public override void Act(out object[] actual)
            {
                // NEW Lines
                var lines = new PrivateProfileLine[this.RawLines.Length];

                // for Lines
                for (int i = 0; i < this.RawLines.Length; i++)
                {
                    // NEW Line
                    lines[i] = new PrivateProfileLine(this.RawLines[i]);
                }

                // ACT: NEW Section
                var section = new PrivateProfileSection(lines);

                // GET Actual
                actual = this.GetTestData(section);
            }
        }

        // Test Data for NewSectionFromPrivateProfileLinesTest
        // (NewSectionTestData: Same as NewSectionFromRawLinesTest)

        [DataTestMethod]
        [DynamicData(nameof(NewSectionTestData))]
        public void NewSectionFromPrivateProfileLinesTest(string name, string[][] entries, string[] lines)
        {
            // SET Parameter
            NewSectionFromPrivateProfileLinesTestParameter param = new NewSectionFromPrivateProfileLinesTestParameter
            {
                Name = name,
                Entries = entries,
                RawLines = lines
            };

            // ASSERT
            param.Validate(autoEnumerable: false);
        }


        // ----------------------------------------------------------------
        // Tests of Name Property Change
        // ----------------------------------------------------------------

        // TestParameter for NewSectionFromLinesTest
        public class NamePropertyChangeTestParameter : SectionTestParameter
        {
            // Raw Lines before change
            public string[] BeforeRawLines = null;


            // Placeholder of Section
            protected PrivateProfileSection Section = null;

            // ARRANGE
            public override void Arrange(out object[] expected)
            {
                // SET Expected
                expected = this.GetExpected();

                // NEW Section
                this.Section = new PrivateProfileSection(this.BeforeRawLines);

                // Print Section Name
                Console.WriteLine("Before Change:");
                Console.WriteLine($"Section Name = \"{this.Section.Name}\"");
                Console.WriteLine();
            }

            // ACT
            public override void Act(out object[] actual)
            {
                // ACT: Change Section Name
                this.Section.Name = this.Name;

                // GET Actual
                actual = this.GetTestData(this.Section);
            }
        }

        // Test Data for SectionNamePropertyChangeTest
        public static IEnumerable<object[]> SectionNamePropertyChangeTestData => new List<object[]>()
        {
            // 1)
            // (None)

            // 2)
            new object[]
            {
                // Section Name
                "SECTION",

                // Entries
                new string[][]
                {
                    new string[] { "KEY", "VALUE" }
                },

                // Raw Lines
                new string[]
                {
                    "[SECTION]",
                    "KEY=VALUE"
                },

                // BeforeRawLines: Additional Data for SectionNamePropertyChangeTest
                new string[]
                {
                    "[Before]",
                    "KEY=VALUE"
                },
            },
        };

        [DataTestMethod]
        [DynamicData(nameof(SectionNamePropertyChangeTestData))]
        public void NamePropertyChangeTest(string name, string[][] entries, string[] lines, string[] beforeRawLines)
        {
            // SET Parameter
            NamePropertyChangeTestParameter param = new NamePropertyChangeTestParameter
            {
                Name = name,
                Entries = entries,
                RawLines = lines,
                BeforeRawLines = beforeRawLines
            };

            // ASSERT
            param.Validate(autoEnumerable: false);
        }


        // ----------------------------------------------------------------
        // Tests of Append Method
        // ----------------------------------------------------------------

        // TestParameter for AppendMethodTest
        public class AppendMethodTestParameter : SectionTestParameter
        {
            // Placeholder of Section
            protected PrivateProfileSection Section = null;

            // Raw Lines before change
            public string[] BeforeRawLines = null;

            // Line to be Appended
            public string NewLine = null;


            // ARRANGE
            public override void Arrange(out object[] expected)
            {
                // SET Expected
                expected = this.GetExpected();

                // NEW Section
                this.Section = new PrivateProfileSection(this.BeforeRawLines);

                // Print Entries before change
                Console.WriteLine("Before Change:");
                PrivateProfileSectionTests.PrintSection(this.Section);
                Console.WriteLine();
            }

            // ACT
            public override void Act(out object[] actual)
            {
                // ACT: Append NEW Line
                this.Section.Append(new PrivateProfileLine(this.NewLine));

                // GET Actual
                actual = this.GetTestData(this.Section);
            }
        }

        // Test Data for AppendMethodTest
        public static IEnumerable<object[]> AppendMethodTestData => new List<object[]>()
        {
            // 1)
            // (None)

            // 2)
            new object[]
            {
                // Section Name
                "SECTION",

                // Entries
                new string[][]
                {
                    new string[] { "KEY1", "VALUE1" },
                    new string[] { "KEY2", "VALUE2" }
                },

                // Raw Lines
                new string[]
                {
                    "[SECTION]",
                    "KEY1=VALUE1",
                    "KEY2=VALUE2"
                },

                // BeforeRawLines: Additional Data for AppendMethodTest (1)
                new string[]
                {
                    "[SECTION]",
                    "KEY1=VALUE1"
                },

                // NewLine: Additional Data for AppendMethodTest (2)
                "KEY2=VALUE2"
            },
        };

        [DataTestMethod]
        [DynamicData(nameof(AppendMethodTestData))]
        public void AppendMethodTest(string name, string[][] entries, string[] lines, string[] beforeRawLines, string line)
        {
            // SET Parameter
            AppendMethodTestParameter param = new AppendMethodTestParameter
            {
                Name = name,
                Entries = entries,
                RawLines = lines,
                BeforeRawLines = beforeRawLines,
                NewLine = line
            };

            // ASSERT
            param.Validate(autoEnumerable: false);
        }


        // ----------------------------------------------------------------
        // Tests of Update Method
        // ----------------------------------------------------------------

        // TestParameter for UpdateMethodTest
        public class UpdateMethodTestParameter : SectionTestParameter
        {
            // Placeholder of Section
            protected PrivateProfileSection Section = null;

            // Raw Lines before change
            public string[] BeforeRawLines = null;


            // KEY to Update
            public string Key = null;

            // New VALUE
            public string NewValue = null;


            // ARRANGE
            public override void Arrange(out object[] expected)
            {
                // SET Expected
                expected = this.GetExpected();

                // NEW Section
                this.Section = new PrivateProfileSection(this.BeforeRawLines);

                // Print Entries before change
                Console.WriteLine("Before Change:");
                PrivateProfileSectionTests.PrintSection(this.Section);
                Console.WriteLine();
            }

            // ACT
            public override void Act(out object[] actual)
            {
                // ACT: Append NEW Line
                this.Section.Update(this.Key, this.NewValue);

                // GET Actual
                actual = this.GetTestData(this.Section);
            }
        }

        // Test Data for UpdateMethodTest
        public static IEnumerable<object[]> UpdateMethodTestData => new List<object[]>()
        {
            // 1)
            // (None)

            // 2)
            new object[]
            {
                // Section Name
                "SECTION",

                // Entries
                new string[][]
                {
                    new string[] { "KEY", "VALUE" }
                },

                // Raw Lines
                new string[]
                {
                    "[SECTION]",
                    "KEY=VALUE",
                },

                // BeforeRawLines: Additional Data for UpdateMethodTestData (1)
                new string[]
                {
                    "[SECTION]",
                    "KEY=Before"
                },

                // Key: Additional Data for UpdateMethodTestData (2)
                "KEY",

                // NewValue: Additional Data for UpdateMethodTestData (3)
                "VALUE"
            },
        };

        [DataTestMethod]
        [DynamicData(nameof(UpdateMethodTestData))]
        public void UpdateMethodTest(string name, string[][] entries, string[] lines, string[] beforeRawLines, string key, string value)
        {
            // SET Parameter
            UpdateMethodTestParameter param = new UpdateMethodTestParameter
            {
                Name = name,
                Entries = entries,
                RawLines = lines,
                BeforeRawLines = beforeRawLines,
                Key = key,
                NewValue = value
            };

            // ASSERT
            param.Validate(autoEnumerable: false);
        }


        // ----------------------------------------------------------------
        // Tests of Remove Method
        // ----------------------------------------------------------------

        // TestParameter for RemoveMethodTest
        public class RemoveMethodTestParameter : SectionTestParameter
        {
            // Placeholder of Section
            protected PrivateProfileSection Section = null;

            // Raw Lines before change
            public string[] BeforeRawLines = null;


            // KEY to Remove
            public string Key = null;

            // Expected result of Remove() method
            public bool Result;

            // Return value of Remove() method
            private bool result;


            // ARRANGE
            public override void Arrange(out object[] expected)
            {
                // SET Expected
                expected = this.GetExpected();

                // NEW Section
                this.Section = new PrivateProfileSection(this.BeforeRawLines);

                // Print Entries before change
                Console.WriteLine("Before Change:");
                PrivateProfileSectionTests.PrintSection(this.Section);
                Console.WriteLine();
            }

            // ACT
            public override void Act(out object[] actual)
            {
                // ACT: Append NEW Line
                this.result = this.Section.Remove(this.Key);

                // Print return value of Remove() method
                Console.WriteLine($"Return value of Remove() method = {this.result}");

                // GET Actual
                actual = this.GetTestData(this.Section);
            }

            // ASSERT
            public override void Assert<TItem>(TItem expected, TItem actual)
            {
                // BASE
                base.Assert(expected, actual);

                // ASSERT for return value of Remove() method
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(this.Result, this.result);
            }
        }

        // Test Data for RemoveMethodTest
        public static IEnumerable<object[]> RemoveMethodTestData => new List<object[]>()
        {
            // 1)
            // (None)

            // 2)
            new object[]
            {
                // Section Name
                "SECTION",

                // Entries
                new string[][]
                {
                    new string[] { "KEY1", "VALUE1" }
                },

                // Raw Lines
                new string[]
                {
                    "[SECTION]",
                    "KEY1=VALUE1",
                },

                // BeforeRawLines: Additional Data for RemoveMethodTest (1)
                new string[]
                {
                    "[SECTION]",
                    "KEY1=VALUE1",
                    "KEY2=VALUE2"
                },

                // Key: Additional Data for RemoveMethodTest (2)
                "KEY2",

                // NewValue: Additional Data for RemoveMethodTest (3)
                true
            },
        };

        [DataTestMethod]
        [DynamicData(nameof(RemoveMethodTestData))]
        public void RemoveMethodTest(string name, string[][] entries, string[] lines, string[] beforeRawLines, string key, bool result)
        {
            // SET Parameter
            RemoveMethodTestParameter param = new RemoveMethodTestParameter
            {
                Name = name,
                Entries = entries,
                RawLines = lines,
                BeforeRawLines = beforeRawLines,
                Key = key,
                Result = result
            };

            // ASSERT
            param.Validate(autoEnumerable: false);
        }
    }
}
