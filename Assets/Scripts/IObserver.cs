public interface IObserver
{
    public void Alert();
    public void RegisterObserver();
    public void DeregisterObserver();
    public string GetName();
}