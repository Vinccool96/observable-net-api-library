using ObservableNetApi.Internal.Binding;

namespace ObservableNetApi.Beans.Value;

/// <summary>
/// A convenience class for creating implementations of <see cref="IObservableValue{T}"/>. It contains all the infrastructure
/// support for value invalidation- and change event notification.
/// </summary>
/// <remarks>
/// This implementation can handle adding and removing listeners while the observers are being notified, but it is not
/// thread-safe.
/// </remarks>
/// <typeparam name="T">The type of the wrapped value.</typeparam>
public abstract class ObservableValueBase<T> : IObservableValue<T>
{
    private ExpressionHelper<T>? _helper;

    public void AddListener(IInvalidationListener listener)
    {
        if (!HasListener(listener))
        {
            _helper = ExpressionHelper<T>.AddListener(_helper, this, listener);
        }
    }

    public void RemoveListener(IInvalidationListener listener)
    {
        if (HasListener(listener))
        {
            _helper = ExpressionHelper<T>.RemoveListener(_helper, listener);
        }
    }

    public bool HasListener(IInvalidationListener listener)
    {
        return _helper?.GetIInvalidationListeners().Contains(listener) ?? false;
    }

    public void AddListener(IChangeListener<T> listener)
    {
        if (!HasListener(listener))
        {
            _helper = ExpressionHelper<T>.AddListener(_helper, this, listener);
        }
    }

    public void RemoveListener(IChangeListener<T> listener)
    {
        if (HasListener(listener))
        {
            _helper = ExpressionHelper<T>.RemoveListener(_helper, listener);
        }
    }

    public bool HasListener(IChangeListener<T> listener)
    {
        return _helper?.GetIChangeListeners().Contains(listener) ?? false;
    }

    public abstract T GetValue();

    /// <summary>
    /// Notify the currently registered observers of a value change.
    /// </summary>
    /// <remarks>
    /// This implementation will ignore all adds and removes of observers that are done while a notification is processed.
    /// The changes take effect in the following call to fireValueChangedEvent.
    /// </remarks>
    protected void FireValueChangedEvent()
    {
        ExpressionHelper<T>.FireValueChangedEvent(_helper);
    }
}