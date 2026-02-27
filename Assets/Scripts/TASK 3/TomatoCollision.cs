using UnityEngine;

public class TomatoCollision : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        // 1. EVITAR AUTODESTRUCCIÓN: Si el tomate choca con el jugador, ignoramos la colisión
        // Asegúrate de que tu Personaje tenga el Tag "Player" en el Inspector
        if (collision.gameObject.CompareTag("Player")) 
        {
            return; 
        }

        // Buscamos al Manager en la escena usando el método moderno
        Task3Manager manager = Object.FindFirstObjectByType<Task3Manager>();
        
        if (manager == null) {
            Debug.LogError("¡No se encontró el Task3Manager en la escena!");
            return;
        }

        // Detectar si el objeto golpeado tiene uno de los tags de la tarea
        string tagGolpeado = collision.gameObject.tag;

        // Comparamos el tag del objeto con la fase actual del manager
        if (tagGolpeado == manager.faseActual.ToString())
        {
            Debug.Log("¡Acierto detectado en: " + tagGolpeado + "!");

            // 1. Cambiar color a verde
            Renderer rend = collision.gameObject.GetComponent<Renderer>();
            if (rend != null) rend.material.color = Color.green;
            
            // 2. Activar gravedad (desactivar kinematic)
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = false;

            // 3. Avisar al manager para sonido y conteo
            manager.RegistrarAcierto(tagGolpeado);
        }
        else 
        {
            // Mensaje útil para saber qué estamos golpeando realmente
            Debug.Log("Golpeaste: " + collision.gameObject.name + " con tag: " + tagGolpeado + ". La fase es: " + manager.faseActual);
        }
        
        // Destruir el tomate después de un pequeño tiempo para que se vea el impacto
        Destroy(gameObject, 10f);
    }
}