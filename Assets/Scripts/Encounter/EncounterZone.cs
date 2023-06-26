using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class EncounterZone : MonoBehaviour
{
    public int EncounterRate { get; private set; }

    [SerializeField] [Range(0, 100)] private float encounterRatePercent;
    [SerializeField] private List<EncounterPair> encounterPairs = new();

    private void OnEnable()
    {
        EncounterRate = (int)(10 * encounterRatePercent);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameStateManager.Instance.SetEncounterZone(this);
        }
    }

    public EncounterGroupScriptableObject SelectRandomEncounter()
    {
        if (encounterPairs.Count == 0)
            return null;

        int randomWeight = Random.Range(0, EncounterController.MaxWeight);
        foreach (EncounterPair encounter in encounterPairs)
        {
            if (randomWeight < encounter.Weight)
            {
                return encounter.EncounterGroup;
            }

            randomWeight -= encounter.Weight;
        }

        return null;
    }
    
#if UNITY_EDITOR
    public void AdjustEncounterWeights()
    {
        float totalWeight = 0;
        for (int i = encounterPairs.Count - 1; i >= 0; i--)
        {
            // Remove encounters with weight 0
            if (encounterPairs[i].Weight == 0)
            {
                encounterPairs.RemoveAt(i);
            }
            else
            {
                totalWeight += encounterPairs[i].Weight;
            }
        }

        float adjustmentRatio = totalWeight > 0 ? EncounterController.MaxWeight / totalWeight : 0f;
        foreach (EncounterPair encounter in encounterPairs)
        {
            encounter.Weight = Mathf.RoundToInt(encounter.Weight * adjustmentRatio);
        }
    }
    
    public void SortEncounterPairsByWeight()
    {
        encounterPairs.Sort((pair1, pair2) => pair1.Weight.CompareTo(pair2.Weight));
    }
#endif
}