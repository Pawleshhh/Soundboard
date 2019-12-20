using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Collections;

namespace ManiacSoundboard.Model
{
    /// <summary>
    /// Abstract class that defines cache collection of players.
    /// </summary>
    public abstract class SoundboardCacheCollection : IList<IPlayer>, IDisposable
    {

        #region Constructors

        /// <summary>
        /// Initializes new instance of <see cref="SoundboardCacheCollection"/> class.
        /// </summary>
        protected SoundboardCacheCollection()
        {
            _list = new List<IPlayer>();
        }

        /// <summary>
        /// Initializes new instance of <see cref="SoundboardCacheCollection"/> class.
        /// </summary>
        /// <param name="capacity">Capacity of this collection.</param>
        protected SoundboardCacheCollection(int capacity)
        {
            _list = new List<IPlayer>(capacity);
        }

        #endregion

        #region Private & protected fields

        /// <summary>
        /// Cache lock for safe reading and writing to list of players.
        /// </summary>
        protected readonly ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();

        /// <summary>
        /// List of players.
        /// </summary>
        protected readonly List<IPlayer> _list;

        #endregion

        #region Properties

        public IPlayer this[int index]
        {
            get => _Read(index);
            set => _Write(index, value);
        }

        public int Count => _list.Count;

        public int Capacity => _list.Capacity;

        public bool IsReadOnly => false;

        #endregion

        #region Methods

        public void Add(IPlayer item)
        {
            if (item == null) return;

            cacheLock.EnterWriteLock();
            try
            {
                SubscribeSound(item);
                _list.Add(item);
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }

        public bool Contains(IPlayer item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(IPlayer[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public int IndexOf(IPlayer item)
        {
            cacheLock.EnterReadLock();
            try
            {
                return _list.IndexOf(item);
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
        }

        public void Insert(int index, IPlayer item)
        {
            if (item == null) return;

            cacheLock.EnterWriteLock();
            try
            {
                SubscribeSound(item);
                _list.Insert(index, item);
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }

        public bool Remove(IPlayer item)
        {
            if (item == null) return false;

            cacheLock.EnterWriteLock();
            try
            {
                bool result = _list.Remove(item);

                if (result)
                    DissentSound(item);

                return result;
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }

        public void RemoveAt(int index)
        {
            cacheLock.EnterWriteLock();
            try
            {
                DissentSound(_list[index]);
                _list.RemoveAt(index);
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }

        public void Clear()
        {
            cacheLock.EnterWriteLock();
            try
            {
                _list.ForEach(sound => DissentSound(sound));

                _list.Clear();
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }

        public IEnumerator<IPlayer> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Dispose()
        {
            cacheLock.Dispose();
        }

        #endregion

        #region Private & protected methods

        /// <summary>
        /// Unsubscribes events from <see cref="IPlayer"/> that this collection stores.
        /// </summary>
        /// <param name="player">Player with events to be unsubscribed.</param>
        protected abstract void DissentSound(IPlayer player);

        /// <summary>
        /// Subscribes events from <see cref="IPlayer"/> that this collection stores.
        /// </summary>
        /// <param name="player">Player with events to be subscribed.</param>
        protected abstract void SubscribeSound(IPlayer player);

        private IPlayer _Read(int index)
        {
            cacheLock.EnterReadLock();
            try
            {
                return _list[index];
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
        }

        private void _Write(int index, IPlayer player)
        {
            cacheLock.EnterWriteLock();
            try
            {
                _list[index] = player;
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }

        #endregion

    }
}
