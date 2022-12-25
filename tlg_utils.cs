using Telegram.Bot;
using Telegram.Bot.Types.Enums;

public static class tlg{
    
     public static T[] filter<T,T1>(in IEnumerable<T1> array, Func<T1,T> lambda){
        T[] result= new T[array.Count()];
        int i=0;
        
        foreach(T1 item in array){
            result[i]= lambda(item);
            i++;
        }

        return result;
    } 

}