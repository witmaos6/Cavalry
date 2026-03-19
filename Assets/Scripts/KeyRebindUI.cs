using Mono.Cecil;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyRebindUI : MonoBehaviour
{
    [SerializeField] private InputActionReference actionReference;

    [SerializeField] private TMP_Text settingKeyText;
    [SerializeField] private TMP_Text descriptionText;

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    void Start()
    {
        UpdateDisplay();

        if(descriptionText != null)
        {
            descriptionText.gameObject.SetActive(false);
        }
    }

    public void OnRebindButtonClick()
    {
        if(rebindingOperation != null)
        {
            CancelRebinding();
        }
        else
        {
            StartRebinding();
        }
    }

    private void UpdateDisplay()
    {
        var action = InputManager.instance.controls.asset.FindAction(actionReference.action.id);
        settingKeyText.text = action.GetBindingDisplayString();
    }

    public void StartRebinding()
    {
        var targetAction = InputManager.instance.controls.asset.FindAction(actionReference.action.id);
        targetAction.Disable();

        if(descriptionText != null)
        {
            descriptionText.gameObject.SetActive(true);
        }

        var rebind = targetAction.PerformInteractiveRebinding()
            .WithCancelingThrough("<Keyboard>/escape")
            .WithControlsExcluding("<Mouse>")
            .OnMatchWaitForAnother(0.1f);

        rebindingOperation = rebind
            .OnComplete(operation => FinishRebinding(targetAction))
            .OnCancel(operation => FinishRebinding(targetAction))
            .Start();
    }

    public void CancelRebinding() => rebindingOperation?.Cancel();

    private void FinishRebinding(InputAction action)
    {
        rebindingOperation?.Dispose();
        rebindingOperation = null;

        if(descriptionText != null)
        {
            descriptionText.gameObject.SetActive(false);
        }
        action.Enable();

        UpdateDisplay();
        SaveBinding();
    }

    private void SaveBinding()
    {
        InputManager.instance.SaveAllBindings();
    }
}
