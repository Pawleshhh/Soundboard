using FileSignatures;

namespace ManiacSoundboard.ViewModel
{
    public class Wav : FileFormat
    {

        public Wav() : base(new byte[] { 0x52, 0x49, 0x46, 0x46 }, "audio/wav", "wav", 0)
        {

        }

    }
}
