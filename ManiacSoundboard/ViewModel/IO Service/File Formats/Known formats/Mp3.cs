using FileSignatures;

namespace ManiacSoundboard.ViewModel
{
    public class Mp3 : FileFormat
    {

        public Mp3() : base(new byte[] { 0xFF, 0xFB }, "audio/mpeg", "mp3", 0)
        {
            
        }

    }
}
