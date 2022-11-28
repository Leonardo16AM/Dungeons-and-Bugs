using Newtonsoft.Json;


class adventure{
    public string adv;
    public dynamic file { get; set; }
    // public string json { get; set; }
    
    public adventure(string name){
        adv=name;
        file = DataAdventure.Adventures[name];
    }

    public int count_dynamic(dynamic obj){
        int cnt=0;
        foreach(var i in obj){cnt++;}
        return cnt;
    }
}