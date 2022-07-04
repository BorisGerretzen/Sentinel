namespace Sentinel.Extensions;

public static class generic {
    public static void ForEach<T>(this T[] arr, Action<T> action) {
        foreach (var elem in arr)
            action(elem);
    }
}