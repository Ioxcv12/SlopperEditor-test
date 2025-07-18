using OpenTK.Mathematics;
using SlopperEngine.SceneObjects;
using SlopperEngine.UI.Base;
using SlopperEngine.UI.Interaction;
using SlopperEngine.UI.Layout;
using SlopperEditor.UI;

namespace SlopperEditor.Toolbar;

public class SceneTab : Tab
{
    public SceneTab(Editor editor, Toolbar bar) : base("Scene", editor, bar)
    {
    }

    protected override void SetupOptions(Popup options)
    {
        var layout = new LinearArrangedLayout();
        options.UIContainer.Layout.Value = layout;
        layout.IsLayoutHorizontal = false;
        layout.Padding = UISize.RelativeToParent(Vector2.Zero);

        {
            var button = new TextButton();
            options.UIContainer.UIChildren.Add(button);
            button.Text = "Create default";
            button.OnButtonPressed += _ => HideOptions();
            button.OnButtonPressed += _ =>
            {
                var sc = editor.OpenScene = Scene.CreateDefault();
                sc.Active = false;
            };
        }
        {
            var button = new TextButton();
            options.UIContainer.UIChildren.Add(button);
            button.Text = "Create empty";
            button.OnButtonPressed += _ => HideOptions();
            button.OnButtonPressed += _ =>
            {
                var sc = editor.OpenScene = Scene.CreateEmpty();
                sc.Active = false;
            };
        }
        {
            var button = new TextButton();
            options.UIContainer.UIChildren.Add(button);
            button.Text = "Load...";
            button.OnButtonPressed += _ => HideOptions();
        }
        {
            var button = new TextButton();
            options.UIContainer.UIChildren.Add(button);
            button.Text = "Save";
            button.OnButtonPressed += _ => HideOptions();
        }
        {
            var button = new TextButton();
            options.UIContainer.UIChildren.Add(button);
            button.Text = "Save as...";
            button.OnButtonPressed += _ => HideOptions();
        }
    }
}