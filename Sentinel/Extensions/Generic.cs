namespace Sentinel.Extensions;

public static class Generic {
    public static void ForEach<T>(this T[] arr, Action<T> action) {
        foreach (T elem in arr)
            action(elem);
    }
}