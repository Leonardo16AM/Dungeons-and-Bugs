
using Telegram.Bot;
using System;
using System.Threading;
using Telegram.Bot.Types.ReplyMarkups;
class party: IAdventure {
    public string adv {get; protected set;}
    public dynamic file { get; protected set;}
    public int id,leader,turn=0,chosen_heroes=0,deads=0;
    public List<player> members;
    bool[] heroSelection;
    public bool isStarted = false, finished =false;
    IClient Client;
    Dictionary<string,int> vars=new Dictionary<string,int>();

    int stage=0;
    villain  vill=new villain();
    Random rnd = new Random();

    public party(IClient client,int id,string adv_name,int leader,string leader_name,string leader_user){
        adv=adv_name;
        file = DataAdventure.Adventures[adv_name];
        vill.c_name="Villain";
        this.id=id;
        this.leader=leader;
        members=new List<player>();
        heroSelection = new bool[utils.count_dynamic(file.heroes)];
        members.Add(new player(leader,leader_name,leader_user));
        foreach(var h in file.heroes){
            Console.WriteLine((string)h.hname);
            vars.Add((string)h.name+".life",0);
        }
        Client= client;
    }

    public void add_member(int member, string name,string user){
        // Adds a member to the party
        members.Add(new player(member,name,user));
        string message=$"@{user} joined the adventure";
        Client.notify(
            members.map<int, player>((m)=> {return m.chat_id;}),
            new ClientParams(message)
        );
        if(members.Count()==heroSelection.Length)
            start_adventure();
    }
    public void chat(string message, int sender){
        Client.notify(
          members.map<int, player>((m)=> {return m.chat_id;}),
          new ClientParams(message),
          new List<int> {sender}  
        );
    }

    public void print_vars(int chat_id){
        // Prints all variables of the party to the chat_id
        Dictionary<string,int>vars=context();
        string vs="Variables: \n";
        vs+=$"{vill.c_name}.life: {vill.life} \n";
        foreach(player member in members){    
            foreach(var prop in vars){
                if(prop.Key.StartsWith(member.c_name))
                    vs+=$"{prop.Key}: {prop.Value} \n";
            }
        }
        Client.notify(
            chat_id,
            new ClientParams(vs)
        );
    }
    private int count_powers(int chatid){
        int n=0;
        foreach(player member in members)
            if(member.chat_id==chatid)
                foreach(power pw in member.powers)
                    n++;
        return n;          
    }
    public void actions(int chat_id){
        string vs="Player actions: \n";
        InlineKeyboardButton[] payload= new InlineKeyboardButton[count_powers(chat_id)];
        foreach(player member in members){
            if(member.chat_id==chat_id){
                int wr=1;
                foreach(power pw in member.powers){
                    vs+=$"{wr} - {pw.name}: {pw.descr} \n";
                    payload[wr-1]= InlineKeyboardButton.WithCallbackData(text: pw.name ,callbackData: "/do "+wr.ToString());
                    wr++;
                }
            }
        }
        vs+="Selecciona una de las acciones anteriores";
        Client.notify(
            chat_id,
            new ClientParams( vs, rS: new InlineKeyboardMarkup(payload)) 
        );
    }
    public Dictionary<string,int> context(){
        // Returns a dictionary of all variables of the party
        Dictionary<string,int>ret=new Dictionary<string,int>(vars);
        ret.Add("deads",deads);
        foreach(player p in members){
            Dictionary<string,int>player_dict=p.context();
            foreach(var prop in player_dict){
                ret[prop.Key]=prop.Value;
            }
        }
        foreach(var prop in vill.context())
            ret.Add(prop.Key,prop.Value);
        return ret;
    }

    public void from_context(Dictionary<string,int>cont){
        // Sets the variables of the party from a dictionary
        char[] delims={'.'};
        List<string>del=new List<string>();
        foreach(var s in cont){
            string[] tokens=s.Key.Split(delims);
            for(int i=0;i<members.Count();i++){
                if( tokens[0]==members[i].c_name ){
                    del.Add(s.Key);
                    members[i].upd_param(tokens[1],s.Value);
                    break;
                }
            }
            if(tokens[0]=="Villain"){
                vill.upd_param(tokens[1],s.Value);
                del.Add(s.Key);    
            }
        }
        deads=cont["deads"];
        del.Add("deads");
        foreach(var s in del)
            cont.Remove(s);
        vars=cont;
    }

