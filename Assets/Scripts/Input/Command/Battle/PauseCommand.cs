public class PauseCommand : ICommand
{
    public void Execute()
    {
        BattleManager.Instance.TogglePause();
    }
}