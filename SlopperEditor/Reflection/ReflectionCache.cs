using System.Collections.ObjectModel;
using System.Reflection;
using SlopperEngine.Core.Collections;
using SlopperEngine.EditorIntegration;

namespace SlopperEditor.Reflection;

/// <summary>
/// Caches reflection info related to the editor.
/// </summary>
public class ReflectionCache
{
    readonly Cache<Type, ReadOnlyCollection<ReadOnlyMemory<ValueMember>>> _settableMembers = new();

    private const BindingFlags All = BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public;
    private static ReflectionCache _instance = new();

    private ReflectionCache(){}
    static ReflectionCache()
    {
        Editor.OnNewAssemblyLoaded += () => _instance = new();
    }

    /// <summary>
    /// Gets all members the editor is allowed to set in a type.
    /// </summary>
    /// <param name="type">The type to get the members of.</param>
    public static ReadOnlyCollection<ReadOnlyMemory<ValueMember>> GetSettableMembers(Type type)
    {
        var res = _instance._settableMembers.Get(type);
        if (res != null)
            return res;

        List<ReadOnlyMemory<ValueMember>> settableMembers = new();
        GetSettableMembersRecursive(type);
        res = settableMembers.AsReadOnly();
        _instance._settableMembers.Set(type, res);
        return res;

        void GetSettableMembersRecursive(Type type)
        {
            List<ValueMember> declaredTypeMembers = new();
            foreach (var p in type.GetProperties(All))
            {
                if (p.GetCustomAttribute<HideInInspectorAttribute>() != null)
                    continue;

                bool getPublic = p.GetMethod?.IsPublic ?? false;
                bool setPublic = p.SetMethod?.IsPublic ?? false;
                bool showAnyway = p.GetCustomAttribute<ShowInInspectorAttribute>() != null;
                bool? editable = p.GetCustomAttribute<EditableInInspectorAttribute>()?.Editable;

                if (!getPublic && !showAnyway)
                    continue;

                declaredTypeMembers.Add(new(p, editable ?? setPublic | showAnyway));
            }
            foreach (var f in type.GetFields(All))
            {
                if (f.GetCustomAttribute<HideInInspectorAttribute>() != null)
                    continue;

                if (!f.IsPublic && f.GetCustomAttribute<ShowInInspectorAttribute>() == null)
                    continue;

                bool? editable = f.GetCustomAttribute<EditableInInspectorAttribute>()?.Editable;
                declaredTypeMembers.Add(new(f, editable ?? !f.IsInitOnly));
            }

            settableMembers.Add(declaredTypeMembers.ToArray());

            if (type.BaseType != null)
                GetSettableMembersRecursive(type.BaseType);
        }
    }
}
