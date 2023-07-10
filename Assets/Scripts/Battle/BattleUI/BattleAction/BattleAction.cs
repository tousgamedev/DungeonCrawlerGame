using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BattleAction : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI actionText;
    
    private SkillScriptableObject action;
    private Button actionButton;

    private void Awake()
    {
        if (!TryGetComponent(out actionButton))
        {
            LogHelper.Report("Battle Action button not found!", LogType.Error, LogGroup.Battle);
        }
    }

    public void InitializeAction(SkillScriptableObject skillObject)
    {
        action = skillObject;
        actionText.text = skillObject.SkillName;
        DisableAction();
    }

    public void EnableAction()
    {
        actionButton.interactable = true;
    }

    public void DisableAction()
    {
        actionButton.interactable = false;
    }
    
    public void SelectAction()
    {
        action.PerformActionSelection();
        LogHelper.DebugLog("Action Selected");
    }
}
