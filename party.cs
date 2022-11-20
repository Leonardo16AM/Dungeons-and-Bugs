using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

class party{
    public int id;
    public int leader;
    public List<player> members;

    int turn;
    public bool isStarted = false;

    public party(int id,int leader,string leader_name,string leader_user){
        this.id=id;
        this.leader=leader;
        members=new List<player>();
        members.Add(new player(leader,leader_name,leader_user));
    }


    public void notify_members(ITelegramBotClient botClient,string message){
        foreach(player member in members){
            Console.WriteLine($" {member} notified");
            botClient.SendTextMessageAsync(member.chat_id,message);
        }
    }

    public void add_member(ITelegramBotClient botClient ,int member, string name,string user){
        members.Add(new player(member,name,user));
        Console.WriteLine($"notifieng memebers {member} {name}");
        notify_members(botClient, $"{name} joined the adventure");
   
        Console.WriteLine(" memebers notified");
    }


    public void pass_turn(){
        turn++;
        if(turn>=members.Count){
            turn=0;
        }
    }

    public int get_turn(){
        return members[turn].chat_id;
    }

    public void action(ITelegramBotClient botClient,int chat_id,string action){
        if(chat_id==get_turn()){
            notify_members(botClient,action);
            pass_turn();
        }
    }



}
