using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberPanel : MonoBehaviour
{
    private readonly int triggerShow = Animator.StringToHash("Show");
    private readonly int triggerHide = Animator.StringToHash("Hide");
    private readonly int triggerSelected = Animator.StringToHash("ActionSelected");

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private Image hpBar;
    [SerializeField] private TextMeshProUGUI mpText;
    [SerializeField] private Image mpBar;
    [SerializeField] private BattleActionController actionController;

    private BattleUnit unit;
    private Animator animator;

    private void Awake()
    {
        if (!TryGetComponent(out animator))
        {
            LogHelper.Report("Party Member Panel animator missing!", LogType.Error, LogGroup.Battle);
        }

        if (actionController != null)
            return;

        actionController = GetComponentInChildren<BattleActionController>();
        if (actionController == null)
        {
            LogHelper.Report("BattleActionController missing!", LogType.Error, LogGroup.Battle);
        }
    }

    private void OnEnable()
    {
        BattleEvents.OnTurnReady += ShowActionList;
        BattleEvents.OnActionSelected += ShowSelectedAction;
        BattleEvents.OnHealthChange += ChangeCurrentHealth;
        BattleEvents.OnActionComplete += HideActionList;
        BattleEvents.OnBattleEnd += HideActionList;
    }

    public void Initialize(BattleUnit battleUnit)
    {
        unit = battleUnit;

        nameText.text = unit.Name;
        SetHealthText(unit.Stats.CurrentHealth, unit.Stats.MaxHealth);
        SetMagicPointsText(500, 500);
        actionController.InitializeActions(unit);
    }

    private void ShowActionList(BattleUnit battleUnit)
    {
        if (battleUnit != unit)
            return;

        animator.SetBool(triggerShow, true);
        animator.SetBool(triggerSelected, false);
        animator.SetBool(triggerHide, false);
    }

    private void ShowSelectedAction(BattleUnit battleUnit, UnitActionScriptableObject action)
    {
        if (battleUnit != unit)
            return;
        
        animator.SetBool(triggerShow, false);
        animator.SetBool(triggerSelected, true);
        animator.SetBool(triggerHide, false);
    }

    private void HideActionList(BattleUnit battleUnit)
    {
        if (battleUnit != unit)
            return;
        
        animator.SetBool(triggerShow, false);
        animator.SetBool(triggerSelected, false);
        animator.SetBool(triggerHide, true);
    }

    private void SetHealthText(int currentHealth, int maxHealth)
    {
        hpText.text = $"HP {currentHealth}/{maxHealth}";
    }

    private void SetMagicPointsText(int currentMagicPoints, int maxMagicPoints)
    {
        mpText.text = $"MP {currentMagicPoints}/{maxMagicPoints}";
    }

    private void ChangeCurrentHealth(BattleUnit battleUnit)
    {
        if (battleUnit != unit)
            return;

        SetHealthText(unit.Stats.CurrentHealth, unit.Stats.MaxHealth);
    }

    public void RemovePanel()
    {

    }

    private void OnDisable()
    {
        BattleEvents.OnTurnReady -= ShowActionList;
        BattleEvents.OnActionSelected -= ShowSelectedAction;
        BattleEvents.OnHealthChange -= ChangeCurrentHealth;
        BattleEvents.OnActionComplete -= HideActionList;
        BattleEvents.OnBattleEnd -= HideActionList;
    }
}