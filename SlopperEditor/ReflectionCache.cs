using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using SlopperEngine.Core.Collections;
using SlopperEngine.EditorIntegration;
using SlopperEngine.SceneObjects;
using SlopperEditor.Inspector;

namespace SlopperEditor;

/// <summary>
/// Caches reflection info related to the editor.
/// </summary>
public class ReflectionCache
{
    readonly Cache<Type, Tuple<ConstructorInfo?>> _constructors = new();
    readonly Cache<Type, ReadOnlyCollection<ValueMember>> _childContainers = new();
    readonly Cache<Type, ReadOnlyCollection<ReadOnlyMemory<ValueMember>>> _settableMembers = new();

    private const BindingFlags All = BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public;
    private static ReflectionCache _instance = new();

    private ReflectionCache() { }
    static ReflectionCache()
    {
        Editor.OnNewAssemblyLoaded += () => _instance = new();
    }

    /// <summary>
    /// Whether or not a type has a usable constructor.
    /// </summary>
    /// <param name="type">The type to find a constructor for.</param>
    public static bool HasConstructor(Type type)
    {
        var ctor = _instance._constructors.Get(type);
        if (ctor != null)
            return ctor.Item1 != null;

        var res = type.GetConstructor(All, Array.Empty<Type>());
        if (res != null && res.IsAbstract)
            res = null;
        _instance._constructors.Set(type, new(res));
        return res != null;
    }

    /// <summary>
    /// Tries to create a new instance of a specific type.
    /// </summary>
    /// <param name="type">The type to create an instance of.</param>
    /// <param name="obj">The created instance.</param>
    /// <returns>Whether or not the instance was successfully created.</returns>
    public static bool TryCreate(Type type, [NotNullWhen(true)] out object? obj)
    {
        obj = null;
        var ctor = _instance._constructors.Get(type);
        if (ctor != null)
        {
            if (ctor.Item1 == null)
                return false;

            obj = ctor.Item1.Invoke(null, Array.Empty<object>())!;
            return true;
        }

        if (HasConstructor(type))
            return TryCreate(type, out obj);
        else return false;
    }

    /// <summary>
    /// Gets all childcontainers the editor is allowed to modify.
    /// </summary>
    /// <param name="type">The type to get the childcontainers of.</param>
    /// <returns>A list containing arrays of members - the list is sorted by declaring type.</returns>
    public static ReadOnlyCollection<ValueMember> GetChildContainers(Type type)
    {
        var res = _instance._childContainers.Get(type);
        if (res != null)
            return res;

        List<ValueMember> gettableContainers = new();
        GetGettableChildContainersRecursive(type);
        res = gettableContainers.AsReadOnly();
        _instance._childContainers.Set(type, res);
        return res;

        void GetGettableChildContainersRecursive(Type type)
        {
            foreach (var p in type.GetProperties(All))
            {
                if (!p.PropertyType.IsAssignableTo(typeof(SceneObject.ChildContainer)))
                    continue;

                if (p.GetCustomAttribute<HideInInspectorAttribute>() != null)
                    continue;

                bool getPublic = p.GetMethod?.IsPublic ?? false;
                bool showAnyway = p.GetCustomAttribute<ShowInInspectorAttribute>() != null;

                if (!getPublic && !showAnyway)
                    continue;

                gettableContainers.Add(new(p, false));
            }
            foreach (var f in type.GetFields(All))
            {
                if (!f.FieldType.IsAssignableTo(typeof(SceneObject.ChildContainer)))
                    continue;

                if (!f.IsPublic && f.GetCustomAttribute<ShowInInspectorAttribute>() == null)
                    continue;

                if (f.GetCustomAttribute<HideInInspectorAttribute>() != null)
                    continue;

                gettableContainers.Add(new(f, false));
            }

            if (type.BaseType != null)
                GetGettableChildContainersRecursive(type.BaseType);
        }
    }

    /// <summary>
    /// Gets all members the editor is allowed to set in a type.
    /// </summary>
    /// <param name="type">The type to get the members of.</param>
    /// <returns>A list containing arrays of members - the list is sorted by declaring type.</returns>
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
