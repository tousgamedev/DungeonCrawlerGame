using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "Character/New Skill")]
public class SkillScriptableObject : ScriptableObject
{
    public string SkillName => skillSkillName;
    public GameObject EffectsPrefab => effectPrefab;
    public float BaseExecutionSpeed => baseExecutionSpeed;
    public int BaseAttack => baseAttack;
        
    [SerializeField] private string skillSkillName;
    [SerializeField] private SkillSelectionBehavior selectionBehavior;
    [SerializeField] private SkillOptionList optionList;
    [SerializeField] private GameObject effectPrefab;
    [SerializeField] private float baseExecutionSpeed;
    [SerializeField] private SkillTarget target;
    [SerializeField] private int baseAttack;

    public void PerformActionSelection()
    {
        switch (selectionBehavior)
        {
            case SkillSelectionBehavior.SelectTarget:
                BattleManager.Instance.SelectPartyMemberAction(this);
                break;
            case SkillSelectionBehavior.OpenOptionMenu:
                break;
            case SkillSelectionBehavior.SpecialInput:
                break;
            default:
                LogHelper.DebugLog("Invalid SkillSelectionBehavior.", LogType.Error);
                break;
        }
    }
}