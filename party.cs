
using Telegram.Bot;
using System;
using System.Threading;

class party:adventure{
    public int id,leader,turn=0,chosen_heroes=0,deads=0;
    public List<player> members;
    bool[] heroSelection;
    public bool isStarted = false, finished =false;
    ITelegramBotClient botClient;

    int stage=0;
    villain  vill=new villain();
    Random rnd = new Random();

    public party(ITelegramBotClient botClient,int id,string adv_name,int leader,string leader_name,string leader_user):base(adv_name){
        vill.c_name="Villain";
        this.id=id;
        this.botClient=botClient;
        this.leader=leader;
        members=new List<player>();
        heroSelection = new bool[count_dynamic(file.heroes)];
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


    public List<int>chat_ids(){
        // Returns a list of chat_ids of all members of the party
        List<int>ids=new List<int>();
        foreach(player member in members){
            ids.Add(member.chat_id);
        }
        return ids;
    }

    public void notify_members_with_picture(string message,string picture){
        // Sends a message with a picture to all members of the party
        foreach(player member in members){
            tlg.send_picture(botClient,message,member.chat_id,picture);
        }
    }
    public void add_member(int member, string name,string user){
        // Adds a member to the party
        members.Add(new player(member,name,user));
        string message=$"@{user} joined the adventure";
        notify_members( message,new long[0]);
        if(members.Count()==heroSelection.Length)
            start_adventure();
    }

    public void print_vars(int chat_id){
        // Prints all variables of the party to the chat_id
        Dictionary<string,int>vars=context();
        string vs="Variables: \n";
        vs+=$"{vill.c_name}.life: {vill.life} \n";
        foreach(var prop in vars){
            if(prop.Key=="deads")continue;
            vs+=$"{prop.Key}: {prop.Value} \n";
        }
        tlg.send_message(botClient,chat_id,vs);
    }

    public void actions(int chat_id){
        string vs="Player actions: \n";
        foreach(player member in members){
            if(member.chat_id==chat_id){
                int wr=1;
                foreach(power pw in member.powers){
                    vs+=$"{wr} - {pw.name}: {pw.descr} \n";
                }
            }
        }
        vs+="Para ejecutar alguna de estas acciones usa /do {action numer}";
        tlg.send_message(botClient,chat_id,vs);


    }
    public Dictionary<string,int> context(){
        // Returns a dictionary of all variables of the party
        Dictionary<string,int>ret=new Dictionary<string,int>();
        ret.Add("deads",deads);
        foreach(player p in members){
            Dictionary<string,int>player_dict=p.context();
            foreach(var prop in player_dict){
                ret.Add(prop.Key,prop.Value);
            }
        }
        return ret;
    }

    public void from_context(Dictionary<string,int>cont){
        // Sets the variables of the party from a dictionary
        char[] delims={'.'};
        foreach(var s in cont){
            string[] tokens=s.Key.Split(delims);
            for(int i=0;i<members.Count();i++){
                if( tokens[0]==members[i].c_name ){
                    members[i].upd_param(tokens[1],s.Value);
                    break;
                }
            }
        }
        deads=cont["deads"];
    }



    public void start_adventure(){
        // Starts the adventure
        isStarted=true;
        string message="La aventura ha comenzado!";
        notify_members( message, new long[0]);
        message="Elije a tu heroe:\n";
        int cont=0;        
        foreach(var hero in file.heroes){
            cont++;
            message+=cont.ToString()+" - "+hero.name+"\n";
        }
        message+="Para elegir un heroe escriba:\n /choose_hero [numero]";
        notify_members( message, new long[0]);
    }

    public void choose_hero( int chat_id, int hero_id){
        // Chooses a hero for a player
        hero_id--;
        if(heroSelection[hero_id]){
            tlg.send_message(botClient,chat_id,"Ese heroe ya ha sido seleccionado, pruebe con otro.");
            return;
        }
        foreach(player member in members){
            if(member.chat_id==chat_id){
                chosen_heroes++;
                member.h_ref=file.heroes[hero_id].token;
                member.c_name=file.heroes[hero_id].name;
                member.h_hist=file.heroes[hero_id].desc;

                member.life=file.heroes[hero_id].life;
                member.strength=file.heroes[hero_id].strength;
                member.agility=file.heroes[hero_id].agility;
                member.mana=file.heroes[hero_id].mana;

                foreach(var pw in file.heroes[hero_id].powers){
                    member.powers.Add(new power((string)pw[0],(string)pw[1],(string)pw[2]));
                }

                string message=$"@{member.user} ha elegido a {member.c_name}:\n {member.h_hist}\n Life: {member.life}\n Strength: {member.strength}\n Agility: {member.agility}\n Mana: {member.mana}";
                string picture=file.heroes[hero_id].img;
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
        int enc=rnd.Next(0, count_dynamic(file.story[stage].events[curr.h_ref]));
        string encount=(string)file.story[stage].events[curr.h_ref][enc];
        Thread.Sleep(300);
        interpreter interp=new interpreter(botClient,encount,context(),chat_ids() );
        interp.run();
        from_context(interp.context); 
    }

    public void print_turn(){ 
        if(members[turn].life>0){
            string message="Turno de: @";
            message+=members[turn].user;
            Thread.Sleep(300);
            notify_members(message,new long[0]);
            encounter(members[turn]);
            if(members[turn].life<0){
                notify_members($"🪦 {members[turn].c_name} ha muerto!",new long[0]);        
                deads++;
                end_turn();
            }
        }else{end_turn();}
    }

    public void action(){
        vill.life-=100;
        notify_members("Se le han hecho 100 puntos de daño al enemigo",new long[0]);
    }

    public void end_turn(){
        turn++;
        if(turn==members.Count())
            turn=0;
        
        if(vill.life<=0){
            end_stage();   
        }
        if(!finished)print_turn();
    }



    public void start_stage(bool beg=false){
        Thread.Sleep(1000);
        vill.life=(int)file.story[stage].villain.life;
        if(file.story[stage].beg_pic=="null")
            notify_members(file.story[stage].beg_story,new long[0]);
        else
            notify_members_with_picture((string)file.story[stage].beg_story+$"\n Life: {vill.life}",(string)file.story[stage].beg_pic); 
        if(beg)print_turn();
    }


    public void end_game(){
        Thread.Sleep(500);
        notify_members("El juego ha terminado",new long[0]);
        finished=true;
    }


    public void end_stage(){
        Thread.Sleep(1000);
        if(file.story[stage].end_pic=="null")
            notify_members((string)file.story[stage].end_story,new long[0]);
        else
            notify_members_with_picture((string)file.story[stage].end_story,(string)file.story[stage].end_pic); 
        stage++;
        if(stage==count_dynamic(file.story)){
            end_game();
            return;
        }
        start_stage();
    }


}
