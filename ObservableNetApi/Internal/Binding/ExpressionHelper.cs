using ObservableNetApi.Beans;
using ObservableNetApi.Beans.Value;
using ObservableNetApi.Internal.Extensions;
using ObservableNetApi.Internal.Utils;

namespace ObservableNetApi.Internal.Binding;

public abstract class ExpressionHelper<T> : ExpressionHelperBase
{
    protected IObservableValue<T> Observable;

    protected ExpressionHelper(IObservableValue<T> observable)
    {
        Observable = observable;
    }

    protected abstract ExpressionHelper<T> AddListener(IInvalidationListener listener);

    protected abstract ExpressionHelper<T>? RemoveListener(IInvalidationListener listener);

    protected abstract bool HasListener(IInvalidationListener listener);

    protected abstract ExpressionHelper<T> AddListener(IChangeListener<T> listener);

    protected abstract ExpressionHelper<T>? RemoveListener(IChangeListener<T> listener);

    protected abstract bool HasListener(IChangeListener<T> listener);

    protected abstract void FireValueChangedEvent();

    public abstract IInvalidationListener[] GetIInvalidationListeners();

    public abstract IChangeListener<T>[] GetIChangeListeners();

    private class SingleInvalidation : ExpressionHelper<T>
    {
        private IInvalidationListener _listener;

        public SingleInvalidation(IObservableValue<T> observable, IInvalidationListener listener) : base(
            observable)
        {
            _listener = listener;
        }

        protected override ExpressionHelper<T> AddListener(IInvalidationListener listener)
        {
            return this;
        }

        protected override ExpressionHelper<T>? RemoveListener(IInvalidationListener listener)
        {
            return listener != _listener ? this : null;
        }

        protected override bool HasListener(IInvalidationListener listener)
        {
            return _listener == listener;
        }

        protected override ExpressionHelper<T> AddListener(IChangeListener<T> listener)
        {
            return this;
        }

        protected override ExpressionHelper<T> RemoveListener(IChangeListener<T> listener)
        {
            return this;
        }

        protected override bool HasListener(IChangeListener<T> listener)
        {
            return false;
        }

        protected override void FireValueChangedEvent()
        {
            try
            {
                _listener.Invalidated(Observable);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }
        }

        public override IInvalidationListener[] GetIInvalidationListeners()
        {
            return [_listener];
        }

        public override IChangeListener<T>[] GetIChangeListeners()
        {
            return [];
        }
    }

    private class SingleChange : ExpressionHelper<T>
    {
        private IChangeListener<T> _listener;

        private T _currentValue;

        public SingleChange(IObservableValue<T> observable, IChangeListener<T> listener) : base(observable)
        {
            _listener = listener;
            _currentValue = Observable.GetValue();
        }

        protected override ExpressionHelper<T> AddListener(IInvalidationListener listener)
        {
            return this;
        }

        protected override ExpressionHelper<T> RemoveListener(IInvalidationListener listener)
        {
            return this;
        }

        protected override bool HasListener(IInvalidationListener listener)
        {
            return false;
        }

        protected override ExpressionHelper<T> AddListener(IChangeListener<T> listener)
        {
            return this;
        }

        protected override ExpressionHelper<T>? RemoveListener(IChangeListener<T> listener)
        {
            return listener != _listener ? this : null;
        }

        protected override bool HasListener(IChangeListener<T> listener)
        {
            return _listener == listener;
        }

        protected override void FireValueChangedEvent()
        {
            var oldValue = _currentValue;
            _currentValue = Observable.GetValue();
            var changed = _currentValue == null ? oldValue != null : !_currentValue.Equals(oldValue);
            if (!changed)
            {
                return;
            }

            try
            {
                _listener.Changed(Observable, oldValue, _currentValue);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }
        }

        public override IInvalidationListener[] GetIInvalidationListeners()
        {
            return [];
        }

        public override IChangeListener<T>[] GetIChangeListeners()
        {
            return [_listener];
        }
    }

    private class Generic : ExpressionHelper<T>
    {
        private IInvalidationListener?[] _invalidationListeners;

        private IChangeListener<T>?[] _changeListeners;

        private int _invalidationSize;

        private int _changeSize;

        private bool _locked = false;

