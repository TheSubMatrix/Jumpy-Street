using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MatrixUtils.GenericDatatypes
{
    /// <summary>
    /// A serializable wrapper for Unity asset references that works inside [SerializeReference] fields.
    /// Stores the asset using GUID and LocalID to support sub-assets (like InputActionReference).
    /// </summary>
    [Serializable]
    public class SerializableAssetReference<T> where T : UnityEngine.Object
    {
        [SerializeField] string m_assetGuid;
        [SerializeField] long m_localId; // For sub-assets
        [SerializeField] string m_assetPath; // Store path for runtime loading
        
        [NonSerialized] T m_cachedAsset;
        [NonSerialized] bool m_hasAttemptedLoad;

        public T Asset
        {
            get
            {
                if (m_cachedAsset != null) return m_cachedAsset;
                if (m_hasAttemptedLoad) return null;
                
                m_hasAttemptedLoad = true;
                
                #if UNITY_EDITOR
                    if (string.IsNullOrEmpty(m_assetGuid)) return m_cachedAsset;
                    
                    string assetPath = AssetDatabase.GUIDToAssetPath(m_assetGuid);
                    if (string.IsNullOrEmpty(assetPath)) return m_cachedAsset;
                    
                    // If we have a LocalID, this is a sub-asset
                    if (m_localId != 0)
                    {
                        UnityEngine.Object[] allAssets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
                        
                        foreach (var asset in allAssets)
                        {
                            if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out string guid, out long localId))
                            {
                                if (guid == m_assetGuid && localId == m_localId && asset is T typedAsset)
                                {
                                    m_cachedAsset = typedAsset;
                                    return m_cachedAsset;
                                }
                            }
                        }
                    }
                    else
                    {
                        // Regular asset (not a sub-asset)
                        m_cachedAsset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                    }
                #else
                    // Runtime: Try to load from Resources
                    if (!string.IsNullOrEmpty(m_assetPath))
                    {
                        string resourcesPath = ExtractResourcesPath(m_assetPath);
                        if (!string.IsNullOrEmpty(resourcesPath))
                        {
                            m_cachedAsset = Resources.Load<T>(resourcesPath);
                            if (m_cachedAsset == null)
                            {
                                Debug.LogWarning($"SerializableAssetReference: Failed to load {typeof(T).Name} from Resources at path: {resourcesPath}");
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"SerializableAssetReference: Asset at '{m_assetPath}' is not in a Resources folder. Consider using Addressables for runtime loading.");
                        }
                    }
                #endif
                
                return m_cachedAsset;
            }
        }

        public bool IsValid => Asset != null;
        
        public string Guid => m_assetGuid;
        public string Path => m_assetPath;

        public SerializableAssetReference() { }

        public SerializableAssetReference(T asset)
        {
            SetAsset(asset);
        }

        public void SetAsset(T asset)
        {
            m_cachedAsset = asset;
            m_hasAttemptedLoad = true;
            
            #if UNITY_EDITOR
            if (asset != null)
            {
                string assetPath = AssetDatabase.GetAssetPath(asset);
                m_assetPath = assetPath;
                
                if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out string guid, out long localId))
                {
                    m_assetGuid = guid;
                    m_localId = localId;
                }
                else
                {
                    m_assetGuid = AssetDatabase.AssetPathToGUID(assetPath);
                    m_localId = 0;
                }
            }
            else
            {
                m_assetGuid = string.Empty;
                m_assetPath = string.Empty;
                m_localId = 0;
            }
            #endif
        }

        public void ClearCache()
        {
            m_cachedAsset = null;
            m_hasAttemptedLoad = false;
        }

        static string ExtractResourcesPath(string fullPath)
        {
            // Remove extension
            int extensionIndex = fullPath.LastIndexOf('.');
            if (extensionIndex > 0)
            {
                fullPath = fullPath[..extensionIndex];
            }
            
            // Find the "Resources" folder in the path
            string[] pathParts = fullPath.Split('/');
            int resourcesIndex = -1;
            
            for (int i = 0; i < pathParts.Length; i++)
            {
                if (!pathParts[i].Equals("Resources", StringComparison.OrdinalIgnoreCase)) continue;
                resourcesIndex = i;
                break;
            }
            
            if (resourcesIndex < 0 || resourcesIndex == pathParts.Length - 1)
                return null;
            
            // Build path from the Resources folder onwards
            return string.Join("/", pathParts, resourcesIndex + 1, pathParts.Length - resourcesIndex - 1);
        }

        public static implicit operator T(SerializableAssetReference<T> reference)
        {
            return reference?.Asset;
        }
    }
}