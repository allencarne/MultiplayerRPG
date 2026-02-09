using Unity.Netcode;
using UnityEngine;

public class PrayStatue : MonoBehaviour, IInteractable
{
    [SerializeField] GetPlayerReference getPlayer;

    [SerializeField] GameObject sparkleParticle;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] Material startMat;
    [SerializeField] Material endMat;

    [SerializeField] SpriteRenderer miniMapIcon;

    [SerializeField] string area;
    [SerializeField] int index;
    PlayerStats PlayerStats;

    public string DisplayName => "Praying Statue";

    public void Initalize()
    {
        if (getPlayer != null)
        {
            PlayerStats = getPlayer.player.GetComponent<PlayerStats>();
            if (PlayerStats == null) return;

            PlayerStats.net_CharacterSlot.OnValueChanged += OnCharacterSlotChanged;
            UpdateVisual(PlayerStats.net_CharacterSlot.Value);
        }
    }

    private void OnDestroy()
    {
        if (PlayerStats != null && PlayerStats.net_CharacterSlot != null)
        {
            PlayerStats.net_CharacterSlot.OnValueChanged -= OnCharacterSlotChanged;
        }
    }

    private void OnCharacterSlotChanged(int previousValue, int newValue)
    {
        UpdateVisual(newValue);
    }

    public void UpdateVisual(int characterNumber)
    {
        string status = PlayerPrefs.GetString($"Character{characterNumber}_{area}_Statue_{index}", "Incomplete");

        if (status == "Completed")
        {
            sprite.material = endMat;
            miniMapIcon.color = Color.white;
            sparkleParticle.SetActive(false);
        }
        else
        {
            sprite.material = startMat;
            miniMapIcon.color = Color.black;
        }
    }

    public void Interact(PlayerInteract player)
    {
        PlayerStats stats = player.GetComponentInParent<PlayerStats>();
        if (stats == null || !stats.IsOwner) return;

        int characterNumber = stats.net_CharacterSlot.Value;
        string status = PlayerPrefs.GetString($"Character{characterNumber}_{area}_Statue_{index}", "Incomplete");

        if (status == "Completed") return;

        PlayerPrefs.SetString($"Character{characterNumber}_{area}_Statue_{index}", "Completed");
        PlayerPrefs.Save();

        sprite.material = endMat;
        miniMapIcon.color = Color.white;
        sparkleParticle.SetActive(false);
        stats.IncreaseAttribuePoints();

        PlayerQuest quest = player.GetComponentInParent<PlayerQuest>();
        if (quest != null)
        {
            // Check for "Praying Statue 0"

            quest.UpdateObjective(ObjectiveType.Complete, "Praying Statue");
        }
    }
}
