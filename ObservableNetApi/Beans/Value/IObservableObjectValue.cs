namespace ObservableNetApi.Beans.Value;

/// <summary>
/// An observable typed <c>object</c> value.
/// </summary>
/// <typeparam name="T">The type of the wrapped value</typeparam>
/// <seealso cref="IObservableValue{T}"/>
public interface IObservableObjectValue<T> : IObservableValue<T>
{
    /// <summary>
    /// Returns the current value of this <c>ObservableObjectValue</c>
    /// </summary>
    /// <returns>
    /// The current value
    /// </returns>
    T Get();
}