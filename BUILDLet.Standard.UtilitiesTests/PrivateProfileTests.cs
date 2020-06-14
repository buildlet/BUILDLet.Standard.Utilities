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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

using BUILDLet.UnitTest.Utilities; // for TestParameter Class

namespace BUILDLet.Standard.Utilities.Tests
{
    [TestClass]
    public class PrivateProfileTests
    {
        // ----------------------------------------------------------------
        // Base Type of TestParameter
        // ----------------------------------------------------------------

        // Base Type of TestParameter for PrivateProfile Class Tests
        public abstract class PrivateProfileTestParameter : TestParameter<object[]>
        {
            public string FilePath;

            public string[] SectionNames = null;
            public string[][][] Entries = null;
            public string[] RawLines = null;

            // GET Expected
            protected object[] GetExpected() => new object[] { this.SectionNames, this.Entries, this.RawLines };

            // GET Object[] from Section
            protected object[] GetTestData(PrivateProfile profile)
            {
                // Copy Section Names
                string[] section_names = new string[profile.Sections.Count];
                profile.Sections.Keys.CopyTo(section_names, 0);

                // Copy Entries
                string[][][] entries = new string[profile.Sections.Count][][];
                var i = 0;
                foreach (var section_name in profile.Sections.Keys)
                {
                    // Copy Entries in this Section
                    entries[i] = new string[profile.Sections[section_name].Entries.Count][];
                    var j = 0;
                    foreach (var key in profile.Sections[section_name].Entries.Keys)
                    {
                        // for Entry
                        entries[i][j++] = new string[] { key, profile.Sections[section_name].Entries[key] };
                    }
                    i++;
                }

                // RETURN
                return new object[]
                {
                    SectionNames = section_names,
                    Entries = entries,
                    RawLines = profile.GetRawLines()
                };
            }

            // ASSERT
            public override void Assert<TItem>(TItem expected, TItem actual)
            {
                string[] expected_SectionNames = (expected as object[])[0] as string[];
                string[][][] expected_Entries = (expected as object[])[1] as string[][][];
                string[] expected_RawLines = (expected as object[])[2] as string[];

                string[] actual_SectionNames = (expected as object[])[0] as string[];
                string[][][] actual_Entries = (expected as object[])[1] as string[][][];
                string[] actual_RawLines = (expected as object[])[2] as string[];


                // Print Number of Sections
                Console.WriteLine($"Number of Sections: Expected = {expected_SectionNames.Length}, Actual = {actual_SectionNames.Length}");

                // ASSERT Number of Sections
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected_SectionNames.Length, actual_SectionNames.Length);

                // for Sections
                for (int i = 0; i < expected_SectionNames.Length; i++)
                {
                    // Print Section Name
                    Console.WriteLine($"Section Name [{i}]: Expected\t= \"{expected_SectionNames[i]}\"");
                    Console.WriteLine($"Section Name [{i}]: Actual\t= \"{actual_SectionNames[i]}\"");

                    // ASSERT Section Name
                    Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected_SectionNames[i], actual_SectionNames[i]);
                }


                // Print Number of Entries
                Console.WriteLine($"Number of Entries: Expected = {expected_Entries.Length}, Actual = {actual_Entries.Length}");

                // ASSERT Number of Entries
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected_Entries.Length, actual_Entries.Length);

                // for Entries
                for (int i = 0; i < expected_Entries.Length; i++)
                {
                    // Print Number of Entries[i]
                    Console.WriteLine($"Number of Entries[{i}]: Expected = {expected_Entries[i].Length}, Actual = {actual_Entries[i].Length}");

                    // ASSERT Number of Entries[i]
                    Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected_Entries[i].Length, actual_Entries[i].Length);

