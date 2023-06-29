using System.Collections.Generic;
using UnityEngine;

public class PlayerPartyManager : ManagerBase<PlayerPartyManager>
{
    public List<UnitScriptableObject> PlayerParty => playerParty;

    [SerializeField] private PartyUIController partyUI;
    [SerializeField] private List<UnitScriptableObject> playerParty;

    private new void Awake()
    {
        base.Awake();
        CreateTeam();
    }

    public void PopPartyPanels()
    {
        partyUI.PopPartyPanels();
    }

    public void StowPartyPanels()
    {
        partyUI.StowPartyPanels();
    }

    private void CreateTeam()
    {
        foreach (UnitScriptableObject member in playerParty)
        {
            // TODO: Replace this class with something better.
            BattleUnit newMember = new(member, 0, 0);
            partyUI.AddUnit(newMember);
        }
    }
}
