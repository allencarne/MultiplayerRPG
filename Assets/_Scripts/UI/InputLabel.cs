using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputLabel : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] GameObject inputLabelObject;
    [SerializeField] string labelName;
    [SerializeField] TextMeshProUGUI labelText;

    [SerializeField] InputActionReference interactAction;
    [SerializeField] PlayerInput playerInput;

    public void OnSelect(BaseEventData eventData)
    {
        UpdateText();
        inputLabelObject.SetActive(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        inputLabelObject.SetActive(false);
    }

    void UpdateText()
    {
        int bindingIndex = GetBindingIndexForCurrentScheme(playerInput.currentControlScheme);
        string bindName = interactAction.action.GetBindingDisplayString(bindingIndex);

        labelText.text = $"<color=#00FF00>\"{bindName}\"</color> {labelName}";
    }

    private int GetBindingIndexForCurrentScheme(string scheme)
    {
        if (string.IsNullOrEmpty(scheme)) return 0;

        for (int i = 0; i < interactAction.action.bindings.Count; i++)
        {
            InputBinding binding = interactAction.action.bindings[i];
            if (binding.groups.Contains(scheme))
            {
                return i;
            }
        }

        return 0;
    }
}
