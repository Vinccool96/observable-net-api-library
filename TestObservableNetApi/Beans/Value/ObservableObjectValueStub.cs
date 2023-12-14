using ObservableNetApi.Beans.Value;

namespace TestObservableNetApi.Beans.Value;

public class ObservableObjectValueStub<T>(T initialValue) : ObservableValueBase<T>, IObservableObjectValue<T>
{
    private T _value = initialValue;

    public void Set(T value)
    {
        _value = value;
    }

    public T Get()
    {
        return _value;
    }

    public override T GetValue()
    {
        return Get();
    }

    public void FireChange()
    {
        FireValueChangedEvent();
    }
}