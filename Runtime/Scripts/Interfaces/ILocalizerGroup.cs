using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinaX.I18N
{
    public interface ILocalizerGroup
    {
        string this[string key, params string[] formatArgs] { get; }
        string GetText(string key);
        string GetText(string key, string defaultValue, params string[] formatArgs);
    }
}
