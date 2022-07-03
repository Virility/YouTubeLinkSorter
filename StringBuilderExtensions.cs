using System.Text;

namespace YouTubeLinkSorter
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendIndented(this StringBuilder sb, string textBlock)
        {
            foreach (var line in textBlock.TrimEnd().Split('\n'))
                if (!string.IsNullOrWhiteSpace(line))
                    sb.Append($"\t{line}\n");
            return sb;
        }
    }
}