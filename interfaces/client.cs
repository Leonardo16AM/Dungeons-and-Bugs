public interface IClient {
    void notify(int [] users, IClientParams param, int []? except=null);

    void notifyAdmins(IClientParams param);
}

public interface IClientParams{
    string text {get;}
    
}