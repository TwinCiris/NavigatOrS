using UnityEngine;
using System.Collections;

public class ObjectRespawn : MonoBehaviour
{
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Rigidbody rb;

    void Start()
    {
        // Guardamos la posición exacta donde pusiste la RAM al inicio (sobre la barra)
        startPosition = transform.position;
        startRotation = transform.rotation;
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Si toca el suelo o el plano de la zona de tiro
        if (collision.gameObject.CompareTag("Floor"))
        {
            StartCoroutine(RespawnSequence());
        }
    }

    IEnumerator RespawnSequence()
    {
        // Esperamos 1 segundo después de que toque el suelo
        yield return new WaitForSeconds(1.0f);

        // Solo reseteamos si el objeto NO está siendo cargado por el jugador actualmente
        if (transform.parent == null)
        {
            // Detenemos cualquier movimiento que tenga
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Lo teletransportamos de vuelta a la barra de madera
            transform.position = startPosition;
            transform.rotation = startRotation;
        }
    }
}