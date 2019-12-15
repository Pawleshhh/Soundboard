using FileSignatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacSoundboard.ViewModel
{
    public static class FileFolderHelper
    {

        public static string GetWindowsExplorerFilters(FileFormat[] formats)
        {
            string result = "";
            string resultAtTheEndFirstPart = "";
            string resultAtTheEndSecondPart = "";

            for (int i = 0; i < formats.Length; i++)
            {
                string format = formats[i].Extension;
                string ext = string.Format("*.{0}", format);
                result += string.Format("{0}|{1}|", format.ToUpperInvariant(), ext);
                if (i < formats.Length - 1)
                {
                    resultAtTheEndFirstPart += string.Format("{0}, ", ext);
                    resultAtTheEndSecondPart += string.Format("{0};", ext);
                }
                else
                {
                    resultAtTheEndFirstPart += string.Format("{0}", ext);
                    resultAtTheEndSecondPart += string.Format("{0}", ext);
                }
            }

            return $"{result}All file types ({resultAtTheEndFirstPart})|{resultAtTheEndSecondPart}";
        }

    }

    public class DistinctFileFormatComparer : IEqualityComparer<FileFormat>
    {
        public bool Equals(FileFormat x, FileFormat y)
        {
            return x.Extension == y.Extension;
        }

        public int GetHashCode(FileFormat obj)
        {
            return obj.Extension.GetHashCode();
        }
    }
}
