using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class WarioWareGameManager : MonoBehaviour
{
    public static WarioWareGameManager Instance { get; private set; }
    
    [Header("Game Settings")]
    [SerializeField] private MicrogameList microgameList;
    [SerializeField] private string startupSceneName = "StartupScene";
    [SerializeField] private string commandSceneName = "CommandScene";
    [SerializeField] private string gameOverSceneName = "GameOverScene";
    
    [Header("Game State")]
    public int lives = 3;
    public int currentScore = 0;
    public MicrogameData currentMicrogame { get; private set; }
    
    private List<MicrogameData> shuffledMicrogames = new List<MicrogameData>();
    private int currentMicrogameIndex = 0;
    private bool isGameActive = false;
    
    public enum GameState
    {
        Startup,
        Command,
        Microgame,
        GameOver
    }
    
    public GameState currentState { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGameManager();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeGameManager()
    {
        if (microgameList == null || microgameList.microgames.Count == 0)
        {
            Debug.LogError("WarioWareGameManager: No microgame list assigned or list is empty!");
            return;
        }
        
        ShuffleMicrogames();
        currentState = GameState.Startup;
    }
    
    private void ShuffleMicrogames()
    {
        shuffledMicrogames.Clear();
        shuffledMicrogames.AddRange(microgameList.microgames);
        
        for (int i = 0; i < shuffledMicrogames.Count; i++)
        {
            MicrogameData temp = shuffledMicrogames[i];
            int randomIndex = Random.Range(i, shuffledMicrogames.Count);
            shuffledMicrogames[i] = shuffledMicrogames[randomIndex];
            shuffledMicrogames[randomIndex] = temp;
        }
        
        currentMicrogameIndex = 0;
        Debug.Log($"Shuffled {shuffledMicrogames.Count} microgames");
    }
    
    public void StartGame()
    {
        if (!isGameActive)
        {
            isGameActive = true;
            lives = 3;
            currentScore = 0;
            ShuffleMicrogames();
            LoadCommandScreen();
        }
    }
    
    private void LoadCommandScreen()
    {
        currentState = GameState.Command;
        SetCurrentMicrogame();
        StartCoroutine(LoadSceneAsync(commandSceneName));
    }
    
    private void SetCurrentMicrogame()
    {
        if (shuffledMicrogames.Count > 0)
        {
            currentMicrogame = shuffledMicrogames[currentMicrogameIndex % shuffledMicrogames.Count];
        }
    }
    
    private void LoadMicrogame()
    {
        if (currentMicrogame != null)
        {
            currentState = GameState.Microgame;
            StartCoroutine(LoadSceneAsync(currentMicrogame.sceneName));
        }
        else
        {
            Debug.LogError("No current microgame set!");
            GameOver();
        }
    }
    
    public void GameWon()
    {
        if (!isGameActive) return;
        
        currentScore++;
        currentMicrogameIndex++;
        
        if (currentMicrogameIndex >= shuffledMicrogames.Count)
        {
            ShuffleMicrogames();
        }
        
        Debug.Log($"Game Won! Score: {currentScore}, Lives: {lives}");
        LoadCommandScreen();
    }
    
    public void GameLost()
    {
        if (!isGameActive) return;
        
        lives--;
        Debug.Log($"Game Lost! Lives remaining: {lives}");
        
        if (lives <= 0)
        {
            GameOver();
        }
        else
        {
            currentMicrogameIndex++;
            
            if (currentMicrogameIndex >= shuffledMicrogames.Count)
            {
                ShuffleMicrogames();
            }
            
            LoadCommandScreen();
        }
    }
    
    public void GameOver()
    {
        isGameActive = false;
        currentState = GameState.GameOver;
        Debug.Log($"Game Over! Final Score: {currentScore}");
        
        if (!string.IsNullOrEmpty(gameOverSceneName))
        {
            StartCoroutine(LoadSceneAsync(gameOverSceneName));
        }
    }
    
    public void LoadNextMicrogame()
    {
        LoadMicrogame();
    }
    
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;
        
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }
        
        yield return new WaitForEndOfFrame();
        
        asyncLoad.allowSceneActivation = true;
        
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        Debug.Log($"Loaded scene: {sceneName}");
    }
    
    public void RestartGame()
    {
        isGameActive = false;
        StartGame();
    }
    
    public void QuitGame()
    {
        isGameActive = false;
        currentState = GameState.Startup;
        
        if (!string.IsNullOrEmpty(startupSceneName))
        {
            StartCoroutine(LoadSceneAsync(startupSceneName));
        }
    }
    
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}