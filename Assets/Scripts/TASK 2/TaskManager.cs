using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TaskManager : MonoBehaviour
{
    [System.Serializable]
    public class Pregunta {
        public string texto;
        public bool esRAM; // true si es RAM, false si es ROM
    }

    [Header("UI y Objetos")]
    public TextMeshProUGUI textoPregunta; 
    public TextMeshProUGUI textoMensaje;
    public GameObject liquidoRAM;
    public GameObject liquidoROM;
    public PickUpScript pickUpScript; 

    [Header("Configuracion")]
    public List<Pregunta> preguntas;
    private int preguntaActualIndex = 0;
    private int aciertosRAM = 0; 
    private int aciertosROM = 0; 
    private bool juegoBloqueado = false;

    [Header("Sonidos")]
    public AudioSource fuenteAudio; 
    public AudioClip sonidoCorrecto; 
    public AudioClip sonidoError;
    public AudioClip sonidoVictoria; // Nueva casilla para el sonido final

    void Start() 
    {
        if (textoMensaje != null) textoMensaje.text = "";
        ActualizarPregunta();

        int ramLayer = LayerMask.NameToLayer("Ram model");
        int recipRAMLayer = LayerMask.NameToLayer("RecipienteRAM");
        int recipROMLayer = LayerMask.NameToLayer("RecipienteROM");

        if (ramLayer != -1) 
        {
            if (recipRAMLayer != -1) 
                Physics.IgnoreLayerCollision(ramLayer, recipRAMLayer, false);

            if (recipROMLayer != -1) 
                Physics.IgnoreLayerCollision(ramLayer, recipROMLayer, false);
                
            Debug.Log("Físicas configuradas: RAM detectará recipientes.");
        }
    }

   
void ActualizarPregunta() {
    if (preguntaActualIndex < preguntas.Count) {
        textoPregunta.text = preguntas[preguntaActualIndex].texto;
        textoPregunta.color = Color.white;
    } else {
        textoPregunta.text = "";
        textoMensaje.color = Color.green;
        textoMensaje.text = "¡Tarea Completada!";
        
        if (fuenteAudio != null && sonidoVictoria != null)
            fuenteAudio.PlayOneShot(sonidoVictoria);

        // Desactivamos paredes y el script de agarre al final
        if(pickUpScript != null) 
        {
            if(pickUpScript.paredesBloqueo != null) 
                pickUpScript.paredesBloqueo.SetActive(false);
                
            pickUpScript.enabled = false;
        }
        
        juegoBloqueado = true; 
    }
}

    public void ProcesarRespuesta(bool atinoARAM) {
        if (juegoBloqueado || preguntaActualIndex >= preguntas.Count) return;

        bool esCorrecto = atinoARAM == preguntas[preguntaActualIndex].esRAM;

        if (esCorrecto) {
            if (fuenteAudio != null && sonidoCorrecto != null)
                fuenteAudio.PlayOneShot(sonidoCorrecto);

            StartCoroutine(FeedbackVisual(Color.green, "¡CORRECTO!"));

            // 0.2f asegura que al 5to acierto la escala Y llegue a 1.0 (lleno)
            if (atinoARAM) {
                aciertosRAM++;
                SubirLiquido(liquidoRAM, aciertosRAM * 1f);
            } else {
                aciertosROM++;
                SubirLiquido(liquidoROM, aciertosROM * 1f);
            }
            
            preguntaActualIndex++;
            ActualizarPregunta();
        } else {
            if (fuenteAudio != null && sonidoError != null)
                fuenteAudio.PlayOneShot(sonidoError);
                
            StartCoroutine(CastigoError());
        }
    }

    void SubirLiquido(GameObject liquido, float escalaY) {
        if (liquido != null) {
            liquido.transform.localScale = new Vector3(1, escalaY, 1);
            Debug.Log("Líquido " + liquido.name + " subió a " + escalaY);
        }
    }

    IEnumerator FeedbackVisual(Color color, string mensaje) {
        textoMensaje.color = color;
        textoMensaje.text = mensaje;
        yield return new WaitForSeconds(1f);
        if (preguntaActualIndex < preguntas.Count && !juegoBloqueado) 
            textoMensaje.text = "";
    }

    IEnumerator CastigoError() {
        juegoBloqueado = true;
        textoMensaje.color = Color.red;
        textoMensaje.text = "¡ERROR! Espera 3 segundos...";
        
        if (pickUpScript != null) pickUpScript.enabled = false;

        yield return new WaitForSeconds(3f);

        if (preguntaActualIndex < preguntas.Count) {
            if (pickUpScript != null) pickUpScript.enabled = true;
            textoMensaje.text = "";
            juegoBloqueado = false;
        }
    }
}