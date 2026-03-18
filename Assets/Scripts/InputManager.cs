using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance { get; private set; }

    public PlayerControls controls;
    private const string rebindsKey = "Combined_Input_Rebinds";

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            controls = new PlayerControls();

            LoadAllBindings();
        }
    }

    public void SaveAllBindings()
    {
        string rebinds = controls.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString(rebindsKey, rebinds);
        PlayerPrefs.Save();
    }

    private void LoadAllBindings()
    {
        string rebinds = PlayerPrefs.GetString(rebindsKey, string.Empty);
        if(!string.IsNullOrEmpty(rebinds))
        {
            controls.LoadBindingOverridesFromJson(rebinds);
        }
    }

    public void ResetAllBindings()
    {
        foreach(InputAction map in controls)
        {
            map.RemoveAllBindingOverrides();
        }
        PlayerPrefs.DeleteKey(rebindsKey);
    }
}
