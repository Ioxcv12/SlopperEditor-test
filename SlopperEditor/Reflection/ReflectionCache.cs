using System.Collections.ObjectModel;
using System.Reflection;
using SlopperEngine.Core.Collections;

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
        GetDeclaredPropertiesFieldsRecursive(type);
        res = settableMembers.AsReadOnly();
        _instance._settableMembers.Set(type, res);
        return res;

        void GetDeclaredPropertiesFieldsRecursive(Type type)
        {
            List<ValueMember> declaredTypeMembers = new();
            foreach (var p in type.GetProperties(All))
                declaredTypeMembers.Add(new(p));
            foreach (var f in type.GetFields(All))
                declaredTypeMembers.Add(new(f));

            settableMembers.Add(declaredTypeMembers.ToArray());

            if (type.BaseType != null)
                GetDeclaredPropertiesFieldsRecursive(type.BaseType);
        }
    }
}
