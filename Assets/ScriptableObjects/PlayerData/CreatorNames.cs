using UnityEngine;

[CreateAssetMenu(fileName = "CreatorNames", menuName = "Scriptable Objects/CreatorNames")]
public class CreatorNames : ScriptableObject
{
    public string[] bannedWords;
    public string[] randomNames;
}
