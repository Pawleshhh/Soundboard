using FileSignatures;
using System;
using System.Collections.Generic;

namespace ManiacSoundboard.ViewModel
{
    public static class RecognizableFormats
    {

        public static IEnumerable<FileFormat> GetAllFormats()
        {
            return new FileFormat[] { new Mp3(), new Mp3ID3v2(), new Wav() };
        }

        public static bool IsKnown(FileFormat format)
        {
            if (format == null)
                return false;

            Type formatType = format.GetType();
            foreach(var f in GetAllFormats())
            {
                if (f.GetType() == formatType)
                    return true;
            }

            return false;
        }

    }
}
