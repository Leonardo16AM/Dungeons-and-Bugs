static class tlg
{

    public static IEnumerable<T> map<T, T1>(this IEnumerable<T1> array, Func<T1, T> lambda){
        foreach (T1 item in array)
            yield return lambda(item);
    }
    



}