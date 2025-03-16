using UnityEngine;

public class Move : MonoBehaviour
{
    public float speed = 5.0f;
    public float rotationSpeed = 100.0f;

    void Update()
    {
        float forwardMovement = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float rotationMovement = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;

        float pitchMovement = 0;
        if (Input.GetKey(KeyCode.Q))
        {
            pitchMovement = rotationSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            pitchMovement = -rotationSpeed * Time.deltaTime;
        }

        transform.Translate(0, 0, forwardMovement);

        transform.Rotate(0, rotationMovement, 0);

        transform.Rotate(pitchMovement, 0, 0);
    }
}
