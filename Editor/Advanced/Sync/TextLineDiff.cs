using System.Collections.Generic;

namespace emiteat.NexUI.Designer.Editor.Sync
{
    /// <summary>One line in a computed diff.</summary>
    public struct DiffLine
    {
        public enum Kind { Unchanged, Added, Removed }
        public Kind Type;
        public string Text;
    }

    /// <summary>
    /// Pure line-level diff via longest-common-subsequence (brief §32 Review Diff). Produces a merged
    /// sequence of unchanged / removed (in A, not B) / added (in B, not A) lines. No Unity dependency;
    /// unit-tested.
    /// </summary>
    public static class TextLineDiff
    {
        public static List<DiffLine> Diff(string a, string b)
        {
            var linesA = SplitLines(a);
            var linesB = SplitLines(b);
            var lcs = LcsTable(linesA, linesB);

            var result = new List<DiffLine>();
            int i = 0, j = 0;
            while (i < linesA.Count && j < linesB.Count)
            {
                if (linesA[i] == linesB[j])
                {
                    result.Add(new DiffLine { Type = DiffLine.Kind.Unchanged, Text = linesA[i] });
                    i++; j++;
                }
                else if (lcs[i + 1, j] >= lcs[i, j + 1])
                {
                    result.Add(new DiffLine { Type = DiffLine.Kind.Removed, Text = linesA[i] });
                    i++;
                }
                else
                {
                    result.Add(new DiffLine { Type = DiffLine.Kind.Added, Text = linesB[j] });
                    j++;
                }
            }
            while (i < linesA.Count) result.Add(new DiffLine { Type = DiffLine.Kind.Removed, Text = linesA[i++] });
            while (j < linesB.Count) result.Add(new DiffLine { Type = DiffLine.Kind.Added, Text = linesB[j++] });
            return result;
        }

        /// <summary>Number of changed (added/removed) lines in a diff.</summary>
        public static int ChangeCount(IReadOnlyList<DiffLine> diff)
        {
            var count = 0;
            for (int i = 0; i < diff.Count; i++)
                if (diff[i].Type != DiffLine.Kind.Unchanged) count++;
            return count;
        }

        private static List<string> SplitLines(string text)
        {
            var list = new List<string>();
            if (string.IsNullOrEmpty(text)) return list;
            list.AddRange(text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n'));
            return list;
        }

        private static int[,] LcsTable(List<string> a, List<string> b)
        {
            var table = new int[a.Count + 1, b.Count + 1];
            for (int i = a.Count - 1; i >= 0; i--)
                for (int j = b.Count - 1; j >= 0; j--)
                    table[i, j] = a[i] == b[j]
                        ? table[i + 1, j + 1] + 1
                        : (table[i + 1, j] >= table[i, j + 1] ? table[i + 1, j] : table[i, j + 1]);
            return table;
        }
    }
}
