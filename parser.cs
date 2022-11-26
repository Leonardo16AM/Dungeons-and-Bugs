
public static class parser{
    public static List<string>get_list(string s){
        char[] delimiters = {' ','\t', '\n' };
        string[] ret=s.Split(delimiters);
        List<string> lst=new List<string>(ret); 
        return lst;
    }
}