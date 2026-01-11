using UnityEngine;

public class PrayStatue : MonoBehaviour, IInteractable
{
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] Material startMat;
    [SerializeField] Material endMat;

    [SerializeField] string area;
    [SerializeField] int index;

    public string DisplayName => "Praying Statue";

    private void Start()
    {
        int characterNumber = PlayerPrefs.GetInt("SelectedCharacter");
        string status = PlayerPrefs.GetString($"Character{characterNumber}_{area}_Statue_{index}", "Incomplete");

        if (status == "Completed")
        {
            Debug.Log("Completed");
            sprite.material = endMat;
        }
        else
        {
            Debug.Log("Incomplete");
            sprite.material = startMat;
        }
    }

    public void Interact(PlayerInteract player)
    {
        int characterNumber = PlayerPrefs.GetInt("SelectedCharacter");
        string status = PlayerPrefs.GetString($"Character{characterNumber}_{area}_Statue_{index}", "Incomplete");

        if (status == "Completed") return;

        PlayerStats stats = player.GetComponentInParent<PlayerStats>();
        if (stats != null)
        {
            sprite.material = endMat;
            stats.IncreaseAttribuePoints();

            PlayerPrefs.SetString($"Character{characterNumber}_{area}_Statue_{index}", "Completed");
            PlayerPrefs.Save();
        }
    }
}
