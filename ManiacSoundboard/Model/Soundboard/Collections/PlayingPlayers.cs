using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace ManiacSoundboard.Model
{

    /// <summary>
    /// Stores all players that are currently playing.
    /// </summary>
    public class PlayingPlayers : SoundboardCacheCollection
    {

        #region Constructors

        public PlayingPlayers() : base()
        {

        }

        public PlayingPlayers(int capacity) : base(capacity)
        {

        }

        #endregion

        #region Private & protected methods

        protected override void SubscribeSound(IPlayer player)
        {
            player.AudioPaused += BoundData_AudioPaused;
            player.AudioStopped += BoundData_AudioStopped;
            player.AudioAutoStopped += BoundData_AudioStopped;
        }

        protected override void DissentSound(IPlayer player)
        {
            player.AudioPaused -= BoundData_AudioPaused;
            player.AudioStopped -= BoundData_AudioStopped;
            player.AudioAutoStopped -= BoundData_AudioStopped;
        }

        private void BoundData_AudioStopped(object sender, EventArgs e)
        {
            if(sender is IPlayer player)
                Remove(player);
        }

        private void BoundData_AudioPaused(object sender, EventArgs e)
        {
            if (sender is IPlayer player)
                Remove(player);
        }

        #endregion

    }
}
