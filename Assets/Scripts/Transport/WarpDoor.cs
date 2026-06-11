using UnityEngine;

public class WarpDoor : MonoBehaviour
{
    [SerializeField] private GameObject markLocation;
    [SerializeField] private float distanceFromMark = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && markLocation != null)
        {
            other.transform.position = markLocation.transform.position + (Vector3.right * distanceFromMark);
        }
    }
}
