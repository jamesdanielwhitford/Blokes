using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    
    [Header("Score Display Settings")]
    [SerializeField] private string finalScoreFormat = "Final Score: {0}";
    [SerializeField] private string highScoreFormat = "High Score: {0}";
    [SerializeField] private string newHighScoreText = "NEW HIGH SCORE: {0}";
    
    [Header("PlayerPrefs Keys")]
    [SerializeField] private string highScoreKey = "HighScore";
    
    private WarioWareGameManager gameManager;
    private int currentHighScore;
    private bool isNewHighScore = false;
    
    private void Start()
    {
        gameManager = WarioWareGameManager.Instance;
        
        if (gameManager == null)
        {
            Debug.LogError("GameOverController: WarioWareGameManager instance not found!");
            return;
        }
        
        LoadHighScore();
        CheckForNewHighScore();
        UpdateScoreDisplays();
        SetupButtons();
    }
    
    private void LoadHighScore()
    {
        currentHighScore = PlayerPrefs.GetInt(highScoreKey, 0);
    }
    
    private void CheckForNewHighScore()
    {
        if (gameManager.currentScore > currentHighScore)
        {
            isNewHighScore = true;
            currentHighScore = gameManager.currentScore;
            SaveHighScore();
            Debug.Log($"GameOverController: New high score achieved: {currentHighScore}");
        }
    }
    
    private void SaveHighScore()
    {
        PlayerPrefs.SetInt(highScoreKey, currentHighScore);
        PlayerPrefs.Save();
    }
    
    private void UpdateScoreDisplays()
    {
        // Update final score display
        if (finalScoreText != null)
        {
            finalScoreText.text = string.Format(finalScoreFormat, gameManager.currentScore);
        }
        else
        {
            Debug.LogWarning("GameOverController: Final Score Text component not assigned!");
        }
        
        // Update high score display
        if (highScoreText != null)
        {
            if (isNewHighScore)
            {
                highScoreText.text = string.Format(newHighScoreText, currentHighScore);
                // Could add color change or animation here
                highScoreText.color = Color.yellow;
            }
            else
            {
                highScoreText.text = string.Format(highScoreFormat, currentHighScore);
            }
        }
        else
        {
            Debug.LogWarning("GameOverController: High Score Text component not assigned!");
        }
    }
    
    private void SetupButtons()
    {
        // Setup restart button
        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(OnRestartClicked);
        }
        else
        {
            Debug.LogWarning("GameOverController: Restart Button not assigned!");
        }
        
        // Setup main menu button
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveAllListeners();
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        }
        else
        {
            Debug.LogWarning("GameOverController: Main Menu Button not assigned!");
        }
    }
    
    public void OnRestartClicked()
    {
        Debug.Log("GameOverController: Restart button clicked");
        
        if (gameManager != null)
        {
            gameManager.RestartGame();
        }
        else
        {
            Debug.LogError("GameOverController: Cannot restart game - GameManager is null!");
        }
    }
    
    public void OnMainMenuClicked()
    {
        Debug.Log("GameOverController: Main Menu button clicked");
        
        if (gameManager != null)
        {
            gameManager.QuitGame();
        }
        else
        {
            Debug.LogError("GameOverController: Cannot return to main menu - GameManager is null!");
        }
    }
    
    // Legacy method for backwards compatibility
    public void RestartGame()
    {
        OnRestartClicked();
    }
}