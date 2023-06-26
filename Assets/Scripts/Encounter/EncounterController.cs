using UnityEngine;

public static class EncounterController
{
    public const int MaxWeight = 1000;
    
    public static bool StartEncounter(EncounterZone zone)
    {
        if (zone.EncounterRate > MaxWeight)
            return true;
        
        int roll = Random.Range(0, MaxWeight);
        return roll <= zone.EncounterRate;
    }
}
