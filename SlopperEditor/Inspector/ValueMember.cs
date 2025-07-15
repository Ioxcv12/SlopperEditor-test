using System.Reflection;

namespace SlopperEditor.Inspector;

/// <summary>
/// Represents a field or property.
/// </summary>
public readonly record struct ValueMember
{
    /// <summary>
    /// The type of member this ValueMember stores.
    /// </summary>
	public Type MemberType => _property?.PropertyType ?? _field?.FieldType!;

    /// <summary>
    /// The type that declared this ValueMember.
    /// </summary>
    public Type DeclaringType => _property?.DeclaringType ?? _field!.DeclaringType!; // only one can be null

    /// <summary>
    /// Gets whether or not the member is settable.
    /// </summary>
    public readonly bool IsSettable;

    readonly PropertyInfo? _property;
    readonly FieldInfo? _field;

    public ValueMember(PropertyInfo property, bool canSet)
    {
        _property = property;
        IsSettable = canSet;
    }

    public ValueMember(FieldInfo field, bool canSet)
    {
        _field = field;
        IsSettable = canSet;
    }

    /// <summary>
    /// Gets the value of this member in obj. 
    /// </summary>
    /// <param name="obj">The object to retrieve this member's value from.</param>
    public object? GetValue(object obj)
    {
        if (_field != null)
            return _field.GetValue(obj);
        return _property!.GetValue(obj);
    }

    /// <summary>
    /// Tries to set the value of this member in obj. Note that this can still throw exceptions when the member's type does not match the given value.
    /// </summary>
    /// <param name="obj">The object to set this member's value in.</param>
    public bool TrySetValue(object obj, object? value)
    {
        if (!IsSettable)
            return false;

        if (_field != null)
        {
            _field.SetValue(obj, value);
            return true;
        }

        if (_property?.SetMethod != null)
        {
            _property.SetValue(obj, value);
            return true;
        }
        return false;
    }

}
