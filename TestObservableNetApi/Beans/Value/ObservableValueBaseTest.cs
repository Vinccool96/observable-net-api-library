using NUnit.Framework;
using ObservableNetApi.Beans;

namespace TestObservableNetApi.Beans.Value;

[TestFixture]
public class ObservableValueBaseTest
{
    private static readonly object UndefinedValue = new();

    private static readonly object DefaultValue = new();

    private static readonly object V1 = new();

    private static readonly object V2 = new();

    private ObservableObjectValueStub<object?> _valueModel = new(DefaultValue);

    private InvalidationListenerMock _invalidationListener = new();

    private ChangeListenerMock<object?> _changeListener = new(UndefinedValue);

    [SetUp]
    public void SetUp()
    {
        _valueModel = new ObservableObjectValueStub<object?>(DefaultValue);
        _invalidationListener = new InvalidationListenerMock();
        _changeListener = new ChangeListenerMock<object?>(UndefinedValue);
    }

    [Test]
    public void TestInitialState()
    {
        // no exceptions etc.
        _valueModel.FireChange();
    }

    [Test]
    public void TestOneInvalidationListener()
    {
        // adding one observer
        _valueModel.AddListener(_invalidationListener);
        _valueModel.Set(V1);
        _invalidationListener.Check(_valueModel, 1);

        // remove observer
        _valueModel.RemoveListener(_invalidationListener);
        _valueModel.Set(V2);
        _invalidationListener.Check(null, 0);

        // remove observer again
        _valueModel.RemoveListener(_invalidationListener);
        _valueModel.Set(V1);
        _invalidationListener.Check(null, 0);
    }

    [Test]
    public void TestOneChangeListener()
    {
        // adding one observer
        _valueModel.AddListener(_changeListener);
        _valueModel.Set(V1);
        _changeListener.Check(_valueModel, DefaultValue, V1, 1);

        // set same value again
        _valueModel.Set(V1);
        _changeListener.Check(null, UndefinedValue, UndefinedValue, 0);

        // set null
        _valueModel.Set(null);
        _changeListener.Check(_valueModel, V1, null, 1);
        _valueModel.Set(null);
        _changeListener.Check(null, UndefinedValue, UndefinedValue, 0);

        // remove observer
        _valueModel.RemoveListener(_changeListener);
        _valueModel.Set(V2);
        _changeListener.Check(null, UndefinedValue, UndefinedValue, 0);

        // remove observer again
        _valueModel.RemoveListener(_changeListener);
        _valueModel.Set(V1);
        _changeListener.Check(null, UndefinedValue, UndefinedValue, 0);
    }

    [Test]
    public void TestTwoObservers()
    {
        var observer2 = new InvalidationListenerMock();

        // adding two observers
        _valueModel.AddListener(_invalidationListener);
        _valueModel.AddListener(observer2);
        _valueModel.FireChange();
        _invalidationListener.Check(_valueModel, 1);
        observer2.Check(_valueModel, 1);

        // remove first observer
        _valueModel.RemoveListener(_invalidationListener);
        _valueModel.FireChange();
        _invalidationListener.Check(null, 0);
        observer2.Check(_valueModel, 1);

        // remove second observer
        _valueModel.RemoveListener(observer2);
        _valueModel.FireChange();
        _invalidationListener.Check(null, 0);
        observer2.Check(null, 0);

        // remove observers in reverse order
        _valueModel.RemoveListener(observer2);
        _valueModel.RemoveListener(_invalidationListener);
        _valueModel.FireChange();
        _invalidationListener.Check(null, 0);
        observer2.Check(null, 0);
    }

    [Test]
    public void TestConcurrentAdd()
    {
        InvalidationListenerMock observer2 = new AddingListenerMock(this);
        _valueModel.AddListener(observer2);

        // fire event that adds a second observer
        // Note: there is no assumption if observer that is being added is notified
        _valueModel.FireChange();
        observer2.Check(_valueModel, 1);

        // fire event again, this time both observers need to be notified
        _invalidationListener.Reset();
        _valueModel.FireChange();
        _invalidationListener.Check(_valueModel, 1);
        observer2.Check(_valueModel, 1);
    }

    [Test]
    public void TestConcurrentRemove()
    {
        InvalidationListenerMock observer2 = new RemovingListenerMock(this);
        _valueModel.AddListener(observer2);
        _valueModel.AddListener(_invalidationListener);

        // fire event that removes a second observer
        // Note: there is no assumption if observer that is being removed is notified
        _valueModel.FireChange();
        observer2.Check(_valueModel, 1);

        // fire event again, this time only non-removed observer is notified
        _invalidationListener.Reset();
        _valueModel.FireChange();
        _invalidationListener.Check(null, 0);
        observer2.Check(_valueModel, 1);
    }

    private class AddingListenerMock(ObservableValueBaseTest parent) : InvalidationListenerMock
    {
        public override void Invalidated(IObservable observable)
        {
            base.Invalidated(observable);
            observable.AddListener(parent._invalidationListener);
        }
    }

    private class RemovingListenerMock(ObservableValueBaseTest parent) : InvalidationListenerMock
    {
        public override void Invalidated(IObservable observable)
        {
            base.Invalidated(observable);
            observable.RemoveListener(parent._invalidationListener);
        }
    }
}