# Microgame Setup Guide

This guide explains how to add new microgames to the WarioWare-style game system.

## Architecture Overview

The game follows a modular architecture where each microgame is:
1. **Self-contained** - Has its own scene with all required assets
2. **Timer-driven** - Players must complete objectives within a time limit
3. **Win/Lose based** - Games end in either victory or defeat
4. **Automatically integrated** - Added to the game loop through ScriptableObject configuration

## Game Flow

```
Start Screen → Command Screen → Microgame → Result → Next Game (loop)
                      ↓                           ↓
                Lives: 3 ----------------------- Lives-1
                                                 ↓
                                          (if lives = 0) Game Over Screen
```

## Adding a New Microgame - Step by Step

### Important: TextMeshPro Usage
This project uses **TextMeshPro** instead of Unity's legacy Text component. Make sure to:
- Use `TextMeshPro - Text (UI)` components for all UI text
- Import `using TMPro;` in your scripts
- Use `TextMeshProUGUI` type instead of `Text` in script references

### 1. Create the Scene

1. **Create new scene**: `Assets/Scenes/Microgames/YourGameName.unity`
2. **Scene hierarchy requirements**:
   ```
   YourGameName Scene
   ├── GameManager (with MicrogameController script)
   ├── UI Canvas
   │   ├── Timer Text (TextMeshPro - Text UI)
   │   ├── Instruction Text (TextMeshPro - Text UI, optional)
   │   └── Debug Text (TextMeshPro - Text UI, optional for testing)
   ├── Game Objects (your game-specific objects)
   └── Main Camera
   ```

### 2. Setup MicrogameController

**Required Components on GameManager GameObject**:
- `MicrogameController` script (handles timer and win/lose logic)

**MicrogameController Configuration**:
```csharp
[Header("UI References")]
public TextMeshProUGUI timerText;           // Shows countdown timer

[Header("Timer Settings")]
public bool useCustomTimeLimit = false;  // Use scene-specific timer
public float customTimeLimit = 5f;       // Override ScriptableObject timer

[Header("Unity Events")]
public UnityEvent OnGameStart;   // Called when game begins
public UnityEvent OnGameWon;     // Called when player wins
public UnityEvent OnGameLost;    // Called when player loses
```

### 3. Create Game Logic Script

Create a script that inherits from `MonoBehaviour` for your specific game logic:

```csharp
using UnityEngine;
using TMPro;

public class YourGameController : MonoBehaviour
{
    private MicrogameController microgameController;
    
    private void Start()
    {
        microgameController = FindObjectOfType<MicrogameController>();
        InitializeGame();
    }
    
    private void InitializeGame()
    {
        // Setup your game objects, positions, etc.
    }
    
    private void Update()
    {
        // Game logic here
        if (CheckWinCondition())
        {
            microgameController.TriggerWin();
        }
    }
    
    private bool CheckWinCondition()
    {
        // Your win condition logic
        return false;
    }
    
    // Optional: Handle lose conditions manually
    private void TriggerLose()
    {
        microgameController.TriggerLose();
    }
}
```

### 4. Create MicrogameData ScriptableObject

1. **Right-click** in `Assets/ScriptableObjects/MicrogameData/`
2. **Create → WarioWare → Microgame Data**
3. **Configure the data**:
   ```
   Scene Name: YourGameName (must match scene file name)
   Command Text: "Your instruction!" (shown on command screen)
   Time Limit: 3-8 seconds (recommended range)
   Is Unlocked: true
   ```

### 5. Add to MicrogameList

1. **Open** `Assets/MicrogameList.asset`
2. **Add your MicrogameData** to the list
3. **Save** the asset

### 6. Update Build Settings

**Important**: Add your scene to build settings:
1. **File → Build Settings**
2. **Add Open Scenes** (with your scene open)
3. **Ensure it's enabled** in the list

## UI Setup Requirements

### Timer Display
- Create a **TextMeshPro - Text (UI)** component for countdown display
- **Assign to MicrogameController.timerText**
- **Recommended settings**:
  - Font Size: 48-72
  - Anchor: Top-center
  - Color: White (changes to red when urgent)

### Optional UI Elements
- **Instruction Text**: Shows game-specific instructions (TextMeshPro - Text UI)
- **Debug Text**: Shows timer/state info during development (TextMeshPro - Text UI)

## Win/Lose Conditions

### Triggering Win
```csharp
microgameController.TriggerWin();
```

### Triggering Lose
```csharp
microgameController.TriggerLose();
```

### Auto-Lose (Default)
- Games automatically lose when timer reaches 0
- No manual lose trigger needed for timeout

## Best Practices

### Timing
- **Keep games short**: 3-8 seconds max
- **Make objectives clear**: Players should understand immediately
- **Test extensively**: Ensure consistent difficulty

### Visual Design
- **High contrast**: Easy to see objectives
- **Minimal UI**: Focus on gameplay
- **Responsive feedback**: Clear visual/audio cues

### Code Structure
- **Single responsibility**: One clear objective per game
- **Clean initialization**: Setup in Start(), not Awake()
- **Null checks**: Always verify MicrogameController exists

## Testing Your Microgame

### 1. Scene Testing
- **Play scene directly** to test game logic
- **Check timer functionality**
- **Verify win/lose conditions**

### 2. Integration Testing
- **Play from StartupScene**
- **Verify command screen shows correct text**
- **Test multiple game loops**
- **Confirm game over after 3 losses**

### 3. Debug Tools
Use `TestMicrogameController` for debugging:
- Shows timer remaining
- Displays game state
- Allows manual win/lose triggering

## Common Issues & Solutions

### Scene Not Loading
- **Check scene name** matches ScriptableObject exactly
- **Verify scene** is in build settings
- **Ensure scene** is enabled in build settings

### Timer Not Working
- **Assign timerText** in MicrogameController
- **Check UI Canvas** render mode
- **Verify TextMeshPro component** exists and is active

### Win/Lose Not Triggering
- **Find MicrogameController** properly in Start()
- **Call TriggerWin/TriggerLose** only once
- **Check for null references**

### Game Loop Issues
- **Verify MicrogameData** is in MicrogameList
- **Check ScriptableObject configuration**
- **Ensure WarioWareGameManager** persists between scenes

## Example: Simple Collection Game

```csharp
public class CollectGameController : MonoBehaviour
{
    [SerializeField] private GameObject[] collectibles;
    [SerializeField] private int requiredCount = 3;
    
    private MicrogameController microgameController;
    private int collectedCount = 0;
    
    private void Start()
    {
        microgameController = FindObjectOfType<MicrogameController>();
        SpawnCollectibles();
    }
    
    private void SpawnCollectibles()
    {
        // Randomly position collectibles
        foreach (GameObject collectible in collectibles)
        {
            Vector3 randomPos = new Vector3(
                Random.Range(-5f, 5f),
                Random.Range(-3f, 3f),
                0f
            );
            collectible.transform.position = randomPos;
        }
    }
    
    public void OnCollectibleCollected()
    {
        collectedCount++;
        if (collectedCount >= requiredCount)
        {
            microgameController.TriggerWin();
        }
    }
}
```

## File Structure Summary

```
Assets/
├── Scenes/
│   └── Microgames/
│       └── YourGameName.unity
├── Scripts/
│   └── YourGameController.cs
└── ScriptableObjects/
    └── MicrogameData/
        └── YourGameData.asset
```

Remember: Each microgame should be simple, focused, and completable within the time limit!