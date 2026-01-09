using UnityEngine;

public class LevelIteration : MonoBehaviour
{
    public GameObject baseTile;
    public GameObject tileTrigger;
    public GameObject targetTrigger;
    private int tileNumber = 0;
    //public GameObject targetTrigger;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("TileTrig"))
        {
            Vector3 tile = other.transform.position;
            Instantiate(baseTile, new Vector3(tile.x, tile.y, tile.z + 50), Quaternion.identity);
            if (tileNumber >= 1) 
            {  
                
            }
            tileNumber++;
        }

       /* if (other.gameObject.CompareTag("TileTrig"))
        {
            tileTrigger.SetActive(true);
        }*/
    }

}
