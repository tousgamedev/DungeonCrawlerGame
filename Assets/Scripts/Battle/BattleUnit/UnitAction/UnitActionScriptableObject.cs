using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "Character/New Skill")]
public class UnitActionScriptableObject : ScriptableObject
{
    public string ActionName => actionName;
    public GameObject EffectsPrefab => effectPrefab;
    public float BaseExecutionSpeed => baseExecutionSpeed;
    public int BasePower => basePower;
        
    [SerializeField] private string actionName;
    [SerializeField] private ActionSelectionBehavior selectionBehavior;
    [SerializeField] private ActionOptionList optionList;
    [SerializeField] private GameObject effectPrefab;
    [SerializeField] private float baseExecutionSpeed;
    [SerializeField] private ActionTarget target;
    [SerializeField] private int basePower;

    public void PerformActionSelection()
    {
        switch (selectionBehavior)
        {
            case ActionSelectionBehavior.SelectTarget:
                BattleManager.Instance.SelectPartyMemberAction(this);
                break;
            case ActionSelectionBehavior.OpenOptionMenu:
                break;
            case ActionSelectionBehavior.SpecialInput:
                break;
            default:
                LogHelper.DebugLog("Invalid SkillSelectionBehavior.", LogType.Error);
                break;
        }
    }
}