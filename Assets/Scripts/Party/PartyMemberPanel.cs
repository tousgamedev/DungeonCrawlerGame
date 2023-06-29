using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberPanel : MonoBehaviour
{
    private readonly int triggerPop = Animator.StringToHash("Pop");
    private readonly int triggerStow = Animator.StringToHash("Stow");
    
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private Image hpBar;
    [SerializeField] private TextMeshProUGUI mpText;
    [SerializeField] private Image mpBar;

    private BattleUnit partyMember;
    private Animator animator;

    private void Awake()
    {
        if (!TryGetComponent(out animator))
        {
            LogHelper.Report("Party Member Panel animator missing!", LogGroup.Debug, LogType.Error);
        }
    }

    public void Initialize(BattleUnit unit)
    {
        partyMember = unit;

        nameText.text = partyMember.Name;
        hpText.text = SetHPText(1000);
        hpText.text = SetMPText(1000);
    }

    public void PopPanel()
    {
        animator.SetTrigger(triggerPop);
    }
    
    public void StowPanel()
    {
        animator.SetTrigger(triggerStow);
    }
    
    private string SetHPText(int currentHP)
    {
        int maxHP = 1000;
        return $"HP {currentHP}/{maxHP}";
    }
    
    private string SetMPText(int currentMP)
    {
        int maxMP = 1000;
        return $"HP {currentMP}/{maxMP}";
    }
}
