using UnityEngine;

[CreateAssetMenu(fileName = "New Microgame", menuName = "WarioWare/Microgame Data")]
public class MicrogameData : ScriptableObject
{
    [Header("Scene Settings")]
    public string sceneName;
    
    [Header("Command Display")]
    public string commandText = "Do something!";
    
    [Header("Timing")]
    [Range(1f, 10f)]
    public float timeLimit = 5f;
    
    [Header("Progression")]
    public bool isUnlocked = true;
}