using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;


public class TlgClient : IClient
{
    IEnumerable<int> admins= new List<int> { 789850916, 639646249};
    TelegramBotClient botClient;

    public TlgClient(string botID)
    {
        botClient = new TelegramBotClient(botID);
    }
    public void notify(IEnumerable<int> chatIds, IClientParams param, IEnumerable<int>? exc=null)
    {
        foreach (int chatId in chatIds)
        {   
            bool next=false;
            if(exc!=null)
                foreach(int integer in exc)
                    if(chatId==integer)
                        next=true;

            if(next)
                continue;
                
            notify(chatId,param);
        }
        
    }
    
    public void notify(int chatId, IClientParams param){
        if (((ClientParams)param).picUrl == null)
            {
                botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: param.text,
                    parseMode: ((ClientParams)param).parseMode == null ? null : ((ClientParams)param).parseMode,
                    disableWebPagePreview: ((ClientParams)param).disablePreview == null ? null : ((ClientParams)param).disablePreview,
                    disableNotification: ((ClientParams)param).disableNotif == null ? null : ((ClientParams)param).disableNotif,
                    replyToMessageId: ((ClientParams)param).replyMessage != null ? ((ClientParams)param).replyMessage : null,
                    replyMarkup: ((ClientParams)param).replyStyle == null ? null : ((ClientParams)param).replyStyle
                );
            }
            else
            {
                botClient.SendPhotoAsync(
                    chatId: chatId,
                    caption: param.text,
                    photo: ((ClientParams)param).picUrl!,
                    parseMode: ParseMode.Html,
                    disableNotification: ((ClientParams)param).disableNotif == null ? null : ((ClientParams)param).disableNotif,
                    replyToMessageId: ((ClientParams)param).replyMessage != null ? ((ClientParams)param).replyMessage : null,
                    replyMarkup: ((ClientParams)param).replyStyle == null ? null : ((ClientParams)param).replyStyle
                );
            }
    }
    public void notifyAdmins(IClientParams param){
        notify(admins, param);
    }
}
public class ClientParams : IClientParams
{
    public string text { get; }
    public ParseMode? parseMode;
    public bool? disablePreview;
    public bool? disableNotif;
    public int? replyMessage;
    public string? picUrl;
    public IReplyMarkup? replyStyle;
    public ClientParams(
            string txt,
            string? pU = null,
            int? rM = null,
            bool? dN = null,
            bool? dP = null,
            IReplyMarkup? rS = null,
            ParseMode? pm = null
    )
    {
        text = txt;
        parseMode = pm;
        disablePreview = dP;
        disableNotif = dN;
        replyMessage = rM;
        picUrl = pU;
        replyStyle = rS;
    }
}