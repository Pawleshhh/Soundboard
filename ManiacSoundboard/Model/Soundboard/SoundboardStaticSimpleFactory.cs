using System;

namespace ManiacSoundboard.Model
{
    public static class SoundboardStaticSimpleFactory
    {

        public static Soundboard GetSoundboard(AudioOutputApi api)
        {
            if (!Enum.IsDefined(typeof(AudioOutputApi), api))
                throw new ArgumentException("Wrong value of AudioOutputApi", "api");

            switch(api)
            {
                case AudioOutputApi.WaveEvent:
                    return new SoundboardWaveEvent();
            }

            return null;
        }

    }

    public enum AudioOutputApi
    { 
        WaveEvent = 1,
    }
}
