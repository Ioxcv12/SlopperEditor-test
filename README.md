# SlopperEditor
SlopperEngine is a fully C# game engine, designed to be flexible and performant.
It supports 3D rendering, physics, basic 2D UI, serialization, and clean "scripting".
Planned features are a fully custom shading language, scalable multithreading, and builtin modding support.

This is the editor. For more information about the engine, check out https://github.com/capslock1/SlopperEngine

Compile instructions: 
- create a SlopperEditor.csproj.user file (next to the non-user file) and fill it with the following:
```xml
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="Current" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <ReferencePath>Absolute\Path\To\Some\Copy\Of\SlopperEngine\On\Your\Computer</ReferencePath>
  </PropertyGroup>
</Project>
```
- click the build button on your IDE

SlopperEditor uses brazilnut2000's C# gitignore template: https://gist.github.com/brazilnut2000/8226958