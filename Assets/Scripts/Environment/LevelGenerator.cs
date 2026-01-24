using UnityEngine;
using System;
using UnityEngine.Pool;
using Object = UnityEngine.Object;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    [Header("Tile Settings")]
    [SerializeField] Vector3 m_tileSize = new(10f, 0f, 10f);
    [SerializeField] List<EnvironmentTilePool> m_tilePools = new();
    
    [Header("Generation Settings")]
    [SerializeField] int m_tilesAhead = 5;
    [SerializeField] int m_tilesBehind = 2;

    readonly LinkedList<ActiveTile> m_activeTiles = new();
    float m_nextSpawnZ;
    float m_cleanupThreshold;

    void Start()
    {
        foreach(EnvironmentTilePool tilePool in m_tilePools)
        {
            tilePool.Initialize();
        }
        
        m_nextSpawnZ = -m_tilesBehind * m_tileSize.z;
        
        for (int i = 0; i < m_tilesAhead + m_tilesBehind; i++)
        {
            GenerateTile();
        }
        
        m_cleanupThreshold = -m_tilesBehind * m_tileSize.z;
    }

    public void OnPlayerMoved(Vector3 playerPosition)
    {
        while (m_nextSpawnZ < playerPosition.z + m_tilesAhead * m_tileSize.z)
        {
            GenerateTile();
        }
        CleanupOldTiles(playerPosition);
    }

    void GenerateTile()
    {
        Vector3 position = new(0f, 0f, m_nextSpawnZ);
    
        EnvironmentTilePool pool = m_tilePools[UnityEngine.Random.Range(0, m_tilePools.Count)];
        ActiveTile newTile = pool.Get();
        newTile.Tile.transform.position = position;
        newTile.Tile.SetActive(true);
    
        m_activeTiles.AddLast(newTile);
        m_nextSpawnZ += m_tileSize.z;
    }

    void CleanupOldTiles(Vector3 playerPosition)
    {
        float cleanupZ = playerPosition.z + m_cleanupThreshold;
        
        while (m_activeTiles.Count > 0)
        {
            ActiveTile tile = m_activeTiles.First.Value;
            
            if (tile.Tile.transform.position.z < cleanupZ)
            {
                m_activeTiles.RemoveFirst();
                tile.Pool.Release(tile);
            }
            else
            {
                break;
            }
        }
    }

    void OnDestroy()
    {
        foreach (ActiveTile tile in m_activeTiles)
        {
            if (tile.Tile != null)
            {
                Destroy(tile.Tile);
            }
        }
        m_activeTiles.Clear();
    }
}

public class ActiveTile
{
    public ActiveTile(GameObject tile, EnvironmentTilePool pool)
    {
        Tile = tile;
        Pool = pool;
    }

    public readonly GameObject Tile;
    public readonly EnvironmentTilePool Pool;
}

[Serializable]
public class EnvironmentTilePool : IObjectPool<ActiveTile>
{
    [SerializeField] GameObject m_tilePrefab;
    ObjectPool<ActiveTile> m_pool;

    public void Initialize()
    {
        m_pool = new(
            createFunc: () =>
            {
                GameObject obj = Object.Instantiate(m_tilePrefab.gameObject);
                obj.SetActive(false); // Start inactive
                return new(obj, this);
            },
            actionOnGet: activeTile => { }, // Don't activate here
            actionOnRelease: activeTile => activeTile.Tile.SetActive(false),
            actionOnDestroy: activeTile => Object.Destroy(activeTile.Tile),
            defaultCapacity: 10,
            maxSize: 100
        );
    }
    
    public ActiveTile Get() => m_pool.Get();
    public PooledObject<ActiveTile> Get(out ActiveTile v) => m_pool.Get(out v);
    public void Release(ActiveTile element) => m_pool.Release(element);
    public void Clear() => m_pool.Clear();
    public int CountInactive => m_pool.CountInactive;
}