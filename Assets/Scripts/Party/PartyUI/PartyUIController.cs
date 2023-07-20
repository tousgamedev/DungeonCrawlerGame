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
    
    public override void AddUnit(BattleUnit unit)
    {
        if (!TryGetComponentFromPoolObject(unit, out PartyMemberPanel panel))
            return;

        panel.Initialize(unit);
        panel.gameObject.SetActive(true);
        ActiveUnits.Add(unit, panel);
    }
    
    public override void RemoveUnit(BattleUnit unit)
    {
        if (!TryGetComponentFromPoolObject(unit, out PartyMemberPanel panel))
            return;
        
        panel.RemovePanel();
        panel.gameObject.SetActive(false);
        ReturnPoolObject(gameObject);
    }
}