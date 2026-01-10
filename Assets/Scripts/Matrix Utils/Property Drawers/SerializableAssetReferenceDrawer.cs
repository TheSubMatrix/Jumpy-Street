#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using MatrixUtils.GenericDatatypes;
using System;
using System.Reflection;

namespace MatrixUtils.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(SerializableAssetReference<>), true)]
    public class SerializableAssetReferenceDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            // For managed references, get or create the instance
            if (property.propertyType == SerializedPropertyType.ManagedReference)
            {
                object instance = property.managedReferenceValue;
                
                if (instance == null)
                {
                    Type expectedType = GetExpectedType(property);
                    if (expectedType != null)
                    {
                        instance = Activator.CreateInstance(expectedType);
                        property.managedReferenceValue = instance;
                        property.serializedObject.ApplyModifiedProperties();
                    }
                }
                
                if (instance != null)
                {
                    return CreateObjectFieldForManagedReference(property, instance);
                }
                
                return new HelpBox("Could not create SerializableAssetReference instance", HelpBoxMessageType.Error);
            }
            
            // For regular serialized fields
            return CreateObjectFieldForSerializedField(property);
        }
        
        VisualElement CreateObjectFieldForManagedReference(SerializedProperty property, object instance)
        {
            Type instanceType = instance.GetType();
            Type assetType = instanceType.GetGenericArguments()[0];
            
            ObjectField objectField = new(property.displayName)
            {
                objectType = assetType,
                allowSceneObjects = false
            };
            
            // Get current value
            PropertyInfo assetProperty = instanceType.GetProperty("Asset");
            if (assetProperty != null)
            {
                UnityEngine.Object currentAsset = assetProperty.GetValue(instance) as UnityEngine.Object;
                objectField.SetValueWithoutNotify(currentAsset);
            }
            
            // Track external changes
            objectField.schedule.Execute(() =>
            {
                object updatedInstance = property.managedReferenceValue;
                if (updatedInstance != null && assetProperty != null)
                {
                    UnityEngine.Object asset = assetProperty.GetValue(updatedInstance) as UnityEngine.Object;
                    if (objectField.value != asset)
                    {
                        objectField.SetValueWithoutNotify(asset);
                    }
                }
            }).Every(100);
            
            // Handle user changes
            objectField.RegisterValueChangedCallback(evt =>
            {
                object currentInstance = property.managedReferenceValue;
                if (currentInstance == null) return;
                
                MethodInfo setAssetMethod = instanceType.GetMethod("SetAsset");
                if (setAssetMethod != null)
                {
                    setAssetMethod.Invoke(currentInstance, new object[] { evt.newValue });
                    
                    // Re-assign to trigger serialization
                    property.managedReferenceValue = currentInstance;
                    property.serializedObject.ApplyModifiedProperties();
                    EditorUtility.SetDirty(property.serializedObject.targetObject);
                }
            });
            
            return objectField;
        }
        
        VisualElement CreateObjectFieldForSerializedField(SerializedProperty property)
        {
            SerializedProperty guidProp = property.FindPropertyRelative("m_assetGuid");
            SerializedProperty localIdProp = property.FindPropertyRelative("m_localId");
            SerializedProperty pathProp = property.FindPropertyRelative("m_assetPath");
            
            if (guidProp == null || localIdProp == null || pathProp == null)
            {
                return new HelpBox("SerializableAssetReference: Could not find serialized properties", HelpBoxMessageType.Error);
            }
            
            Type assetType = GetAssetTypeFromFieldInfo();
            
            ObjectField objectField = new(property.displayName)
            {
                objectType = assetType,
                allowSceneObjects = false
            };
            
            // Load current asset
            UnityEngine.Object currentAsset = LoadAsset(guidProp.stringValue, localIdProp.longValue, assetType);
            objectField.SetValueWithoutNotify(currentAsset);
            
            // Track changes
            objectField.TrackPropertyValue(guidProp, _ =>
            {
                UnityEngine.Object asset = LoadAsset(guidProp.stringValue, localIdProp.longValue, assetType);
                objectField.SetValueWithoutNotify(asset);
            });
            
            // Handle user changes
            objectField.RegisterValueChangedCallback(evt =>
            {
                UnityEngine.Object newAsset = evt.newValue;
                
                if (newAsset == null)
                {
                    guidProp.stringValue = string.Empty;
                    localIdProp.longValue = 0;
                    pathProp.stringValue = string.Empty;
                }
                else
                {
                    string assetPath = AssetDatabase.GetAssetPath(newAsset);
                    
                    if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(newAsset, out string guid, out long localId))
                    {
                        guidProp.stringValue = guid;
                        localIdProp.longValue = localId;
                        pathProp.stringValue = assetPath;
                    }
                    else
                    {
                        guidProp.stringValue = AssetDatabase.AssetPathToGUID(assetPath);
                        localIdProp.longValue = 0;
                        pathProp.stringValue = assetPath;
                    }
                }
                
                property.serializedObject.ApplyModifiedProperties();
            });
            
            return objectField;
        }
        
        UnityEngine.Object LoadAsset(string guid, long localId, Type assetType)
        {
            if (string.IsNullOrEmpty(guid)) return null;
            
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(assetPath)) return null;
            
            if (localId != 0)
            {
                // Sub-asset
                UnityEngine.Object[] allAssets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
                foreach (var asset in allAssets)
                {
                    if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out string assetGuid, out long assetLocalId))
                    {
                        if (assetGuid == guid && assetLocalId == localId)
                        {
                            return asset;
                        }
                    }
                }
            }
            else
            {
                // Regular asset
                return AssetDatabase.LoadAssetAtPath(assetPath, assetType);
            }
            
            return null;
        }
        
        Type GetAssetTypeFromFieldInfo()
        {
            if (fieldInfo != null && fieldInfo.FieldType.IsGenericType)
            {
                Type[] args = fieldInfo.FieldType.GetGenericArguments();
                if (args.Length > 0) return args[0];
            }
            return typeof(UnityEngine.Object);
        }
        
        Type GetExpectedType(SerializedProperty property)
        {
            object targetObject = property.serializedObject.targetObject;
            if (targetObject == null) return null;
            
            string[] pathParts = property.propertyPath.Split('.');
            Type currentType = targetObject.GetType();
            
            foreach (string part in pathParts)
            {
                if (part.Contains("[")) continue;
                
                FieldInfo field = currentType.GetField(part, 
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                
                if (field == null) return null;
                currentType = field.FieldType;
            }
            
            return currentType;
        }
    }
}
#endif