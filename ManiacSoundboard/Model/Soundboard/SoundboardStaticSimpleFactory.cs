using System;

namespace ManiacSoundboard.Model
{

    /// <summary>
    /// Simple factory that creates implemenations of <see cref="Soundboard"/> interface.
    /// </summary>
    public static class SoundboardStaticSimpleFactory
    {

        /// <summary>
        /// Gets specified <see cref="Soundboard"/> implementation by the audio api.
        /// </summary>
        /// <param name="api">Audio output api to specify implementation of <see cref="Soundboard"/></param>
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
