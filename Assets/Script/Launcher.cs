using UnityEngine;

public class Launcher : MonoBehaviour
{
    public GameObject spherePrefab;
    public Transform spawnPoint;
    public float launchForce = 500f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LaunchSphere();
        }
    }

    void LaunchSphere()
    {
        if (spherePrefab != null && spawnPoint != null)
        {
            GameObject sphere = Instantiate(spherePrefab, spawnPoint.position, spawnPoint.rotation);
            
            Rigidbody rb = sphere.GetComponent<Rigidbody>();
            
            if (rb != null)
            {
                Vector3 launchDirection = spawnPoint.forward;
                launchDirection.Normalize();

                rb.AddForce(launchDirection * launchForce);
            }
        }
        else
        {
            Debug.LogError("Sphere prefab or spawn point not set.");
        }
    }
}
