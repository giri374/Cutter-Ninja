using UnityEngine;

public class Fruit : MonoBehaviour
{
    public GameObject whole;
    public GameObject sliced;
    public GameObject juiceEffect;

    private Rigidbody fruitRigidbody;
    private Collider fruitCollider;

    public int points = 1;

    private void Awake()
    {
        fruitRigidbody = GetComponent<Rigidbody>();
        fruitCollider = GetComponent<Collider>();
    }

    private void Slice(Vector3 direction, Vector3 position, float force)
    {
        GameManager.Instance.IncreaseScore(points);

        // Disable the whole fruit
        fruitCollider.enabled = false;
        whole.SetActive(false);

        // Only process sliced fruit if it's assigned
        if (sliced != null)
        {
            sliced.SetActive(true);

            // Rotate based on the slice angle
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            sliced.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            Rigidbody[] slices = sliced.GetComponentsInChildren<Rigidbody>();

            // Add a force to each slice based on the blade direction
            foreach (Rigidbody slice in slices)
            {
                slice.linearVelocity = fruitRigidbody.linearVelocity;
                slice.AddForceAtPosition(direction * force, position, ForceMode.Impulse);
            }
        }
        else
        {
            Debug.LogWarning($"[Fruit] Sliced version not assigned on {gameObject.name}. Skipping slice visuals.", this);
        }

        if (juiceEffect != null)
        {
            juiceEffect.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Blade blade = other.GetComponent<Blade>();
            Slice(blade.direction, blade.transform.position, blade.sliceForce);
        }
    }

}
