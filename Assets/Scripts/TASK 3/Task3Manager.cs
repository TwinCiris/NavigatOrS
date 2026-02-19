using UnityEngine;
using TMPro;

public class Task3Manager : MonoBehaviour
{
    public enum FaseTask { Entrada, Salida, Mixto, Finalizada }
    public FaseTask faseActual = FaseTask.Entrada;

    [Header("Interfaz")]
    public TextMeshProUGUI letreroInstruccion;
    public GameObject letreroVictoria;

    [Header("Conteo")]
    public int aciertosActuales = 0;
    private int objetivosPorFase = 10;

    [Header("Audio")]
    public AudioSource audioAcierto;
    public AudioSource audioVictoria;

    [Header("Referencias")]
    public TomatoThrower scriptTomates; // Para bloquear la recarga al final

   void Start()
{
    // Forzamos el estado inicial
    faseActual = FaseTask.Entrada; 
    aciertosActuales = 0;
    
    // Actualizamos el texto para que el jugador sepa qué hacer
    ActualizarLetrero(); 
    
    // Nos aseguramos de que el letrero de victoria esté apagado
    if (letreroVictoria != null)
        letreroVictoria.SetActive(false);
}

    public void RegistrarAcierto(string tipoDispositivo)
    {
        // Validar si el golpe corresponde a la fase actual
        if ((faseActual == FaseTask.Entrada && tipoDispositivo == "Entrada") ||
            (faseActual == FaseTask.Salida && tipoDispositivo == "Salida") ||
            (faseActual == FaseTask.Mixto && tipoDispositivo == "Mixto"))
        {
            aciertosActuales++;
            audioAcierto.Play();

            if (aciertosActuales >= objetivosPorFase)
            {
                SiguienteFase();
            }
        }
    }

    void SiguienteFase()
    {
        aciertosActuales = 0;
        if (faseActual == FaseTask.Entrada) faseActual = FaseTask.Salida;
        else if (faseActual == FaseTask.Salida) faseActual = FaseTask.Mixto;
        else FinalizarTarea();

        ActualizarLetrero();
    }

    void ActualizarLetrero()
    {
        if (faseActual == FaseTask.Entrada) letreroInstruccion.text = "Busca: Dispositivos de Entrada";
        else if (faseActual == FaseTask.Salida) letreroInstruccion.text = "Busca: Dispositivos de Salida";
        else if (faseActual == FaseTask.Mixto) letreroInstruccion.text = "Busca: Dispositivos de E/S";
    }

    void FinalizarTarea()
    {
        faseActual = FaseTask.Finalizada;
        letreroInstruccion.text = "¡TAREA COMPLETADA!";
        letreroVictoria.SetActive(true);
        audioVictoria.Play();
        
        // Bloquear tomates
        scriptTomates.municionActual = 0;
        scriptTomates.enabled = false; 
    }
}