using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacSoundboard.ViewModel
{
 
    /// <summary>
    /// Helper interface for xml serialization that sets data for specified objects.
    /// </summary>
    public interface IXmlWritableSerialization
    {

        void SetData(Dictionary<string, string> settings);

    }

    /// <summary>
    /// Helper interface for xml serialization that loads data from specified objects.
    /// </summary>
    public interface IXmlReadableSerialization
    {

        Dictionary<string, string> LoadData();

    }

    /// <summary>
    /// Helper interface for xml serialization that loads data from and sets for specified objects.
    /// </summary>
    public interface IXmlSerialization : IXmlReadableSerialization, IXmlWritableSerialization
    {

    }

}
