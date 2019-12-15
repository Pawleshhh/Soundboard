using FileSignatures;

namespace ManiacSoundboard.ViewModel
{
    public class Mp3ID3v2 : FileFormat
    {
        public Mp3ID3v2() : base(new byte[] { 0x49, 0x44, 0x33 }, "audio/mpeg", "mp3", 0)
        {
            
        }
    }
}
