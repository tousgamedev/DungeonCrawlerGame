using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "Unit/New Character")]
public class UnitBaseScriptableObject : ScriptableObject
{
    public string Name => unitName;
    public Sprite BattleSprite => battleSprite;
    public Sprite TurnBarSprite => turnBarSprite;
    public float Speed => speed;
    public float MaxStartProgress => maxStartProgress;
    public List<SkillScriptableObject> SkillList => skillList;
    
    [SerializeField] private string unitName;
    [SerializeField] private Sprite battleSprite;
    [SerializeField] private Sprite turnBarSprite;
    [SerializeField] private float speed = 60f;
    [SerializeField] [Range(0,1f)] private float maxStartProgress = .3f;
    [SerializeField] private List<SkillScriptableObject> skillList;
}