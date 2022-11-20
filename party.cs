using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

class party{
    public int id;
    public int leader;
    public List<int> members;

    int turn;
    public bool isStarted = false;

    public party(int id,int leader){
        this.id=id;
        this.leader=leader;
        members=new List<int>();
        members.Add(leader);
    }


    public void notify_members(ITelegramBotClient botClient,string message){
        foreach(int member in members){
            Console.WriteLine($" {member} notified");
            botClient.SendTextMessageAsync(member,message);
        }
    }

    public void add_member(ITelegramBotClient botClient ,int member, string name){
        members.Add(member);
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
        return members[turn];
    }

    public void action(ITelegramBotClient botClient,int chat_id,string action){
        if(chat_id==get_turn()){
            notify_members(botClient,action);
            pass_turn();
        }
    }



}
