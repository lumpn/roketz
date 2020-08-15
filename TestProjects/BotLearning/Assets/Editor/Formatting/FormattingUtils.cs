//----------------------------------------
// MIT License
// Copyright(c) 2020 Jonas Boetel
//----------------------------------------
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace Lumpn.Formatting
{
    public static class FormattingUtils
    {
        public const int tabSize = 4;
        private static readonly int[] bom = { 0xEF, 0xBB, 0xBF };

        [MenuItem("Utils/Formatting/Fix formatting (all files)")]
        public static void FixFormatting()
        {
            var guids = AssetDatabase.FindAssets("t:script t:shader");
            FixFormatting(guids);
        }

        [MenuItem("Utils/Formatting/Fix formatting (selected file)")]
        public static void FixFormattingSelected()
        {
            var guids = Selection.assetGUIDs;
            FixFormatting(guids);
        }

        public static void FixFormatting(string[] guids)
        {
            RunAction(guids, "Fix formatting", FixFormatting);
        }

        public static void FixFormatting(string path)
        {
            var outPath = path + ".formatting";
            using (var input = File.OpenRead(path))
            {
                using (var output = File.OpenWrite(outPath))
                {
                    var bytes = Read(input);

                    bytes = RemoveByteOrderMark(bytes);
                    bytes = FixLineEndings(bytes);
                    bytes = FixTabsVersusSpaces(bytes);
                    bytes = FixTrailingWhitespaces(bytes);
                    bytes = FixFinalNewLine(bytes);

                    foreach (var value in bytes)
                    {
                        output.WriteByte((byte)value);
                    }
                }
            }
            File.Replace(outPath, path, null);
        }

        public static void RunAction(string[] guids, string title, System.Action<string> action)
        {
            using (var pb = ProgressBarUtils.Create(title, guids.Length))
            {
                foreach (var guid in guids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    if (pb.Update(path)) break;

                    if (IsNonScript(path)) continue;
                    action(path);
                }
            }
        }

        private static bool IsNonScript(string path)
        {
            // NOTE: AssetDatabase includes packages and built in resources
            if (!path.StartsWith("Assets/", System.StringComparison.OrdinalIgnoreCase)) return true;

            // NOTE: for some reason Unity classifies DLLs as scripts
            if (path.EndsWith(".dll", System.StringComparison.OrdinalIgnoreCase)) return true;

            return false;
        }

        public static IEnumerable<int> Read(Stream stream)
        {
            int value;
            while ((value = stream.ReadByte()) >= 0)
            {
                yield return value;
            }
        }

        private static IEnumerable<int> FixLineEndings(IEnumerable<int> stream)
        {
            foreach (var value in stream)
            {
                if (value == 13) continue;
                yield return value;
            }
        }

        private static IEnumerable<int> FixTabsVersusSpaces(IEnumerable<int> stream)
        {
            foreach (var value in stream)
            {
                if (value == 9)
                {
                    for (int i = 0; i < tabSize; i++)
                    {
                        yield return 32;
                    }
                }
                else
                {
                    yield return value;
                }
            }
        }

        private static IEnumerable<int> FixTrailingWhitespaces(IEnumerable<int> stream)
        {
            int consecutiveSpaces = 0;
            foreach (var value in stream)
            {
                if (value == 32)
                {
                    consecutiveSpaces++;
                }
                else
                {
                    if (value != 10)
                    {
                        // not the end of the line. emit accumulated spaces.
                        for (int i = 0; i < consecutiveSpaces; i++)
                        {
                            yield return 32;
                        }
                    }
                    yield return value;
                    consecutiveSpaces = 0;
                }
            }
        }

        private static IEnumerable<int> FixFinalNewLine(IEnumerable<int> stream)
        {
            int consecutiveLinefeeds = 0;
            foreach (var value in stream)
            {
                if (value == 10)
                {
                    consecutiveLinefeeds++;
                }
                else
                {
                    // not the end of the file. emit accumulated linefeeds.
                    for (int i = 0; i < consecutiveLinefeeds; i++)
                    {
                        yield return 10;
                    }
                    yield return value;
                    consecutiveLinefeeds = 0;
                }
            }

            // end of the file. emit exactly one linefeed.
            yield return 10;
        }

        private static IEnumerable<int> FixPlainASCII(IEnumerable<int> stream)
        {
            foreach (var value in stream)
            {
                if (value < 128) yield return value;
            }
        }

        private static IEnumerable<int> RemoveByteOrderMark(IEnumerable<int> stream)
        {
            int i = -1;
            foreach (var value in stream)
            {
                i++;
                if (i < bom.Length)
                {
                    // skip BOM
                    if (value == bom[i]) continue;

                    // not BOM. emit skipped bytes.
                    for (int j = 0; j < i; j++)
                    {
                        yield return bom[j];
                    }

                    // disable further BOM checking.
                    i = bom.Length;
                }
                yield return value;
            }
        }
    }
}
