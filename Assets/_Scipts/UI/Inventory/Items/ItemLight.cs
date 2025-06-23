using UnityEngine;

public class ItemLight : MonoBehaviour
{
    [SerializeField] ItemPickup item;
    [SerializeField] ItemRarityInfo info;
    [SerializeField] SpriteRenderer itemLight;
    [SerializeField] SpriteRenderer itemLight2;
    [SerializeField] ParticleSystem particles;

    private void Start()
    {
        switch (item.Item.ItemRarity)
        {
            case ItemRarity.Common:
                itemLight.color = info.CommonColor;
                itemLight2.color = info.CommonColor;

                ParticleSystem.MainModule ma = particles.main;
                ma.startColor = info.CommonColor;


                break;
            case ItemRarity.Uncommon:
                itemLight.color = info.UnCommonColor;
                itemLight2.color = info.UnCommonColor;

                ParticleSystem.MainModule ma2 = particles.main;
                ma2.startColor = info.UnCommonColor;

                break;
            case ItemRarity.Rare:
                itemLight.color = info.RareColor;
                itemLight2.color = info.RareColor;

                ParticleSystem.MainModule ma3 = particles.main;
                ma3.startColor = info.RareColor;

                break;
            case ItemRarity.Epic:
                itemLight.color = info.EpicColor;
                itemLight2.color = info.EpicColor;

                ParticleSystem.MainModule ma4 = particles.main;
                ma4.startColor = info.EpicColor;

                break;
            case ItemRarity.Exotic:
                itemLight.color = info.ExoticColor;
                itemLight2.color = info.ExoticColor;

                ParticleSystem.MainModule ma5 = particles.main;
                ma5.startColor = info.ExoticColor;

                break;
            case ItemRarity.Mythic:
                itemLight.color = info.MythicColor;
                itemLight2.color = info.MythicColor;

                ParticleSystem.MainModule ma6 = particles.main;
                ma6.startColor = info.MythicColor;

                break;
            case ItemRarity.Legendary:
                itemLight.color = info.LegendaryColor;
                itemLight2.color = info.LegendaryColor;

                ParticleSystem.MainModule ma7 = particles.main;
                ma7.startColor = info.LegendaryColor;

                break;
        }

        var tempColor = itemLight2.color;
        tempColor.a = .15f;
        itemLight2.color = tempColor;
    }
}
