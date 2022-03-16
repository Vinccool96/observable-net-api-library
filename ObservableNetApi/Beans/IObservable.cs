namespace ObservableNetApi.Beans;

/// <summary>
///     An <c>IObservable</c> is an entity that wraps content and allows to observe the content for invalidations.
/// </summary>
/// <remarks>
///     An implementation of <c>IObservable</c> may support lazy evaluation, which means that the content is not
///     immediately
///     recomputed after changes, but lazily the next time it is requested. All binding and properties in this library
///     support lazy evaluation.
///     <para>
///         Implementations of this class should strive to generate as few events as possible to avoid wasting too much
///         time in
///         event handlers. Implementations in this library mark themselves as invalid when the first invalidation event
///         occurs.
///         They do not generate any more invalidation events until their value is recomputed and valid again.
///     </para>
/// </remarks>
/// <seealso cref="IInvalidationListener" />
public interface IObservable
{
    /// <summary>
    ///     Adds an <see cref="IInvalidationListener" /> which will be notified whenever the <c>IObservable</c> becomes
    ///     invalid.
    /// </summary>
    /// <remarks>
    ///     If the same listener is added more than once, then it will be notified more than once. That is, no check is
    ///     made to ensure uniqueness.
    ///     <para>
    ///         Note that the same actual <c>IInvalidationListener</c> instance may be safely registered for different
    ///         <c>IObservable</c>.
    ///     </para>
    ///     The <c>IObservable</c> stores a strong reference to the listener which will prevent the listener from being
    ///     garbage collected and may result in a memory leak. It is recommended to either unregister a listener by
    ///     calling <see cref="RemoveListener" /> after use or to use an instance of
    ///     <see cref="WeakInvalidationListener" /> to avoid this situation.
    /// </remarks>
    /// <param name="listener">The listener to register</param>
    /// <seealso cref="RemoveListener" />
    public void AddListener(IInvalidationListener listener);

    /// <summary>
    ///     Removes the given listener from the collections of listeners, that are notified whenever the value of the
    ///     <c>IObservable</c> becomes invalid.
    /// </summary>
    /// <remarks>
    ///     If the given listener has not been previously registered (i.e. it was never added) then this method call is
    ///     a no-op. If it had been previously added then it will be removed. If it had been added more than once, then
    ///     only the first occurrence will be removed.
    /// </remarks>
    /// <param name="listener">The listener to remove</param>
    /// <seealso cref="AddListener" />
    public void RemoveListener(IInvalidationListener listener);

    /// <summary>
    ///     Verify if the specified <c>IInvalidationListener</c> already exists for this <c>IObservable</c>.
    /// </summary>
    /// <param name="listener">The <c>IInvalidationListener</c> to verify</param>
    /// <returns><c>true</c>, if the listener already listens, <c>false</c> otherwise.</returns>
    public bool IsListenerAlreadyAdded(IInvalidationListener listener);
}