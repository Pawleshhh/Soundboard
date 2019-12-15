using System;
using System.Collections;
using System.Collections.Generic;

namespace ManiacSoundboard.Model
{

    public class PausedPlayers : SoundboardCacheCollection
    {

        #region Constructors

        public PausedPlayers() : base()
        {

        }

        public PausedPlayers(int capacity) : base(capacity)
        {

        }

        #endregion

        #region Private methods

        protected override void SubscribeSound(IPlayer player)
        {
            player.AudioPlayed += BoundData_AudioPlayed;
            player.AudioStopped += BoundData_AudioStopped;
            player.AudioAutoStopped += BoundData_AudioStopped;
        }

        protected override void DissentSound(IPlayer player)
        {
            player.AudioPaused -= BoundData_AudioPlayed;
            player.AudioStopped -= BoundData_AudioStopped;
            player.AudioAutoStopped -= BoundData_AudioStopped;
        }

        private void BoundData_AudioStopped(object sender, EventArgs e)
        {
            if (sender is IPlayer player)
                Remove(player);
        }

        private void BoundData_AudioPlayed(object sender, EventArgs e)
        {
            if (sender is IPlayer player)
                Remove(player);
        }

        #endregion

    }
}
