using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using SlopperEngine.EditorIntegration;
using SlopperEngine.SceneObjects;
using SlopperEditor.Inspector;
using SlopperEditor.Inspector.BuiltinInspectors;
using System;
using System.Collections.Generic;

namespace SlopperEditor;

/// <summary>
/// Caches reflection info related to the editor.
/// </summary>
public class ReflectionCache
{
    ReadOnlyMemory<Type>? _sceneObjectDerivedTypes = null;
    Dictionary<Type, IMemberInspectorHandler>? _inspectorValueHandlers = null;
    readonly Dictionary<Type, ConstructorInfo?> _constructors = new();
    readonly Dictionary<Type, ReadOnlyCollection<ValueMember>> _childContainers = new();
    readonly Dictionary<Type, ReadOnlyCollection<ReadOnlyMemory<ValueMember>>> _settableMembers = new();

    private const BindingFlags All = BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public;
    private static ReflectionCache _instance = new();

    private ReflectionCache() { }
    static ReflectionCache()
    {
        Editor.OnNewAssemblyLoaded += () => _instance = new();
    }

    /// <summary>
    /// Gets the MemberInspectorHandler for a certain type, allowing creating UI elements in the inspector.
    /// </summary>
    public static IMemberInspectorHandler GetMemberInspectorHandler(Type type)
    {
        if (_instance._inspectorValueHandlers == null)
            CacheMemberInspectorHandlers();

        while (true)
        {
            if (_instance._inspectorValueHandlers!.TryGetValue(type, out var res))
                return res;

            if (type.BaseType == null)
                return new DefaultObjectInspector(); // like what even
            type = type.BaseType;
        }
    }

    static void CacheMemberInspectorHandlers()
    {
        _instance._inspectorValueHandlers = new();
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly.IsDynamic)
                continue;

            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch
            {
                types = assembly.GetExportedTypes(); // okay bro
            }
            foreach (var t in types)
            {
                if (!t.IsAssignableTo(typeof(IMemberInspectorHandler)))
                    continue;

                if (!TryCreate(t, out var instance))
                {
                    System.Console.WriteLine($"Could not create an inspector value handler {t}");
                    continue;
                }
                var inst = (IMemberInspectorHandler)instance;

                if (!_instance._inspectorValueHandlers.TryAdd(inst.GetInspectedType(), inst))
                    System.Console.WriteLine($"Could not add inspector value handler {t} at {inst.GetInspectedType()} - seems like {_instance._inspectorValueHandlers[inst.GetInspectedType()]} already assigned to this type?");
            }
        }
    }

    /// <summary>
    /// Gets all types deriving from SceneObject.
    /// </summary>
    public static ReadOnlyMemory<Type> GetAllSceneObjects()
    {
        if (_instance._sceneObjectDerivedTypes.HasValue)
            return _instance._sceneObjectDerivedTypes.Value;

        List<Type> types = new();
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly.IsDynamic)
                continue;

            if (assembly == typeof(ReflectionCache).Assembly)
                continue;

            foreach (var t in assembly.GetExportedTypes())
            {
                if (!t.IsAssignableTo(typeof(SceneObject)))
                    continue;
                if (t == typeof(Scene))
                    continue;

                types.Add(t);
            }
        }
        _instance._sceneObjectDerivedTypes = types.ToArray();
        return _instance._sceneObjectDerivedTypes.Value;
    }

    /// <summary>
    /// Whether or not a type has a usable constructor.
    /// </summary>
    /// <param name="type">The type to find a constructor for.</param>
    public static bool HasConstructor(Type type)
    {
        if(_instance._constructors.TryGetValue(type, out var ctor))
            return ctor != null;

        var res = type.GetConstructor(All, Array.Empty<Type>());
        if (res != null && (res.IsAbstract || type.IsAbstract))
            res = null;
        _instance._constructors[type] = res;
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
        if(_instance._constructors.TryGetValue(type, out var ctor))
        {
            if (ctor == null)
                return false;

            obj = ctor.Invoke(Array.Empty<object>())!;
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
        if(_instance._childContainers.TryGetValue(type, out var res))
            return res;

        List<ValueMember> gettableContainers = new();
        GetGettableChildContainersRecursive(type);
        res = gettableContainers.AsReadOnly();
        _instance._childContainers[type] = res;
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
    /// Gets all members the editor is allowed to show in a type.
    /// </summary>
    /// <param name="type">The type to get the members of.</param>
    /// <returns>A list containing arrays of members - the list is sorted by declaring type.</returns>
    public static ReadOnlyCollection<ReadOnlyMemory<ValueMember>> GetPublicMembers(Type type)
    {
        if(_instance._settableMembers.TryGetValue(type, out var res))
            return res;

        List<ReadOnlyMemory<ValueMember>> settableMembers = new();
        GetSettableMembersRecursive(type);
        res = settableMembers.AsReadOnly();
        _instance._settableMembers[type] = res;
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
