using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;

    void Update()
    {
        // Movimenta��o da c�mera
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        transform.Translate(new Vector3(horizontal, 0, vertical) * moveSpeed * Time.deltaTime);

        // Rota��o da c�mera
        if (Input.GetMouseButton(1)) // Bot�o direito do mouse
        {
            float rotation = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            transform.Rotate(0, rotation, 0);
        }
    }
}
