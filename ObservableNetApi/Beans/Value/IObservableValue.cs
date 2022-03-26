namespace ObservableNetApi.Beans.Value;

/// <summary>
///     An <c>IObservableValue</c> is an entity that wraps a value and allows to observe the value for changes.
/// </summary>
/// <remarks>
///     In general this interface should not be implemented directly but one of its sub-interfaces
///     (<c>ObservableBooleanValue</c>, etc.).
///     <para>
///         The value of the <c>IObservableValue</c> can be requested with [value].
///     </para>
///     An implementation of <c>IObservableValue</c> may support lazy evaluation, which means that the value is not
///     immediately recomputed after changes, but lazily the next time the value is requested. All binding and properties
///     in
///     this library support lazy evaluation.
///     <para>
///         An <c>IObservableValue</c> generates two types of events: change events and invalidation events. A change event
///         indicates that the value has changed. An invalidation event is generated, if the current value is not valid
///         anymore.
///         This distinction becomes important, if the <c>IObservableValue</c> supports lazy evaluation, because for a
///         lazily
///         evaluated value one does not know if an invalid value really has changed until it is recomputed. For this
///         reason,
///         generating change events requires eager evaluation while invalidation events can be generated for eager and
///         lazy
///         implementations.
///     </para>
///     Implementations of this class should strive to generate as few events as possible to avoid wasting too much time in
///     event handlers. Implementations in this library mark themselves as invalid when the first invalidation event
///     occurs.
///     They do not generate any more invalidation events until their value is recomputed and valid again.
///     <para>
///         Two types of listeners can be attached to an <c>IObservableValue</c>:
///         <see cref="ObservableNetApi.Beans.IInvalidationListener">InvalidationListener</see> to listen to invalidation
///         events
///         and <see cref="IChangeListener{T}" /> to listen to change events.
///     </para>
///     Important note: attaching a <c>IChangeListener</c> enforces eager computation even if the implementation of the
///     <c>IObservableValue</c> supports lazy evaluation.
/// </remarks>
/// <typeparam name="T">The type of the wrapped value.</typeparam>
public interface IObservableValue<out T> : IObservable
{
    public T GetValue();

    /// <summary>
    ///     Adds a <see cref="IChangeListener{T}" /> which will be notified whenever the value of the <c>IObservableValue</c>
    ///     changes.
    /// </summary>
    /// <remarks>
    ///     If the same
    ///     listener is added more than once, then it will be notified more than once. That is, no check is made to ensure
    ///     uniqueness.
    ///     <para>
    ///         Note that the same actual <c>IChangeListener</c> instance may be safely registered for different
    ///         <c>IObservableValue</c>.
    ///     </para>
    ///     The <c>IObservableValue</c> stores a strong reference to the listener which will prevent the listener from being
    ///     garbage collected and may result in a memory leak. It is recommended to either unregister a listener by calling
    ///     [removeListener] after use or to use an instance of [WeakChangeListener] avoid this situation.
    /// </remarks>
    /// <param name="listener">The listener to register</param>
    /// <seealso cref="RemoveListener" />
    public void AddListener(IChangeListener<T> listener);

    /// <summary>
    ///     Removes the given listener from the list of listeners, that are notified whenever the value of the
    ///     <c>IObservableValue</c> changes.
    /// </summary>
    /// <remarks>
    ///     If the given listener has not been previously registered (i.e. it was never added) then this method call is a
    ///     no-op. If it had been previously added then it will be removed. If it had been added more than once, then only
    ///     the first occurrence will be removed.
    /// </remarks>
    /// <param name="listener">The listener to remove</param>
    /// <seealso cref="AddListener" />
    public void RemoveListener(IChangeListener<T> listener);

    /// <summary>
    ///     Verify if the specified <c>IChangeListener</c> already exists for this <c>IObservableValue</c>.
    /// </summary>
    /// <param name="listener">The <c>IChangeListener</c> to verify</param>
    /// <returns><c>true</c>, if the listener already listens, <c>false</c> otherwise.</returns>
    public bool HasListener(IChangeListener<T> listener);
}