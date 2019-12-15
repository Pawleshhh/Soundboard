using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacSoundboard.ViewModel
{
    
    public interface IXmlWritableSerialization
    {

        void SetData(Dictionary<string, string> settings);

    }

    public interface IXmlReadableSerialization
    {

        Dictionary<string, string> LoadData();

    }

    public interface IXmlSerialization : IXmlReadableSerialization, IXmlWritableSerialization
    {

    }

}
