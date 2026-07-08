using UnityEngine;

public class ItemLight : MonoBehaviour
{
    // Item
    [SerializeField] ItemStatGenerator generator;
    [SerializeField] ItemPickup item;

    // Light
    [SerializeField] SpriteRenderer itemLight;
    [SerializeField] SpriteRenderer itemLight2;
    [SerializeField] ParticleSystem particles;

    // Icon
    [SerializeField] SpriteRenderer mapIcon;

    private void OnEnable()
    {
        generator.net_ItemRarity.OnValueChanged += OnRarityChanged;
    }

    private void OnDisable()
    {
        generator.net_ItemRarity.OnValueChanged -= OnRarityChanged;
    }

    private void Start()
    {
        ApplyRarityVisuals(generator.net_ItemRarity.Value);
    }

    private void OnRarityChanged(ItemRarity previous, ItemRarity current)
    {
        ApplyRarityVisuals(current);
    }

    private void ApplyRarityVisuals(ItemRarity rarity)
    {
        // Get Rarity Color
        Color color = generator.Item.GetRarityColor(generator.net_ItemRarity.Value);

        itemLight.color = color;
        itemLight2.color = color;
        mapIcon.color = color;

        ParticleSystem.MainModule main = particles.main;
        main.startColor = color;

        Color light1 = itemLight.color;
        light1.a = .50f;
        itemLight.color = light1;

        Color light2 = itemLight2.color;
        light2.a = .15f;
        itemLight2.color = light2;
    }
}
