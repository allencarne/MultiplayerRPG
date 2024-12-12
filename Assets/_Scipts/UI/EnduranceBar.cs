using UnityEngine;
using UnityEngine.UI;

public class EnduranceBar : MonoBehaviour
{
    [SerializeField] Image enduranceBar;

    public void UpdateEnduranceBar(float maxEndurance, float currentEndurance)
    {
        enduranceBar.fillAmount = maxEndurance / currentEndurance;
    }
}
