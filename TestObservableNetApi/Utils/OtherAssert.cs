using System;
using NUnit.Framework;
using ObservableNetApi.Internal.Extensions;

namespace TestObservableNetApi.Utils;

public class OtherAssert
{
    public static string MessagePrefix(string? message)
    {
        return message != null ? $"{message}. " : "";
    }

    public static void AssertContentEquals<T>(T[]? expected, T[]? actual, string? message = null)
    {
        AssertArrayContentEquals(message, expected, actual, array => array.Length, (arr, i) => arr[i],
            arr => arr.ContentToString(), (arr1, arr2) => arr1?.ContentEquals(arr2) ?? arr2 == null);
    }

    private static void AssertArrayContentEquals<T>(string? message, T? expected, T? actual, Func<T, int> size,
        Func<T, int, object?> get, Func<T, string> contentToString, Func<T?, T?, bool> contentEquals)
    {
        if (contentEquals(expected, actual))
        {
            return;
        }

        const string typeName = "Array";

        if (CheckReferenceAndNullEquality(typeName, message, expected, actual, contentToString))
        {
            return;
        }

        var expectedSize = size(expected!);
        var actualSize = size(actual!);

        if (expectedSize != actualSize)
        {
            var sizesDifferMessage =
                $"{typeName} sizes differ. Expected size is {expectedSize}, actual size is {actualSize}.";

            var toString = $"Expected <{contentToString(expected!)}>, actual <{contentToString(actual!)}>.";

            Assert.Fail(MessagePrefix(message) + sizesDifferMessage + "\n" + toString);
        }

        for (var index = 0; index < expectedSize; index++)
        {
            var expectedElement = get(expected!, index);
            var actualElement = get(actual!, index);

            if (!expectedElement?.Equals(actualElement) ?? actualElement != null)
            {
                var elementsDifferMessage = ElementsDifferMessage(typeName, index, expectedElement, actualElement);
                var toString = $"Expected <{contentToString(expected!)}>, actual <{contentToString(actual!)}>.";

                Assert.Fail(MessagePrefix(message) + elementsDifferMessage + "\n" + toString);
            }
        }
    }

    private static bool CheckReferenceAndNullEquality<T>(string typeName, string? message, T? expected, T? actual,
        Func<T, string> contentToString)
    {
        if ((object?)expected == (object?)actual)
        {
            return true;
        }

        if (expected == null)
        {
            Assert.Fail(MessagePrefix(message) + $"Expected <null> {typeName}, actual <{contentToString(actual!)}>.");
        }

        if (actual == null)
        {
            Assert.Fail(MessagePrefix(message) +
                        $"Expected non-null {typeName} <{contentToString(expected!)}>, actual <null>.");
        }

        return false;
    }

    private static string ElementsDifferMessage(string typeName, int index, object? expectedElement, object? actualElement)
    {
        return
            $"{typeName} elements differ at index {index}. Expected element <{expectedElement}>, actual element <{actualElement}>.";
    }
}