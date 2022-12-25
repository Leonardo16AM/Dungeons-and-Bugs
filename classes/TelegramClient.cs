using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;


public class TlgClient : IClient
{
    int[] admins= { 789850916, 639646249};
    TelegramBotClient botClient;

    public TlgClient(string botID)
    {
        botClient = new TelegramBotClient(botID);
    }
    public void notify(int[] chatIds, IClientParams param, int[]? exc=null)
    {
        if (((ClientParams)param).picUrl == null)
        {
            foreach (int chatId in chatIds)
            {
                botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: param.text,
                    parseMode: ((ClientParams)param).parseMode == null ? null : ((ClientParams)param).parseMode,
                    disableWebPagePreview: ((ClientParams)param).disablePreview == null ? null : ((ClientParams)param).disablePreview,
                    disableNotification: ((ClientParams)param).disableNotif == null ? null : ((ClientParams)param).disableNotif,
                    replyToMessageId: chatIds.Length == 1 && ((ClientParams)param).replyMessage != null ? ((ClientParams)param).replyMessage : null,
                    replyMarkup: ((ClientParams)param).replyStyle == null ? null : ((ClientParams)param).replyStyle
                );
            }
        }
        else
        {
            foreach (int chatId in chatIds)
            {
                botClient.SendPhotoAsync(
                    chatId: chatId,
                    caption: param.text,
                    photo: ((ClientParams)param).picUrl!,
                    parseMode: ParseMode.Html,
                    disableNotification: ((ClientParams)param).disableNotif == null ? null : ((ClientParams)param).disableNotif,
                    replyToMessageId: chatIds.Length == 1 && ((ClientParams)param).replyMessage != null ? ((ClientParams)param).replyMessage : null,
                    replyMarkup: ((ClientParams)param).replyStyle == null ? null : ((ClientParams)param).replyStyle
                );
            }
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