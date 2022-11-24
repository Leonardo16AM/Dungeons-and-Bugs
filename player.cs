
class hero{

    public string h_name,h_ref,h_hist;
    public int h_id;
    public int life,strength,agility,mana;

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