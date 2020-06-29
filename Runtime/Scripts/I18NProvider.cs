using System.Threading.Tasks;
using TinaX.I18N.Const;
using TinaX.I18N.Internal;
using TinaX.Services;

namespace TinaX.I18N
{
    [XServiceProviderOrder(100)]
    public class I18NProvider : IXServiceProvider
    {
        private II18NInternal mI18NInternal;
        public string ServiceName => I18NConst.ServiceName;

        public Task<XException> OnInit(IXCore core)
            => Task.FromResult<XException>(null);

        public void OnServiceRegister(IXCore core)
        {
            core.Services.BindBuiltInService<ILocalizationService, II18N, I18NManager>()
                .SetAlias<II18NInternal>();
        }

        public Task<XException> OnStart(IXCore core)
            => core.Services.Get<II18NInternal>().Start();

        public void OnQuit() { }

        public Task OnRestart() => Task.CompletedTask;
        
    }
}
