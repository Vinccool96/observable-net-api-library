namespace ObservableNetApi.Beans;

/// <summary>
///     An <c>IInvalidationListener</c> is notified whenever an [Observable] becomes invalid.
/// </summary>
/// <remarks>
///     It can be registered and unregistered with [Observable.addListener] and [Observable.removeListener] respectively.
///     <para>
///         For an in-depth explanation of invalidation events and how they differ from change events, see the
///         documentation of
///         `ObservableValue`.
///     </para>
///     The same instance of <c>IInvalidationListener</c> can be registered to listen to multiple `Observables`.
/// </remarks>
public interface IInvalidationListener
{
    /// <summary>
    ///     This method needs to be provided by an implementation of <c>IInvalidationListener</c>.
    /// </summary>
    /// <remarks>
    ///     It is called if an [Observable] becomes invalid.
    ///     <para>
    ///         In general, it is considered bad practice to modify the observed value in this method.
    ///     </para>
    /// </remarks>
    /// <param name="observable">The <c>IObservable</c> that became invalid</param>
    public void Invalidated(IObservable observable);
}