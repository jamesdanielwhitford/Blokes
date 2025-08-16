using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class MicrogameController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI timerText;
    
    [Header("Timer Settings")]
    [SerializeField] private bool useCustomTimeLimit = false;
    [SerializeField] private float customTimeLimit = 5f;
    
    [Header("Display Settings")]
    [SerializeField] private string timerFormat = "{0:F1}";
    [SerializeField] private Color normalTimerColor = Color.white;
    [SerializeField] private Color urgentTimerColor = Color.red;
    [SerializeField] private float urgentThreshold = 2f;
    
    [Header("Unity Events")]
    public UnityEvent OnGameStart;
    public UnityEvent OnGameWon;
    public UnityEvent OnGameLost;
    
    private WarioWareGameManager gameManager;
    private float currentTime;
    private float timeLimit;
    private bool gameActive = false;
    private bool gameEnded = false;
    
    private void Start()
    {
        gameManager = WarioWareGameManager.Instance;
        
        if (gameManager == null)
        {
            Debug.LogError("MicrogameController: WarioWareGameManager instance not found!");
            return;
        }
        
        InitializeTimer();
        StartGame();
    }
    
    private void InitializeTimer()
    {
        // Determine time limit
        if (useCustomTimeLimit)
        {
            timeLimit = customTimeLimit;
        }
        else if (gameManager.currentMicrogame != null)
        {
            timeLimit = gameManager.currentMicrogame.timeLimit;
        }
        else
        {
            timeLimit = 5f; // Default fallback
            Debug.LogWarning("MicrogameController: No microgame data found, using default time limit");
        }
        
        currentTime = timeLimit;
        
        // Initialize timer display
        if (timerText != null)
        {
            UpdateTimerDisplay();
        }
        else
        {
            Debug.LogWarning("MicrogameController: Timer Text UI component not assigned!");
        }
    }
    
    private void StartGame()
    {
        gameActive = true;
        gameEnded = false;
        
        Debug.Log($"MicrogameController: Game started with {timeLimit} second time limit");
        
        // Invoke game start event
        OnGameStart?.Invoke();
        
        // Start countdown coroutine
        StartCoroutine(CountdownTimer());
    }
    
    private IEnumerator CountdownTimer()
    {
        while (gameActive && currentTime > 0 && !gameEnded)
        {
            yield return null;
            
            currentTime -= Time.deltaTime;
            UpdateTimerDisplay();
            
            // Check for timeout
            if (currentTime <= 0)
            {
                currentTime = 0;
                UpdateTimerDisplay();
                TriggerTimeout();
                break;
            }
        }
    }
    
    private void UpdateTimerDisplay()
    {
        if (timerText == null) return;
        
        // Update timer text
        timerText.text = string.Format(timerFormat, currentTime);
        
        // Change color based on urgency
        if (currentTime <= urgentThreshold)
        {
            timerText.color = urgentTimerColor;
        }
        else
        {
            timerText.color = normalTimerColor;
        }
    }
    
    public void TriggerWin()
    {
        if (gameEnded) return;
        
        gameEnded = true;
        gameActive = false;
        
        Debug.Log("MicrogameController: Player won the microgame!");
        
        // Invoke win event
        OnGameWon?.Invoke();
        
        // Notify game manager
        if (gameManager != null)
        {
            gameManager.GameWon();
        }
    }
    
    public void TriggerLose()
    {
        if (gameEnded) return;
        
        gameEnded = true;
        gameActive = false;
        
        Debug.Log("MicrogameController: Player lost the microgame!");
        
        // Invoke lose event
        OnGameLost?.Invoke();
        
        // Notify game manager
        if (gameManager != null)
        {
            gameManager.GameLost();
        }
    }
    
    private void TriggerTimeout()
    {
        if (gameEnded) return;
        
        Debug.Log("MicrogameController: Time's up! Auto-triggering lose condition");
        TriggerLose();
    }
    
    // Public getters for game state
    public float CurrentTime => currentTime;
    public float TimeLimit => timeLimit;
    public bool IsGameActive => gameActive && !gameEnded;
    public bool HasGameEnded => gameEnded;
    
    // Utility methods
    public float GetTimeRemaining()
    {
        return Mathf.Max(0f, currentTime);
    }
    
    public float GetTimeProgress()
    {
        return timeLimit > 0 ? (timeLimit - currentTime) / timeLimit : 1f;
    }
    
    public float GetTimeRemainingNormalized()
    {
        return timeLimit > 0 ? currentTime / timeLimit : 0f;
    }
    
    // Method to pause/resume timer (useful for game pausing)
    public void SetTimerActive(bool active)
    {
        gameActive = active;
    }
    
    // Method to add bonus time (useful for power-ups)
    public void AddBonusTime(float bonusSeconds)
    {
        if (!gameEnded)
        {
            currentTime += bonusSeconds;
            currentTime = Mathf.Min(currentTime, timeLimit * 2f); // Cap at double original time
            Debug.Log($"MicrogameController: Added {bonusSeconds} bonus seconds");
        }
    }
    
    private void OnDestroy()
    {
        // Clean up any running coroutines
        StopAllCoroutines();
    }
}