    public void start_adventure(){
        // Starts the adventure
        isStarted=true;
        
        interpreter interp=new interpreter(
            Client,
            (string)file.start_code,
            new Dictionary<string, int>(),
            new List<int>(members.map<int, player>((m)=> {return m.chat_id;}))
        );
        interp.run();
        foreach(var h in file.heroes){
                Thread.Sleep(4000);
                Client.notify( 
                    members.map<int, player>((m)=> {return m.chat_id;}),
                    new ClientParams(
                        (string)h.desc,
                        pU: (string)h.img
                    )
                );
        }
        Thread.Sleep(2000);
        string message="Elije a tu heroe:";
        int cont=0;  
        InlineKeyboardButton[] payload= new InlineKeyboardButton[heroSelection.Length];  
        foreach(var hero in file.heroes){
            cont++;
            payload[cont-1]= InlineKeyboardButton.WithCallbackData(text: hero.name.ToString(), callbackData: "/choose_hero "+(cont).ToString());
        }
        Client.notify( 
            members.map<int, player>((m)=> {return m.chat_id;}),
            new ClientParams(
                message,
                rS: new InlineKeyboardMarkup(payload)
            )
        );
    }

    public void choose_hero( int chat_id, int hero_id){
        // Chooses a hero for a player
        hero_id--;
        if(heroSelection[hero_id]){
            Client.notify(
                chat_id,
                new ClientParams("Ese heroe ya ha sido seleccionado, pruebe con otro.")
            );
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

                foreach(var pw in file.heroes[hero_id].powers)
                    member.powers.Add(new power((string)pw[0],(string)pw[1],(string)pw[2]));
                
                string message=$"@{member.user} ha elegido a {member.c_name}:\n Life: {member.life}     Strength: {member.strength}\n Agility: {member.agility}   Mana: {member.mana}";
                heroSelection[hero_id]=true;
                Client.notify( 
                    members.map<int, player>((m)=> {return m.chat_id;}),
                    new ClientParams(message)
                );
                Thread.Sleep(1000);
                break;
            }
        }
        if(chosen_heroes==members.Count())
            start_stage(true);
    }

    public void run_script(string script){
        interpreter interp=new interpreter(
            Client,
            script,
            context(),
            new List<int>(members.map<int, player>((m)=> {return m.chat_id;}))
        );
        interp.run();
        from_context(interp.context); 
        foreach(string a in interp.actions){
            string[] token=a.Split('%');
            if(token[0]=="add")
                foreach(player member in members)
                    if(member.c_name==token[1]){
                        Console.WriteLine("Adding power: "+token[2]+" "+token[3]+" "+token[4]);
                        member.powers.Add(new power(token[2],token[3],token[4]));
                    }
            if(token[0]=="del")
                foreach(player member in members)
                    if(member.c_name==token[1]){
                        Console.WriteLine("deleting power: "+token[2]);
                        member.powers.Remove(member.powers.Find(x => x.name==token[2]));
                    }
                
            Console.WriteLine(a);
        }
    }

    public void encounter(player curr){
        int enc=rnd.Next(0, utils.count_dynamic(file.story[stage].events[curr.h_ref]));
        string encount=(string)file.story[stage].events[curr.h_ref][enc];
        Thread.Sleep(300);
        run_script(encount);
    }

    
    public void do_action(int chat_id,int num){
        if(members[turn].chat_id==chat_id){
            foreach(player member in members){
                if(member.chat_id==chat_id){
                    string action=member.powers[num-1].script;
                    run_script(action);               
                }
            }
            end_turn();
        }else{
            Client.notify(
                (int)chat_id,
                new ClientParams("Solo puedes jugar durante tu turno")
            ); 
        }
    }

    public void print_turn(){ 
        if(members[turn].life>0){
            string message="Turno de: @";
            message+=members[turn].user;
            Thread.Sleep(300);
            Client.notify(
                members.map<int, player>((m)=> {return m.chat_id;}),
                new ClientParams(message)
            );
            encounter(members[turn]);
            if(members[turn].life<0){
                Client.notify(
                    members.map<int, player>((m)=> {return m.chat_id;}),
                    new ClientParams($"🪦 {members[turn].c_name} ha muerto!")
                );        
                deads++;
                end_turn();
            }
        }
        else{
            if(!validateParty()){
                GameOver();
                return;
            }
            end_turn();
        }
    }
    private bool validateParty(){
        foreach(player member in members)
            if(member.life>0)
                return true;

        return false;
    }
    private void GameOver(){
        Client.notify(
            members.map<int, player>((m)=> {return m.chat_id;}),
            new ClientParams("☠️Game Over☠️")
        );
    }

    public void end_turn(){
        turn++;
        if(turn==members.Count())
            turn=0;
        if(vill.life<=0)
            end_stage();   
        if(!finished)print_turn();
    }

    public void start_stage(bool beg=false){
        Thread.Sleep(1000);
        vill.life=(int)file.story[stage].villain.life;
        run_script((string)file.story[stage].beg_code);   
        if(beg)print_turn();
    }

    public void end_game(){
        Thread.Sleep(500);
        Client.notify(
            members.map<int, player>((m)=> {return m.chat_id;}),
            new ClientParams("El juego ha terminado")
        );
        finished=true;
    }

    public void end_stage(){
        run_script((string)file.story[stage].end_code);
        Thread.Sleep(1000);
        stage++;
        if(stage== utils.count_dynamic(file.story)){
            end_game();
            return;
        }
        start_stage();
    }

}
