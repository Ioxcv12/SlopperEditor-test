using OpenTK.Mathematics;
using SlopperEngine.UI.Base;
using SlopperEngine.UI.Interaction;
using SlopperEngine.UI.Layout;

namespace SlopperEditor.Toolbar;

public class SceneTab : Tab
{
    public SceneTab(Editor editor) : base("Scene", editor)
    {
        var layout = new LinearArrangedLayout();
        options.UIContainer.Layout.Value = layout;
        layout.IsLayoutHorizontal = false;
        layout.Padding = UISize.RelativeToParent(Vector2.Zero);

        {
            var button = new TextButton();
            options.UIContainer.UIChildren.Add(button);
            button.Text = "Create new";
        }
        {
            var button = new TextButton();
            options.UIContainer.UIChildren.Add(button);
            button.Text = "Load...";
        }
        {
            var button = new TextButton();
            options.UIContainer.UIChildren.Add(button);
            button.Text = "Save";
        }
        {
            var button = new TextButton();
            options.UIContainer.UIChildren.Add(button);
            button.Text = "Save as...";
        }
    }
}