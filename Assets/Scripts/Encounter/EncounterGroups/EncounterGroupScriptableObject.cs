using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EncounterGroup", menuName = "Battle/Encounter Group")]
public class EncounterGroupScriptableObject : ScriptableObject
{
    public List<EnemyScriptableObject> Enemies => enemies;
    [SerializeField] private List<EnemyScriptableObject> enemies = new();
}
