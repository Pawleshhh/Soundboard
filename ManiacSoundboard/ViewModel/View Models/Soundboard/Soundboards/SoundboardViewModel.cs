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

    public class SoundboardViewModel : BaseViewModel, IXmlSerializable
    {

        #region Constructor

        public SoundboardViewModel() 
        {
            _messageBoxService = AppServices.MessageBoxService;
            _fileDolderDialogService = AppServices.FileFolderDialogService;
            _soundboard = SoundboardViewModelConfiguration.ConfigurationService.GetConfiguredModel();
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

        public bool IsSimpleSoundboardEnabled
        {
            get => isSimpleSoundboardEnabled;
            set
            {
                isSimpleSoundboardEnabled = value;
                OnPropertyChanged(nameof(IsSimpleSoundboardEnabled));
            }
        }

        public NotifyTaskCompletion<ObservableCollection<SoundViewModel>> Sounds { get; private set; }

        public ObservableCollection<IAudioDevice> AllDevices { get; private set; } = new ObservableCollection<IAudioDevice>();

        public IAudioDevice FirstDevice
        {
            get => _soundboard.FirstDevice;
            set
            {
                SetFirstDevice(value);
            }
        }

        public IAudioDevice SecondDevice
        {
            get => _soundboard.SecondDevice;
            set
            {
                SetSecondDevice(value);
            }
        }

        public bool IsFirstDeviceEnabled
        {
            get => _soundboard.IsFirstDeviceEnabled;
            set
            {
                _soundboard.IsFirstDeviceEnabled = value;
                OnPropertyChanged("IsFirstDeviceEnabled");
            }
        }

        public bool IsSecondDeviceEnabled
        {
            get => _soundboard.IsSecondDeviceEnabled;
            set
            {
                _soundboard.IsSecondDeviceEnabled = value;
                OnPropertyChanged("IsSecondDeviceEnabled");
            }
        }

        public float Volume
        {
            get => _soundboard.Volume;
            set
            {
                _soundboard.Volume = value;

                OnPropertyChanged("Volume");
            }
        }

        public float VolumeStep
        {
            get => _soundboard.VolumeStep;
            set
            {
                _soundboard.VolumeStep = value;
                OnPropertyChanged("VolumeStep");
            }
        }

        public bool IsMuted
        {
            get => _soundboard.IsMuted;
            set
            {
                _soundboard.IsMuted = value;

                OnPropertyChanged("IsMuted");
            }
        }

        public bool IsPlaying => _soundboard.IsPlaying;

        public bool IsBusy => IsChangingDevice || (Sounds != null && Sounds.IsNotCompleted);

        public bool IsNotBusy => !IsBusy;

        private bool isChangingDevice = false;

        public bool IsChangingDevice
        {
            get => isChangingDevice;
            set
            {
                isChangingDevice = value;

                //if (value)
                //    _soundboard.HandlesKeyEvents = !value;
                //else
                //    HandlesKeyEvents = !value;

                OnPropertyChanged("IsChangingDevice", "IsNotChangingDevice", "IsBusy", "IsNotBusy");
            }
        }

        public bool IsNotChangingDevice => !IsChangingDevice;

        #endregion

        #region Methods

        public void PlayPaused()
        {
            _soundboard.PlayPaused();
        }

        public void PauseAll()
        {
            _soundboard.PauseAll();
        }

        public void StopAll()
        {
            _soundboard.StopAll();
        }

        public void AddSoundsByPaths(string[] paths)
        {
            _prevCount = Sounds.Result.Count;
            WorkOnSoundCollection(AddSoundsByPathsAsync(Sounds.Result.ToList(), paths));

            OnPropertyChanged("Sounds");
        }

        public void RemoveSounds(params SoundViewModel[] sounds)
        {
            _prevCount = Sounds.Result.Count;
            WorkOnSoundCollection(RemoveSoundsAsync(Sounds.Result.ToList(), sounds));

            OnPropertyChanged("Sounds");
        }

        public void ClearSounds()
        {
            StopAll();
            _prevCount = Sounds.Result.Count;
            WorkOnSoundCollection(ClearAllSoundsAsync(), true);
            OnPropertyChanged("Sounds");
        }

        public async void ReloadDevices()
        {
            StopAll();
            IsChangingDevice = true;
            await ReloadDevicesAsync();
            IsChangingDevice = false;
            OnPropertyChanged("FirstDevice", "SecondDevice", "AllDevices", "IsFirstDeviceEnabled", "IsSecondDeviceEnabled");
        }

        public async void SetFirstDevice(IAudioDevice device)
        {
            StopAll();
            IsChangingDevice = true;
            await Task.Run(() => _soundboard.FirstDevice = device);
            IsChangingDevice = false;
            OnPropertyChanged("FirstDevice");
        }

        public async void SetSecondDevice(IAudioDevice device)
        {
            StopAll();
            IsChangingDevice = true;
            await Task.Run(() => _soundboard.SecondDevice = device);
            IsChangingDevice = false;
            OnPropertyChanged("SecondDevice");
        }

        public void IncreaseVolume()
        {
            _soundboard.IncreaseVolume();
            OnPropertyChanged("Volume");
        }

        public void DecreaseVolume()
        {
            _soundboard.DecreaseVolume();
            OnPropertyChanged("Volume");
        }

        public virtual SoundViewModel GetSoundViewModel(IPlayer player)
        {
            return new SoundViewModel(player, this);
        }

        public override void Dispose()
        {
            _soundboard.Dispose();
            _soundboard = null;
        }

        public IPlayer GetPlayer(string path)
        {
            return _soundboard.GetPlayer(path);
        }

        #endregion

        #region Private methods

        protected Task ReloadDevicesAsync()
        {
            return Task.Run(() =>
            {
                _ReloadDevices();
            });
        }

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

        protected Task<ObservableCollection<SoundViewModel>> AddSoundsByPathsAsync(List<SoundViewModel> previous, params string[] paths)
        {
            return Task.Run(() =>
            {
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

        protected Task<ObservableCollection<SoundViewModel>> ClearAllSoundsAsync()
        {
            return Task.Run(() =>
            {
                _soundboard.RemoveAllSounds();
                return UpdateSounds();
            });
        }

        protected ObservableCollection<SoundViewModel> UpdateSounds()
        {
            return new ObservableCollection<SoundViewModel>(_soundboard.AllPlayers.Select(n => GetSoundViewModel(n)));
        }

        protected Task<ObservableCollection<SoundViewModel>> UpdateSoundsAsync()
        {
            return Task.Run(() => UpdateSounds());
        }

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

        protected virtual void Sounds_Finished()
        {
            OnPropertyChanged("IsBusy", "IsNotBusy", "Sounds");
        }

        protected (bool exists, MessageBoxResult result) CheckIfFileExists(string path, string info, string caption, MessageBoxButton button, MessageBoxImage image)
        {
            if (!File.Exists(path))
            {
                var result = _messageBoxService.ShowMessageBoxDecision(info, caption, button, image);

                return (false, result);
            }

            return (true, MessageBoxResult.None);
        }

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

        private void Client_DevicesChanged(object sender, EventArgs e)
        {
            ReloadDevices();
        }

        #region Xml

        public virtual XmlSchema GetSchema()
        {
            return null;
        }

        protected virtual SoundViewModel GetSoundViewModelFromXml(XmlReader r)
        {
            string path = r.ReadElementContentAsString(nameof(SoundViewModel.AudioPath), "");

            var fileExistsResult = CheckIfFileExists(path, $"File {path} does not exist.", "File not found", MessageBoxButton.OK, MessageBoxImage.Warning);

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

        public void ReadXml(XmlReader r)
        {
            //<Soundboard>
            r.ReadStartElement();
            ReadXmlSettings(r);
            InitializeSoundsCollectionFromXml(r);
            r.ReadEndElement();
            //</Soundboard>
        }

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

        #endregion

    }
}
