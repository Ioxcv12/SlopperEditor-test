using SlopperEngine.Core;
using SlopperEngine.Graphics.Loaders;
using SlopperEngine.UI.Interaction;

namespace SlopperEditor.UI;

public static class TexturedButtons
{
    public static ToggleButton CreateCollapseButton() => new(
        TextureLoader.FromFilepath(Assets.GetPath("textures/closeMenu.png", "EditorAssets")),
        TextureLoader.FromFilepath(Assets.GetPath("textures/openMenu.png", "EditorAssets"))
    );

    public static ToggleButton CreateCloseButton() => new(
        TextureLoader.FromFilepath(Assets.GetPath("textures/closeMark.png", "EditorAssets")),
        TextureLoader.FromFilepath(Assets.GetPath("textures/closeMark.png", "EditorAssets"))
    );
}