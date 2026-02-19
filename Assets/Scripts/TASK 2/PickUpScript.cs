using UnityEngine;
using System.Collections; // Necesario para el retraso

public class PickUpScript : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject player;
    public Transform holdPos;
    public PlayerCam2 mouseLookScript;
    public GameObject paredesBloqueo; 

    [Header("Configuracion")]
    public float pickUpRange = 10f; 
    public float throwForce = 500f;
    public float rotationSensitivity = 1f;
    
    private GameObject heldObj;
    private Rigidbody heldObjRb;
    private bool canDrop = true;
    private int LayerNumber;
    private int ramLayerIndex; 
    private Vector3 originalScale;

    void Start()
    {
        int layerCheck = LayerMask.NameToLayer("holdLayer");
        LayerNumber = (layerCheck != -1) ? layerCheck : 0; 
        ramLayerIndex = LayerMask.NameToLayer("Ram model");

        if (player == null) player = transform.root.gameObject;
        if (mouseLookScript == null) mouseLookScript = GetComponent<PlayerCam2>();

        if (ramLayerIndex != -1) Physics.IgnoreLayerCollision(ramLayerIndex, 0, true);

        if (paredesBloqueo != null) paredesBloqueo.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObj == null)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, pickUpRange))
                {
                    if (hit.transform.CompareTag("canPickUp"))
                    {
                        PickUpObject(hit.transform.gameObject);
                    }
                }
            }
            else if (canDrop)
            {
                DropObject();
            }
        }

        if (heldObj != null)
        {
            MoveObject();
            RotateObject();

            if (Input.GetKeyDown(KeyCode.Mouse0) && canDrop)
            {
                ThrowObject();
            }
        }
    }

    void PickUpObject(GameObject pickUpObj)
    {
        Debug.Log("Mirando a: " + pickUpObj.name + " con Tag: " + pickUpObj.tag);
        if (pickUpObj.name.Contains("Tomate") || pickUpObj.CompareTag("Tomate"))
    {
        TomatoThrower thrower = GetComponent<TomatoThrower>();
        if (thrower != null) 
        {
            thrower.Recargar();
            // Aquí puedes añadir un sonido de "recarga" si quieres
            return; // Salimos de la función para no "pegarlo" a la mano
        }
    }
        
        Rigidbody rb = pickUpObj.GetComponent<Rigidbody>();
        if (rb != null) 
        {
            heldObj = pickUpObj; 
            heldObjRb = rb;
            originalScale = heldObj.transform.localScale;

            heldObjRb.isKinematic = true; 
            heldObjRb.useGravity = false;

            heldObj.transform.SetParent(holdPos); 
            heldObj.transform.localPosition = Vector3.zero; 
            heldObj.transform.localEulerAngles = Vector3.zero; 
            heldObj.transform.localScale = new Vector3(1f, 1f, 1f); 
            heldObj.layer = LayerNumber; 

            // Se activan mientras la cargas
            if (paredesBloqueo != null) paredesBloqueo.SetActive(true);

            if(heldObj.GetComponent<Collider>())
                heldObj.GetComponent<Collider>().enabled = false;
        }
    }

    void DropObject()
    {
        // Se desactivan inmediatamente al soltar normal
        if (paredesBloqueo != null) paredesBloqueo.SetActive(false); 
        ReleaseLogic();
    }

    void ThrowObject()
    {
        Rigidbody rbToThrow = heldObjRb;
        ReleaseLogic();
        rbToThrow.AddForce(transform.forward * throwForce);

        // Al lanzar, esperamos un suspiro para que la RAM choque y luego se apagan
        StartCoroutine(DesactivarParedesPostLanzamiento());
    }

    IEnumerator DesactivarParedesPostLanzamiento()
    {
        yield return new WaitForSeconds(1f); // Tiempo mínimo para detectar el choque
        if (paredesBloqueo != null) paredesBloqueo.SetActive(false);
    }

    void ReleaseLogic()
    {
        if(heldObj.GetComponent<Collider>())
            heldObj.GetComponent<Collider>().enabled = true;

        heldObj.transform.localScale = originalScale;
        heldObj.layer = (ramLayerIndex != -1) ? ramLayerIndex : 0; 
        
        heldObjRb.isKinematic = false;
        heldObjRb.useGravity = true;
        heldObj.transform.SetParent(null);
        
        heldObj = null;
        heldObjRb = null;
    }

    void MoveObject()
    {
        heldObj.transform.position = holdPos.position;
        heldObj.transform.rotation = holdPos.rotation;
    }

    void RotateObject()
    {
        if (Input.GetKey(KeyCode.R))
        {
            canDrop = false;
            if(mouseLookScript != null) mouseLookScript.enabled = false;
            Cursor.lockState = CursorLockMode.None;
            float XaxisRotation = Input.GetAxis("Mouse X") * rotationSensitivity;
            float YaxisRotation = Input.GetAxis("Mouse Y") * rotationSensitivity;
            heldObj.transform.Rotate(Vector3.down, XaxisRotation, Space.World);
            heldObj.transform.Rotate(Vector3.right, YaxisRotation, Space.World);
        }
        else
        {
            if(mouseLookScript != null) mouseLookScript.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            canDrop = true;
        }
    }
}