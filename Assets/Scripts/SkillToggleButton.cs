using UnityEngine;
using UnityEngine.UI;

public class SkillToggleButton : MonoBehaviour
{
    public bool isActivated = false;

    [Header("Visuals")]
    public Color activeColor = Color.white;
    public Color inactiveColor = Color.gray;

    private Button button;
    private Image buttonImage;

    private void Awake()
    {
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();

        UpdateVisual();
    }

    public void ToggleSkill()
    {
        isActivated = !isActivated;
        UpdateVisual();
    }

    void UpdateVisual()
    {
        if(buttonImage != null)
        {
            buttonImage.color = isActivated ? activeColor : inactiveColor;
        }
    }
}
