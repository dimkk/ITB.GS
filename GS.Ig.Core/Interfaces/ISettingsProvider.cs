using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Ig.Core.Interfaces
{
    public interface ISettingsProvider
    {
        IEnumerable<ISettingsProvider> GetAll();

        ISettingsProvider Get(string key);

        string this[string key] { get; set; }

        string Value { get; set; }

        string Key { get; set; }
    }
}
