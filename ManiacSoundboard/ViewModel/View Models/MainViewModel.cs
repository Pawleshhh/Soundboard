using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ManiacSoundboard.ViewModel
{
    [XmlRoot(ElementName = "ManiacSoundboard")]
    public class MainViewModel : BaseViewModel, IXmlSerializable
    {

        #region Constructors

        public MainViewModel()
        {
            SoundboardViewModel = SoundboardViewModelConfiguration.ConfigurationService.GetConfiguredViewModel();
        }

        #endregion

        #region Private fields

        #endregion

        #region Properties

        public string IconPath { get; set; } = "/View/Images/Icons/soundboardIcon_1.png";

        public string Title { get; set; } = "Maniac Soundboard";

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

        public SoundboardViewModel SoundboardViewModel { get; private set; }

        #endregion

        #region Methods

        public override void WhenClosing()
        {
            XmlSerializationService.Serialize(this, "soundboardData.xml");
            Dispose();
        }

        public override void Dispose()
        {
            SoundboardViewModel.Dispose();
        }

        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();
            SoundboardViewModel.ReadXml(reader);
            reader.ReadEndElement();
            
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Soundboard");
            //writer.WriteAttributeString("Interaction", SoundboardViewModelConfiguration.ConfigurationService.Configurations["Interaction"]);
            SoundboardViewModel.WriteXml(writer);
            writer.WriteEndElement();
        }

        #endregion

        #region Private methods

        #endregion

    }
}
