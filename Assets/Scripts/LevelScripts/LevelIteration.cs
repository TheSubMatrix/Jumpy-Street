using UnityEngine;

public class LevelIteration : MonoBehaviour
{
    public GameObject baseTile;
    public GameObject tileTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("TileTrig"))
        {
            Vector3 tile = other.transform.position;
            Instantiate(baseTile, new Vector3(tile.x, tile.y, tile.z + 50), Quaternion.identity);
            tileTrigger.SetActive(false);
            tileTrigger.transform.position = new Vector3(tile.x, tile.y, tile.z + 50);
            tileTrigger.SetActive(true);
        }

       /* if (other.gameObject.CompareTag("TileTrig"))
        {
            tileTrigger.SetActive(true);
        }*/
    }

}
