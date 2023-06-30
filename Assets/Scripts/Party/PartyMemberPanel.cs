using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberPanel : MonoBehaviour
{
    private const string PopAnimationName = "PanelPopUp";
    private const string StowAnimationName = "PanelStow";

    public float PopAnimationLength { get; private set; }
    public float StowAnimationLength { get; private set; }

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private Image hpBar;
    [SerializeField] private TextMeshProUGUI mpText;
    [SerializeField] private Image mpBar;

    private BattleUnit partyMember;
    private Animator animator;

    private readonly int triggerPop = Animator.StringToHash("Pop");
    private readonly int triggerStow = Animator.StringToHash("Stow");

    private void Awake()
    {
        if (!TryGetComponent(out animator))
        {
            LogHelper.Report("Party Member Panel animator missing!", LogGroup.Debug, LogType.Error);
        }

        PopAnimationLength = Utilities.GetAnimationLength(animator, PopAnimationName);
        StowAnimationLength = Utilities.GetAnimationLength(animator, StowAnimationName);
    }

    public void Initialize(BattleUnit unit)
    {
        partyMember = unit;

        nameText.text = partyMember.Name;
        SetHpText(1000, 1000);
        SetMpText(500, 500);
    }

    public void PopPanel()
    {
        animator.SetTrigger(triggerPop);
    }

    public void StowPanel()
    {
        animator.SetTrigger(triggerStow);
    }

    private void SetHpText(int currentHp, int maxHp)
    {
        hpText.text = $"HP {currentHp}/{maxHp}";
    }

    private void SetMpText(int currentMp, int maxMp)
    {
        mpText.text = $"MP {currentMp}/{maxMp}";
    }
}