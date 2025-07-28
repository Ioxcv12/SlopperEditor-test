using SlopperEngine.UI.Style;
using SlopperEngine.UI.Text;

namespace SlopperEditor.UI;

/// <summary>
/// A number of easily accesible styles.
/// </summary>
public static class DefaultStyles
{
    public readonly static BasicStyle Selected = new(
        new(0.2f, 0.2f, 0.2f, 0.75f),
        new(0.3f, 0.3f, 0.3f, 0.75f),
        new(0.6f, 0.6f, 0.6f, 1),
        new(0.7f, 0.7f, 0.7f, 1),
        new(1f, 1f, 1f, 1f),
        new(1, 0, 1, 0.4f),
        RasterFont.EightXSixteen, 1);
}