using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Composites;
using UnityEngine.UI;
using static GameData;

public class PlayerSkillManager : MonoBehaviour
{
    public GameObject skillPanel;

    [Header("Active Skill")]
    public Button guardSkill;
    public Button reflectionSkill;
    public Button dummySkill;
    public Button dashSkill;

    [Header("Passive Skill")]
    public Button multipleShotSkill;
    public Button onemoreTimeSkill;
    public Button bigArrowSkill;
    public Button multiKillSkill;

    private GameData gameData;
    // To do: 스킬 포인트 제한 추가

    private void Awake()
    {
        SaveData saveData = SaveData.instance;
        if (saveData != null)
        {
            gameData = saveData.LoadGame();
        }

        guardSkill.onClick.AddListener(OnClickGuardSkill);
        reflectionSkill.onClick.AddListener(OnClickReflectionSkill);
        dummySkill.onClick.AddListener(OnClickDummySkill);
        dashSkill.onClick.AddListener(OnClickDashSkill);

        multipleShotSkill.onClick.AddListener(OnClickMultipleShotSkill);
        onemoreTimeSkill.onClick.AddListener(OnClickOnemoreTimeShotSkill);
        bigArrowSkill.onClick.AddListener(OnClickBigArrowSkill);
        multiKillSkill.onClick.AddListener(OnClickMultiKillSkill);
    }

    private void Start()
    {
        InitSkillSet();
    }

    private void Update()
    {
        if (!GameManager.instance.isSkillManagerActive)
            return;

        if(Input.GetKeyDown(KeyCode.M))
        {
            GameManager.instance.ToggleSkillPanel();
        }
    }

    void InitSkillSet()
    {
        PlayerController playerController = GetComponent<PlayerController>();
        if(playerController == null)
        {
            return;
        }

        foreach(SkillID skillId in gameData.skillSet)
        {
            playerController.SkillUnlock(skillId);

            if(skillId == SkillID.Dummy)
            {
                ToggleOn(dummySkill);
            }
            else if(skillId == SkillID.Reflection)
            {
                ToggleOn(reflectionSkill);
            }
            else if(skillId == SkillID.Guard)
            {
                ToggleOn(guardSkill);
            }
            else if(skillId == SkillID.Dash)
            {
                ToggleOn(dashSkill);
            }
            else if(skillId == SkillID.MultipleShot)
            {
                ToggleOn(multipleShotSkill);
            }
            else if(skillId == SkillID.OnemoreTimeShot)
            {
                ToggleOn(onemoreTimeSkill);
            }
            else if(skillId == SkillID.BigArrow)
            {
                ToggleOn(bigArrowSkill);
            }
            else if(skillId == SkillID.MultiKill)
            {
                ToggleOn(multiKillSkill);
            }
        }
    }

    void ToggleOn(Button inButton)
    {
        SkillToggleButton button = inButton.GetComponent<SkillToggleButton>();
        button.ToggleSkill();
    }

    public GameData GetGameData()
    {
        if (gameData == null)
        {
            SaveData saveData = SaveData.instance;
            if (saveData != null)
            {
                gameData = saveData.LoadGame();
            }
        }
        return gameData;
    }

    public void OnClickGuardSkill()
    {
        SkillLock(reflectionSkill, SkillID.Reflection);

        SkillButtonClick(guardSkill, SkillID.Guard);
    }

    public void OnClickReflectionSkill()
    {
        SkillLock(guardSkill, SkillID.Guard);

        SkillButtonClick(reflectionSkill, SkillID.Reflection);
    }

    public void OnClickDummySkill()
    {
        SkillButtonClick(dummySkill, SkillID.Dummy);
    }

    public void OnClickDashSkill()
    {
        SkillButtonClick(dashSkill, SkillID.Dash);
    }

    public void OnClickMultipleShotSkill()
    {
        SkillButtonClick(multipleShotSkill, SkillID.MultipleShot);
    }

    public void OnClickOnemoreTimeShotSkill()
    {
        SkillButtonClick(onemoreTimeSkill, SkillID.OnemoreTimeShot);
    }

    public void OnClickBigArrowSkill()
    {
        SkillButtonClick(bigArrowSkill, SkillID.BigArrow);
    }

    public void OnClickMultiKillSkill()
    {
        SkillButtonClick(multiKillSkill, SkillID.MultiKill);
    }

    void SkillButtonClick(Button inButton, SkillID skillID)
    {
        if (inButton == null)
            return;

        SkillToggleButton button = inButton.GetComponent<SkillToggleButton>();
        if(button != null)
        {
            if(button.isActivated)
            {
                RefundSkillPoint(skillID);
                button.ToggleSkill();
            }
            else
            {
                if(UseSkillPoint(skillID))
                {
                    button.ToggleSkill();
                }
            }
        }
    }

    void SkillLock(Button inButton, SkillID skillID)
    {
        if (inButton == null)
            return;

        SkillToggleButton toggle = inButton.GetComponent<SkillToggleButton>();
        if (toggle != null)
        {
            if (toggle.isActivated)
            {
                RefundSkillPoint(skillID);
                toggle.ToggleSkill();
            }
        }
    }

    bool UseSkillPoint(SkillID skillID)
    {
        if(gameData.remainPoint >= 1)
        {
            if(gameData.skillSet.Contains(skillID))
            {
                Debug.Log("already exist");
                return false;
            }
            gameData.skillSet.Add(skillID);
            gameData.remainPoint--;
            gameData.allocatedPoint++;

            if (gameData.totalSkillPoint != gameData.remainPoint + gameData.allocatedPoint)
            {
                Debug.Log("Skill Point Error");
                return false;
            }

            SkillUnlock(skillID);
            SaveSkillSet();
            return true;
        }
        Debug.Log("Not enough Skill Point");
        return false;
    }

    void RefundSkillPoint(SkillID skillID)
    {
        if (!gameData.skillSet.Contains(skillID))
        {
            Debug.Log("It doesn't exist");
            return;
        }
        gameData.skillSet.Remove(skillID);
        gameData.remainPoint++;
        gameData.allocatedPoint--;

        if (gameData.totalSkillPoint != gameData.remainPoint + gameData.allocatedPoint)
        {
            Debug.Log("Skill Point Error");
            return;
        }

        SkillLock(skillID);
        SaveSkillSet();
    }

    void SkillUnlock(SkillID skillID)
    {
        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.SkillUnlock(skillID);
        }
    }

    void SkillLock(SkillID skillID)
    {
        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.SkillLock(skillID);
        }
    }

    void SaveSkillSet()
    {
        SaveData saveData = SaveData.instance;
        if (saveData != null)
        {
            saveData.SaveGame(gameData);
        }
    }
}
