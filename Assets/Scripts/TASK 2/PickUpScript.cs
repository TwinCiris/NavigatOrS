using UnityEngine;

public class PickUpScript : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject player;
    public Transform holdPos;
    public PlayerCam2 mouseLookScript; 

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
        // Buscamos holdLayer para la mano
        int layerCheck = LayerMask.NameToLayer("holdLayer");
        LayerNumber = (layerCheck != -1) ? layerCheck : 0; 
        
        // Buscamos Ram model para cuando el objeto vuela
        ramLayerIndex = LayerMask.NameToLayer("Ram model");

        if (player == null) player = transform.root.gameObject;
        if (mouseLookScript == null) mouseLookScript = GetComponent<PlayerCam2>();

        // Forzamos que la capa Ram model ignore a la capa Default (0)
        if (ramLayerIndex != -1)
        {
            Physics.IgnoreLayerCollision(ramLayerIndex, 0, true);
        }
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
        Rigidbody rb = pickUpObj.GetComponent<Rigidbody>();
        if (rb != null) 
        {
            heldObj = pickUpObj; 
            heldObjRb = rb;
            
            originalScale = heldObj.transform.localScale;

            heldObjRb.isKinematic = true; 
            heldObjRb.useGravity = false;
            heldObjRb.detectCollisions = false;

            heldObj.transform.SetParent(holdPos); 
            heldObj.transform.localPosition = Vector3.zero; 
            heldObj.transform.localEulerAngles = Vector3.zero; 

            heldObj.transform.localScale = new Vector3(1f, 1f, 1f); 

            heldObj.layer = LayerNumber; 

            if(heldObj.GetComponent<Collider>())
                heldObj.GetComponent<Collider>().enabled = false;
        }
    }

    void DropObject()
    {
        if (heldObj == null) return;

        if(heldObj.GetComponent<Collider>())
            heldObj.GetComponent<Collider>().enabled = true;

        heldObj.transform.localScale = originalScale;

        // Regresamos a la capa de la RAM (que ignora muros)
        heldObj.layer = (ramLayerIndex != -1) ? ramLayerIndex : 0; 

        heldObjRb.isKinematic = false;
        heldObjRb.useGravity = true;
        heldObjRb.detectCollisions = true; 
        
        heldObj.transform.SetParent(null);
        heldObj = null;
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

    void ThrowObject()
    {
        if (heldObj == null) return;

        if(heldObj.GetComponent<Collider>())
            heldObj.GetComponent<Collider>().enabled = true;

        heldObj.transform.localScale = originalScale;

        // Regresamos a la capa de la RAM (que ignora muros)
        heldObj.layer = (ramLayerIndex != -1) ? ramLayerIndex : 0; 

        heldObjRb.isKinematic = false;
        heldObjRb.useGravity = true;
        heldObjRb.detectCollisions = true;
        
        heldObj.transform.SetParent(null);
        heldObjRb.AddForce(transform.forward * throwForce);
        heldObj = null;
    }
}