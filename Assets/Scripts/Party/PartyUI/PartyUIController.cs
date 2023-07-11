using UnityEngine;

public class PartyUIController : UnitObjectPoolController<PartyMemberPanel>
{
    protected override GameObject PoolPrefab => partyMemberPrefab;
    protected override int PoolSize => memberPoolSize;

    [SerializeField] private GameObject partyContainer;
    [SerializeField] private GameObject partyMemberPrefab;
    [SerializeField] private int memberPoolSize = 6;

    private void Awake()
    {
        InitializeObjectPool(partyContainer.transform);
    }

    public override void RemoveUnit(BattleUnit unit)
    {
    }

    public override void AddUnit(BattleUnit unit)
    {
        if (!TryGetComponentFromPoolObject(unit, out PartyMemberPanel panel))
            return;

        panel.Initialize(unit);
        panel.gameObject.SetActive(true);
        ActiveUnits.Add(unit, panel);
    }

    public void EnableMemberActionList(BattleUnit unit)
    {
        if (ActiveUnits.TryGetValue(unit, out PartyMemberPanel panel))
        {
            panel.ShowActionList();
        }
    }

    public void ShowSelectedPartyMemberAction(BattleUnit unit)
    {
        if (ActiveUnits.TryGetValue(unit, out PartyMemberPanel panel))
        {
            panel.ShowSelectedAction();
        }
    }
    
    public void DisableMemberActionList(BattleUnit unit)
    {
        if (ActiveUnits.TryGetValue(unit, out PartyMemberPanel panel))
        {
            panel.HideActionList();
        }
    }
}