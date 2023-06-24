using UnityEngine;

public class Logger : MonoBehaviour
{
    private static Logger instance;

    [SerializeField] private bool showSystemLogs = true;
    [SerializeField] private bool showDebugLogs = true;
    [SerializeField] private bool showAILogs = true;
    [SerializeField] private bool showAudioLogs = true;
    [SerializeField] private bool showBattleLogs = true;
    [SerializeField] private bool showPlayerLogs = true;
    [SerializeField] private bool showTravelLogs = true;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null && instance != this)
        {
            Report("Duplicate Logger destroyed!", LogGroup.System, LogType.Warning);
            Destroy(this);
        }

        DontDestroyOnLoad(this);
    }

    public static void Report(string message, LogGroup group = LogGroup.Debug, LogType type = LogType.Log)
    {
        if (CanShowGroup(group))
        {
            ShowLog(message, type);
        }
    }

    public static bool CanShowGroup(LogGroup group)
    {
        switch (group)
        {
            case LogGroup.Debug:
                return instance.showDebugLogs;
            case LogGroup.System:
                return instance.showSystemLogs;
            case LogGroup.Battle:
                return instance.showBattleLogs;
            case LogGroup.Travel:
                return instance.showTravelLogs;
            case LogGroup.Player:
                return instance.showPlayerLogs;
            case LogGroup.AI:
                return instance.showAILogs;
            case LogGroup.Audio:
                return instance.showAudioLogs;
            default:
                return false;
        }
    }

    public static void ShowLog(string message, LogType type)
    {
        switch (type)
        {
            case LogType.Log:
                Debug.Log(message);
                break;
            case LogType.Warning:
                Debug.LogWarning(message);
                break;
            case LogType.Error:
                Debug.LogError(message);
                break;
        }
    }
}
