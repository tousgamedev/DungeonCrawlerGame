using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerUnitManager : ManagerBase<PlayerUnitManager>
{
    public List<BattleUnit> PlayerUnits { get; } = new();
    
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
            BattleUnit newUnit = new(member, true);
            PlayerUnits.Add(newUnit);
            uiController.AddUnit(newUnit);
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

    public BattleUnit SelectRandomLivingUnit()
    {
        var liveUnits = new List<BattleUnit>();
        foreach (BattleUnit unit in PlayerUnits)
        {
            if(unit.Stats.IsDead)
                continue;
            
            liveUnits.Add(unit);
        }

        if (liveUnits.Count == 0)
            return null;
        
        int unitIndex = Random.Range(0, liveUnits.Count);
        return liveUnits[unitIndex];
    }

    public bool IsPartyDefeated()
    {
        foreach (BattleUnit unit in PlayerUnits)
        {
            if (!unit.Stats.IsDead)
                return false;
        }

        return true;
    }
}