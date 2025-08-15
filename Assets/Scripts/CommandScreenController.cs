using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CommandScreenController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Text commandText;
    [SerializeField] private Text countdownText;
    
    [Header("Animation Settings")]
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float commandDisplayDuration = 2f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private float countdownInterval = 1f;
    
    private WarioWareGameManager gameManager;
    
    private void Start()
    {
        gameManager = WarioWareGameManager.Instance;
        
        if (gameManager == null)
        {
            Debug.LogError("CommandScreenController: WarioWareGameManager instance not found!");
            return;
        }
        
        if (commandText == null)
        {
            Debug.LogError("CommandScreenController: Command Text UI component not assigned!");
            return;
        }
        
        if (countdownText == null)
        {
            Debug.LogError("CommandScreenController: Countdown Text UI component not assigned!");
            return;
        }
        
        StartCoroutine(ShowCommandSequence());
    }
    
    private IEnumerator ShowCommandSequence()
    {
        // Initialize UI elements
        SetTextAlpha(commandText, 0f);
        SetTextAlpha(countdownText, 0f);
        countdownText.gameObject.SetActive(false);
        
        // Get current microgame data
        MicrogameData currentMicrogame = gameManager.currentMicrogame;
        
        if (currentMicrogame == null)
        {
            Debug.LogError("CommandScreenController: No current microgame data available!");
            yield break;
        }
        
        // Set command text
        commandText.text = currentMicrogame.commandText;
        
        // Fade in command text
        yield return StartCoroutine(FadeText(commandText, 0f, 1f, fadeInDuration));
        
        // Display command for specified duration
        yield return new WaitForSeconds(commandDisplayDuration);
        
        // Fade out command text
        yield return StartCoroutine(FadeText(commandText, 1f, 0f, fadeOutDuration));
        
        // Hide command text and show countdown
        commandText.gameObject.SetActive(false);
        countdownText.gameObject.SetActive(true);
        
        // Show countdown (3, 2, 1)
        yield return StartCoroutine(ShowCountdown());
        
        // Load the microgame scene
        gameManager.LoadNextMicrogame();
    }
    
    private IEnumerator ShowCountdown()
    {
        string[] countdownNumbers = { "3", "2", "1" };
        
        foreach (string number in countdownNumbers)
        {
            // Set countdown text
            countdownText.text = number;
            
            // Fade in countdown number
            yield return StartCoroutine(FadeText(countdownText, 0f, 1f, fadeInDuration * 0.5f));
            
            // Hold the number
            yield return new WaitForSeconds(countdownInterval - (fadeInDuration * 0.5f) - (fadeOutDuration * 0.5f));
            
            // Fade out countdown number
            yield return StartCoroutine(FadeText(countdownText, 1f, 0f, fadeOutDuration * 0.5f));
        }
    }
    
    private IEnumerator FadeText(Text textComponent, float startAlpha, float endAlpha, float duration)
    {
        if (textComponent == null || duration <= 0f)
            yield break;
            
        float elapsedTime = 0f;
        Color startColor = textComponent.color;
        Color endColor = startColor;
        
        startColor.a = startAlpha;
        endColor.a = endAlpha;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / duration;
            
            // Use smooth step for better animation curve
            normalizedTime = Mathf.SmoothStep(0f, 1f, normalizedTime);
            
            textComponent.color = Color.Lerp(startColor, endColor, normalizedTime);
            
            yield return null;
        }
        
        // Ensure final alpha is set exactly
        textComponent.color = endColor;
    }
    
    private void SetTextAlpha(Text textComponent, float alpha)
    {
        if (textComponent != null)
        {
            Color color = textComponent.color;
            color.a = alpha;
            textComponent.color = color;
        }
    }
}