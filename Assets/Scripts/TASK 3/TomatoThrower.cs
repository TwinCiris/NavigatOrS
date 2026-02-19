using UnityEngine;
using UnityEngine.UI; 

public class TomatoThrower : MonoBehaviour
{
    [Header("Configuración de Tiro")]
    public GameObject tomatePrefab;
    public Transform spawnPoint;
    public float throwForce = 15f;
    
    [Header("Munición")]
    public int municionActual = 0;
    public int maxMunicion = 5;
    
    [Header("HUD Visual")]
    public GameObject[] iconosTomates; 

    void Update()
    {
        // Solo lanzamos si hay munición y presionamos clic izquierdo
        if (municionActual > 0 && Input.GetKeyDown(KeyCode.Mouse0))
        {
            LanzarTomate();
        }
        ActualizarHUD();
    }

    public void Recargar()
    {
        municionActual = maxMunicion;
        Debug.Log("Munición recargada. Actual: " + municionActual);
    }

    void LanzarTomate()
    {
        municionActual--;
        
        // Creamos el tomate en el spawnPoint
        GameObject nuevoTomate = Instantiate(tomatePrefab, spawnPoint.position, spawnPoint.rotation);
        
        Rigidbody rb = nuevoTomate.GetComponent<Rigidbody>();
        
        if (rb != null)
        {
            // Forzamos que el tomate lanzado NO sea kinematic para que use física real
            rb.isKinematic = false; 
            rb.AddForce(spawnPoint.forward * throwForce, ForceMode.Impulse);
        }

        // Se destruye después de 5 segundos para no llenar la escena de basura
        Destroy(nuevoTomate, 5f);
    }

    void ActualizarHUD()
    {
        if (iconosTomates == null) return;

        for (int i = 0; i < iconosTomates.Length; i++)
        {
            if (iconosTomates[i] != null)
            {
                iconosTomates[i].SetActive(i < municionActual);
            }
        }
    }
}