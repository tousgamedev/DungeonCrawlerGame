using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EncounterGroup", menuName = "Battle/Encounter Group")]
public class EncounterGroupScriptableObject : ScriptableObject
{
    public List<UnitBaseScriptableObject> Enemies => enemies;
    [SerializeField] private List<UnitBaseScriptableObject> enemies = new();
}
