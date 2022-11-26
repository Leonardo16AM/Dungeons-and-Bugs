
class hero{

    public string h_name,h_ref,h_hist;
    public int h_id;
    public int life,strength,agility,mana;
    Dictionary<string,int>others=new Dictionary<string,int>();

    public Dictionary<string,int> context(){
        Dictionary<string,int>ret=new Dictionary<string,int>();
        ret.Add($"{h_name}.life",life);
        ret.Add($"{h_name}.strength",strength);
        ret.Add($"{h_name}.agility",agility);
        ret.Add($"{h_name}.mana",mana);
        foreach(var prop in others){
            ret.Add($"{h_name}.{prop.Key}",prop.Value);
        }
        return ret;
    }

    public void upd_param( string s ,int i){
        if(s=="life")life=i;
        if(s=="strength")strength=i;
        if(s=="agility")agility=i;
        if(s=="mana")mana=i;
    }

}



class player:hero{
    public int chat_id;
    public string name,user;

    public player(int chat_id,string name,string username){
        this.chat_id=chat_id;
        this.name=name;
        this.user=username;
    }

}