using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberPanel : MonoBehaviour
{
    private readonly int triggerPop = Animator.StringToHash("Show");
    private readonly int triggerHide = Animator.StringToHash("Hide");
    private readonly int triggerSelected = Animator.StringToHash("ActionSelected");

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private Image hpBar;
    [SerializeField] private TextMeshProUGUI mpText;
    [SerializeField] private Image mpBar;
    [SerializeField] private BattleActionController actionController;
    
    private BattleUnit partyMember;
    private Animator animator;

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

    public void ShowSelectedAction(UnitActionScriptableObject action)
    {
        actionController.HideAllExcept(action);
        animator.SetTrigger(triggerSelected);
    }
    
    public void HideActionList()
    {
        actionController.DisableActions();
        animator.SetTrigger(triggerHide);
    }

    private void SetHealthText(int currentHealth, int maxHealth)
    {
        hpText.text = $"HP {currentHealth}/{maxHealth}";
    }

    private void SetMagicPointsText(int currentMagicPoints, int maxMagicPoints)
    {
        mpText.text = $"MP {currentMagicPoints}/{maxMagicPoints}";
    }

    private void ChangeCurrentHealth(BattleUnit unit)
    {
        SetHealthText(unit.Stats.CurrentHealth,unit.Stats.MaxHealth);
    }
}