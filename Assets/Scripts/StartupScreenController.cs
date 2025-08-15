using UnityEngine;
using UnityEngine.UI;

public class StartupScreenController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Text titleText;
    [SerializeField] private Text highScoreText;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button quitButton;
    
    [Header("UI Settings")]
    [SerializeField] private string titleString = "MICROGAMES";
    [SerializeField] private string highScoreFormat = "High Score: {0}";
    [SerializeField] private string noHighScoreText = "High Score: 0";
    
    [Header("PlayerPrefs Keys")]
    [SerializeField] private string highScoreKey = "HighScore";
    
    private WarioWareGameManager gameManager;
    private int currentHighScore;
    
    private void Start()
    {
        gameManager = WarioWareGameManager.Instance;
        
        if (gameManager == null)
        {
            Debug.LogError("StartupScreenController: WarioWareGameManager instance not found!");
        }
        
        InitializeUI();
        LoadHighScore();
        UpdateHighScoreDisplay();
        SetupButtons();
    }
    
    private void InitializeUI()
    {
        // Set title text
        if (titleText != null)
        {
            titleText.text = titleString;
        }
        else
        {
            Debug.LogWarning("StartupScreenController: Title Text component not assigned!");
        }
        
        // Setup quit button visibility based on platform
        if (quitButton != null)
        {
            // Show quit button only for standalone builds
            bool showQuitButton = Application.platform == RuntimePlatform.WindowsPlayer ||
                                 Application.platform == RuntimePlatform.OSXPlayer ||
                                 Application.platform == RuntimePlatform.LinuxPlayer ||
                                 Application.isEditor; // Show in editor for testing
            
            quitButton.gameObject.SetActive(showQuitButton);
        }
    }
    
    private void SetupButtons()
    {
        // Setup start game button
        if (startGameButton != null)
        {
            startGameButton.onClick.RemoveAllListeners();
            startGameButton.onClick.AddListener(OnStartGameClicked);
        }
        else
        {
            Debug.LogError("StartupScreenController: Start Game Button not assigned!");
        }
        
        // Setup quit button
        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(OnQuitClicked);
        }
    }
    
    private void LoadHighScore()
    {
        currentHighScore = PlayerPrefs.GetInt(highScoreKey, 0);
        Debug.Log($"StartupScreenController: Loaded high score: {currentHighScore}");
    }
    
    private void UpdateHighScoreDisplay()
    {
        if (highScoreText != null)
        {
            if (currentHighScore > 0)
            {
                highScoreText.text = string.Format(highScoreFormat, currentHighScore);
            }
            else
            {
                highScoreText.text = noHighScoreText;
            }
        }
        else
        {
            Debug.LogWarning("StartupScreenController: High Score Text component not assigned!");
        }
    }
    
    public void OnStartGameClicked()
    {
        Debug.Log("StartupScreenController: Start Game button clicked");
        
        if (gameManager != null)
        {
            gameManager.StartGame();
        }
        else
        {
            Debug.LogError("StartupScreenController: Cannot start game - GameManager is null!");
        }
    }
    
    public void OnQuitClicked()
    {
        Debug.Log("StartupScreenController: Quit button clicked");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    public void CheckAndUpdateHighScore(int newScore)
    {
        if (newScore > currentHighScore)
        {
            currentHighScore = newScore;
            SaveHighScore();
            UpdateHighScoreDisplay();
            Debug.Log($"StartupScreenController: New high score achieved: {currentHighScore}");
        }
    }
    
    private void SaveHighScore()
    {
        PlayerPrefs.SetInt(highScoreKey, currentHighScore);
        PlayerPrefs.Save();
        Debug.Log($"StartupScreenController: High score saved: {currentHighScore}");
    }
    
    // Public method to manually refresh high score (useful if changed externally)
    public void RefreshHighScore()
    {
        LoadHighScore();
        UpdateHighScoreDisplay();
    }
    
    // Method to reset high score (useful for testing or settings menu)
    public void ResetHighScore()
    {
        currentHighScore = 0;
        SaveHighScore();
        UpdateHighScoreDisplay();
        Debug.Log("StartupScreenController: High score reset to 0");
    }
    
    // Getters for external access
    public int GetHighScore()
    {
        return currentHighScore;
    }
    
    public string GetHighScoreKey()
    {
        return highScoreKey;
    }
    
    private void OnEnable()
    {
        // Refresh high score when screen becomes active
        // (useful if returning from a game session)
        RefreshHighScore();
    }
    
    private void OnApplicationPause(bool pauseStatus)
    {
        // Save high score when application is paused (mobile)
        if (pauseStatus)
        {
            PlayerPrefs.Save();
        }
    }
    
    private void OnApplicationFocus(bool hasFocus)
    {
        // Save high score when application loses focus
        if (!hasFocus)
        {
            PlayerPrefs.Save();
        }
    }
}