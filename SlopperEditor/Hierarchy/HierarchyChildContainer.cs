using SlopperEngine.UI.Base;
using SlopperEngine.UI.Interaction;
using SlopperEngine.UI.Layout;
using SlopperEngine.UI.Text;
using SlopperEditor.UI;

namespace SlopperEditor.Hierarchy;

public class HierarchyChildContainer : UIElement
{
    public readonly ChildContainer RepresentedContainer;

    readonly ToggleButton _showToggle;
    int _currentCount;

    public HierarchyChildContainer(ChildContainer representedContainer, string name) : base(default)
    {
        RepresentedContainer = representedContainer;

        Layout.Value = DefaultLayouts.DefaultVertical;

        var header = new Spacer
        {
            MinHeight = 16,
            LocalShape = new(0, 1, 1, 1),
        };
        header.Layout.Value = DefaultLayouts.DefaultHorizontal;
        UIChildren.Add(header);

        header.UIChildren.Add(_showToggle = new());
        _showToggle.OnToggle += ToggleShow;

        header.UIChildren.Add(new TextButton(name));
    }

    void ToggleShow(bool show)
    {
        if (show)
        {
            int ct = RepresentedContainer.Count;
            for (int i = 0; i < ct; i++)
            {
                var ch = RepresentedContainer.GetByIndex(i);
                UIChildren.Add(new HierarchyObject(ch));
            }
            _currentCount = ct;
        }
        else
        {
            foreach (var ch in UIChildren.AllOfType<HierarchyObject>())
                ch.Destroy();
            _currentCount = 0;
        }
    }

    public void Update()
    {
        if (!_showToggle.Checked)
            return;

        int ct = RepresentedContainer.Count;
        if (_currentCount == ct)
        {
            foreach (var ch in UIChildren.All)
                (ch as HierarchyObject)?.Update();
            return;
        }

        // check for changed children (cant be bothered rn)
    }

    protected override UIElementSize GetSizeConstraints() => new(Alignment.Max, Alignment.Min, 16, 16);
}