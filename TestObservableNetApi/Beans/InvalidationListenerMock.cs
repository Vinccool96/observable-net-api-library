using NUnit.Framework;
using ObservableNetApi.Beans;

namespace TestObservableNetApi.Beans;

public class InvalidationListenerMock : IInvalidationListener
{
    private uint _counter;
    private IObservable? _observable;

    public void Invalidated(IObservable observable)
    {
        _observable = observable;
        _counter++;
    }

    public void Reset()
    {
        _observable = null;
        _counter = 0;
    }

    public void Check(IObservable? observable, uint counter)
    {
        Assert.AreEqual(observable, _observable);
        Assert.AreEqual(counter, _counter);
        Reset();
    }
}