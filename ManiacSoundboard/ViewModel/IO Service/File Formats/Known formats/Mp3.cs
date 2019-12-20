using FileSignatures;

namespace ManiacSoundboard.ViewModel
{

    /// <summary>
    /// Mp3 file format.
    /// </summary>
    public class Mp3 : FileFormat
    {

        public Mp3() : base(new byte[] { 0xFF, 0xFB }, "audio/mpeg", "mp3", 0)
        {
            
        }

    }
}
