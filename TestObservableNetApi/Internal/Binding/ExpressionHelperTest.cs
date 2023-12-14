using System;
using System.Collections;
using NUnit.Framework;
using ObservableNetApi.Beans;
using ObservableNetApi.Beans.Value;
using ObservableNetApi.Internal.Binding;
using TestObservableNetApi.Beans;
using TestObservableNetApi.Beans.Value;

namespace TestObservableNetApi.Internal.Binding;

[TestFixture]
public class ExpressionHelperTest
{
    private ExpressionHelper<object>? _helper;

    private ObservableObjectValueStub<object> _observable = new(new());

    private InvalidationListenerMock[] _invalidationListeners = [];

    private ChangeListenerMock<object>[] _changeListeners = [];

    private static readonly object Undefined = new();

    private static readonly object Data1 = new();

    private static readonly object Data2 = new();

    [SetUp]
    public void SetUp()
    {
        _helper = null;
        _observable = new ObservableObjectValueStub<object>(Data1);
        _invalidationListeners =
        [
            new InvalidationListenerMock(), new InvalidationListenerMock(), new InvalidationListenerMock(),
            new InvalidationListenerMock()
        ];

        _changeListeners =
        [
            new ChangeListenerMock<object>(Undefined), new ChangeListenerMock<object>(Undefined),
            new ChangeListenerMock<object>(Undefined), new ChangeListenerMock<object>(Undefined)
        ];
    }

    [Test]
    public void TestEmptyHelper()
    {
        // all of these calls should be no-ops
        ExpressionHelper<object>.RemoveListener(_helper, _invalidationListeners[0]);
        ExpressionHelper<object>.RemoveListener(_helper, _changeListeners[0]);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
    }

    [Test]
    public void TestSingeInvalidation()
    {
        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, _invalidationListeners[0]);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _invalidationListeners[0].Check(_observable, 1);

        _helper = ExpressionHelper<object>.RemoveListener(_helper, _invalidationListeners[1]);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _invalidationListeners[0].Check(_observable, 1);
        _invalidationListeners[1].Check(null, 0);

