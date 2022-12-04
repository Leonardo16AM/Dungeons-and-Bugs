using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
var botClient = new TelegramBotClient("5747520451:AAFUXAYgxTJK7tU4m3HLk3N5ec-5Ks0xGDs");

using var cts = new CancellationTokenSource();

var receiverOptions = new ReceiverOptions{
    AllowedUpdates = Array.Empty<UpdateType>()
};
///////////////////////////////////////////////////////////////////////////////////////////////////////

List<party> parties=new List<party>();
Dictionary<int,int>player_party=new Dictionary<int, int>();
DataAdventure.loadData();
int current_party=1000000;

void add_party(ITelegramBotClient botClient,int leader,string adv_name,string leader_name, string leader_user){
    parties.Add(new party(botClient,current_party,adv_name,leader,leader_name,leader_user));
    player_party.Add(leader,current_party-1000000);
    current_party++;
}

void add_member(int member, string name,string user, int party_id){
    player_party.Add(member,party_id-1000000);
    parties[party_id-1000000].add_member( member, name, user);
}

void send_message(ITelegramBotClient botClient, int chat_id,string message, int reply= -1){
    if(reply != -1)
        botClient.SendTextMessageAsync(chat_id,message,parseMode: ParseMode.MarkdownV2, replyToMessageId: reply);
    else
        botClient.SendTextMessageAsync(chat_id,message,parseMode: ParseMode.MarkdownV2);
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////


botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();
Console.WriteLine($"Server started correctly");
tlg.notify_admins(botClient,"Server started correctly");
Console.ReadLine();
cts.Cancel();





async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken){
    try{
        if (update.Message is not { } message)return;
        if (message.Text is not { } messageText)return;
        var chatId = message.Chat.Id;

        Console.WriteLine($"{chatId}({message.From.FirstName}): {messageText}");
        
        if (messageText.StartsWith("/test") ){// Only for developers
            lexer l=new lexer("3+4*5");
            interpreter i=new interpreter(l);
            Console.WriteLine("startin");
            Console.WriteLine(i.expr());
            Console.WriteLine("endin");
            return;
        }

        if(messageText=="/available_adventures"){
            DataAdventure.printAllAdventures(botClient, chatId, message.MessageId);
            return;
        }

        if (messageText.StartsWith("/new_adventure")){	
            string adv_name=messageText.Substring(15);
            add_party(botClient,(int)chatId,adv_name,message.From.FirstName,message.From.Username);
            send_message(botClient,(int)chatId,$"Adventure created : {current_party-1}", message.MessageId);
            return;
        }
        
        if (messageText.StartsWith("/join_adventure") ){
            int party=int.Parse(messageText.Substring(16,7));

            if(player_party.ContainsKey((int)chatId) && player_party[(int)chatId]==(int)party ){
                send_message(botClient,(int)chatId,"You are already in this adventure");
                return;
            }
            if(parties[party-1000000].isStarted){
                send_message(botClient,(int)chatId,"That adventure is already started, try another or create a new one");
                return;
            }

            add_member((int)chatId ,message.From.FirstName,message.From.Username, party);
            return;
        }

        if(messageText=="/start_adventure"){
            if( !player_party.ContainsKey((int)chatId) ){
                send_message(botClient,(int)chatId,"You have to host an adventure first, type new_adventure to host it", (int)message.MessageId);
                return;
            }
            parties[player_party[(int)chatId]].start_adventure();
            return;
        }

        if(messageText.StartsWith("/choose_hero")){
            parties[player_party[(int)chatId]].choose_hero((int)chatId,int.Parse(messageText.Substring(13)) );
            return;
        }

        if(!player_party.ContainsKey((int)chatId)){
            send_message(botClient,(int)chatId,"You are not in any adventure, create one or join one", message.MessageId);
            return;
        }        

        if (messageText.StartsWith("/chat") ){
            string mess=messageText.Substring(5);
            if(mess=="") return;
            parties[player_party[(int)chatId]].notify_members($"@{message.From.Username}: {mess}", new long[] {chatId});
            return;
            
        }

        if (messageText.StartsWith("/action") ){
            parties[player_party[(int)chatId]].action();
            parties[player_party[(int)chatId]].end_turn();
            return;
        }

        if (messageText.StartsWith("/variables") ){
            parties[player_party[(int)chatId]].print_vars((int)chatId);
            return;
        }
        send_message(botClient,(int)chatId,"Unknown command");
    
    }catch (Exception e){

        Console.WriteLine("{0}Exception caught.", e);
        if (update.Message is not { } message)return;
        if (message.Text is not { } messageText)return;
        var chatId = message.Chat.Id;
        send_message(botClient,(int)chatId,"Bad command ussage or error");
    }
}











Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken){
    var ErrorMessage = exception switch{
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };
    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}
