using UnityEngine;

public class ObjectReset : MonoBehaviour
{
    private Vector3 positionOriginal;
    private Quaternion rotationOriginal;
    private Rigidbody rb;

    void Start()
    {
        positionOriginal = transform.position;
        rotationOriginal = transform.rotation;
        rb = GetComponent<Rigidbody>();
        Debug.Log("Punto de inicio guardado en: " + positionOriginal);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Esto te dirá en la consola con qué chocó exactamente
        Debug.Log("Choqué con: " + collision.gameObject.name + " Tag: " + collision.gameObject.tag);

        if (collision.gameObject.CompareTag("Floor"))
        {
            RegresarAMesa();
        }
    }

    public void RegresarAMesa()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = positionOriginal;
        transform.rotation = rotationOriginal;
        Debug.Log("¡Regresando a la mesa!");
    }
}