using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

class party:adventure{
    public int id;
    public int leader;
    public List<player> members;

    int turn;
    public bool isStarted = false;
    public adventure adv;

    public party(int id,string adv_name,int leader,string leader_name,string leader_user):base(adv_name){
        this.id=id;
        this.leader=leader;
        adv=new adventure(adv_name);
        members=new List<player>();
        members.Add(new player(leader,leader_name,leader_user));
    }

    void send_message(ITelegramBotClient botClient, int chat_id,string message, int reply= -1){
        if(reply != -1)
            botClient.SendTextMessageAsync(chat_id,message,parseMode: ParseMode.MarkdownV2, replyToMessageId: reply);
        else
            botClient.SendTextMessageAsync(chat_id,message,parseMode: ParseMode.MarkdownV2);
    }


    public void notify_members(ITelegramBotClient botClient ,string message){
        foreach(player member in members){
            send_message(botClient,member.chat_id,message);
        }
    }

    public void add_member(ITelegramBotClient botClient ,int member, string name,string user){
        members.Add(new player(member,name,user));
        string message=$"@{user} joined the adventure";
        notify_members(botClient, message);
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
