using System;
using NUnit.Framework;
using ObservableNetApi.Beans;
using ObservableNetApi.Beans.Value;

namespace TestObservableNetApi.Beans;

[TestFixture]
public class WeakInvalidationListenerTest
{
    private WeakInvalidationListener? _weakListener;

    private readonly ObservableMock _o = new();

    [Test]
    public void TestHandle()
    {
        TestHandlerStart();

        // GC-ed call
        GC.Collect(0);
        Assert.True(_weakListener!.WasGarbageCollected());
        _weakListener.Invalidated(_o);
        Assert.AreEqual(1, _o.Counter);
    }

    private void TestHandlerStart()
    {
        var listener = new InvalidationListenerMock();
        _weakListener = new WeakInvalidationListener(listener);

        // regular call
        _o.Reset();
        _weakListener.Invalidated(_o);
        listener.Check(_o, 1);
        Assert.False(_weakListener.WasGarbageCollected());


        // ReSharper disable once RedundantAssignment
        listener = null;
    }

    private class ObservableMock : IObservableValue<string?>
    {
        public uint Counter;

        public void RemoveListener(IInvalidationListener listener)
        {
            Counter++;
        }

        public void AddListener(IInvalidationListener listener)
        {
            // not used
        }

        public bool IsListenerAlreadyAdded(IInvalidationListener listener)
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