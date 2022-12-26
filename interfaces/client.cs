public interface IClient {
    void notify(IEnumerable<int> users, IClientParams param, IEnumerable<int>? except=null);

    void notifyAdmins(IClientParams param);
}

public interface IClientParams{
    string text {get;}
    
}