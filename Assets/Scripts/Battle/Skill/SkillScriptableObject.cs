using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "Battle/Skill")]
public class SkillScriptableObject : ScriptableObject
{
    public string Name => skillName;
    public GameObject EffectsPrefab => effectPrefab;
    public float BaseExecutionSpeed => baseExecutionSpeed;
    public float BaseAttack => baseAttack;
        
    [SerializeField] private string skillName;
    [SerializeField] private GameObject effectPrefab;
    [SerializeField] private float baseExecutionSpeed;
    [SerializeField] private float baseAttack;
}