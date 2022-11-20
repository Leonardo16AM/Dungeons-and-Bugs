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
int current_party=1000000;

void add_party(int leader){
    parties.Add(new party(current_party,leader));
    player_party.Add(leader,current_party-1000000);
    current_party++;
}

void add_member(int member, string name, int party_id){
    parties[party_id-1000000].add_member(botClient, member, name);
    player_party.Add(member,party_id-1000000);
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
Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();
cts.Cancel();





async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken){
    if (update.Message is not { } message)return;
    if (message.Text is not { } messageText)return;
    var chatId = message.Chat.Id;

    Console.WriteLine($"{chatId}({message.From.FirstName}): {messageText}");

    if (messageText=="/new_adventure"){	
        add_party((int)chatId);
        send_message(botClient,(int)chatId,$"Adventure created : {current_party-1}", message.MessageId);
        return;
    }
    
    if (messageText.StartsWith("/join_adventure") ){
        int party=int.Parse(messageText.Substring(16,7));
        if(parties[party-1000000].isStarted){
            send_message(botClient,(int)chatId,"That adventure is already started, try another or create a new one");
            return;
        }
        add_member((int)chatId ,message.From.Username, party);
        send_message(botClient,(int)chatId,$"Joined to adventure {party}", message.MessageId);
        return;
    }

    if(messageText=="/start_adventure"){
        parties[player_party[(int)chatId]].isStarted = true;
        parties[player_party[(int)chatId]].notify_members(botClient,$"Adventure Time!!! @{message.From.Username} has started the adventure");
        return;
    }

    if(!player_party.ContainsKey((int)chatId)){
        send_message(botClient,(int)chatId,"You are not in any adventure, create one or join one", message.MessageId);
        return;
    }


    if (messageText.StartsWith("/chat") ){
        string mess=messageText.Substring(5);
        parties[player_party[(int)chatId]].notify_members(botClient,$"Message from @{message.From.Username}: {mess}");
        return;
    }
    

    send_message(botClient,(int)chatId,"Unknown command");
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
