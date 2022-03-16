namespace ObservableNetApi.Beans.Value;

/// <summary>
///     A <c>IChangeListener</c> is notified whenever the value of an <see cref="IObservableValue{T}" /> changes.
/// </summary>
/// <remarks>
///     It can be registered and unregistered with
///     <see cref="IObservableValue{T}.AddListener(IChangeListener{T})">IObservableValue.AddListener</see> and
///     <see cref="IObservableValue{T}.RemoveListener(IChangeListener{T})">IObservableValue.RemoveListener</see>
///     respectively.
///     <para>
///         For an in-depth explanation of change events and how they differ from invalidation events, see the
///         documentation of
///         <c>IObservableValue</c>.
///     </para>
///     The same instance of <c>IChangeListener</c> can be registered to listen to multiple <c>ObservableValues</c>.
/// </remarks>
/// <typeparam name="T">The type of the value for which the <c>IChangeListener</c> listens to.</typeparam>
public interface IChangeListener<in T>
{
    /// <summary>
    ///     This method needs to be provided by an implementation of <c>IChangeListener</c>. It is called if the value of an
    ///     <see cref="IObservableValue{T}" /> changes.
    /// </summary>
    /// <remarks>
    ///     In general, it is considered bad practice to modify the observed value in this method.
    /// </remarks>
    /// <param name="observable">The <c>IObservableValue</c> which value changed</param>
    /// <param name="oldValue">The old value</param>
    /// <param name="newValue">The new value</param>
    public void Changed(IObservableValue<T> observable, T oldValue, T newValue);
}