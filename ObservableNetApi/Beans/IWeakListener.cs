namespace ObservableNetApi.Beans;

/// <summary>
///     <c>IWeakListener</c> is the super interface of all weak listener implementations of the API runtime.
/// </summary>
/// <remarks>
///     Usually it should not be used directly, but instead one of the sub-interfaces will be used.
/// </remarks>
public interface IWeakListener
{
    /// <summary>
    ///     Returns `true` if the linked listener was garbage-collected.
    /// </summary>
    /// <remarks>
    ///     In this case, the listener can be removed from the observable.
    /// </remarks>
    /// <returns>`true` if the linked listener was garbage-collected.</returns>
    public bool WasGarbageCollected();
}