        private T _currentValue;

        public Generic(IObservableValue<T> observable, IInvalidationListener listener0, IInvalidationListener listener1) :
            base(observable)
        {
            _invalidationListeners = [listener0, listener1];
            _changeListeners = [];
            _invalidationSize = 2;
            _changeSize = 0;
            _currentValue = Observable.GetValue();
        }

        public Generic(IObservableValue<T> observable, IInvalidationListener invalidationListener,
            IChangeListener<T> changeListener) : base(observable)
        {
            _invalidationListeners = [invalidationListener];
            _changeListeners = [changeListener];
            _invalidationSize = 1;
            _changeSize = 1;
            _currentValue = Observable.GetValue();
        }

        public Generic(IObservableValue<T> observable, IChangeListener<T> listener0, IChangeListener<T> listener1) :
            base(observable)
        {
            _invalidationListeners = [];
            _changeListeners = [listener0, listener1];
            _invalidationSize = 0;
            _changeSize = 2;
            _currentValue = Observable.GetValue();
        }

        protected override ExpressionHelper<T> AddListener(IInvalidationListener listener)
        {
            if (_invalidationListeners.Length == 0)
            {
                _invalidationListeners = [listener];
                _invalidationSize = 1;
            }
            else
            {
                var oldSize = _invalidationListeners.Length;
                if (_locked)
                {
                    var newSize = _invalidationSize < oldSize ? oldSize : oldSize * 3 / 2 + 1;
                    Array.Resize(ref _invalidationListeners, newSize);
                }
                else if (_invalidationSize == oldSize)
                {
                    // ReSharper disable once CoVariantArrayConversion
                    _invalidationSize = Trim(_invalidationSize, _invalidationListeners);
                    if (_invalidationSize != oldSize)
                    {
                        return this;
                    }

                    var newSize = _invalidationSize < oldSize ? oldSize : oldSize * 3 / 2 + 1;
                    Array.Resize(ref _invalidationListeners, newSize);
                }
            }

            return this;
        }

        protected override ExpressionHelper<T> RemoveListener(IInvalidationListener listener)
        {
            for (var i = 0; i < _invalidationSize; i++)
            {
                if (listener != _invalidationListeners[i])
                {
                    continue;
                }

                if (_invalidationSize == 1)
                {
                    if (_changeSize == 1)
                    {
                        return new SingleChange(Observable, _changeListeners[0]!);
                    }

                    _invalidationListeners = [];
                    _invalidationSize = 0;
                }
                else if (_invalidationSize == 2 && _changeSize == 0)
                {
                    return new SingleInvalidation(Observable, _invalidationListeners[1 - i]!);
                }
                else
                {
                    var numMoved = _invalidationSize - i - 1;
                    var oldListeners = _invalidationListeners;
                    if (_locked)
                    {
                        _invalidationListeners = new IInvalidationListener?[_invalidationListeners.Length];
                        oldListeners.CopyIto(_invalidationListeners, 0, 0, i + 1);
                    }

                    if (numMoved > 0)
                    {
                        oldListeners.CopyIto(_invalidationListeners, i, i + 1, _invalidationSize);
                    }

                    _invalidationSize--;
                    if (!_locked)
                    {
                        _invalidationListeners[_invalidationSize] = null; // Let gc do its work
                    }
                }

                break;
            }

            return this;
        }

        protected override bool HasListener(IInvalidationListener listener)
        {
            foreach (var invalidationListener in _invalidationListeners)
            {
                if (listener == invalidationListener)
                {
                    return true;
                }

                if (invalidationListener == null)
                {
                    return false;
                }
            }

            return false;
        }

        protected override ExpressionHelper<T> AddListener(IChangeListener<T> listener)
        {
            if (_changeListeners.Length == 0)
            {
                _changeListeners = [listener];
                _changeSize = 1;
            }
            else
            {
                var oldSize = _changeListeners.Length;
                if (_locked)
                {
                    var newSize = _changeSize < oldSize ? oldSize : oldSize * 3 / 2 + 1;
                    Array.Resize(ref _changeListeners, newSize);
                }
                else if (_changeSize == oldSize)
                {
                    // ReSharper disable once CoVariantArrayConversion
                    _changeSize = Trim(_changeSize, _changeListeners);
                    if (_changeSize != oldSize)
                    {
                        return this;
                    }

                    var newSize = _changeSize < oldSize ? oldSize : oldSize * 3 / 2 + 1;
                    Array.Resize(ref _changeListeners, newSize);
                }
            }

            return this;
        }

