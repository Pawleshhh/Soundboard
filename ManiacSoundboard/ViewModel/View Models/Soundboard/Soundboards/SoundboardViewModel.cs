using FileSignatures;
using ManiacSoundboard.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ManiacSoundboard.ViewModel
{

    /// <summary>
    /// Base class of the soundboard view model.
    /// </summary>
    public class SoundboardViewModel : BaseViewModel, IXmlSerializable
    {

        #region Constructor

        public SoundboardViewModel() 
        {
            _messageBoxService = AppServices.MessageBoxService;
            _fileDolderDialogService = AppServices.FileFolderDialogService;
            _soundboard = SoundboardViewModelSimpleFactory.Factory.GetConfiguredModel();
            _fileFormatInspector = new FileFormatInspector(RecognizableFormats.GetAllFormats());

            Initialize();
        }

        public SoundboardViewModel(Soundboard soundboard, IMessageBoxService msgBoxServ, IFileFolderDialogService fileFolderServ)
        {
            _messageBoxService = msgBoxServ;
            _fileDolderDialogService = fileFolderServ;
            _soundboard = soundboard;
            _fileFormatInspector = new FileFormatInspector(RecognizableFormats.GetAllFormats());

            Initialize();
        }

        private void Initialize()
        {
            Sounds = new NotifyTaskCompletion<ObservableCollection<SoundViewModel>>(Task.FromResult(new ObservableCollection<SoundViewModel>()));

            _soundboard.AudioDevices.Notifications.DefaultDeviceChaned += Client_DevicesChanged;
            _soundboard.AudioDevices.Notifications.DeviceAdded += Client_DevicesChanged;
            _soundboard.AudioDevices.Notifications.DeviceRemoved += Client_DevicesChanged;
            _soundboard.AudioDevices.Notifications.DeviceStateChanged += Client_DevicesChanged;
            _ReloadDevices();
        }

        #endregion

        #region Private fields

        protected Soundboard _soundboard;

        protected FileFormatInspector _fileFormatInspector;

        protected IMessageBoxService _messageBoxService;

        protected IFileFolderDialogService _fileDolderDialogService;

        private int _prevCount = 0;

        #endregion

        #region Properties

        private bool isSimpleSoundboardEnabled;

        /// <summary>
        /// Gets or sets whether the simple soundboard is enabled or not.
        /// </summary>
        public bool IsSimpleSoundboardEnabled
        {
            get => isSimpleSoundboardEnabled;
            set
            {
                isSimpleSoundboardEnabled = value;
                OnPropertyChanged(nameof(IsSimpleSoundboardEnabled));
            }
        }

        /// <summary>
        /// Gets task with <see cref="ObservableCollection{T}"/> that stores all <see cref="SoundViewModel"/>s.
        /// </summary>
        public NotifyTaskCompletion<ObservableCollection<SoundViewModel>> Sounds { get; private set; }

        /// <summary>
        /// Gets all audio devices.
        /// </summary>
        public ObservableCollection<IAudioDevice> AllDevices { get; private set; } = new ObservableCollection<IAudioDevice>();

        /// <summary>
        /// Gets or sets the first audio device.
        /// </summary>
        public IAudioDevice FirstDevice
        {
            get => _soundboard.FirstDevice;
            set
            {
                SetFirstDevice(value);
            }
        }

        /// <summary>
        /// Gets or sets the second audio device.
        /// </summary>
        public IAudioDevice SecondDevice
        {
            get => _soundboard.SecondDevice;
            set
            {
                SetSecondDevice(value);
            }
        }

        /// <summary>
        /// Gets or sets whether the first device is enabled or not.
        /// </summary>
        public bool IsFirstDeviceEnabled
        {
            get => _soundboard.IsFirstDeviceEnabled;
            set
            {
                _soundboard.IsFirstDeviceEnabled = value;
                OnPropertyChanged("IsFirstDeviceEnabled");
            }
        }

        /// <summary>
        /// Gets or sets whether the second device is enabled or not.
        /// </summary>
        public bool IsSecondDeviceEnabled
        {
            get => _soundboard.IsSecondDeviceEnabled;
            set
            {
                _soundboard.IsSecondDeviceEnabled = value;
                OnPropertyChanged("IsSecondDeviceEnabled");
            }
        }

        /// <summary>
        /// Gets or sets volume of the soundboard.
        /// </summary>
        public float Volume
        {
            get => _soundboard.Volume;
            set
            {
                _soundboard.Volume = value;

                OnPropertyChanged("Volume");
            }
        }

        /// <summary>
        /// Gets or sets the volume step of in/decreasing volume.
        /// </summary>
        public float VolumeStep
        {
            get => _soundboard.VolumeStep;
            set
            {
                _soundboard.VolumeStep = value;
                OnPropertyChanged("VolumeStep");
            }
        }

        /// <summary>
        /// Gets or sets whether the soundboard is muted or not.
        /// </summary>
        public bool IsMuted
        {
            get => _soundboard.IsMuted;
            set
            {
                _soundboard.IsMuted = value;

                OnPropertyChanged("IsMuted");
            }
        }

        /// <summary>
        /// Gets whether any player is playing or not.
        /// </summary>
        public bool IsPlaying => _soundboard.IsPlaying;

        /// <summary>
        /// Gets whether soundboard view model is busy with some task.
        /// </summary>
        public bool IsBusy => IsChangingDevice || (Sounds != null && Sounds.IsNotCompleted);

        /// <summary>
        /// Gets negated value of <see cref="IsBusy"/>.
        /// </summary>
        public bool IsNotBusy => !IsBusy;

        private bool isChangingDevice = false;

        /// <summary>
        /// Gets whether devices are changing or not.
        /// </summary>
        public bool IsChangingDevice
        {
            get => isChangingDevice;
            protected set
            {
                isChangingDevice = value;

                //if (value)
                //    _soundboard.HandlesKeyEvents = !value;
                //else
                //    HandlesKeyEvents = !value;

                OnPropertyChanged("IsChangingDevice", "IsNotChangingDevice", "IsBusy", "IsNotBusy");
            }
        }

        /// <summary>
        /// Gets negated value of <see cref="IsChangingDevice"/>.
        /// </summary>
        public bool IsNotChangingDevice => !IsChangingDevice;

        #endregion

        #region Methods

        /// <summary>
        /// Play paused players.
        /// </summary>
        public void PlayPaused()
        {
            _soundboard.PlayPaused();
        }

        /// <summary>
        /// Pauses all playing players.
        /// </summary>
        public void PauseAll()
        {
            _soundboard.PauseAll();
        }

        /// <summary>
        /// Stops all playing/paused players.
        /// </summary>
        public void StopAll()
        {
            _soundboard.StopAll();
        }

        /// <summary>
        /// Adds sounds by paths to audio files asynchronously.
        /// </summary>
        /// <param name="paths">Array of paths to audio files.</param>
        public void AddSoundsByPaths(string[] paths)
        {
            _prevCount = Sounds.Result.Count;
            WorkOnSoundCollection(AddSoundsByPathsAsync(Sounds.Result.ToList(), paths));

            OnPropertyChanged("Sounds");
        }

        /// <summary>
        /// Removes sounds asynchronously.
        /// </summary>
        /// <param name="sounds">Sounds to be removed.</param>
        public void RemoveSounds(params SoundViewModel[] sounds)
        {
            _prevCount = Sounds.Result.Count;
            WorkOnSoundCollection(RemoveSoundsAsync(Sounds.Result.ToList(), sounds));

            OnPropertyChanged("Sounds");
        }

        /// <summary>
        /// Remove all sounds asynchronously.
        /// </summary>
        public void ClearSounds()
        {
            StopAll();
            _prevCount = Sounds.Result.Count;
            WorkOnSoundCollection(ClearAllSoundsAsync(), true);
            OnPropertyChanged("Sounds");
        }

        /// <summary>
        /// Reloads <see cref="AllDevices"/> asynchronously.
        /// </summary>
        public async void ReloadDevices()
        {
            StopAll();
            IsChangingDevice = true;
            await ReloadDevicesAsync();
            IsChangingDevice = false;
            OnPropertyChanged("FirstDevice", "SecondDevice", "AllDevices", "IsFirstDeviceEnabled", "IsSecondDeviceEnabled");
        }

        /// <summary>
        /// Sets <see cref="FirstDevice"/> asynchronously.
        /// </summary>
        /// <param name="device">Device to be set.</param>
        public async void SetFirstDevice(IAudioDevice device)
        {
            StopAll();
            IsChangingDevice = true;
            await Task.Run(() => _soundboard.FirstDevice = device);
            IsChangingDevice = false;
            OnPropertyChanged("FirstDevice");
        }

        /// <summary>
        /// Sets <see cref="SecondDevice"/> asynchronously.
        /// </summary>
        /// <param name="device">Device to be set.</param>
        public async void SetSecondDevice(IAudioDevice device)
        {
            StopAll();
            IsChangingDevice = true;
            await Task.Run(() => _soundboard.SecondDevice = device);
            IsChangingDevice = false;
            OnPropertyChanged("SecondDevice");
        }

        /// <summary>
        /// Increases <see cref="Volume"/> by <see cref="VolumeStep"/>.
        /// </summary>
        public void IncreaseVolume()
        {
            _soundboard.IncreaseVolume();
            OnPropertyChanged("Volume");
        }

        /// <summary>
        /// Decreases <see cref="Volume"/> by <see cref="VolumeStep"/>.
        /// </summary>
        public void DecreaseVolume()
        {
            _soundboard.DecreaseVolume();
            OnPropertyChanged("Volume");
        }

        /// <summary>
        /// Gets implementation of <see cref="SoundViewModel"/> specified by implementation of this soundboard view model.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public virtual SoundViewModel GetSoundViewModel(IPlayer player)
        {
            return new SoundViewModel(player, this);
        }

        public override void Dispose()
        {
            _soundboard.Dispose();
            _soundboard = null;
        }

        //public IPlayer GetPlayer(string path)
        //{
        //    return _soundboard.GetPlayer(path);
        //}

        #endregion

        #region Private methods

        /// <summary>
        /// Gets task that reloads devices.
        /// </summary>
        protected Task ReloadDevicesAsync()
        {
            return Task.Run(() =>
            {
                _ReloadDevices();
            });
        }

        /// <summary>
        /// Reloads devices synchronously.
        /// </summary>
        protected void _ReloadDevices()
        {
            var prevFirstDevice = FirstDevice;
            var prevSecondDevice = SecondDevice;

            _soundboard.AudioDevices.ReloadOutDevices();

            AllDevices = new ObservableCollection<IAudioDevice>(_soundboard.AudioDevices.OutDevices);

            if (AllDevices.Count >= 1)
            {
                if (prevFirstDevice != null && AllDevices.Contains(prevFirstDevice))
                    _soundboard.FirstDevice = AllDevices.Where(n => n.Equals(prevFirstDevice)).Single();
                else
                    _soundboard.FirstDevice = AllDevices[0];
            }

            if (AllDevices.Count >= 2)
            {
                if (prevSecondDevice != null && AllDevices.Contains(prevSecondDevice))
                    _soundboard.SecondDevice = AllDevices.Where(n => n.Equals(prevSecondDevice)).Single();
                else
                    _soundboard.SecondDevice = AllDevices[1];
            }
        }

        /// <summary>
        /// Gets task that adds sounds by paths to audio files.
        /// </summary>
        /// <param name="previous">Already stored sounds.</param>
        /// <param name="paths">Paths to the new sounds.</param>
        protected Task<ObservableCollection<SoundViewModel>> AddSoundsByPathsAsync(List<SoundViewModel> previous, params string[] paths)
        {
            return Task.Run(() =>
            {
                //j variable is for checking if the MaxSize is not exceeded
                for (int i = 0, j = 0; i < paths.Length; i++, j++)
                {
                    if (j + 1 + _prevCount > Soundboard.MaxSize)
                    {
                        _messageBoxService.ShowMessageBox($"{_prevCount + paths.Length - Soundboard.MaxSize} sounds could not be added because of the max possible size ({SoundboardWaveEvent.MaxSize})",
                            "Too many sounds", MessageBoxImage.Information);
                        break;
                    }

                    var fileExists = CheckIfFileExists(paths[i], $"Path {paths[i]} does not exist. Continue?", "Path does not exist", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (!fileExists.exists)
                    {
                        if (fileExists.result == MessageBoxResult.Yes)
                        {
                            j--;
                            continue;
                        }

                        break;
                    }

                    var recognizableFormat = CheckIfFileFormatIsRecognizable(paths[i], $"Format of {paths[i]} is not recognizable. Continue?", "Unrecognizable file.", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (!recognizableFormat.recognizableAndUnlocked)
                    {
                        if (recognizableFormat.result == MessageBoxResult.Yes)
                        {
                            j--;
                            continue;
                        }

                        break;
                    }

                    IPlayer player = _soundboard.GetPlayer(paths[i]);
                    previous.Add(GetSoundViewModel(player));

                    _soundboard.AddSound(player);
                }

                return new ObservableCollection<SoundViewModel>(previous);
            });
        }

        /// <summary>
        /// Gets task that removes given sounds.
        /// </summary>
        /// <param name="previous">Already stored sounds.</param>
        /// <param name="sounds">Sounds to be removed.</param>
        protected Task<ObservableCollection<SoundViewModel>> RemoveSoundsAsync(List<SoundViewModel> previous, params SoundViewModel[] sounds)
        {
            return Task.Run(() =>
            {
                for(int i = 0; i < sounds.Length; i++)
                {
                    sounds[i].Stop();
                    IPlayer player = SoundViewModel.GetPlayer(sounds[i]);
                    sounds[i].Dispose();
                    previous.Remove(sounds[i]);
                    _soundboard.RemoveSound(player);
                }

                return new ObservableCollection<SoundViewModel>(previous);
            });
        }

        /// <summary>
        /// Gets task that removes all sounds from the collection.
        /// </summary>
        protected Task<ObservableCollection<SoundViewModel>> ClearAllSoundsAsync()
        {
            return Task.Run(() =>
            {
                _soundboard.RemoveAllSounds();
                return UpdateSounds();
            });
        }

        /// <summary>
        /// Updates sounds collection from model synchronously.
        /// </summary>
        protected ObservableCollection<SoundViewModel> UpdateSounds()
        {
            return new ObservableCollection<SoundViewModel>(_soundboard.AllPlayers.Select(n => GetSoundViewModel(n)));
        }

        /// <summary>
        /// Gets task that updates sounds collection from model.
        /// </summary>
        protected Task<ObservableCollection<SoundViewModel>> UpdateSoundsAsync()
        {
            return Task.Run(() => UpdateSounds());
        }

        /// <summary>
        /// Helper method to work safely on the collection of sounds asynchronously.
        /// </summary>
        /// <param name="task">Task to be done on collection.</param>
        /// <param name="disposeSounds">Indicates whether all already stored sounds must be disposed or not.</param>
        protected void WorkOnSoundCollection(Task<ObservableCollection<SoundViewModel>> task, bool disposeSounds = false)
        {
            if (disposeSounds && Sounds != null && Sounds.Result != null)
            {
                for (int i = 0; i < Sounds.Result.Count; i++)
                    Sounds.Result[i].Dispose();
            }

            Sounds = new NotifyTaskCompletion<ObservableCollection<SoundViewModel>>(task, Sounds_Finished);
            OnPropertyChanged("IsBusy", "IsNotBusy");
        }

        /// <summary>
        /// Method that rises when any task stopped working.
        /// </summary>
        protected virtual void Sounds_Finished()
        {
            OnPropertyChanged("IsBusy", "IsNotBusy", "Sounds");
        }

        /// <summary>
        /// Checks if file exists and if not then informs user about it by message box.
        /// </summary>
        /// <returns></returns>
        protected (bool exists, MessageBoxResult result) CheckIfFileExists(string path, string info, string caption, MessageBoxButton button, MessageBoxImage image)
        {
            if (!File.Exists(path))
            {
                var result = _messageBoxService.ShowMessageBoxDecision(info, caption, button, image);

                return (false, result);
            }

            return (true, MessageBoxResult.None);
        }

        /// <summary>
        /// Checks if file format is recognizable (and if file is not locked). If file is not recognizable then it informs user about it by message box.
        /// </summary>
        protected (bool recognizableAndUnlocked, MessageBoxResult result) CheckIfFileFormatIsRecognizable(string path, string info, string caption, MessageBoxButton button, MessageBoxImage image)
        {
            try
            {
                using (var stream = new MemoryStream(File.ReadAllBytes(path)))
                {
                    var format = _fileFormatInspector.DetermineFileFormat(stream);
                    if (!RecognizableFormats.IsKnown(format))
                    {
                        var result = _messageBoxService.ShowMessageBoxDecision(info, caption, button, image);

                        return (false, result);
                    }
                }
            }
            catch
            {
                var result = _messageBoxService.ShowMessageBoxDecision($"{path} cannot be read because it's locked. Continue?", "File is locked.",
                                        button, MessageBoxImage.Warning);

                return (false, result);
            }

            return (true, MessageBoxResult.None);
        }

        /// <summary>
        /// Method subscribed to the audio device service. Rises when anything associated with the audio devices changed.
        /// </summary>
        private void Client_DevicesChanged(object sender, EventArgs e)
        {
            ReloadDevices();
        }

        #region Xml

        public virtual XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Gets specified implementation of <see cref="SoundViewModel"/> from xml file.
        /// </summary>
        protected virtual SoundViewModel GetSoundViewModelFromXml(XmlReader r)
        {
            string path = r.ReadElementContentAsString(nameof(SoundViewModel.AudioPath), "");

            var fileExistsResult = CheckIfFileExists(path, $"File {path} could not be found.", "File not found", MessageBoxButton.OK, MessageBoxImage.Warning);

            if (!fileExistsResult.exists)
                return null;

            var formatUnrecognizableOrLocekd = CheckIfFileFormatIsRecognizable(path, $"Format of {path} is not recognizable.", "Format is not recognizable", MessageBoxButton.OK, MessageBoxImage.Warning);

            if (!formatUnrecognizableOrLocekd.recognizableAndUnlocked)
                return null;
           
            string tagName = r.ReadElementContentAsString(nameof(SoundViewModel.TagName), "");

            SoundViewModel soundVM = GetSoundViewModel(_soundboard.GetPlayer(path));
            soundVM.TagName = tagName;

            return soundVM;
        }

        /// <summary>
        /// Initializes the collection of sounds from xml file.
        /// </summary>
        protected void InitializeSoundsCollectionFromXml(XmlReader r)
        {
            bool isEmpty = r.IsEmptyElement;
            //<Sounds>
            r.ReadStartElement();
            if (isEmpty) return;

            while(r.NodeType == XmlNodeType.Element)
            {
                //<SoundViewModel>
                r.ReadStartElement();

                SoundViewModel soundViewModel = GetSoundViewModelFromXml(r);
                if(soundViewModel == null)
                {
                    while(r.LocalName != "SoundViewModel")
                        r.Read();

                    r.ReadEndElement();
                    continue;
                }

                IPlayer player = SoundViewModel.GetPlayer(soundViewModel);

                _soundboard.AddSound(player);
                Sounds.Result.Add(soundViewModel);

                r.ReadEndElement();
                //</SoundViewModel>
            }

            r.ReadEndElement();
            //</Sounds>
        }

        /// <summary>
        /// Reads xml settings specified by the implementation of <see cref="SoundboardViewModel"/>.
        /// </summary>
        protected virtual void ReadXmlSettings(XmlReader r)
        {
            //<Settings>
            r.ReadStartElement();
            IsSimpleSoundboardEnabled = r.ReadElementContentAsBoolean(nameof(IsSimpleSoundboardEnabled), "");
            IsFirstDeviceEnabled = r.ReadElementContentAsBoolean(nameof(IsFirstDeviceEnabled), "");
            IsSecondDeviceEnabled = r.ReadElementContentAsBoolean(nameof(IsSecondDeviceEnabled), "");
            IsMuted = r.ReadElementContentAsBoolean(nameof(IsMuted), "");
            Volume = r.ReadElementContentAsFloat(nameof(Volume), "");
            VolumeStep = r.ReadElementContentAsFloat(nameof(VolumeStep), "");
            r.ReadEndElement();
            //</Settings>

        }

        /// <summary>
        /// Reads data from xml file.
        /// </summary>
        /// <param name="r"></param>
        public void ReadXml(XmlReader r)
        {
            //<Soundboard>
            r.ReadStartElement();
            ReadXmlSettings(r);
            InitializeSoundsCollectionFromXml(r);
            r.ReadEndElement();
            //</Soundboard>
        }

        /// <summary>
        /// Writes to xml file settings specified by the implementation of <see cref="SoundboardViewModel"/>.
        /// </summary>
        protected virtual void WriteXmlSettings(XmlWriter w)
        {
            //<Settings>
            w.WriteStartElement("Settings");

            //<IsSimpleSoundboardEnabled>
            w.WriteStartElement(nameof(IsSimpleSoundboardEnabled));
            w.WriteValue(IsSimpleSoundboardEnabled);
            w.WriteEndElement();
            //</IsSimpleSoundboardEnabled>

            //<IsFirstDeviceEnabled>
            w.WriteStartElement(nameof(IsFirstDeviceEnabled));
            w.WriteValue(IsFirstDeviceEnabled);
            w.WriteEndElement();
            //</IsFirstDeviceEnabled>

            //<IsSecondDeviceEnabled>
            w.WriteStartElement(nameof(IsSecondDeviceEnabled));
            w.WriteValue(IsSecondDeviceEnabled);
            w.WriteEndElement();
            //</IsSecondDeviceEnabled>

            //<IsMuted>
            w.WriteStartElement(nameof(IsMuted));
            w.WriteValue(IsMuted);
            w.WriteEndElement();
            //</IsMuted>

            //<Volume>
            w.WriteStartElement(nameof(Volume));
            w.WriteValue(Volume);
            w.WriteEndElement();
            //</Volume>

            //<VolumeStep>
            w.WriteStartElement(nameof(VolumeStep));
            w.WriteValue(VolumeStep);
            w.WriteEndElement();
            //</VolumeStep>

            w.WriteEndElement();
            //</Settings>
        }

        /// <summary>
        /// Writes data to xml file.
        /// </summary>
        public void WriteXml(XmlWriter w)
        {
            WriteXmlSettings(w);

            //<Sounds>
            w.WriteStartElement("Sounds");
            foreach (var sound in Sounds.Result)
            {
                w.WriteStartElement("SoundViewModel");
                foreach (var keyValuePair in sound.LoadData())
                {
                    w.WriteElementString(keyValuePair.Key, keyValuePair.Value);
                }
                w.WriteEndElement();
            }
            w.WriteEndElement();
            //</Sounds>
        }

        #endregion

        #endregion

        #region Commands

        private ICommand addSoundsCommand;

        public ICommand AddSoundsCommand
        {
            get
            {
                if (addSoundsCommand == null)
                    addSoundsCommand = new RelayCommand(o =>
                    {
                        AddSoundsByPaths(_fileDolderDialogService.OpenMultipleFiles().Paths);
                    });

                return addSoundsCommand;
            }
        }

        private ICommand removeSoundsCommand;

        public ICommand RemoveSoundsCommand
        {
            get
            {
                if (removeSoundsCommand == null)
                    removeSoundsCommand = new RelayCommand(o =>
                    {
                        IEnumerable<SoundViewModel> sounds = ((IEnumerable)o).Cast<SoundViewModel>();

                        RemoveSounds(sounds.ToArray());
                    });

                return removeSoundsCommand;
            }
        }

        private ICommand removeAllSoundsCommand;

        public ICommand RemoveAllSoundsCommand
        {
            get
            {
                if (removeAllSoundsCommand == null)
                    removeAllSoundsCommand = new RelayCommand(o =>
                    {
                        ClearSounds();
                    });

                return removeAllSoundsCommand;
            }
        }

        private ICommand playPausedCommand;

        public ICommand PlayPausedCommand
        {
            get
            {
                if (playPausedCommand == null)
                    playPausedCommand = new RelayCommand(o =>
                    {
                        PlayPaused();
                    });

                return playPausedCommand;
            }
        }

        private ICommand pauseAllCommand;

        public ICommand PauseAllCommand
        {
            get
            {
                if (pauseAllCommand == null)
                    pauseAllCommand = new RelayCommand(o =>
                    {
                        PauseAll();
                    });

                return pauseAllCommand;
            }
        }

        private ICommand stopAllCommand;

        public ICommand StopAllCommand
        {
            get
            {
                if (stopAllCommand == null)
                    stopAllCommand = new RelayCommand(o =>
                    {
                        StopAll();
                    });

                return stopAllCommand;
            }
        }

        private ICommand reloadDevicesCommand;

        public ICommand ReloadDevicesCommand
        {
            get
            {
                if (reloadDevicesCommand == null)
                    reloadDevicesCommand = new RelayCommand(o =>
                    {
                        ReloadDevices();
                    });

                return reloadDevicesCommand;
            }
        }

        private ICommand decreaseVolumeCommand;

        public ICommand DecreaseVolumeCommand
        {
            get
            {
                if (decreaseVolumeCommand == null)
                    decreaseVolumeCommand = new RelayCommand(o =>
                    {
                        DecreaseVolume();
                    });

                return decreaseVolumeCommand;
            }
        }

        private ICommand increaseVolumeCommand;

        public ICommand IncreaseVolumeCommand
        {
            get
            {
                if (increaseVolumeCommand == null)
                    increaseVolumeCommand = new RelayCommand(o =>
                    {
                        IncreaseVolume();
                    });

                return increaseVolumeCommand;
            }
        }

        private ICommand muteVolumeCommand;

        public ICommand MuteVolumeCommand
        {
            get
            {
                if (muteVolumeCommand == null)
                    muteVolumeCommand = new RelayCommand(o =>
                    {
                        IsMuted = !IsMuted;
                    });

                return muteVolumeCommand;
            }
        }


        private ICommand playPauseCommand;

        /// <summary>
        /// Gets command that plays audio or pauses it if it's already playing.
        /// </summary>
        public ICommand PlayPauseCommand
        {
            get
            {
                if (playPauseCommand == null)
                    playPauseCommand = new RelayCommand(o =>
                    {
                        var soundVM = (SoundViewModel)o;

                        if (soundVM.State == PlayerState.Playing)
                            soundVM.Pause();
                        else
                            soundVM.Play();
                    });

                return playPauseCommand;
            }
        }

        private ICommand playStopCommand;

        /// <summary>
        /// Gets command that plays audio or stops it if it's already playing.
        /// </summary>
        public ICommand PlayStopCommand
        {
            get
            {
                if (playStopCommand == null)
                    playStopCommand = new RelayCommand(o =>
                    {
                        var soundVM = (SoundViewModel)o;

                        if (soundVM.State == PlayerState.Playing)
                            soundVM.Stop();
                        else
                            soundVM.Play();
                    });

                return playStopCommand;
            }
        }

        private ICommand stopCommand;

        /// <summary>
        /// Gets command that stops audio.
        /// </summary>
        public ICommand StopCommand
        {
            get
            {
                if (stopCommand == null)
                    stopCommand = new RelayCommand(o =>
                    {
                        var soundVM = (SoundViewModel)o;
                        soundVM.Stop();
                    });

                return stopCommand;
            }
        }


        #endregion

    }
}
