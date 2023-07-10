using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class LogHelper : ManagerBase<LogHelper>
{
    [SerializeField] private bool showSystemLogs = true;
    [SerializeField] private bool showDebugLogs = true;
    [SerializeField] private bool showAILogs = true;
    [SerializeField] private bool showAudioLogs = true;
    [SerializeField] private bool showBattleLogs = true;
    [SerializeField] private bool showPlayerLogs = true;
    [SerializeField] private bool showTravelLogs = true;

#pragma warning disable CS0108, CS0114
    private void Awake()
#pragma warning restore CS0108, CS0114
    {
        base.Awake();
    }

    public static void Report(string message, LogType type, LogGroup group)
    {
        if (CanShowGroup(group))
        {
            WriteToConsole(message, type);
        }
    }
    
    public static void DebugLog(string message, LogType type = LogType.Log)
    {
        Report(message, type, LogGroup.Debug);
    }
    
    private static bool CanShowGroup(LogGroup group)
    {
        return group switch
        {
            LogGroup.Debug => Instance.showDebugLogs,
            LogGroup.System => Instance.showSystemLogs,
            LogGroup.Battle => Instance.showBattleLogs,
            LogGroup.Travel => Instance.showTravelLogs,
            LogGroup.Player => Instance.showPlayerLogs,
            LogGroup.AI => Instance.showAILogs,
            LogGroup.Audio => Instance.showAudioLogs,
            _ => false
        };
    }

    private static void WriteToConsole(string message, LogType type)
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
