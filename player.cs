

class power{
    public string name,descr,script;
    public power(string name,string descr,string script){
        this.name=name;
        this.descr=descr;
        this.script=script;
    }
}


public abstract class character{
    //type specify the kind of character will be in the inhereited class
    //villain (for virtual villains), active (for real players), virtual(for friendly NPC) 
    public virtual string type {get; protected set;}
    public int life,strength,agility,mana;
    public Dictionary<string,int>others=new Dictionary<string,int>();
    public string c_name="";

    public Dictionary<string,int> context(){
        Dictionary<string,int>ret=new Dictionary<string,int>();
        ret.Add($"{c_name}.life",life);
        ret.Add($"{c_name}.strength",strength);
        ret.Add($"{c_name}.agility",agility);
        ret.Add($"{c_name}.mana",mana);
        foreach(var prop in others){
            if(!ret.ContainsKey($"{c_name}.{prop.Key}"))ret.Add($"{c_name}.{prop.Key}",prop.Value);
        }
        return ret;
    }

    public void upd_param( string s ,int val){
        if(s=="life")life=val;
        if(s=="strength")strength=val;
        if(s=="agility")agility=val;
        if(s=="mana")mana=val;
        if(others.ContainsKey(s))others[s]=val;
        else others.Add(s,val);
    }
}

class villain:character{
    public villain(){
        type= "villain";
    }
}

class hero:character{
    public string h_ref="",h_hist="";
    public int h_id;
    Dictionary<string,string>actions=new Dictionary<string, string>();
}



class player:hero{
    public int chat_id;
    public string name,user;
    public List<power>powers=new List<power>();
    public bool robot=true;
    public List<string>act_order=new List<string>();
    public player(int chat_id,string name,string username){
        this.chat_id=chat_id;
        this.name=name;
        this.user=username;
        this.type="active";
    }
}