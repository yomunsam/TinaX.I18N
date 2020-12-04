namespace TinaX.I18N
{
    public class LocalizerGroup : ILocalizerGroup
    {
        public readonly string GroupName;

        private II18N _i18N;

        public LocalizerGroup(string groupName, II18N i18n)
        {
            GroupName = groupName;
            _i18N = i18n;
        }

        public string this [string key, params string[] formatArgs]
        {
            get
            {
                if (formatArgs == null || formatArgs.Length == 0)
                    return _i18N.GetText(key, GroupName);
                else
                    return string.Format(_i18N.GetText(key, GroupName), formatArgs);
            }
        }

        public string GetText(string key)
            => this[key];

        public string GetText(string key, string defaultValue, params string[] formatArgs)
        {
            if (formatArgs == null || formatArgs.Length == 0)
                return _i18N.GetText(key, GroupName, defaultValue);
            else
                return string.Format(_i18N.GetText(key, GroupName, defaultValue), formatArgs);
        }

    }
}
