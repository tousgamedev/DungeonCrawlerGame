using UnityEngine;

public class BattleUIController : MonoBehaviour
{
    [SerializeField] private TurnGaugeController turnGauge;
    [SerializeField] private BattlefieldController battlefield;

    private void Awake()
    {
        if (turnGauge == null)
        {
            LogHelper.Report("Turn Gauge is null!", LogType.Error, LogGroup.System);
        }
        
        if (battlefield == null)
        {
            LogHelper.Report("Battlefield is null!", LogType.Error, LogGroup.System);
        }
    }

    private void OnEnable()
    {
        BattleEvents.OnBattleUIInit += EnableBattleUI;
        BattleEvents.OnBattleEnd += DisableBattleUI;
    }

    private void EnableBattleUI()
    {
        turnGauge.gameObject.SetActive(true);
        battlefield.gameObject.SetActive(true);
    }

    private void DisableBattleUI(BattleUnit unit)
    {
        turnGauge.gameObject.SetActive(false);
        battlefield.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        BattleEvents.OnBattleStart -= EnableBattleUI;
        BattleEvents.OnBattleEnd -= DisableBattleUI;
    }
}