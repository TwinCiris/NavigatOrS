using UnityEngine;
using UnityEngine.UI;

public class PlayerCam2 : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public Transform orientation;

    float xRotation;
    float yRotation;

    [Header("Interacción")]
    public float range = 5f; // Distancia máxima para tocar botones

    private void Start()
    {
        // Bloqueo de cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // 1. Lógica de Rotación (Ya la tenías perfecta)
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);

        // 2. Lógica del Raycast para la Crosshair (El "Clic" en la mira)
        if (Input.GetMouseButtonDown(0)) // Clic izquierdo
        {
            // Lanzamos rayo desde el centro de la cámara hacia adelante
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            // Dibujamos el rayo en el editor para que puedas verlo (opcional)
            Debug.DrawRay(transform.position, transform.forward * range, Color.red, 1f);

            if (Physics.Raycast(ray, out hit, range))
            {
                // Buscamos si el objeto que golpeamos tiene un botón
                Button btn = hit.collider.GetComponentInParent<Button>();
                if (btn != null && btn.interactable)
                {
                    btn.onClick.Invoke(); // Ejecutamos la función del botón
                }
            }
        }
    }
}