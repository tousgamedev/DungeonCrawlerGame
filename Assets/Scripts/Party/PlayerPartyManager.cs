using System.Collections.Generic;
using UnityEngine;

public class PlayerPartyManager : ManagerBase<PlayerPartyManager>
{
    public List<BattleUnit> PlayerParty { get; } = new();
    
    /// <summary>
    /// The maximum number of actions a party member can have in the base list. Does not include the 'Item' command.
    /// </summary>
    public int MaxBaseActionCommands => maxBaseActionCommands;

    [SerializeField] private PartyUIController uiController;
    [Tooltip("The max actions a party member can have (not including 'Item')")] 
    [SerializeField] private int maxBaseActionCommands = 4;
    [SerializeField] private List<UnitBaseScriptableObject> characterBases;

    private Dictionary<string, UnitBaseScriptableObject> characterBaseDict;

    private new void Awake()
    {
        if (uiController == null)
        {
            LogHelper.Report("UI Controller is null!", LogType.Error, LogGroup.System);
            return;
        }
        
        base.Awake();
        CreateTeam();
        BuildCharacterDictionary();
    }

    private void CreateTeam()
    {
        foreach (UnitBaseScriptableObject member in characterBases)
        {
            // TODO: Replace this class with something better.
            BattleUnit newMember = new(member);
            PlayerParty.Add(newMember);
            uiController.AddUnit(newMember);
        }
    }

    private void BuildCharacterDictionary()
    {
        characterBaseDict = new Dictionary<string, UnitBaseScriptableObject>();
        foreach (UnitBaseScriptableObject characterBase in characterBases)
        {
            characterBaseDict[characterBase.name] = characterBase;
        }
    }

    public BattleUnit SelectRandomPartyMember()
    {
        int unitIndex = Random.Range(0, PlayerParty.Count);
        return PlayerParty[unitIndex];
    }

    public List<BattleUnit> SelectAllPartyMembers()
    {
        return PlayerParty;
    }

    public void EnablePartyMemberActionList(BattleUnit unit)
    {
        uiController.EnableMemberActionList(unit);
    }
    
    public void ShowSelectedPartyMemberAction(BattleUnit unit)
    {
        uiController.ShowSelectedPartyMemberAction(unit);
    }
    
    public void DisablePartyMemberActionList(BattleUnit unit)
    {
        uiController.DisableMemberActionList(unit);
    }
}