using System.Linq.Expressions;
using UnityEngine;

public class BoulderSpawner : MonoBehaviour
{
    public GameObject boulder;
    float spawnTime;
    private bool boulderSpawned = false;
    void Start()
    {
        spawnTime = 0;
        boulder.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (boulderSpawned == false)
        {
            SpawnBoulder();
        }
    }

    void SpawnBoulder()
    {
        /*float randomNum = Random.Range(3f, 7f);
        spawnTime = spawnTime + Time.deltaTime;
        Debug.Log(spawnTime);
        if (randomNum >= spawnTime)
        {
            Instantiate(boulder);
            boulderSpawned = true;
            randomNum = 0f;
            spawnTime = 0f;
        }

        boulderSpawned = false;*/
        
    }
}
