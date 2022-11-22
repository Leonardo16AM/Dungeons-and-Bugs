
using Telegram.Bot;

class party:adventure{
    public int id;
    public int leader;
    public List<player> members;
    bool[] heroSelection;
    int turn;
    public bool isStarted = false;
    public party(int id,string adv_name,int leader,string leader_name,string leader_user):base(adv_name){
        this.id=id;
        this.leader=leader;
        members=new List<player>();
        int n = 0;
        foreach(var hero in file.heroes)
            n++;
        heroSelection = new bool[n];
        members.Add(new player(leader,leader_name,leader_user));
    }

    public void notify_members(ITelegramBotClient botClient ,string message, long[] except){
        foreach(player member in members){
            if(except.Length!= 0){
                bool next= false;
                for(int i=0;i<except.Length; i++){
                    if(member.chat_id == except[i]){
                        next = true;
                        break;
                    }
                }
                if(next)
                    continue;
            }
            tlg.send_message(botClient,member.chat_id,message);
        }
    }
    public void notify_members_with_picture(ITelegramBotClient botClient ,string message,string picture){
        foreach(player member in members){
            tlg.send_picture(botClient,message,member.chat_id,picture);
        }
    }
    public void add_member(ITelegramBotClient botClient ,int member, string name,string user){
        members.Add(new player(member,name,user));
        string message=$"@{user} joined the adventure";
        notify_members(botClient, message,new long[0]);
        if(members.Length==heroSelection.Length)
            start(botClient);
    }


    public void start(ITelegramBotClient botClient){
        isStarted=true;
        string message="La aventura ha comenzado!";
        notify_members(botClient, message, new long[0]);
        message="Elije a tu heroe:\n";
        int cont=0;        
        foreach(var hero in file.heroes){
            cont++;
            message+=cont.ToString()+" - "+hero[1]+"\n";
        }
        message+="Para elegir un heroe escriba:\n /choose_hero [numero]";
        notify_members(botClient, message, new long[0]);
    }

    public void choose_hero(ITelegramBotClient botClient, int chat_id, int hero_id){
        hero_id--;
        if(heroSelection[hero_id]){
            tlg.send_message(botClient,chat_id,"Ese heroe ya ha sido seleccionado, pruebe con otro.");
            return;
        }
        foreach(player member in members){
            if(member.chat_id==chat_id){
                member.hero=file.heroes[hero_id][0];
                member.life=file.heroes[hero_id][2];
                member.strength=file.heroes[hero_id][3];
                member.agility=file.heroes[hero_id][4];
                member.mana=file.heroes[hero_id][5];
                string message=$"@{member.user} has chosen {file.heroes[hero_id][1]}:\n {file.heroes[hero_id][6]}\n Life: {member.life}\n Strength: {member.strength}\n Agility: {member.agility}\n Mana: {member.mana}";
                string picture=file.heroes[hero_id][7];
                heroSelection[hero_id]=true;
                notify_members_with_picture(botClient, message,picture);
                return;
            }
        }
    }

}
