namespace ObservableNetApi.Internal.Utils;

public class ArrayUtils
{
    private static ArrayUtils? _instance;

    private ArrayUtils()
    {
    }

    public static ArrayUtils GetInstance()
    {
        return _instance ??= new ArrayUtils()!;
    }

    public T[] CopyOfNotNulls<T>(T?[] array) where T : notnull
    {
        var nonNulls = array.Where(e => e != null).ToArray();
        if (nonNulls.Length == 0)
        {
            return [];
        }

        var result = Enumerable.Repeat(nonNulls[0]!, nonNulls.Length).ToArray();

        for (int i = 0; i < nonNulls.Length; i++)
        {
            result[i] = nonNulls[i]!;
        }

        return result;
    }

    public T?[] ArrayOfNulls<T>(int size)
    {
        return new T?[size];
    }
}