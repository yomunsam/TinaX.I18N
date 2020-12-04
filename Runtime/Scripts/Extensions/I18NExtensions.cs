using TinaX.I18N.Const;

namespace TinaX.I18N
{
    public static class I18NExtensions
    {
        public static ILocalizerGroup GetGroup(this II18N i18n,string GroupName = I18NConst.DefaultGroupName)
            => new LocalizerGroup(GroupName, i18n);
    }
}
