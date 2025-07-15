using OpenTK.Mathematics;
using SlopperEngine.UI.Base;
using SlopperEngine.UI.Interaction;
using SlopperEngine.UI.Layout;

namespace SlopperEditor.Toolbar;

public class SceneTab : Tab
{
    public SceneTab() : base("Scene")
    {
        var layout = new LinearArrangedLayout();
        options.UIContainer.Layout.Value = layout;
        layout.IsLayoutHorizontal = false;
        layout.Padding = UISize.RelativeToParent(Vector2.Zero);
    }
}