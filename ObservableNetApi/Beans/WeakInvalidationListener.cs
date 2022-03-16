namespace ObservableNetApi.Beans;

/// <summary>
///     A <c>WeakInvalidationListener</c> can be used, if an <see cref="IObservable" /> should only maintain a weak
///     reference to the listener.
/// </summary>
/// <remarks>
///     This helps to avoid memory leaks that can occur if observers are not unregistered from observed objects after
///     use.
///     <para>
///         <c>WeakInvalidationListener</c> are created by passing in the original <see cref="IInvalidationListener" />.
///         The <c>WeakInvalidationListener</c> should then be registered to listen for changes of the observed object.
///     </para>
///     Note: You have to keep a reference to the <c>IInvalidationListener</c> that was passed in as long as it is in
///     use, otherwise it will be garbage collected too soon.
/// </remarks>
public class WeakInvalidationListener : IInvalidationListener, IWeakListener
{
    private readonly WeakReference _ref;

    /// <summary>
    ///     The constructor of <c>WeakInvalidationListener</c>.
    /// </summary>
    /// <param name="listener">The original listener that should be notified</param>
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