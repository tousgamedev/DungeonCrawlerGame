using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EncounterGroup", menuName = "Battle/Encounter Group")]
public class EncounterGroupScriptableObject : ScriptableObject
{
    public List<string> Enemies => enemies;
    [SerializeField] private List<string> enemies = new();
}
