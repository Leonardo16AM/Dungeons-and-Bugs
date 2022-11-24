
using Telegram.Bot;
using System;
using System.Threading;

class party:adventure{
    public int id,leader,turn=0,chosen_heroes=0;
    public List<player> members;
    bool[] heroSelection;
    public bool isStarted = false;
    ITelegramBotClient botClient;

    int stage=0;
    int villain_life=0;
    Random rnd = new Random();

    public party(ITelegramBotClient botClient,int id,string adv_name,int leader,string leader_name,string leader_user):base(adv_name){
        this.id=id;
        this.botClient=botClient;
        this.leader=leader;
        members=new List<player>();
        int n = 0;
        foreach(var hero in file.heroes)
            n++;
        heroSelection = new bool[n];
        members.Add(new player(leader,leader_name,leader_user));
    }

    public void notify_members(string message, long[] except){
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
    public void notify_members_with_picture(string message,string picture){
        foreach(player member in members){
            tlg.send_picture(botClient,message,member.chat_id,picture);
        }
    }
    public void add_member(int member, string name,string user){
        members.Add(new player(member,name,user));
        string message=$"@{user} joined the adventure";
        notify_members( message,new long[0]);
        if(members.Count()==heroSelection.Length)
            start();
    }


    public void start(){
        isStarted=true;
        string message="La aventura ha comenzado!";
        notify_members( message, new long[0]);
        message="Elije a tu heroe:\n";
        int cont=0;        
        foreach(var hero in file.heroes){
            cont++;
            message+=cont.ToString()+" - "+hero[1]+"\n";
        }
        message+="Para elegir un heroe escriba:\n /choose_hero [numero]";
        notify_members( message, new long[0]);
    }

    public void choose_hero( int chat_id, int hero_id){
        hero_id--;
        if(heroSelection[hero_id]){
            tlg.send_message(botClient,chat_id,"Ese heroe ya ha sido seleccionado, pruebe con otro.");
            return;
        }
        foreach(player member in members){
            if(member.chat_id==chat_id){
                chosen_heroes++;
                member.h_ref=file.heroes[hero_id][0];
                member.h_name=file.heroes[hero_id][1];
                member.h_hist=file.heroes[hero_id][6];

                member.life=file.heroes[hero_id][2];
                member.strength=file.heroes[hero_id][3];
                member.agility=file.heroes[hero_id][4];
                member.mana=file.heroes[hero_id][5];
                string message=$"@{member.user} ha elegido a {member.h_name}:\n {member.h_hist}\n Life: {member.life}\n Strength: {member.strength}\n Agility: {member.agility}\n Mana: {member.mana}";
                string picture=file.heroes[hero_id][7];
                heroSelection[hero_id]=true;
                notify_members_with_picture( message,picture);
                Thread.Sleep(1000);
                break;
            }
        }
        if(chosen_heroes==members.Count()){
            string message="Todos los heroes han sido elegidos, que comience la aventura!";
            notify_members(message,new long[0]);
            Thread.Sleep(2000);
            start_stage(true);
        }

    }


    public void encounter(player curr){
        int enc=rnd.Next(0, 2);
        string encount=(string)file.story[stage][3][curr.h_ref][enc];
        Thread.Sleep(300);
        notify_members(encount,new long[0]);
    }

    public void print_turn(){ 
        string message="Turno de: @";
        message+=members[turn].user;
        Thread.Sleep(300);
        notify_members(message,new long[0]);
        encounter(members[turn]);
    }

    public void action(){
        villain_life-=100;
        notify_members("Se le han hecho 100 puntos de daño al enemigo",new long[0]);
    }

    public void end_turn(){
        turn++;
        if(turn==members.Count())
            turn=0;
        
        if(villain_life<=0){
            end_stage();   
        }
        print_turn();
    }



    public void start_stage(bool beg=false){
        Thread.Sleep(1000);
        villain_life=(int)file.story[stage][0].life;
        if(file.story[stage][2].beg_pic=="null")
            notify_members(file.story[stage][1].beg_story,new long[0]);
        else
            notify_members_with_picture((string)file.story[stage][1].beg_story+$"\n Life: {villain_life}",(string)file.story[stage][2].beg_pic); 
        if(beg)print_turn();
    }

    public void end_stage(){
        Thread.Sleep(1000);
        if(file.story[stage][5].end_pic=="null")
            notify_members((string)file.story[stage][4].end_story,new long[0]);
        else
            notify_members_with_picture((string)file.story[stage][4].end_story,(string)file.story[stage][5].end_pic); 
        stage++;
        start_stage();
    }


}
