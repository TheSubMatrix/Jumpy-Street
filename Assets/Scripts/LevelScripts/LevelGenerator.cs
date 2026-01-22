using UnityEngine;
using System;
using UnityEngine.Pool;
using Object = UnityEngine.Object;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] Vector3 m_tileSize;
    [SerializeField] List<EnvironmentTilePool> m_tilePools = new();
    LinkedList<ActiveTile> m_activeTiles;
    Plane[] cameraPlanes;

    private void Start()
    {
        foreach(EnvironmentTilePool tilePool in m_tilePools)
        {
            tilePool.Initialize();
        }
        cameraPlanes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

    }
    GameObject GenerateTile(Vector3 position)
    {
        GameObject newTile = m_tilePools[UnityEngine.Random.Range(0, m_tilePools.Count)].Get().Tile;
        newTile.transform.position = position;
        return newTile;
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
                return new(obj, this);
            },
            actionOnGet: activeTile => activeTile.Tile.SetActive(true),
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