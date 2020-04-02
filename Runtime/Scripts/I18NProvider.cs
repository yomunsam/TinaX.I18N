using System;
using System.Threading.Tasks;
using TinaX.I18N.Const;
using TinaX.I18N.Internal;
using TinaX.Services;

namespace TinaX.I18N
{
    [XServiceProviderOrder(100)]
    public class I18NProvider : IXServiceProvider
    {
        private IXCore mCore;
        private II18NInternal mI18NInternal;
        public string ServiceName => I18NConst.ServiceName;

        public Task<bool> OnInit()
        {
            mCore = XCore.GetMainInstance();
            return Task.FromResult(true);
        }

        public XException GetInitException() => null;


        public void OnServiceRegister()
        {
            mCore.BindSingletonService<II18N, I18NManager>().SetAlias<II18NInternal>();
        }

        public Task<bool> OnStart()
        {
            mI18NInternal = mCore.GetService<II18NInternal>();
            return mI18NInternal.Start();
        }
        public XException GetStartException()
        {
            return mI18NInternal?.GetStartException();
        }

        

        public void OnQuit() { }

        public Task OnRestart() => Task.CompletedTask;

        

        
    }
}
