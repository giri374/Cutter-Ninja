using UnityEngine;

public class Bomb : MonoBehaviour
{
    public GameObject explosionEffect;
    public GameObject bombbody;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bombbody.SetActive(false);
            explosionEffect.SetActive(true);
            GetComponent<Collider>().enabled = false;
            GameManager.Instance.Explode();
        }
    }
}
