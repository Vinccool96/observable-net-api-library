using System;
using NUnit.Framework;
using ObservableNetApi.Beans;
using ObservableNetApi.Beans.Value;

namespace TestObservableNetApi.Beans.Value;

[TestFixture]
public class WeakChangeListenerTest
{
    private WeakChangeListener<object?>? _weakListener;

    private readonly ObservableMock _o = new();

    private readonly object _obj1 = new();

    private readonly object _obj2 = new();

    [Test]
    public void TestHandle()
    {
        TestHandlerStart();

        // GC-ed call
        _o.Reset();
        GC.Collect();
        Assert.True(_weakListener!.WasGarbageCollected());
        _weakListener.Changed(_o, _obj2, _obj1);
        Assert.AreEqual(1, _o.Counter);
    }

    private void TestHandlerStart()
    {
        var listener = new ChangeListenerMock<object?>(new object());
        _weakListener = new WeakChangeListener<object?>(listener);

        // regular call
        _weakListener.Changed(_o, _obj1, _obj2);
        listener.Check(_o, _obj1, _obj2, 1);
        Assert.False(_weakListener.WasGarbageCollected());
        Assert.AreEqual(0, _o.Counter);

        // ReSharper disable once RedundantAssignment
        listener = null;
    }

    private class ObservableMock : IObservableValue<string?>
    {
        public uint Counter;

        public void RemoveListener(IChangeListener<string?> listener)
        {
            Counter++;
        }

        public string? GetValue()
        {
            // not used
            return null;
        }

        public void AddListener(IInvalidationListener listener)
        {
            // not used
        }

        public void RemoveListener(IInvalidationListener listener)
        {
            // not used
        }

        public bool HasListener(IInvalidationListener listener)
        {
            // not used
            return false;
        }

        public void AddListener(IChangeListener<string?> listener)
        {
            // not used
        }

        public bool HasListener(IChangeListener<string?> listener)
        {
            // not used
            return false;
        }

        public void Reset()
        {
            Counter = 0;
        }
    }
}