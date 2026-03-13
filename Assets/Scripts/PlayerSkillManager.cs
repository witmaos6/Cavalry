using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static GameData;

public class PlayerSkillManager : MonoBehaviour
{
    public GameObject skillPanel;
    public Button guardSkill;
    public Button reflectionSkill;
    public Button dummySkill;
    public Button dashSkill;

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
    }

    private void Start()
    {
        InitSkillSet();
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
        if (reflectionSkill != null)
        {
            SkillToggleButton toggle = reflectionSkill.GetComponent<SkillToggleButton>();
            if (toggle != null)
            {
                if (toggle.isActivated)
                {
                    toggle.ToggleSkill();
                    RefundSkillPoint(SkillID.Reflection);
                }   
            }
        }

        if (guardSkill != null)
        {
            SkillToggleButton button = guardSkill.GetComponent<SkillToggleButton>();
            if (button != null)
            {
                if(button.isActivated)
                {
                    RefundSkillPoint(SkillID.Guard);
                }
                else
                {
                    UseSkillPoint(SkillID.Guard);
                }
                button.ToggleSkill();
            }
        }
    }

    public void OnClickReflectionSkill()
    {
        if (guardSkill != null)
        {
            SkillToggleButton toggle = guardSkill.GetComponent<SkillToggleButton>();
            if (toggle != null)
            {
                if (toggle.isActivated)
                {
                    toggle.ToggleSkill();
                    RefundSkillPoint(SkillID.Guard);
                }   
            }
        }

        if(reflectionSkill != null)
        {
            SkillToggleButton button = reflectionSkill.GetComponent<SkillToggleButton>();
            if (button != null)
            {
                if(button.isActivated)
                {
                    RefundSkillPoint(SkillID.Reflection);
                }
                else
                {
                    UseSkillPoint(SkillID.Reflection);
                }
                button.ToggleSkill();
            }
        }
    }

    public void OnClickDummySkill()
    {
        if (dummySkill == null)
            return;

        SkillToggleButton button = dummySkill.GetComponent<SkillToggleButton>();
        if(button != null)
        {
            if (button.isActivated)
            {
                RefundSkillPoint(SkillID.Dummy);
            }
            else
            {
                UseSkillPoint(SkillID.Dummy);
            }
            button.ToggleSkill();
        }
    }

    public void OnClickDashSkill()
    {
        if (dashSkill == null)
            return;

        SkillToggleButton button = dashSkill.GetComponent<SkillToggleButton>();
        if (button != null)
        {
            if (button.isActivated)
            {
                RefundSkillPoint(SkillID.Dash);
            }
            else
            {
                UseSkillPoint(SkillID.Dash);
            }
            button.ToggleSkill();
        }
    }

    void UseSkillPoint(SkillID skillID)
    {
        if(gameData.remainPoint >= 1)
        {
            if(gameData.skillSet.Contains(skillID))
            {
                Debug.Log("already exist");
                return;
            }
            gameData.skillSet.Add(skillID);
            gameData.remainPoint--;
            gameData.allocatedPoint++;

            if (gameData.totalSkillPoint != gameData.remainPoint + gameData.allocatedPoint)
            {
                Debug.Log("Skill Point Error");
                return;
            }

            SkillUnlock(skillID);
            SaveSkillSet();
        }
        else
        {
            Debug.Log("Not enough Skill Point");
        }
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
