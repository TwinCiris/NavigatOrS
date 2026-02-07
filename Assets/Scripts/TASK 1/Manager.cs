using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class Manager : MonoBehaviour
{
    public GameObject[] Levels; 
    public TextMeshProUGUI textoPantalla; 
    
    [Header("Sonidos")]
    public AudioSource fuenteAudio; 
    public AudioClip sonidoCorrecto;
    public AudioClip sonidoIncorrecto;
    public AudioClip sonidoVictoriaFinal;

    [Header("Configuración")]
    public float tiempoCastigo = 3f;
    public float tiempoVictoria = 2f;

    int currentLevel;
    int correctAnswersCount = 0;
    const int requiredAnswers = 3;
    bool isBlocked = false;
    string textoGuardado;

    public void correctAnswer(Button btnPresionado)
    {
        if (isBlocked) return;

        if(fuenteAudio && sonidoCorrecto) fuenteAudio.PlayOneShot(sonidoCorrecto);

        btnPresionado.image.color = Color.green;
        btnPresionado.interactable = false;
        correctAnswersCount++;

        if (correctAnswersCount >= requiredAnswers)
        {
            StartCoroutine(WaitAndNextLevel());
        }
    }

    public void wrongAnswer(Button btnPresionado)
    {
        if (isBlocked) return;
        
        // --- CAMBIO A COLOR NEGRO ---
        btnPresionado.image.color = Color.black;

        if(fuenteAudio && sonidoIncorrecto) fuenteAudio.PlayOneShot(sonidoIncorrecto);
        
        StartCoroutine(PenaltyBlock());
    }

    IEnumerator PenaltyBlock()
    {
        isBlocked = true;
        
        textoGuardado = textoPantalla.text;
        textoPantalla.text = "<color=red>ERROR: SISTEMA BLOQUEADO</color>";

        // Ocultar botones (hijo 0 de la pregunta actual)
        GameObject contenedorBotones = Levels[currentLevel].transform.GetChild(0).gameObject; 
        contenedorBotones.SetActive(false);

        yield return new WaitForSeconds(tiempoCastigo);

        contenedorBotones.SetActive(true);
        textoPantalla.text = textoGuardado;
        isBlocked = false;
    }

    IEnumerator WaitAndNextLevel()
    {
        isBlocked = true;
        yield return new WaitForSeconds(tiempoVictoria);

        if (currentLevel + 1 < Levels.Length)
        {
            Levels[currentLevel].SetActive(false);
            currentLevel++;
            Levels[currentLevel].SetActive(true);
            correctAnswersCount = 0;

            if (currentLevel == Levels.Length - 1)
            {
                if(fuenteAudio && sonidoVictoriaFinal) fuenteAudio.PlayOneShot(sonidoVictoriaFinal);
            }
        }
        isBlocked = false;
    }
}