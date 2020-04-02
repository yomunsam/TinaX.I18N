using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinaX.I18N.Internal
{
    public interface II18NInternal
    {
        XException GetStartException();
        Task<bool> Start();
    }
}
