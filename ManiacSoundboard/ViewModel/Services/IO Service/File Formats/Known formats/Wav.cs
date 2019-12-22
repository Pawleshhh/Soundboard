using FileSignatures;

namespace ManiacSoundboard.ViewModel
{

    /// <summary>
    /// Wav file format.
    /// </summary>
    public class Wav : FileFormat
    {

        public Wav() : base(new byte[] { 0x52, 0x49, 0x46, 0x46 }, "audio/wav", "wav", 0)
        {

        }

    }
}
