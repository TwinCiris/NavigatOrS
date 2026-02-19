using UnityEngine;

public class DetectorRecipiente : MonoBehaviour
{
    public TaskManager taskManager;
    public bool esRecipienteRAM;

    private void OnTriggerEnter(Collider other) 
    {
        // Forzamos la detección ignorando mayúsculas/minúsculas en el Tag
        if (other.CompareTag("canPickUp")) 
        {
            Debug.Log("<color=green>¡RAM detectada en el recipiente!</color>");
            taskManager.ProcesarRespuesta(esRecipienteRAM);
            
            ObjectReset reset = other.GetComponent<ObjectReset>();
            if (reset != null) reset.RegresarAMesa();
        }
        else
        {
            Debug.Log("Objeto detectado pero sin tag correcto: " + other.gameObject.name);
        }
    }
}