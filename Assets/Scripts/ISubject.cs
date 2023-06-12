using System.Collections.Generic;

public interface ISubject
{
    public HashSet<IObserver> Observers { get; set; }
    public void AlertObservers();
    public void RegisterObserver(IObserver observer);
    public void DeregisterObserver(IObserver observer);
}
