namespace ObservableNetApi.Internal.Extensions;

public static class ArrayExtensions
{
    public static T[] CopyIto<T>(this T[] source, T[] destination)
    {
        return source.CopyIto(destination, 0);
    }

    public static T[] CopyIto<T>(this T[] source, T[] destination, int destinationOffset)
    {
        return source.CopyIto(destination, destinationOffset, 0);
    }

    public static T[] CopyIto<T>(this T[] source, T[] destination, int destinationOffset, int startIndex)
    {
        return source.CopyIto(destination, destinationOffset, startIndex, source.Length);
    }

    public static T[] CopyIto<T>(this T[] source, T[] destination, int destinationOffset, int startIndex,
        int endIndex)
    {
        Array.Copy(source, startIndex, destination, destinationOffset, endIndex - startIndex);
        return destination;
    }

    public static string ContentToString<T>(this T[] array)
    {
        return $"[{string.Join(", ", array)}]";
    }

    public static bool ContentEquals<T, U>(this T[] source, U[]? other) where U : T
    {
        if (other == null)
        {
            return false;
        }

        if (source.Length != other.Length)
        {
            return false;
        }

        for (var i = 0; i < source.Length; i++)
        {
            var e = source[i];
            if (e == null)
            {
                if (other[i] != null)
                {
                    return false;
                }
            }
            else if (!e.Equals(other[i]))
            {
                return false;
            }
        }

        return true;
    }
}