using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EncounterGroup", menuName = "Battle/Encounter Group")]
public class EncounterGroupScriptableObject : ScriptableObject
{
    public List<UnitScriptableObject> Enemies => enemies;
    [SerializeField] private List<UnitScriptableObject> enemies = new();
}
