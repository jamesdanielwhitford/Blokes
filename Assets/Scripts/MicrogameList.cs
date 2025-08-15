using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MicrogameList", menuName = "WarioWare/Microgame List")]
public class MicrogameList : ScriptableObject
{
    public List<MicrogameData> microgames = new List<MicrogameData>();
}