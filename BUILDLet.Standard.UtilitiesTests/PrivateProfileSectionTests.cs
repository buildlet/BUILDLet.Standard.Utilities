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


            // ARRANGE: SET Expected
            // { string Name, string[][] Entries, string[] RawLines }
            public override void Arrange(out object[] expected) =>
                expected = new object[] { this.Name, this.Entries, this.RawLines };


            // Utility to convert Section into Object[]
            public static object[] ConvertSectionToObjectArray(PrivateProfileSection section)
            {
                // GET Entries
                string[][] entries = new string[section.Entries.Count][];
                var i = 0;
                foreach (var key in section.Entries.Keys)
                {
                    entries[i++] = new string[] { key, section.Entries[key] };
                }

                // RETURN
                return new object[]
                {
                    section.Name,
                    entries,
                    section.GetRawLines()
                };
            }


            // ASSERT
            public override void Assert<TItem>(TItem expected, TItem actual)
            {
                string expectedSectionName = (expected as object[])[0] as string;
                string[][] expectedEntries = (expected as object[])[1] as string[][];
                string[] expectedRawLines = (expected as object[])[2] as string[];

                string actualSectionName = (actual as object[])[0] as string;
                string[][] actualEntries = (actual as object[])[1] as string[][];
                string[] actualRawLines = (actual as object[])[2] as string[];

                // ASSERT Section
                PrivateProfileSectionTests.AssertSection(
                    expectedSectionName, expectedEntries, expectedRawLines,
                    actualSectionName, actualEntries, actualRawLines);
            }
        }


        // ----------------------------------------------------------------
        // Utilities
        // ----------------------------------------------------------------

        // Assert Section
        public static void AssertSection(
            string expectedSectionName, string[][] expectedEntries, string[] expectedRawLines,
            string actualSectionName, string[][] actualEntries, string[] actualRawLines)
        {
            // Print blank line
            Console.WriteLine();

            // Print Section Name
            Console.WriteLine($"Section Name: Expected \t= \"{expectedSectionName}\"");
            Console.WriteLine($"Section Name: Actual \t= \"{actualSectionName}\"");
            Console.WriteLine();

            // ASSERT Section Name
            Assert.AreEqual(expectedSectionName, actualSectionName);


            // Print Number of Entries
            Console.WriteLine($"Number of Entries: Expected = {expectedEntries.Length}, Actual = {actualEntries.Length}");

            // ASSERT Number of Entries
            Assert.AreEqual(expectedEntries.Length, actualEntries.Length);


            // for Entries
            for (int i = 0; i < expectedEntries.Length; i++)
            {
                // Print Entry (KEY and VALUE)
                Console.WriteLine($"Entries[{i}]: Expected (Key, Value)\t= (\"{expectedEntries[i][0]}\", \"{expectedEntries[i][1]}\")");
                Console.WriteLine($"Entries[{i}]: Actual (Key, Value)\t= (\"{actualEntries[i][0]}\", \"{actualEntries[i][1]}\")");

                // ASSERT Entry
                Assert.AreEqual(2, actualEntries[i].Length);
                Assert.AreEqual(expectedEntries[i][0], actualEntries[i][0]);
                Assert.AreEqual(expectedEntries[i][1], actualEntries[i][1]);
            }


            // for Raw Lines
            if ((expectedRawLines != null) && (actualRawLines != null))
            {
                // Print blank line
                Console.WriteLine();

                // Print Raw Lines
                Console.WriteLine($"Number of Raw Lines: Expected = {expectedRawLines.Length}, Actual = {actualRawLines.Length}");

                // ASSERT Raw Lines
                Assert.AreEqual(expectedRawLines.Length, actualRawLines.Length);


                // for internal validation -> This is NOT applied to Null Section.
                // Assert.AreEqual(expected_RawLines.Length, expected_Entries.Length + 1);


                // for Raw Lines
                for (int i = 0; i < expectedRawLines.Length; i++)
                {
                    // Print Raw Line
                    Console.WriteLine($"Raw Lines[{i}]: Expected\t= \"{expectedRawLines[i]}\"");
                    Console.WriteLine($"Raw Lines[{i}]: Actual\t= \"{actualRawLines[i]}\"");

                    // ASSERT Raw Line
                    Assert.AreEqual(expectedRawLines[i], actualRawLines[i]);
                }
            }
        }


        // Print Section
        public static void PrintSection(PrivateProfileSection section)
        {
            // Print Section Name
            Console.WriteLine("Section Name = " + (section.Name is null ? "(null)" : $"\"{section.Name}\""));

            // for Entries
            var i = 0;
            foreach (var key in section.Entries.Keys)
            {
                // Print Entry
                Console.WriteLine("Entries[{0}] = ({1}, {2})", i++, key, (section.Entries[key] is null ? "null" : $"\"{section.Entries[key]}\""));
            }
        }


        // ----------------------------------------------------------------
        // Tests of New Section
        // ----------------------------------------------------------------

        // TestParameter for NewSectionFromLinesTest
        public class NewSectionFromRawLinesTestParameter : SectionTestParameter
        {
            // ACT
            public override void Act(out object[] actual)
            {
                // ACT: NEW Section
                var section = new PrivateProfileSection(this.RawLines);

                // GET Actual
                actual = SectionTestParameter.ConvertSectionToObjectArray(section);
            }
        }

        // Test Data for New Section; NewSectionFromRawLinesTest and NewSectionFromPrivateProfileLinesTest
        public static IEnumerable<object[]> NewSectionTestData => new List<object[]>()
        {
            // object[]
            // {
            //    // Section Name
            //    string,
            //
            //    // Entries
            //    string[][]
            //    {
            //        string[],
            //        string[],
            //        :
            //    },
            //
            //    // Raw Lines
            //    string[]
            // }

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
            // ACT
            public override void Act(out object[] actual)
            {
                // NEW Lines
                var lines = new PrivateProfileLine[this.RawLines.Length];
                for (int i = 0; i < this.RawLines.Length; i++)
                {
                    // NEW Line
                    lines[i] = new PrivateProfileLine(this.RawLines[i]);
                }


                // ACT: NEW Section
                var section = new PrivateProfileSection(lines);


                // GET Actual
                actual = SectionTestParameter.ConvertSectionToObjectArray(section);
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
            // Placeholder of Section
            protected PrivateProfileSection Section = null;

            // Raw Lines before change
            public string[] BeforeRawLines = null;


            // ARRANGE
            public override void Arrange(out object[] expected)
            {
                // BASE
                base.Arrange(out expected);


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
                actual = SectionTestParameter.ConvertSectionToObjectArray(this.Section);
            }
        }

        // Test Data for SectionNamePropertyChangeTest
        public static IEnumerable<object[]> SectionNamePropertyChangeTestData => new List<object[]>()
        {
            // object[]
            // {
            //    // Section Name
            //    string,
            //
            //    // Entries
            //    string[][] { string[], string[],.. },
            //
            //    // Raw Lines
            //    string[],
            //    
            //    // BeforeRawLines: Additional Data for SectionNamePropertyChangeTest
            //    string[]
            // }

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

            // 3) Chang from Null Section
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
                    "KEY=VALUE"
                },
            },

            // 4) Change into Null Section
            new object[]
            {
                // Section Name (Empty: Null Section)
                "",

                // Entries
                new string[][]
                {
                    new string[] { "KEY", "VALUE" }
                },

                // Raw Lines
                new string[]
                {
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
                // BASE
                base.Arrange(out expected);


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
                actual = SectionTestParameter.ConvertSectionToObjectArray(this.Section);
            }
        }

        // Test Data for AppendMethodTest
        public static IEnumerable<object[]> AppendMethodTestData => new List<object[]>()
        {
            // object[]
            // {
            //    // Section Name
            //    string,
            //
            //    // Entries
            //    string[][] { string[], string[],.. },
            //
            //    // Raw Lines
            //    string[],
            //
            //    // BeforeRawLines: Additional Data for AppendMethodTest (1)
            //    string[],
            //    
            //    // NewLine: Additional Data for AppendMethodTest (2)
            //    string
            // }

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

            // 3) Null Section
            new object[]
            {
                // Section Name
                "",

                // Entries
                new string[][]
                {
                    new string[] { "KEY1", "VALUE1" },
                    new string[] { "KEY2", "VALUE2" }
                },

                // Raw Lines
                new string[]
                {
                    "KEY1=VALUE1",
                    "KEY2=VALUE2"
                },

                // BeforeRawLines: Additional Data for AppendMethodTest (1)
                new string[]
                {
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
                // BASE
                base.Arrange(out expected);


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
                actual = SectionTestParameter.ConvertSectionToObjectArray(this.Section);
            }
        }

        // Test Data for UpdateMethodTest
        public static IEnumerable<object[]> UpdateMethodTestData => new List<object[]>()
        {
            // object[]
            // {
            //    // Section Name
            //    string,
            //
            //    // Entries
            //    string[][] { string[], string[],.. },
            //
            //    // Raw Lines
            //    string[],
            //
            //    // BeforeRawLines: Additional Data for UpdateMethodTest (1)
            //    string[],
            //    
            //    // Key to be updated: Additional Data for UpdateMethodTest (2)
            //    string,
            //    
            //    // NewValue: Additional Data for UpdateMethodTest (3)
            //    string
            // }

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

                // BeforeRawLines: Additional Data for UpdateMethodTest (1)
                new string[]
                {
                    "[SECTION]",
                    "KEY=Before"
                },

                // Key to be updated: Additional Data for UpdateMethodTest (2)
                "KEY",

                // NewValue: Additional Data for UpdateMethodTest (3)
                "VALUE"
            },

            // 3) Null Section
            new object[]
            {
                // Section Name
                "",

                // Entries
                new string[][]
                {
                    new string[] { "KEY", "VALUE" }
                },

                // Raw Lines
                new string[]
                {
                    "KEY=VALUE",
                },

                // BeforeRawLines: Additional Data for UpdateMethodTest (1)
                new string[]
                {
                    "KEY=Before"
                },

                // Key to be updated: Additional Data for UpdateMethodTest (2)
                "KEY",

                // NewValue: Additional Data for UpdateMethodTest (3)
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
                // BASE
                base.Arrange(out expected);

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
                actual = SectionTestParameter.ConvertSectionToObjectArray(this.Section);
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
            // object[]
            // {
            //    // Section Name
            //    string,
            //
            //    // Entries
            //    string[][] { string[], string[],.. },
            //
            //    // Raw Lines
            //    string[],
            //
            //    // BeforeRawLines: Additional Data for RemoveMethodTest (1)
            //    string[],
            //    
            //    // Key to be removed: Additional Data for RemoveMethodTest (2)
            //    string,
            //    
            //    // Return value of Remove() method: Additional Data for RemoveMethodTest (3)
            //    bool
            // }

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

                // Key to be removed: Additional Data for RemoveMethodTest (2)
                "KEY2",

                // Return value of Remove() method: Additional Data for RemoveMethodTest (3)
                true
            },

            // 3)
            new object[]
            {
                // Section Name
                "SECTION",

                // Entries
                new string[][]
                {
                },

                // Raw Lines
                new string[]
                {
                    "[SECTION]",
                },

                // BeforeRawLines: Additional Data for RemoveMethodTest (1)
                new string[]
                {
                    "[SECTION]",
                    "KEY1=VALUE1",
                },

                // Key: Additional Data for RemoveMethodTest (2)
                "KEY1",

                // NewValue: Additional Data for RemoveMethodTest (3)
                true
            },

            // 4) Null Section
            new object[]
            {
                // Section Name
                "",

                // Entries
                new string[][]
                {
                },

                // Raw Lines
                new string[]
                {
                },

                // BeforeRawLines: Additional Data for RemoveMethodTest (1)
                new string[]
                {
                    "KEY1=VALUE1",
                },

                // Key: Additional Data for RemoveMethodTest (2)
                "KEY1",

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