        _helper = ExpressionHelper<object>.RemoveListener(_helper, _changeListeners[1]);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _invalidationListeners[0].Check(_observable, 1);
        _changeListeners[1].Check(null, Undefined, Undefined, 0);

        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, _invalidationListeners[1]);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _invalidationListeners[0].Check(_observable, 1);
        _invalidationListeners[1].Check(_observable, 1);

        _helper = ExpressionHelper<object>.RemoveListener(_helper, _invalidationListeners[1]);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _invalidationListeners[0].Check(_observable, 1);
        _invalidationListeners[1].Check(null, 0);

        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, _changeListeners[1]);
        _observable.Set(Data2);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _invalidationListeners[0].Check(_observable, 1);
        _changeListeners[1].Check(_observable, Data1, Data2, 1);

        _helper = ExpressionHelper<object>.RemoveListener(_helper, _changeListeners[1]);
        _observable.Set(Data1);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _invalidationListeners[0].Check(_observable, 1);
        _changeListeners[1].Check(null, Undefined, Undefined, 0);

        _helper = ExpressionHelper<object>.RemoveListener(_helper, _invalidationListeners[0]);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _invalidationListeners[0].Check(null, 0);
    }

    [Test]
    public void TestSingeChange()
    {
        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, _changeListeners[0]);
        _observable.Set(Data2);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _changeListeners[0].Check(_observable, Data1, Data2, 1);

        _helper = ExpressionHelper<object>.RemoveListener(_helper, _invalidationListeners[1]);
        _observable.Set(Data1);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _changeListeners[0].Check(_observable, Data2, Data1, 1);
        _invalidationListeners[1].Check(null, 0);

        _helper = ExpressionHelper<object>.RemoveListener(_helper, _changeListeners[1]);
        _observable.Set(Data2);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _changeListeners[0].Check(_observable, Data1, Data2, 1);
        _changeListeners[1].Check(null, Undefined, Undefined, 0);

        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, _invalidationListeners[1]);
        _observable.Set(Data1);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _changeListeners[0].Check(_observable, Data2, Data1, 1);
        _invalidationListeners[1].Check(_observable, 1);

        _helper = ExpressionHelper<object>.RemoveListener(_helper, _invalidationListeners[1]);
        _observable.Set(Data2);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _changeListeners[0].Check(_observable, Data1, Data2, 1);
        _invalidationListeners[1].Check(null, 0);

        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, _changeListeners[1]);
        _observable.Set(Data1);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _changeListeners[0].Check(_observable, Data2, Data1, 1);
        _changeListeners[1].Check(_observable, Data2, Data1, 1);

        _helper = ExpressionHelper<object>.RemoveListener(_helper, _changeListeners[1]);
        _observable.Set(Data2);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _changeListeners[0].Check(_observable, Data1, Data2, 1);
        _changeListeners[1].Check(null, Undefined, Undefined, 0);

        _helper = ExpressionHelper<object>.RemoveListener(_helper, _changeListeners[0]);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _changeListeners[0].Check(null, Undefined, Undefined, 0);
    }

    [Test]
    public void TestAddInvalidation()
    {
        IInvalidationListener weakListener = new WeakInvalidationListenerMock();

        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, _changeListeners[0]);
        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, _changeListeners[1]);

        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, weakListener);
        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, _invalidationListeners[0]);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _invalidationListeners[0].Check(_observable, 1);

        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, weakListener);
        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, _invalidationListeners[1]);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _invalidationListeners[0].Check(_observable, 1);
        _invalidationListeners[1].Check(_observable, 1);

        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, weakListener);
        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, _invalidationListeners[2]);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _invalidationListeners[0].Check(_observable, 1);
        _invalidationListeners[1].Check(_observable, 1);
        _invalidationListeners[2].Check(_observable, 1);
    }

    [Test]
    public void TestRemoveInvalidation()
    {
        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, _changeListeners[0]);
        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, _changeListeners[1]);

        _helper = ExpressionHelper<object>.RemoveListener(_helper, _invalidationListeners[1]);

        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, _invalidationListeners[0]);

        _helper = ExpressionHelper<object>.RemoveListener(_helper, _invalidationListeners[1]);

        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, _invalidationListeners[1]);
        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, _invalidationListeners[2]);

        _helper = ExpressionHelper<object>.RemoveListener(_helper, _invalidationListeners[0]);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _invalidationListeners[0].Check(null, 0);
        _invalidationListeners[1].Check(_observable, 1);
        _invalidationListeners[2].Check(_observable, 1);

        _helper = ExpressionHelper<object>.RemoveListener(_helper, _invalidationListeners[1]);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _invalidationListeners[0].Check(null, 0);
        _invalidationListeners[1].Check(null, 0);
        _invalidationListeners[2].Check(_observable, 1);

        _helper = ExpressionHelper<object>.RemoveListener(_helper, _invalidationListeners[2]);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _invalidationListeners[0].Check(null, 0);
        _invalidationListeners[1].Check(null, 0);
        _invalidationListeners[2].Check(null, 0);
    }

    [Test]
    public void TestAddInvalidationWhileLocked()
    {
        IChangeListener<object> addingListener = new AddingInvalidationListener(this);
        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, addingListener);
        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, _changeListeners[0]);

        _observable.Set(Data2);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _invalidationListeners[0].Reset();

        _observable.Set(Data1);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _invalidationListeners[0].Check(_observable, 1);
        _invalidationListeners[1].Reset();

        _observable.Set(Data2);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _invalidationListeners[0].Check(_observable, 1);
        _invalidationListeners[1].Check(_observable, 1);
        _invalidationListeners[2].Reset();

        _observable.Set(Data1);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _invalidationListeners[0].Check(_observable, 1);
        _invalidationListeners[1].Check(_observable, 1);
        _invalidationListeners[2].Check(_observable, 1);
        _invalidationListeners[3].Reset();

        _observable.Set(Data2);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _invalidationListeners[0].Check(_observable, 1);
        _invalidationListeners[1].Check(_observable, 1);
        _invalidationListeners[2].Check(_observable, 1);
        _invalidationListeners[3].Check(_observable, 1);
    }

    [Test]
    public void TestRemoveInvalidationWhileLocked()
    {
        IChangeListener<object> removingListener = new RemovingInvalidationListener(this);
        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, removingListener);
        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, _changeListeners[0]);
        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, _invalidationListeners[0]);
        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, _invalidationListeners[1]);
        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, _invalidationListeners[2]);

        _observable.Set(Data2);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _invalidationListeners[0].Reset();
        _invalidationListeners[1].Check(_observable, 1);
        _invalidationListeners[2].Check(_observable, 1);

        _observable.Set(Data1);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _invalidationListeners[0].Check(null, 0);
        _invalidationListeners[1].Reset();
        _invalidationListeners[2].Check(_observable, 1);

        _observable.Set(Data2);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _invalidationListeners[0].Check(null, 0);
        _invalidationListeners[1].Check(null, 0);
        _invalidationListeners[2].Reset();

        _observable.Set(Data1);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _invalidationListeners[0].Check(null, 0);
        _invalidationListeners[1].Check(null, 0);
        _invalidationListeners[2].Check(null, 0);
    }

    [Test]
    public void TestAddChange()
    {
        IChangeListener<object> weakListener = new WeakChangeListenerMock<object>();

        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, _invalidationListeners[0]);
        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, _invalidationListeners[1]);

        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, weakListener);
        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, _changeListeners[0]);
        _observable.Set(Data2);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _changeListeners[0].Check(_observable, Data1, Data2, 1);

        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, weakListener);
        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, _changeListeners[1]);
        _observable.Set(Data1);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _changeListeners[0].Check(_observable, Data2, Data1, 1);
        _changeListeners[1].Check(_observable, Data2, Data1, 1);

        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, weakListener);
        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, _changeListeners[2]);
        _observable.Set(Data2);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _changeListeners[0].Check(_observable, Data1, Data2, 1);
        _changeListeners[1].Check(_observable, Data1, Data2, 1);
        _changeListeners[2].Check(_observable, Data1, Data2, 1);
    }

    [Test]
    public void TestRemoveChange()
    {
        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, _invalidationListeners[0]);
        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, _invalidationListeners[1]);

        _helper = ExpressionHelper<object>.RemoveListener(_helper, _changeListeners[1]);

        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, _changeListeners[0]);

        _helper = ExpressionHelper<object>.RemoveListener(_helper, _changeListeners[1]);

        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, _changeListeners[1]);
        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, _changeListeners[2]);

        _helper = ExpressionHelper<object>.RemoveListener(_helper, _changeListeners[0]);
        _observable.Set(Data2);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _changeListeners[0].Check(null, Undefined, Undefined, 0);
        _changeListeners[1].Check(_observable, Data1, Data2, 1);
        _changeListeners[2].Check(_observable, Data1, Data2, 1);

        _helper = ExpressionHelper<object>.RemoveListener(_helper, _changeListeners[1]);
        _observable.Set(Data1);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _changeListeners[0].Check(null, Undefined, Undefined, 0);
        _changeListeners[1].Check(null, Undefined, Undefined, 0);
        _changeListeners[2].Check(_observable, Data2, Data1, 1);

        _helper = ExpressionHelper<object>.RemoveListener(_helper, _changeListeners[2]);
        _observable.Set(Data2);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _changeListeners[0].Check(null, Undefined, Undefined, 0);
        _changeListeners[1].Check(null, Undefined, Undefined, 0);
        _changeListeners[2].Check(null, Undefined, Undefined, 0);
    }

    [Test]
    public void TestAddChangeWhileLocked()
    {
        IChangeListener<object> addingListener = new AddingChangeListener(this);
        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, addingListener);
        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, _invalidationListeners[0]);

        _observable.Set(Data2);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _changeListeners[0].Reset();

        _observable.Set(Data1);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _changeListeners[0].Check(_observable, Data2, Data1, 1);
        _changeListeners[1].Reset();

        _observable.Set(Data2);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _changeListeners[0].Check(_observable, Data1, Data2, 1);
        _changeListeners[1].Check(_observable, Data1, Data2, 1);
        _changeListeners[2].Reset();

        _observable.Set(Data1);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _changeListeners[0].Check(_observable, Data2, Data1, 1);
        _changeListeners[1].Check(_observable, Data2, Data1, 1);
        _changeListeners[2].Check(_observable, Data2, Data1, 1);
        _changeListeners[3].Reset();

        _observable.Set(Data2);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _changeListeners[0].Check(_observable, Data1, Data2, 1);
        _changeListeners[1].Check(_observable, Data1, Data2, 1);
        _changeListeners[2].Check(_observable, Data1, Data2, 1);
        _changeListeners[3].Check(_observable, Data1, Data2, 1);
    }

    [Test]
    public void TestRemoveChangeWhileLocked()
    {
        IChangeListener<object> removingListener = new RemovingChangeListener(this);
        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, removingListener);
        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, _invalidationListeners[0]);
        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, _changeListeners[0]);
        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, _changeListeners[1]);
        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, _changeListeners[2]);

        _observable.Set(Data2);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _changeListeners[0].Reset();
        _changeListeners[1].Check(_observable, Data1, Data2, 1);
        _changeListeners[2].Check(_observable, Data1, Data2, 1);

        _observable.Set(Data1);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _changeListeners[0].Check(null, Undefined, Undefined, 0);
        _changeListeners[1].Reset();
        _changeListeners[2].Check(_observable, Data2, Data1, 1);

        _observable.Set(Data2);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _changeListeners[0].Check(null, Undefined, Undefined, 0);
        _changeListeners[1].Check(null, Undefined, Undefined, 0);
        _changeListeners[2].Reset();

        _observable.Set(Data1);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _changeListeners[0].Check(null, Undefined, Undefined, 0);
        _changeListeners[1].Check(null, Undefined, Undefined, 0);
        _changeListeners[2].Check(null, Undefined, Undefined, 0);
    }

    [Test]
    public void TestFireValueChangedEvent()
    {
        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, _invalidationListeners[0]);
        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, _changeListeners[0]);

        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _invalidationListeners[0].Check(_observable, 1);
        _changeListeners[0].Check(null, Undefined, Undefined, 0);

        _observable.Set(Data2);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _invalidationListeners[0].Check(_observable, 1);
        _changeListeners[0].Check(_observable, Data1, Data2, 1);

        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _invalidationListeners[0].Check(_observable, 1);
        _changeListeners[0].Check(null, Undefined, Undefined, 0);

        _observable.Set(Data1);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _invalidationListeners[0].Check(_observable, 1);
        _changeListeners[0].Check(_observable, Data2, Data1, 1);

        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        _invalidationListeners[0].Check(_observable, 1);
        _changeListeners[0].Check(null, Undefined, Undefined, 0);
    }

    [Test]
    public void TestExceptionNotPropagatedFromSingleInvalidation()
    {
        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, new ExceptionSingleInvalidation());
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
    }

    [Test]
    public void TestExceptionNotPropagatedFromMultipleInvalidation()
    {
        var called = new BitArray(2);

        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, new ExceptionMultipleInvalidation(called, 0));
        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, new ExceptionMultipleInvalidation(called, 1));

        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        Assert.IsTrue(called[0]);
        Assert.IsTrue(called[1]);
    }

    [Test]
    public void TestExceptionNotPropagatedFromSingleChange()
    {
        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, new ExceptionSingleChange());
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
    }

    [Test]
    public void TestExceptionNotPropagatedFromMultipleChange()
    {
        var called = new BitArray(2);

        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, new ExceptionMultipleChange(called, 0));
        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, new ExceptionMultipleChange(called, 1));

        _observable.Set(Data2);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        Assert.IsTrue(called[0]);
        Assert.IsTrue(called[1]);
    }

    [Test]
    public void TestExceptionNotPropagatedFromMultipleChangeAndInvalidation()
    {
        var called = new BitArray(4);

        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, new ExceptionMultipleChange(called, 0));
        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, new ExceptionMultipleChange(called, 1));
        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, new ExceptionMultipleInvalidation(called, 2));
        _helper = ExpressionHelper<object>.AddListener(_helper, _observable, new ExceptionMultipleInvalidation(called, 3));

        _observable.Set(Data2);
        ExpressionHelper<object>.FireValueChangedEvent(_helper);
        Assert.IsTrue(called[0]);
        Assert.IsTrue(called[1]);
        Assert.IsTrue(called[2]);
        Assert.IsTrue(called[3]);
    }

    private class MockListenerBase(ExpressionHelperTest parent)
    {
        protected readonly ExpressionHelperTest Parent = parent;
    }

    private class AddingInvalidationListener(ExpressionHelperTest parent) : MockListenerBase(parent), IChangeListener<object>
    {
        private int _index;

        public void Changed(IObservableValue<object> observable, object oldValue, object newValue)
        {
            if (_index < Parent._invalidationListeners.Length)
            {
                Parent._helper = ExpressionHelper<object>.AddListener(Parent._helper, Parent._observable,
                    Parent._invalidationListeners[_index++]);
            }
        }
    }

    private class RemovingInvalidationListener(ExpressionHelperTest parent)
        : MockListenerBase(parent), IChangeListener<object>
    {
        private int _index;

        public void Changed(IObservableValue<object> observable, object oldValue, object newValue)
        {
            if (_index < Parent._invalidationListeners.Length)
            {
                Parent._helper =
                    ExpressionHelper<object>.RemoveListener(Parent._helper, Parent._invalidationListeners[_index++]);
            }
        }
    }

    private class AddingChangeListener(ExpressionHelperTest parent) : MockListenerBase(parent), IChangeListener<object>
    {
        private int _index;

        public void Changed(IObservableValue<object> observable, object oldValue, object newValue)
        {
            if (_index < Parent._changeListeners.Length)
            {
                Parent._helper = ExpressionHelper<object>.AddListener(Parent._helper, Parent._observable,
                    Parent._changeListeners[_index++]);
            }
        }
    }

    private class RemovingChangeListener(ExpressionHelperTest parent) : MockListenerBase(parent), IChangeListener<object>
    {
        private int _index;

        public void Changed(IObservableValue<object> observable, object oldValue, object newValue)
        {
            if (_index < Parent._changeListeners.Length)
            {
                Parent._helper =
                    ExpressionHelper<object>.RemoveListener(Parent._helper, Parent._changeListeners[_index++]);
            }
        }
    }

    private class ExceptionSingleInvalidation : IInvalidationListener
    {
        public void Invalidated(IObservable observable)
        {
            throw new ApplicationException();
        }
    }

    private class ExceptionMultipleInvalidation(BitArray called, int bitIndex) : IInvalidationListener
    {
        public void Invalidated(IObservable observable)
        {
            called.Set(bitIndex, true);
            throw new ApplicationException();
        }
    }

    private class ExceptionSingleChange : IChangeListener<object>
    {
        public void Changed(IObservableValue<object> observable, object oldValue, object newValue)
        {
            throw new ApplicationException();
        }
    }

    private class ExceptionMultipleChange(BitArray called, int bitIndex) : IChangeListener<object>
    {
        public void Changed(IObservableValue<object> observable, object oldValue, object newValue)
        {
            called.Set(bitIndex, true);
            throw new ApplicationException();
        }
    }
}