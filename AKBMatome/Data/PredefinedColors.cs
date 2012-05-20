using System.Collections.ObjectModel;

namespace AKBMatome.Data
{
    public class PredefinedColors
    {
        public Collection<NamedSolidColorBrush> AccentColors
        {
            get
            {
                return Constants.Media.AccentColors;
            }
        }
    }
}
