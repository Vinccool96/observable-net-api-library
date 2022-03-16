using ObservableNetApi.Beans;
using ObservableNetApi.Beans.Value;

namespace TestObservableNetApi.Beans.Value;

public class WeakChangeListenerMock<T> : IChangeListener<T>, IWeakListener
{
    public void Changed(IObservableValue<T> observable, T oldValue, T newValue)
    {
    }

    public bool WasGarbageCollected()
    {
        return true;
    }
}