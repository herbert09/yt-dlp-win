using ReaLTaiizor.Colors;
using ReaLTaiizor.Manager;
using ReaLTaiizor.Util;

namespace YtDlpDownloader;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        var skinManager = MaterialSkinManager.Instance;
        skinManager.Theme = MaterialSkinManager.Themes.DARK;
        skinManager.ColorScheme = new MaterialColorScheme(
            MaterialPrimary.BlueGrey800,
            MaterialPrimary.BlueGrey900,
            MaterialPrimary.BlueGrey500,
            MaterialAccent.LightBlue200,
            MaterialTextShade.WHITE);

        Application.Run(new Form1());
    }
}
