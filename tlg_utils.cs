static class tlg
{

    public static IEnumerable<T> map<T, T1>(this IEnumerable<T1> array, Func<T1, T> lambda){
        foreach (T1 item in array)
            yield return lambda(item);
    }
    
    public static int count_dynamic(dynamic obj){
        int cnt=0;
        foreach(var i in obj){cnt++;}
        return cnt;
    }


}