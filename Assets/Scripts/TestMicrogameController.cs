using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TestMicrogameController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private TextMeshProUGUI debugText;
    
    [Header("Test Settings")]
    [SerializeField] private string testInstruction = "This is a test game - you will lose automatically!";
    [SerializeField] private bool enableDebugInfo = true;
    
    private MicrogameController microgameController;
    private float startTime;
    
    private void Start()
    {
        // Get the microgame controller component
        microgameController = FindObjectOfType<MicrogameController>();
        
        if (microgameController == null)
        {
            Debug.LogError("TestMicrogameController: No MicrogameController found in scene!");
            return;
        }
        
        startTime = Time.time;
        
        // Setup UI
        InitializeUI();
        
        Debug.Log("TestMicrogameController: Test microgame started - will auto-lose when timer expires");
    }
    
    private void InitializeUI()
    {
        if (instructionText != null)
        {
            instructionText.text = testInstruction;
        }
        
        if (debugText != null && enableDebugInfo)
        {
            debugText.gameObject.SetActive(true);
        }
        else if (debugText != null)
        {
            debugText.gameObject.SetActive(false);
        }
    }
    
    private void Update()
    {
        if (microgameController == null) return;
        
        // Update debug information
        if (debugText != null && enableDebugInfo)
        {
            UpdateDebugInfo();
        }
        
        // For testing purposes, you could uncomment the line below to trigger a win
        // (but we want auto-lose for now)
        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            microgameController.TriggerWin();
        }
        */
    }
    
    private void UpdateDebugInfo()
    {
        string debugInfo = $"Time Remaining: {microgameController.GetTimeRemaining():F1}s\n";
        debugInfo += $"Game Active: {microgameController.IsGameActive}\n";
        debugInfo += $"Game Ended: {microgameController.HasGameEnded}\n";
        debugInfo += $"Elapsed: {Time.time - startTime:F1}s";
        
        debugText.text = debugInfo;
    }
    
    // Optional: Method to trigger win for testing purposes
    public void TriggerTestWin()
    {
        if (microgameController != null && microgameController.IsGameActive)
        {
            microgameController.TriggerWin();
        }
    }
    
    // Optional: Method to trigger lose for testing purposes
    public void TriggerTestLose()
    {
        if (microgameController != null && microgameController.IsGameActive)
        {
            microgameController.TriggerLose();
        }
    }
}