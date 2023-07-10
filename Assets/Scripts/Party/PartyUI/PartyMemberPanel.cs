using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberPanel : MonoBehaviour
{
    private const string PopAnimationName = "PanelShow";
    private const string StowAnimationName = "PanelHide";

    public float PopAnimationLength { get; private set; }
    public float StowAnimationLength { get; private set; }

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private Image hpBar;
    [SerializeField] private TextMeshProUGUI mpText;
    [SerializeField] private Image mpBar;
    [SerializeField] private BattleActionController actionController;
    
    private BattleUnit partyMember;
    private Animator animator;

    private readonly int triggerPop = Animator.StringToHash("Show");
    private readonly int triggerHide = Animator.StringToHash("Hide");

    private void Awake()
    {
        if (!TryGetComponent(out animator))
        {
            LogHelper.Report("Party Member Panel animator missing!", LogType.Error, LogGroup.Battle);
        }

        if (actionController == null)
        {
            actionController = GetComponentInChildren<BattleActionController>();
            if (actionController == null)
            {
                LogHelper.Report("BattleActionController missing!", LogType.Error, LogGroup.Battle);
            }
        }
        
        PopAnimationLength = Utilities.GetAnimationLength(animator, PopAnimationName);
        StowAnimationLength = Utilities.GetAnimationLength(animator, StowAnimationName);
    }

    public void Initialize(BattleUnit unit)
    {
        partyMember = unit;

        nameText.text = partyMember.Name;
        SetHealthText(unit.Stats.CurrentHealth, unit.Stats.MaxHealth);
        unit.OnHealthChange += ChangeCurrentHealth;
        SetMagicPointsText(500, 500);
        actionController.InitializeActions(unit.Actions.ActionList);
    }

    public void ShowActionList()
    {
        actionController.EnableActions();
        animator.SetTrigger(triggerPop);
    }

    public void HideActionList()
    {
        actionController.DisableActions();
        animator.SetTrigger(triggerHide);
    }

    private void SetHealthText(int currentHp, int maxHp)
    {
        hpText.text = $"HP {currentHp}/{maxHp}";
    }

    private void SetMagicPointsText(int currentMp, int maxMp)
    {
        mpText.text = $"MP {currentMp}/{maxMp}";
    }

    private void ChangeCurrentHealth(BattleUnit unit)
    {
        SetHealthText(unit.Stats.CurrentHealth,unit.Stats.MaxHealth);
    }
}