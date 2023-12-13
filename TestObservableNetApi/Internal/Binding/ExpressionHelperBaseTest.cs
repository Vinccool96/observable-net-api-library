using NUnit.Framework;
using ObservableNetApi.Beans;
using ObservableNetApi.Internal.Binding;
using ObservableNetApi.Internal.Utils;
using TestObservableNetApi.Utils;

namespace TestObservableNetApi.Internal.Binding;

[TestFixture]
public class ExpressionHelperBaseTest : ExpressionHelperBase
{
    private static object _listener = new();

    private static object _listener2 = new();

    private static IWeakListener _validWeakListener = new ValidWeakListener();

    private static IWeakListener _gcedWeakListener = new GcedWeakListener();

    [Test]
    public void TestEmptyArray()
    {
        object?[] array = [];
        Assert.AreEqual(0, Trim(0, array));
        OtherAssert.AssertContentEquals([], array);

        array = ArrayUtils.GetInstance().ArrayOfNulls<object>(1);
        Assert.AreEqual(0, Trim(0, array));
        OtherAssert.AssertContentEquals(ArrayUtils.GetInstance().ArrayOfNulls<object>(1), array);
    }

    [Test]
    public void TestSingleElement()
    {
        object?[] array = [_listener];
        Assert.AreEqual(1, Trim(1, array));
        OtherAssert.AssertContentEquals([_listener], array);

        array = [_validWeakListener];
        Assert.AreEqual(1, Trim(1, array));
        OtherAssert.AssertContentEquals([_validWeakListener], array);

        array = [_gcedWeakListener];
        Assert.AreEqual(0, Trim(1, array));
        OtherAssert.AssertContentEquals([null], array);

        array = [_listener, null];
        Assert.AreEqual(1, Trim(1, array));
        OtherAssert.AssertContentEquals([_listener, null], array);

        array = [_validWeakListener, null];
        Assert.AreEqual(1, Trim(1, array));
        OtherAssert.AssertContentEquals([_validWeakListener, null], array);

        array = [_gcedWeakListener, null];
        Assert.AreEqual(0, Trim(1, array));
        OtherAssert.AssertContentEquals([null, null], array);
    }

    [Test]
    public void TestMultipleElements()
    {
        object?[] array = [_validWeakListener, _listener, _listener2];
        Assert.AreEqual(3, Trim(3, array));
        OtherAssert.AssertContentEquals([_validWeakListener, _listener, _listener2], array);

        array = [_listener, _validWeakListener, _listener2];
        Assert.AreEqual(3, Trim(3, array));
        OtherAssert.AssertContentEquals([_listener, _validWeakListener, _listener2], array);

        array = [_listener, _listener2, _validWeakListener];
        Assert.AreEqual(3, Trim(3, array));
        OtherAssert.AssertContentEquals([_listener, _listener2, _validWeakListener], array);

        array = [_validWeakListener, _listener, _listener2, null];
        Assert.AreEqual(3, Trim(3, array));
        OtherAssert.AssertContentEquals([_validWeakListener, _listener, _listener2, null], array);

        array = [_listener, _validWeakListener, _listener2, null];
        Assert.AreEqual(3, Trim(3, array));
        OtherAssert.AssertContentEquals([_listener, _validWeakListener, _listener2, null], array);

        array = [_listener, _listener2, _validWeakListener, null];
        Assert.AreEqual(3, Trim(3, array));
        OtherAssert.AssertContentEquals([_listener, _listener2, _validWeakListener, null], array);

        array = [_gcedWeakListener, _validWeakListener, _listener];
        Assert.AreEqual(2, Trim(3, array));
        OtherAssert.AssertContentEquals([_validWeakListener, _listener, null], array);

        array = [_gcedWeakListener, _listener, _validWeakListener];
        Assert.AreEqual(2, Trim(3, array));
        OtherAssert.AssertContentEquals([_listener, _validWeakListener, null], array);

        array = [_gcedWeakListener, _validWeakListener, _listener, null];
        Assert.AreEqual(2, Trim(3, array));
        OtherAssert.AssertContentEquals([_validWeakListener, _listener, null, null], array);

        array = [_gcedWeakListener, _listener, _validWeakListener, null];
        Assert.AreEqual(2, Trim(3, array));
        OtherAssert.AssertContentEquals([_listener, _validWeakListener, null, null], array);

        array = [_validWeakListener, _gcedWeakListener, _listener];
        Assert.AreEqual(2, Trim(3, array));
        OtherAssert.AssertContentEquals([_validWeakListener, _listener, null], array);

        array = [_listener, _gcedWeakListener, _validWeakListener];
        Assert.AreEqual(2, Trim(3, array));
        OtherAssert.AssertContentEquals([_listener, _validWeakListener, null], array);

        array = [_validWeakListener, _gcedWeakListener, _listener, null];
        Assert.AreEqual(2, Trim(3, array));
        OtherAssert.AssertContentEquals([_validWeakListener, _listener, null, null], array);

        array = [_listener, _gcedWeakListener, _validWeakListener, null];
        Assert.AreEqual(2, Trim(3, array));
        OtherAssert.AssertContentEquals([_listener, _validWeakListener, null, null], array);

        array = [_validWeakListener, _listener, _gcedWeakListener];
        Assert.AreEqual(2, Trim(3, array));
        OtherAssert.AssertContentEquals([_validWeakListener, _listener, null], array);

        array = [_listener, _validWeakListener, _gcedWeakListener];
        Assert.AreEqual(2, Trim(3, array));
        OtherAssert.AssertContentEquals([_listener, _validWeakListener, null], array);

        array = [_validWeakListener, _listener, _gcedWeakListener, null];
        Assert.AreEqual(2, Trim(3, array));
        OtherAssert.AssertContentEquals([_validWeakListener, _listener, null, null], array);

        array = [_listener, _validWeakListener, _gcedWeakListener, null];
        Assert.AreEqual(2, Trim(3, array));
        OtherAssert.AssertContentEquals([_listener, _validWeakListener, null, null], array);

        array = [_gcedWeakListener, _gcedWeakListener];
        Assert.AreEqual(0, Trim(2, array));
        OtherAssert.AssertContentEquals(ArrayUtils.GetInstance().ArrayOfNulls<object>(2), array);

        array = [_gcedWeakListener, _gcedWeakListener, null];
        Assert.AreEqual(0, Trim(2, array));
        OtherAssert.AssertContentEquals(ArrayUtils.GetInstance().ArrayOfNulls<object>(3), array);
    }

    private class ValidWeakListener : IWeakListener
    {
        public bool WasGarbageCollected()
        {
            return false;
        }
    }

    private class GcedWeakListener : IWeakListener
    {
        public bool WasGarbageCollected()
        {
            return true;
        }
    }
}