using ObservableNetApi.Beans;

namespace ObservableNetApi.Internal.Binding;

public class ExpressionHelperBase
{
    public static int Trim(int size, object?[] listeners)
    {
        var realSize = size;
        var index = 0;

        while (index < realSize)
        {
            object? listener = listeners[index];
            if (listener is IWeakListener weakListener)
            {
                if (weakListener.WasGarbageCollected())
                {
                    if (realSize - index - 1 > 0)
                    {
                        Array.Copy(listeners, index + 1, listeners, index, realSize - index - 1);
                    }
                    listeners[--realSize] = null; // Let gc do its work
                    index--;
                }
            }
            index++;
        }

        return realSize;
    }
}