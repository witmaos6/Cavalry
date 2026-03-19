using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseRebindUI : MonoBehaviour
{
    [SerializeField] private InputActionReference actionA;
    [SerializeField] private InputActionReference actionB;

    [SerializeField] private TMP_Text textA;
    [SerializeField] private TMP_Text textB;

    private void Start()
    {
        UpdateDisplay();
    }

    public void SwapButtons()
    {
        var realActionA = InputManager.instance.controls.asset.FindAction(actionA.action.id);
        var realActionB = InputManager.instance.controls.asset.FindAction(actionB.action.id);

        string pathA = realActionA.bindings[0].effectivePath;
        string pathB = realActionB.bindings[0].effectivePath;

        realActionA.ApplyBindingOverride(pathB);
        realActionB.ApplyBindingOverride(pathA);

        InputManager.instance.SaveAllBindings();
        UpdateDisplay();

        Debug.Log("마우스 좌/우클릭 스왑 완료!");
    }

    private void UpdateDisplay()
    {
        var realActionA = InputManager.instance.controls.asset.FindAction(actionA.action.id);
        var realActionB = InputManager.instance.controls.asset.FindAction(actionB.action.id);

        if (textA != null)
            textA.text = realActionA.GetBindingDisplayString();

        if (textB != null)
            textB.text = realActionB.GetBindingDisplayString();
    }
}
