using ObservableNetApi.Beans;

namespace TestObservableNetApi.Beans;

public class WeakInvalidationListenerMock : IInvalidationListener, IWeakListener
{
    public void Invalidated(IObservable observable)
    {
        // no-op
    }

    public bool WasGarbageCollected()
    {
        return true;
    }
}