using TinaX.I18N;

namespace TinaX.Services
{
    public static class I18NProviderExtend
    {
        public static IXCore UseI18N(this IXCore core)
        {
            core.RegisterServiceProvider(new I18NProvider());
            return core;
        }
    }
}
