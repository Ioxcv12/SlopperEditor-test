using SlopperEngine.Core;

namespace SlopperEditor;

/// <summary>
/// Handles loading assets from both the edited program and the editor.
/// </summary>
public class EditorAssetHandler : Assets
{
    /// <summary>
    /// The path to the program that is currently being edited.
    /// </summary>
    public string? EditedProgramFolderPath = null;

    protected override string GetPathInternal(string relativePath, string assetFolderName)
    {
        if (assetFolderName != "EditorAssets")
            return base.GetPathInternal(relativePath, assetFolderName);

        var path = FindPathToFolder(assetFolderName, EditedProgramFolderPath);
        return Path.Combine(path, relativePath);
    }
}