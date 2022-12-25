public static class tlg
{

    public static T[] map<T, T1>(IEnumerable<T1> array, Func<T1, T> lambda)
    {
        T[] result = new T[array.Count()];
        int i = 0;

        foreach (T1 item in array)
        {
            result[i] = lambda(item);
            i++;
        }

        return result;
    }

}