                    for (int j = 0; j < expected_Entries[i].Length; j++)
                    {
                        // // Print Numbers in Entry
                        // Console.WriteLine($"Numbers in Entries[{i}][{j}]: Expected = {expected_Entries[i][j].Length}, Actual = {actual_Entries[i][j].Length}");

                        // // ASSERT Numbers in Entry
                        // Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected_Entries[i][j].Length, actual_Entries[i][j].Length);

                        // Print Entry (KEY and VALUE)
                        Console.WriteLine($"Entries[{i}][{j}]: Expected\t(Key, Value) = (\"{expected_Entries[i][j][0]}\", \"{expected_Entries[i][j][1]}\")");
                        Console.WriteLine($"Entries[{i}][{j}]: Actual\t(Key, Value) = (\"{actual_Entries[i][j][0]}\", \"{actual_Entries[i][j][1]}\")");

                        // ASSERT Entry
                        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected_Entries[i][j][0], actual_Entries[i][j][0]);
                        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected_Entries[i][j][1], actual_Entries[i][j][1]);
                    }
                }


                // Print Number of Raw Lines
                Console.WriteLine($"Number of Raw Lines: Expected = {expected_RawLines.Length}, Actual = {actual_RawLines.Length}");

                // ASSERT Number of Raw Lines
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected_RawLines.Length, actual_RawLines.Length);

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

        // Print Profile
        public static void PrintProfile(PrivateProfile profile)
        {
            // Print Information: Null Section, Print Number of Sections, and so on...
            Console.WriteLine($"FileName = " + (profile.FileName is null ? "(null)" : $"\"{profile.FileName}\""));
            Console.WriteLine($"IsOpen = {profile.IsOpen}");
            Console.WriteLine($"IsReadOnly = {profile.IsReadOnly}");
            Console.WriteLine($"IsUpdated = {profile.IsUpdated}");
            Console.WriteLine($"Number of Sections = {profile.Sections.Count}");

            // Section Names
            var i = 0;
            foreach (var section_name in profile.Sections.Keys)
            {
                Console.WriteLine($"Sections[{i++}] Name = \"{profile.Sections[section_name].Name}\"");

                // Entries
                var j = 0;
                foreach (var key in profile.Sections[section_name].Entries.Keys)
                {
                    Console.WriteLine(" Entries[{0}][{1}] (Key, Value) = (\"{2}\", \"{3}\")", i, j++,
                        key, (profile.Sections[section_name].Entries[key] is null ? "null" : $"\"{profile.Sections[section_name].Entries[key]}\""));
                }
            }

            // Raw Lines
            var k = 0;
            foreach (var line in profile.GetRawLines())
            {
                Console.WriteLine($"Raw Lines[{k++}] = \"{line}\"");
            }
        }


        // Create Test File
        public static void CreateTestFile(string path, string[] contents)
        {
            // Create Content of Test File
            var content = new StringBuilder();
            foreach (var line in contents) { content.Append(line + PrivateProfile.LineBreakExpression); }

            // Craete Test File
            File.WriteAllText(path, content.ToString());
        }


        // ----------------------------------------------------------------
        // Tests of Read Profile
        // ----------------------------------------------------------------

        // TestParameter for ReadProfileTest
        public class ReadProfileTestParameter : PrivateProfileTestParameter
        {
            // ARRANGE
            public override void Arrange(out object[] expected)
            {
                // SET Expected
                expected = this.GetExpected();

                // Create Test File
                PrivateProfileTests.CreateTestFile(this.FilePath, this.RawLines);
            }

            // ACT
            public override void Act(out object[] actual)
            {
                // NEW PrivateProfile from INI file
                var profile = new PrivateProfile(this.FilePath);

                // Print Profile
                PrivateProfileTests.PrintProfile(profile);
                Console.WriteLine();

                // GET Actual
                actual = this.GetTestData(profile);
            }
        }

        // Test Data for Reading Profile
        public static IEnumerable<object[]> ReadProfileTestData => new List<object[]>()
        {
            // 1)
            // (None)

            // 2)
            new object[]
            {
                // File Path
                $@".\{nameof(ReadProfileTestData)}_002.ini",

                // Sections
                new string[]
                {
                    "SECTION"
                },

                // Entries
                new string[][][]
                {
                    new string[][]
                    {
                        new string[] { "KEY", "VALUE" }
                    }
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
                // File Path
                $@".\{nameof(ReadProfileTestData)}_003.ini",

                // Sections
                new string[]
                {
                    "SECTION"
                },

                // Entries
                new string[][][]
                {
                    new string[][]
                    {
                        new string[] { "KEY1", "VALUE1" },
                        new string[] { "KEY2", "VALUE2" }
                    }
                },

                // Raw Lines
                new string[]
                {
                    "[SECTION]",
                    "KEY1=VALUE1",
                    "KEY2=VALUE2"
                }
            },

            // 4)
            new object[]
            {
                // File Path
                $@".\{nameof(ReadProfileTestData)}_004.ini",

                // Sections
                new string[]
                {
                    "SECTION1",
                    "SECTION2"
                },

                // Entries
                new string[][][]
                {
                    // Section 1st
                    new string[][]
                    {
                        new string[] { "KEY1-1", "VALUE1-1" },
                        new string[] { "KEY1-2", "VALUE1-2" }
                    },

                    // Section 2nd
                    new string[][]
                    {
                        new string[] { "KEY2-1", "VALUE2-1" },
                        new string[] { "KEY2-2", "VALUE2-2" }
                    }
                },

                // Raw Lines
                new string[]
                {
                    "[SECTION1]",
                    "KEY1-1=VALUE1-1",
                    "KEY1-2=VALUE1-2",
                    "[SECTION2]",
                    "KEY2-1=VALUE2-1",
                    "KEY2-2=VALUE2-2"
                }
            }
        };

        [DataTestMethod]
        [DynamicData(nameof(ReadProfileTestData))]
        public void ReadProfileTest(string path, string[] sections, string[][][] entries, string[] lines)
        {
            // SET Parameter
            ReadProfileTestParameter param = new ReadProfileTestParameter
            {
                FilePath = path,
                SectionNames = sections,
                Entries = entries,
                RawLines = lines
            };

            // ASSERT
            param.Validate(autoEnumerable: false);
        }


        // ----------------------------------------------------------------
        // Base Type of TestParameter for Change Profile
        // ----------------------------------------------------------------

        // TestParameter Base for ChangeProfileTest
        public abstract class ChangeProfileTestParameter : PrivateProfileTestParameter
        {
            // Raw Lines before change
            public string[] BeforeRawLines = null;

            // SECTION of Entry to be changed
            public string Section = null;

            // KEY to change
            public string Key = null;


            // Abstract Method of ACT to be called by drived class
            public abstract void ChangeProfile(PrivateProfile profile);


            // ARRANGE
            public override void Arrange(out object[] expected)
            {
                // SET Expected
                expected = this.GetExpected();

                // Create Original Test File
                PrivateProfileTests.CreateTestFile(this.FilePath, this.BeforeRawLines);
            }

            // ACT
            public override void Act(out object[] actual)
            {
                // Read, Update & Write PrivateProfile
                using (var profile = new PrivateProfile(this.FilePath, false))
                {
                    // Print Profile
                    Console.WriteLine("Before Change:");
                    PrivateProfileTests.PrintProfile(profile);
                    Console.WriteLine();

                    // ACT (1): Change Profile
                    this.ChangeProfile(profile);

                    // Print Profile
                    Console.WriteLine("After Change:");
                    PrivateProfileTests.PrintProfile(profile);
                    Console.WriteLine();

                    // ACT (2): Write to File
                    profile.Write();
                }

                // Re-Read PrivateProfile
                using (var profile = new PrivateProfile(this.FilePath))
                {
                    // Print Profile
                    Console.WriteLine("Re-Read Profile:");
                    PrivateProfileTests.PrintProfile(profile);
                    Console.WriteLine();

                    // SET Actual
                    actual = this.GetTestData(profile);
                }
            }
        }


        // ----------------------------------------------------------------
        // Tests of Update Value
        // ----------------------------------------------------------------

        // TestParameter for UpdateValueTest
        public class UpdateValueTestParameter : ChangeProfileTestParameter
        {
            // New VALUE
            public string NewValue = null;

            // Change Profile
            public override void ChangeProfile(PrivateProfile profile)
            {
                // ACT: Update Value
                profile.SetValue(this.Section, this.Key, this.NewValue);
            }
        }

        // TestData for UpdateValueTest
        public static IEnumerable<object[]> UpdateValueTestData => new List<object[]>()
        {
            // 1)
            // (None)

            // 2)
            new object[]
            {
                // File Path
                $@".\{nameof(UpdateValueTestData)}_002.ini",

                // Sections
                new string[]
                {
                    "SECTION"
                },

                // Entries
                new string[][][]
                {
                    new string[][]
                    {
                        new string[] { "KEY", "VALUE" }
                    }
                },

                // Raw Lines
                new string[]
                {
                    "[SECTION]",
                    "KEY=VALUE"
                },

                // BeforeRawLines: Additional Data for UpdateValueTestData (1)
                new string[]
                {
                    "[SECTION]",
                    "KEY=Old Value"
                },

                // Section: Additional Data for UpdateValueTestData (2)
                "SECTION",

                // Key: Additional Data for UpdateValueTestData (3)
                "KEY",

                // New Value: Additional Data for UpdateValueTestData (4)
                "VALUE"
            },

            // 3)
            new object[]
            {
                // File Path
                $@".\{nameof(UpdateValueTestData)}_003.ini",

                // Sections
                new string[]
                {
                    "SECTION"
                },

                // Entries
                new string[][][]
                {
                    new string[][]
                    {
                        new string[] { "KEY1", "VALUE1" },
                        new string[] { "KEY2", "VALUE2" },
                        new string[] { "KEY3", "VALUE3" }
                    }
                },

                // Raw Lines
                new string[]
                {
                    "[SECTION]",
                    "KEY1=VALUE1",
                    "KEY2=VALUE2",
                    "KEY3=VALUE3"
                },

                // BeforeRawLines: Additional Data for UpdateValueTestData (1)
                new string[]
                {
                    "[SECTION]",
                    "KEY1=VALUE1",
                    "KEY2=Old Value",
                    "KEY3=VALUE3"
                },

                // Section: Additional Data for UpdateValueTestData (2)
                "SECTION",

                // Key: Additional Data for UpdateValueTestData (3)
                "KEY2",

                // New Value: Additional Data for UpdateValueTestData (4)
                "VALUE2"
            },

            // 4)
            new object[]
            {
                // File Path
                $@".\{nameof(UpdateValueTestData)}_004.ini",

                // Sections
                new string[]
                {
                    "SECTION1",
                    "SECTION2"
                },

                // Entries
                new string[][][]
                {
                    // Section 1st
                    new string[][]
                    {
                        new string[] { "KEY1-1", "VALUE1-1" },
                        new string[] { "KEY1-2", "VALUE1-2" }
                    },

                    // Section 2nd
                    new string[][]
                    {
                        new string[] { "KEY2-1", "VALUE2-1" },
                        new string[] { "KEY2-2", "VALUE2-2" },
                        new string[] { "KEY2-3", "VALUE2-3" }
                    }
                },

                // Raw Lines
                new string[]
                {
                    "[SECTION1]",
                    "KEY1-1=VALUE1-1",
                    "KEY1-2=VALUE1-2",
                    "[SECTION2]",
                    "KEY2-1=VALUE2-1",
                    "KEY2-2=VALUE2-2",
                    "KEY2-3=VALUE2-3"
                },

                // BeforeRawLines: Additional Data for UpdateValueTestData (1)
                new string[]
                {
                    "[SECTION1]",
                    "KEY1-1=VALUE1-1",
                    "KEY1-2=VALUE1-2",
                    "[SECTION2]",
                    "KEY2-1=VALUE2-1",
                    "KEY2-2=Old Value",
                    "KEY2-3=VALUE2-3"
                },

                // Section: Additional Data for UpdateValueTestData (2)
                "SECTION2",

                // Key: Additional Data for UpdateValueTestData (3)
                "KEY2-2",

                // New Value: Additional Data for UpdateValueTestData (4)
                "VALUE2-2"
            }
        };

        [DataTestMethod]
        [DynamicData(nameof(UpdateValueTestData))]
        public void UpdateValueTest(string path, string[] sections, string[][][] entries, string[] lines, string[] beforeRawLines, string section, string key, string value)
        {
            // SET Parameter
            UpdateValueTestParameter param = new UpdateValueTestParameter
            {
                FilePath = path,
                SectionNames = sections,
                Entries = entries,
                RawLines = lines,
                BeforeRawLines = beforeRawLines,
                Section = section,
                Key = key,
                NewValue = value
            };

            // ASSERT
            param.Validate(autoEnumerable: false);
        }


        // ----------------------------------------------------------------
        // Tests of Add Entry
        // ----------------------------------------------------------------

        // TestParameter for AddEntryTest
        public class AddEntryTestParameter : ChangeProfileTestParameter
        {
            // VALUE of Entry to be Added
            public string Value = null;

            public override void ChangeProfile(PrivateProfile profile)
            {
                // ACT: Add New Entry
                profile.SetValue(this.Section, this.Key, this.Value);
            }
        }

        // TestData for AddEntryTest
        public static IEnumerable<object[]> AddEntryTestData => new List<object[]>()
        {
            // 1)
            // (None)

            // 2)
            new object[]
            {
                // File Path
                $@".\{nameof(AddEntryTestData)}_002.ini",

                // Sections
                new string[]
                {
                    "SECTION"
                },

                // Entries
                new string[][][]
                {
                    new string[][]
                    {
                        new string[] { "KEY1", "VALUE1" },
                        new string[] { "KEY2", "VALUE2" }
                    }
                },

                // Raw Lines
                new string[]
                {
                    "[SECTION]",
                    "KEY1=VALUE1",
                    "KEY2=VALUE2"
                },

                // BeforeRawLines: Additional Data for AddEntryTestData (1)
                new string[]
                {
                    "[SECTION]",
                    "KEY1=VALUE1"
                },

                // Section: Additional Data for AddEntryTestData (2)
                "SECTION",

                // Key: Additional Data for AddEntryTestData (3)
                "KEY2",

                // Value: Additional Data for AddEntryTestData (4)
                "VALUE2"
            },

            // 3)
            new object[]
            {
                // File Path
                $@".\{nameof(AddEntryTestData)}_003.ini",

                // Sections
                new string[]
                {
                    "SECTION1",
                    "SECTION2"
                },

                // Entries
                new string[][][]
                {
                    // Section 1st
                    new string[][]
                    {
                        new string[] { "KEY1-1", "VALUE1-1" },
                        new string[] { "KEY1-2", "VALUE1-2" },
                        new string[] { "KEY1-3", "VALUE1-3" }
                    },

                    // Section 2nd
                    new string[][]
                    {
                        new string[] { "KEY2-1", "VALUE2-1" },
                        new string[] { "KEY2-2", "VALUE2-2" }
                    }
                },

                // Raw Lines
                new string[]
                {
                    "[SECTION1]",
                    "KEY1-1=VALUE1-1",
                    "KEY1-2=VALUE1-2",
                    "KEY1-3=VALUE1-3",
                    "[SECTION2]",
                    "KEY2-1=VALUE2-1",
                    "KEY2-2=VALUE2-2"
                },

                // BeforeRawLines: Additional Data for AddEntryTestData (1)
                new string[]
                {
                    "[SECTION1]",
                    "KEY1-1=VALUE1-1",
                    "KEY1-2=VALUE1-2",
                    "[SECTION2]",
                    "KEY2-1=VALUE2-1",
                    "KEY2-2=VALUE2-2"
                },

                // Section: Additional Data for AddEntryTestData (2)
                "SECTION1",

                // Key: Additional Data for AddEntryTestData (3)
                "KEY1-3",

                // Section: Additional Data for AddEntryTestData (4)
                "VALUE1-3"
            },

            // 4)
            new object[]
            {
                // File Path
                $@".\{nameof(AddEntryTestData)}_004.ini",

                // Sections
                new string[]
                {
                    "SECTION1",
                    "SECTION2"
                },

                // Entries
                new string[][][]
                {
                    // Section 1st
                    new string[][]
                    {
                        new string[] { "KEY1-1", "VALUE1-1" },
                        new string[] { "KEY1-2", "VALUE1-2" }
                    },

                    // Section 2nd
                    new string[][]
                    {
                        new string[] { "KEY2-1", "VALUE2-1" },
                        new string[] { "KEY2-2", "VALUE2-2" },
                        new string[] { "KEY2-3", "VALUE2-3" }
                    }
                },

                // Raw Lines
                new string[]
                {
                    "[SECTION1]",
                    "KEY1-1=VALUE1-1",
                    "KEY1-2=VALUE1-2",
                    "[SECTION2]",
                    "KEY2-1=VALUE2-1",
                    "KEY2-2=VALUE2-2",
                    "KEY2-3=VALUE2-3"
                },

                // BeforeRawLines: Additional Data for AddEntryTestData (1)
                new string[]
                {
                    "[SECTION1]",
                    "KEY1-1=VALUE1-1",
                    "KEY1-2=VALUE1-2",
                    "[SECTION2]",
                    "KEY2-1=VALUE2-1",
                    "KEY2-2=VALUE2-2"
                },

                // Section: Additional Data for AddEntryTestData (2)
                "SECTION2",

                // Key: Additional Data for AddEntryTestData (3)
                "KEY2-3",

                // Section: Additional Data for UpdateTestData (4)
                "VALUE2-3"
            },

            // 5)
            new object[]
            {
                // File Path
                $@".\{nameof(AddEntryTestData)}_005.ini",

                // Sections
                new string[]
                {
                    "SECTION1",
                    "SECTION2",
                    "SECTION3"
                },

                // Entries
                new string[][][]
                {
                    // Section 1st
                    new string[][]
                    {
                        new string[] { "KEY1-1", "VALUE1-1" },
                        new string[] { "KEY1-2", "VALUE1-2" }
                    },

                    // Section 2nd
                    new string[][]
                    {
                        new string[] { "KEY2-1", "VALUE2-1" },
                        new string[] { "KEY2-2", "VALUE2-2" },
                        new string[] { "KEY2-3", "VALUE2-3" }
                    },

                    // Section 3rd
                    new string[][]
                    {
                        new string[] { "KEY3-1", "VALUE3-1" },
                        new string[] { "KEY3-2", "VALUE3-2" }
                    }
                },

                // Raw Lines
                new string[]
                {
                    "[SECTION1]",
                    "KEY1-1=VALUE1-1",
                    "KEY1-2=VALUE1-2",
                    "[SECTION2]",
                    "KEY2-1=VALUE2-1",
                    "KEY2-2=VALUE2-2",
                    "KEY2-3=VALUE2-3",
                    "[SECTION3]",
                    "KEY3-1=VALUE3-1",
                    "KEY3-2=VALUE3-2"
                },

                // BeforeRawLines: Additional Data for AddEntryTestData (1)
                new string[]
                {
                    "[SECTION1]",
                    "KEY1-1=VALUE1-1",
                    "KEY1-2=VALUE1-2",
                    "[SECTION2]",
                    "KEY2-1=VALUE2-1",
                    "KEY2-2=VALUE2-2",
                    "[SECTION3]",
                    "KEY3-1=VALUE3-1",
                    "KEY3-2=VALUE3-2"
                },

                // Section: Additional Data for AddEntryTestData (2)
                "SECTION2",

                // Key: Additional Data for AddEntryTestData (3)
                "KEY2-3",

                // Section: Additional Data for UpdateTestData (4)
                "VALUE2-3"
            }
        };

        [DataTestMethod]
        [DynamicData(nameof(AddEntryTestData))]
        public void AddEntryTest(string path, string[] sections, string[][][] entries, string[] lines, string[] beforeRawLines, string section, string key, string value)
        {
            // SET Parameter
            AddEntryTestParameter param = new AddEntryTestParameter
            {
                FilePath = path,
                SectionNames = sections,
                Entries = entries,
                RawLines = lines,
                BeforeRawLines = beforeRawLines,
                Section = section,
                Key = key,
                Value = value
            };

            // ASSERT
            param.Validate(autoEnumerable: false);
        }


        // ----------------------------------------------------------------
        // Tests of Remove Entry
        // ----------------------------------------------------------------

        // TestParameter for RemoveEntryTest
        public class RemoveEntryTestParameter : ChangeProfileTestParameter
        {
            // Expected return value of Remove() method
            public bool Result;

            // Actual return value of Remove() method
            private bool result;

            // Change Profile
            public override void ChangeProfile(PrivateProfile profile)
            {
                // ACT: Remove Entry
                this.result = profile.Remove(this.Section, this.Key);

                // Print return value of Remove() method
                Console.WriteLine($"Return value of Remove() method = {this.result}");
            }

            // ASSERT
            public override void Assert<TItem>(TItem expected, TItem actual)
            {
                // ASSERT Original
                base.Assert(expected, actual);

                // ASSERT Additional
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(this.Result, this.result);
            }
        }

        // TestData for RemoveEntryTest
        public static IEnumerable<object[]> RemoveEntryTestData => new List<object[]>()
        {
            // 1)
            // (None)

            // 2)
            new object[]
            {
                // File Path
                $@".\{nameof(RemoveEntryTestData)}_002.ini",

                // Sections
                new string[]
                {
                    "SECTION"
                },

                // Entries
                new string[][][]
                {
                    new string[][]
                    {
                        new string[] { "KEY2", "VALUE2" },
                        new string[] { "KEY3", "VALUE3" }
                    }
                },

                // Raw Lines
                new string[]
                {
                    "[SECTION]",
                    "KEY2=VALUE2",
                    "KEY3=VALUE3"
                },

                // BeforeRawLines: Additional Data for RemoveEntryTestData (1)
                new string[]
                {
                    "[SECTION]",
                    "KEY1=VALUE1",
                    "KEY2=VALUE2",
                    "KEY3=VALUE3"
                },

                // Section: Additional Data for RemoveEntryTestData (2)
                "SECTION",

                // Key: Additional Data for RemoveEntryTestData (3)
                "KEY1",

                // Result: Additional Data for RemoveEntryTestData (4)
                true
            },

            // 3)
            new object[]
            {
                // File Path
                $@".\{nameof(RemoveEntryTestData)}_003.ini",

                // Sections
                new string[]
                {
                    "SECTION"
                },

                // Entries
                new string[][][]
                {
                    new string[][]
                    {
                        new string[] { "KEY1", "VALUE1" },
                        new string[] { "KEY2", "VALUE2" }
                    }
                },

                // Raw Lines
                new string[]
                {
                    "[SECTION]",
                    "KEY1=VALUE1",
                    "KEY2=VALUE2"
                },

                // BeforeRawLines: Additional Data for RemoveEntryTestData (1)
                new string[]
                {
                    "[SECTION]",
                    "KEY1=VALUE1",
                    "KEY2=VALUE2",
                    "KEY3=VALUE3"
                },

                // Section: Additional Data for RemoveEntryTestData (2)
                "SECTION",

                // Key: Additional Data for RemoveEntryTestData (3)
                "KEY3",

                // Result: Additional Data for RemoveEntryTestData (4)
                true
            },

            // 4)
            new object[]
            {
                // File Path
                $@".\{nameof(RemoveEntryTestData)}_004.ini",

                // Sections
                new string[]
                {
                    "SECTION"
                },

                // Entries
                new string[][][]
                {
                    new string[][]
                    {
                        new string[] { "KEY1", "VALUE1" },
                        new string[] { "KEY3", "VALUE3" }
                    }
                },

                // Raw Lines
                new string[]
                {
                    "[SECTION]",
                    "KEY1=VALUE1",
                    "KEY3=VALUE3"
                },

                // BeforeRawLines: Additional Data for RemoveEntryTestData (1)
                new string[]
                {
                    "[SECTION]",
                    "KEY1=VALUE1",
                    "KEY2=VALUE2",
                    "KEY3=VALUE3"
                },

                // Section: Additional Data for RemoveEntryTestData (2)
                "SECTION",

                // Key: Additional Data for RemoveEntryTestData (3)
                "KEY2",

                // Result: Additional Data for RemoveEntryTestData (4)
                true
            }
        };

        [DataTestMethod]
        [DynamicData(nameof(RemoveEntryTestData))]
        public void RemoveEntryTest(string path, string[] sections, string[][][] entries, string[] lines, string[] beforeRawLines, string section, string key, bool result)
        {
            // SET Parameter
            RemoveEntryTestParameter param = new RemoveEntryTestParameter
            {
                FilePath = path,
                SectionNames = sections,
                Entries = entries,
                RawLines = lines,
                BeforeRawLines = beforeRawLines,
                Section = section,
                Key = key,
                Result = result
            };

            // ASSERT
            param.Validate(autoEnumerable: false);
        }
    }
}