        protected override ExpressionHelper<T> RemoveListener(IChangeListener<T> listener)
        {
            for (var i = 0; i < _changeSize; i++)
            {
                if (listener != _changeListeners[i])
                {
                    continue;
                }

                if (_changeSize == 1)
                {
                    if (_invalidationSize == 1)
                    {
                        return new SingleChange(Observable, _changeListeners[0]!);
                    }

                    _changeListeners = [];
                    _changeSize = 0;
                }
                else if (_changeSize == 2 && _invalidationSize == 0)
                {
                    return new SingleChange(Observable, _changeListeners[1 - i]!);
                }
                else
                {
                    var numMoved = _changeSize - i - 1;
                    var oldListeners = _changeListeners;
                    if (_locked)
                    {
                        _changeListeners = new IChangeListener<T>?[_changeListeners.Length];
                        oldListeners.CopyIto(_changeListeners, 0, 0, i + 1);
                    }

                    if (numMoved > 0)
                    {
                        oldListeners.CopyIto(_changeListeners, i, i + 1, _changeSize);
                    }

                    _changeSize--;
                    if (!_locked)
                    {
                        _changeListeners[_changeSize] = null; // Let gc do its work
                    }
                }

                break;
            }

            return this;
        }

        protected override bool HasListener(IChangeListener<T> listener)
        {
            foreach (var changeListener in _changeListeners)
            {
                if (listener == changeListener)
                {
                    return true;
                }

                if (changeListener == null)
                {
                    return false;
                }
            }

            return false;
        }

        protected override void FireValueChangedEvent()
        {
            var curInvalidationListeners = _invalidationListeners;
            var curInvalidationSize = _invalidationSize;
            var curChangeListeners = _changeListeners;
            var curChangeSize = _changeSize;

            try
            {
                _locked = true;
                for (var i = 0; i < curInvalidationSize; i++)
                {
                    try
                    {
                        curInvalidationListeners[i]!.Invalidated(Observable);
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine(e);
                    }
                }

                if (curChangeSize <= 0)
                {
                    return;
                }

                var oldValue = _currentValue;
                _currentValue = Observable.GetValue();
                var changed = _currentValue == null ? oldValue != null : !_currentValue.Equals(oldValue);
                if (!changed)
                {
                    return;
                }

                for (var i = 0; i < curChangeSize; i++)
                {
                    try
                    {
                        curChangeListeners[i]!.Changed(Observable, oldValue, _currentValue);
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine(e);
                    }
                }
            }
            finally
            {
                _locked = false;
            }
        }

        public override IInvalidationListener[] GetIInvalidationListeners()
        {
            return ArrayUtils.GetInstance().CopyOfNotNulls(_invalidationListeners);
        }

        public override IChangeListener<T>[] GetIChangeListeners()
        {
            return ArrayUtils.GetInstance().CopyOfNotNulls(_changeListeners);
        }
    }

    public static ExpressionHelper<T> AddListener(ExpressionHelper<T>? helper, IObservableValue<T> observable,
        IInvalidationListener listener)
    {
        observable.GetValue(); // validate
        return helper?.AddListener(listener) ?? new SingleInvalidation(observable, listener);
    }

    public static ExpressionHelper<T>? RemoveListener(ExpressionHelper<T>? helper, IInvalidationListener listener)
    {
        return helper?.RemoveListener(listener);
    }

    public static ExpressionHelper<T> AddListener(ExpressionHelper<T>? helper, IObservableValue<T> observable,
        IChangeListener<T> listener)
    {
        return helper?.AddListener(listener) ?? new SingleChange(observable, listener);
    }

    public static ExpressionHelper<T>? RemoveListener(ExpressionHelper<T>? helper, IChangeListener<T> listener)
    {
        return helper?.RemoveListener(listener);
    }

    public static void FireValueChangedEvent(ExpressionHelper<T>? helper)
    {
        helper?.FireValueChangedEvent();
    }
}