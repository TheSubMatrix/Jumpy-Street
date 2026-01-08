using UnityEngine;

public class LoadNextCollider : MonoBehaviour
{

    public GameObject exitTrigger;
    public GameObject tileTrigger;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("TileTrig"))
        {
            tileTrigger.SetActive(true);
        }
    }
}
