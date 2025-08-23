using System;
using System.CodeDom.Compiler;
using OpenTK.Mathematics;
using SlopperEngine.Core;
using SlopperEngine.Graphics.GPUResources.Textures;
using SlopperEngine.Graphics.ShadingLanguage;
using SlopperEngine.Rendering;

namespace SlopperEditor.SceneRender;

/// <summary>
/// Renders the scene being edited using a renderer from that scene.
/// </summary>
public class ProxySceneRenderer(SceneRenderer represented) : SceneRenderer
{
    public event Action? OnResize;

    public readonly SceneRenderer RepresentedRenderer = represented;

    public void RenderRepresented(FrameUpdateArgs args) => RepresentedRenderer.Scene?.Render(args);
    public override void AddFragmentMain(SyntaxTree scope, IndentedTextWriter writer) => RepresentedRenderer.AddFragmentMain(scope, writer);
    public override void AddVertexMain(SyntaxTree scope, IndentedTextWriter writer) => RepresentedRenderer.AddVertexMain(scope, writer);
    public override Texture2D GetOutputTexture() => RepresentedRenderer.GetOutputTexture();
    public override Vector2i GetScreenSize() => RepresentedRenderer.GetScreenSize();
    public override void InputUpdate(InputUpdateArgs input) => RepresentedRenderer.InputUpdate(input);
    public override void Resize(Vector2i newSize)
    {
        RepresentedRenderer.Resize(newSize);
        OnResize?.Invoke();
    }
    protected override void RenderInternal() { }
}