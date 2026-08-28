using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Character/Permitted Names")]
public class CreatorNames : ScriptableObject
{
    public string[] bannedWords;
    public string[] randomNames;
}
