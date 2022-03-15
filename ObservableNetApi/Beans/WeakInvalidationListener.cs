namespace ObservableNetApi.Beans;

public class WeakInvalidationListener : IInvalidationListener, IWeakListener
{
    private readonly WeakReference _ref;

    public WeakInvalidationListener(IInvalidationListener listener)
    {
        _ref = new WeakReference(listener);
    }

    public void Invalidated(IObservable observable)
    {
        var listener = (IInvalidationListener?)_ref.Target;
        if (listener != null)
            listener.Invalidated(observable);
        else
            observable.RemoveListener(this);
    }

    public bool WasGarbageCollected()
    {
        return !_ref.IsAlive;
    }
}