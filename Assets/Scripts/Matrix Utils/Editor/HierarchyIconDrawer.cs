#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MatrixUtils.Attributes;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class HierarchyIconDrawer
{
    static readonly Texture2D s_requiredIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Scripts/Matrix Utils/Editor/Editor Assets/RequiredFieldIcon.png");
    static readonly Dictionary<Type, MemberInfo[]> s_cachedRequiredMembers = new();
    static readonly Dictionary<Type, MemberInfo[]> s_cachedSerializedMembers = new();
    static readonly Type s_requiredFieldAttributeType = typeof(RequiredFieldAttribute);
    static readonly Type s_serializeFieldAttributeType = typeof(SerializeField);

    static HierarchyIconDrawer()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyItemOnGUI;
        Undo.undoRedoEvent += OnUndoRedo;
    }

    static void OnUndoRedo(in UndoRedoInfo info)
    {
        // Force the hierarchy window to repaint after undo/redo
        EditorApplication.RepaintHierarchyWindow();
    }

    static void OnHierarchyItemOnGUI(int entityId, Rect selectionRect)
    {
        if (EditorUtility.EntityIdToObject(entityId) is not GameObject gameObject) return;

        if (!gameObject.GetComponents<Component>()
                .Where(component => component != null)
                .Any(component => HasUnassignedRequiredField(component, component.GetType(), component.GetType()))) return;

        Rect iconRect = new Rect(selectionRect.xMax - 20, selectionRect.y, 16, 16);

        if (s_requiredIcon != null)
        {
            GUI.Label(iconRect, new GUIContent(s_requiredIcon, "One or more required fields are missing or empty."));
        }
        else
        {
            EditorGUI.DrawRect(iconRect, Color.red);
        }
    }

    static bool HasUnassignedRequiredField(object targetObject, Type fieldType, Type rootComponentType)
    {
        if (targetObject == null) return false;

        // Check direct required members (fields and properties) on this object
        MemberInfo[] requiredMembers = GetCachedRequiredMembers(fieldType);
        if (requiredMembers.Any(member => IsFieldUnassigned(GetMemberValue(member, targetObject))))
        {
            return true;
        }

        // Recursively check serialized members for nested required fields
        MemberInfo[] serializedMembers = GetCachedSerializedMembers(fieldType);

        foreach (var member in serializedMembers)
        {
            object value = GetMemberValue(member, targetObject);
            if (value == null) continue;

            Type valueType = GetMemberType(member);

            // Skip if it's the root component (prevent infinite recursion)
            if (valueType == rootComponentType) continue;

            // Skip Unity Objects (they're not nested serializable classes)
            if (valueType.IsSubclassOf(typeof(UnityEngine.Object))) continue;

            // Skip primitives and strings
            if (valueType.IsPrimitive || valueType == typeof(string)) continue;

            // Handle arrays
            if (valueType.IsArray)
            {
                if (value is Array array)
                {
                    Type elementType = valueType.GetElementType();
                    if (elementType != null && !elementType.IsPrimitive && elementType != typeof(string) && !elementType.IsSubclassOf(typeof(UnityEngine.Object)))
                    {
                        foreach (var element in array)
                        {
                            if (element != null && HasUnassignedRequiredField(element, elementType, rootComponentType))
                            {
                                return true;
                            }
                        }
                    }
                }
                continue;
            }

            // Handle generic collections (List<T>, etc.)
            if (value is IEnumerable enumerable && valueType.IsGenericType)
            {
                Type[] genericArgs = valueType.GetGenericArguments();
                if (genericArgs.Length > 0)
                {
                    Type elementType = genericArgs[0];
                    if (!elementType.IsPrimitive && elementType != typeof(string) && !elementType.IsSubclassOf(typeof(UnityEngine.Object)))
                    {
                        foreach (var element in enumerable)
                        {
                            if (element != null && HasUnassignedRequiredField(element, elementType, rootComponentType))
                            {
                                return true;
                            }
                        }
                    }
                }
                continue;
            }

            // Handle regular nested classes
            if (valueType.IsClass && HasUnassignedRequiredField(value, valueType, rootComponentType))
            {
                return true;
            }
        }

        return false;
    }

    static object GetMemberValue(MemberInfo member, object obj)
    {
        return member switch
        {
            FieldInfo field => field.GetValue(obj),
            PropertyInfo property => property.GetValue(obj),
            _ => null
        };
    }

    static Type GetMemberType(MemberInfo member)
    {
        return member switch
        {
            FieldInfo field => field.FieldType,
            PropertyInfo property => property.PropertyType,
            _ => null
        };
    }

    static bool IsFieldUnassigned(object fieldValue)
    {
        return RequiredFieldUtility.IsFieldUnassigned(fieldValue);
    }

    static MemberInfo[] GetCachedRequiredMembers(Type componentType)
    {
        if (s_cachedRequiredMembers.TryGetValue(componentType, out MemberInfo[] members)) return members;

        List<MemberInfo> requiredMembers = new List<MemberInfo>();

        // Check fields
        FieldInfo[] allFields = componentType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        requiredMembers.AddRange(allFields.Where(field =>
            (field.IsPublic || field.IsDefined(s_serializeFieldAttributeType)) &&
            field.IsDefined(s_requiredFieldAttributeType, false)));

        // Check properties with [field: SerializeField] and [field: RequiredField]
        PropertyInfo[] allProperties = componentType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var property in allProperties)
        {
            // For auto-properties, check the backing field
            FieldInfo backingField = componentType.GetField($"<{property.Name}>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
            if (backingField != null)
            {
                bool hasSerializeField = backingField.IsDefined(s_serializeFieldAttributeType);
                bool hasRequiredField = backingField.IsDefined(s_requiredFieldAttributeType, false);

                if (hasSerializeField && hasRequiredField)
                {
                    requiredMembers.Add(property);
                }
            }
        }

        MemberInfo[] result = requiredMembers.ToArray();
        s_cachedRequiredMembers[componentType] = result;
        return result;
    }

    static MemberInfo[] GetCachedSerializedMembers(Type componentType)
    {
        if (s_cachedSerializedMembers.TryGetValue(componentType, out MemberInfo[] members)) return members;

        List<MemberInfo> serializedMembers = new List<MemberInfo>();

        // Check fields
        FieldInfo[] allFields = componentType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        serializedMembers.AddRange(allFields.Where(field => field.IsPublic || field.IsDefined(s_serializeFieldAttributeType)));

        // Check properties with [field: SerializeField]
        PropertyInfo[] allProperties = componentType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var property in allProperties)
        {
            FieldInfo backingField = componentType.GetField($"<{property.Name}>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
            if (backingField != null && backingField.IsDefined(s_serializeFieldAttributeType))
            {
                serializedMembers.Add(property);
            }
        }

        MemberInfo[] result = serializedMembers.ToArray();
        s_cachedSerializedMembers[componentType] = result;
        return result;
    }
}
#endif