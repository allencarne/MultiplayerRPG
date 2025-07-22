using TMPro;
using UnityEngine;

public class NPCCustomization : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI npcName;
    // Eye Color
    // Hair
    // Hair Color

    private void Start()
    {
        npcName.text = gameObject.name;
    }
}
