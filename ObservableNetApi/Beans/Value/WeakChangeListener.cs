namespace ObservableNetApi.Beans.Value;

/// <summary>
///     A <c>WeakChangeListener</c> can be used, if an <see cref="IObservableValue{T}" /> should only maintain a weak
///     reference to the listener.
/// </summary>
/// <remarks>
///     This helps to avoid memory leaks, that can occur if observers are not unregistered from observed objects after use.
///     <para>
///         <c>WeakChangeListener</c> are created by passing in the original <see cref="IChangeListener{T}" />. The
///         <c>WeakChangeListener</c> should then be registered to listen for changes of the observed object.
///     </para>
///     Note: You have to keep a reference to the <c>IChangeListener</c>, that was passed in as long as it is in use,
///     otherwise it will be garbage collected too soon.
/// </remarks>
/// <typeparam name="T">The type of the observed value</typeparam>
public class WeakChangeListener<T> : IChangeListener<T>, IWeakListener
{
    private readonly WeakReference _ref;

    /// <summary>
    ///     The constructor of <c>WeakChangeListener</c>
    /// </summary>
    /// <param name="listener">The original listener that should be notified</param>
    public WeakChangeListener(IChangeListener<T> listener)
    {
        _ref = new WeakReference(listener);
    }

    public void Changed(IObservableValue<T> observable, T oldValue, T newValue)
    {
        var listener = (IChangeListener<T>?)_ref.Target;
        if (listener != null)
            listener.Changed(observable, oldValue, newValue);
        else
            observable.RemoveListener(this);
    }

    public bool WasGarbageCollected()
    {
        return !_ref.IsAlive;
    }
}