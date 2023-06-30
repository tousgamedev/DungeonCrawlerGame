using System.Collections;
using UnityEngine;

public class PartyUIController : UnitObjectPoolController<PartyMemberPanel>
{
    protected override GameObject PoolPrefab => partyMemberPrefab;
    protected override int PoolSize => memberPoolSize;

    [SerializeField] private GameObject partyContainer;
    [SerializeField] private GameObject partyMemberPrefab;
    [SerializeField] private int memberPoolSize = 6;
    [SerializeField] [Min(0)] private float cascadeInterval = .25f;

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

    public IEnumerator PopPanelsCoroutine()
    {
        foreach (PartyMemberPanel panel in ActiveUnits.Values)
        {
            panel.PopPanel();
            yield return new WaitForSeconds(cascadeInterval);
        }
        
        yield return new WaitForSeconds(.5f); // waiting for final panel to 
    }
    
    public IEnumerator StowPanelsCoroutine()
    {
        foreach (PartyMemberPanel panel in ActiveUnits.Values)
        {
            panel.StowPanel();
            yield return new WaitForSeconds(cascadeInterval);
        }
    }
}