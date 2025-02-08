using System.Windows;

namespace SteamP2PInfo.Config
{
    public interface IConfigUIElement
    {
        string OptionName { get; }
        string Tooltip { get; }

        bool Multicolumn { get; }

        UIElement CreateUIElement(object source, string path);
    }
}
