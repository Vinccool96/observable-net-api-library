using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using ObservableNetApi.Beans.Value;

namespace TestObservableNetApi.Beans.Value;

public class ChangeListenerMock<T> : IChangeListener<T>
{
    private static readonly double EPSILON_DOUBLE = 1E-12;

    private static readonly float EPSILON_FLOAT = 1E-6f;

    private readonly T _undefined;

    private uint _counter;

    private T _newValue;

    private T _oldValue;

    private IObservableValue<T>? _valueModel;

    public ChangeListenerMock(T undefined)
    {
        _undefined = undefined;
        _oldValue = _undefined;
        _newValue = _undefined;
    }

    public void Changed(IObservableValue<T> observable, T oldValue, T newValue)
    {
        _valueModel = observable;
        _oldValue = oldValue;
        _newValue = newValue;
        _counter++;
    }

    public void Reset()
    {
        _valueModel = null;
        _oldValue = _undefined;
        _newValue = _undefined;
        _counter = 0;
    }

    [SuppressMessage("ReSharper", "ConvertIfStatementToSwitchStatement")]
    public void Check(IObservableValue<T>? observable, T oldValue, T newValue, uint counter)
    {
        Assert.AreEqual(observable, _valueModel);
        if (oldValue is double oldValueD && _oldValue is double currentOldValueD)
            Assert.AreEqual(oldValueD, currentOldValueD, EPSILON_DOUBLE);
        else if (oldValue is float oldValueF && _oldValue is float currentOldValueF)
            Assert.AreEqual(oldValueF, currentOldValueF, EPSILON_FLOAT);
        else
            Assert.AreEqual(oldValue, _oldValue);

        if (oldValue is double newValueD && _oldValue is double currentNewValueD)
            Assert.AreEqual(newValueD, currentNewValueD, EPSILON_DOUBLE);
        else if (oldValue is float newValueF && _oldValue is float currentNewValueF)
            Assert.AreEqual(newValueF, currentNewValueF, EPSILON_FLOAT);
        else
            Assert.AreEqual(newValue, _newValue);

        Assert.AreEqual(counter, _counter);
        Reset();
    }
}