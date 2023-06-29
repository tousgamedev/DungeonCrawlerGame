using System;
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

        unit.Initialize();
        ActiveUnits.Add(unit, panel);
    }

    public void PopPartyPanels()
    {
        CoroutineManager.Instance.RunCoroutine(PopPanelsCoroutine());
    }
    
    public void StowPartyPanels()
    {
        CoroutineManager.Instance.RunCoroutine(StowPanelsCoroutine());
    }

    private IEnumerator PopPanelsCoroutine()
    {
        foreach (PartyMemberPanel panel in ActiveUnits.Values)
        {
            panel.PopPanel();
            yield return new WaitForSeconds(cascadeInterval);
        }
    }
    
    private IEnumerator StowPanelsCoroutine()
    {
        foreach (PartyMemberPanel panel in ActiveUnits.Values)
        {
            panel.StowPanel();
            yield return new WaitForSeconds(cascadeInterval);
        }
    }
}