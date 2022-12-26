using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
var botClient = new TelegramBotClient("5747520451:AAFUXAYgxTJK7tU4m3HLk3N5ec-5Ks0xGDs");

using var cts = new CancellationTokenSource();

var receiverOptions = new ReceiverOptions{
    AllowedUpdates = Array.Empty<UpdateType>()
};
///////////////////////////////////////////////////////////////////////////////////////////////////////

IClient telegram= new TlgClient("5747520451:AAFUXAYgxTJK7tU4m3HLk3N5ec-5Ks0xGDs");

//////////////////////////////////////////////////////////////////////////////////////////////////////
List<party> parties=new List<party>();
Dictionary<int,int>player_party=new Dictionary<int, int>();
DataAdventure Data= new DataAdventure(telegram);
int current_party=1000000;


void add_party(int leader,string adv_name,string leader_name, string leader_user){
    parties.Add(new party(telegram,current_party,adv_name,leader,leader_name,leader_user));
    player_party.Add(leader,current_party-1000000);
    current_party++;
}

void add_member(int member, string name,string user, int party_id){
    player_party.Add(member,party_id-1000000);
    parties[party_id-1000000].add_member( member, name, user);
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
((TlgClient)telegram).notifyAdmins(new ClientParams("Server started correctly"));
Console.ReadLine();
cts.Cancel();





async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken){
    try{
        if (update.Message is not { } message) { //CallBackHandling
            if(update.CallbackQuery is not {} callb) return;
            
            var cId=callb.Message.Chat.Id;
            Console.WriteLine($"{cId}({callb.Message.Chat.FirstName}): {callb.Data}");

            if(callb.Data.StartsWith("/choose_hero")){
                parties[player_party[(int)cId]].choose_hero((int)cId,int.Parse(callb.Data.Substring(13)) );
            }

            if(callb.Data.StartsWith("/new_adv")){
                string adv_name=callb.Data.Substring(9);
                add_party((int)cId,adv_name,callb.Message.Chat.FirstName,callb.Message.Chat.Username);
                telegram.notify(
                    new int[] {(int)cId},
                    new ClientParams(
                        $"Adventure created. Send to your friends the following command to join the adventure /join {current_party-1}",
                        rM: callb.Message.MessageId)
                );
            }
            if (callb.Data.StartsWith("/do") ){
                string mess=callb.Data.Substring(3);
                parties[player_party[(int)cId]].do_action((int)cId,int.Parse(mess));
                return;
            }

            return;
        }
            
        if (message.Text is not { } messageText)return;
        var chatId = message.Chat.Id;

        Console.WriteLine($"{chatId}({message.From.FirstName}): {messageText}");
        
        if (messageText.StartsWith("/run") ){// Only for developers
            if( (int)chatId==789850916 || (int)chatId==639646249 ){
                string script= messageText.Substring(5);
                parties[player_party[(int)chatId]].run_script(script);
            }
            return;
        }

        if (messageText.StartsWith("/new_adventure")){
            if(player_party.ContainsKey((int)chatId)){
                telegram.notify(
                    new int[] {(int)chatId},
                    new ClientParams(
                        "You are already in one adventure, if you want to host a new one /quit the current",
                        rM: (int)message.MessageId
                    )
                );
            }
            Data.printAllAdventures((int)chatId, message.MessageId);	
            return;
        }
        
        if (messageText.StartsWith("/join") ){
            int party=int.Parse(messageText.Substring(6,7));

            if(player_party.ContainsKey((int)chatId) && player_party[(int)chatId]==(int)party ){
                telegram.notify(
                    new int[] {(int)chatId},
                    new ClientParams("You are already in this adventure")
                );
                return;
            }
            if(parties[party-1000000].isStarted){
                telegram.notify(
                    new int[]{(int)chatId},
                    new ClientParams("That adventure is already started, try another or create a new one")
                );
                return;
            }

            add_member((int)chatId, message.From.FirstName, message.From.Username , party);
            return;
        }

        if(messageText=="/start_adventure"){
            if( !player_party.ContainsKey((int)chatId) ){
                telegram.notify(
                    new int[] {(int)chatId},
                    new ClientParams(
                        "You have to host an adventure first, type /new_adventure to host it",
                        rM:(int)message.MessageId
                    )
                );
                return;
            }
            parties[player_party[(int)chatId]].start_adventure();
            return;
        }

        if(!player_party.ContainsKey((int)chatId)){
            telegram.notify(
                new int [] {(int)chatId},
                new ClientParams(
                    "You are not in any adventure, create one or join one",
                    rM: message.MessageId
                )
            );
            return;
        }        

        if(messageText.StartsWith("/variables") ){
            parties[player_party[(int)chatId]].print_vars((int)chatId);
            return;
        }

        if(messageText.StartsWith("/actions") ){
            parties[player_party[(int)chatId]].actions((int)chatId);
            return;
        }

        if( !messageText.StartsWith("/") ){
            if(messageText=="") return;
            parties[player_party[(int)chatId]].chat($"@{message.From.Username}: {messageText}", (int)chatId);
            return;            
        }

        telegram.notify(
            new int []{(int)chatId},
            new ClientParams("Unknown command")
        );
    
    }catch (Exception e){

        Console.WriteLine("{0} Exception caught.", e);
        if (update.Message is not { } message)return;
        if (message.Text is not { } messageText)return;
        var chatId = message.Chat.Id;
        telegram.notify(
            new int [] {(int)chatId},
            new ClientParams ("Bad command ussage or error")
        );
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
