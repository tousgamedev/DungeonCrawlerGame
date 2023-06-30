using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPartyManager : ManagerBase<PlayerPartyManager>
{
    public List<BattleUnit> PlayerParty { get; } = new();
    public IEnumerator GetPopCoroutine => uiController.PopPanelsCoroutine();
    public IEnumerator GetStowCoroutine => uiController.StowPanelsCoroutine();

    [SerializeField] private PartyUIController uiController;
    [SerializeField] private List<UnitBaseScriptableObject> characterBases;

    private Dictionary<string, UnitBaseScriptableObject> characterBaseDict;

    private new void Awake()
    {
        if (uiController == null)
        {
            LogHelper.Report("UI Controller is null!", LogGroup.System, LogType.Error);
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
}