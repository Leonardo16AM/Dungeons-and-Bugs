
using Telegram.Bot;
using Telegram.Bot.Types.Enums;




public static class tlg{
    public static void send_message(ITelegramBotClient botClient, int chat_id,string message, int reply= -1){
        if(reply != -1)
            botClient.SendTextMessageAsync(chat_id,message, replyToMessageId: reply);
        else
            botClient.SendTextMessageAsync(chat_id,message);
    }
    
    public static void send_message_md(ITelegramBotClient botClient, int chat_id,string message, int reply= -1){
        if(reply != -1)
            botClient.SendTextMessageAsync(chat_id,message,parseMode: ParseMode.MarkdownV2, replyToMessageId: reply);
        else
            botClient.SendTextMessageAsync(chat_id,message,parseMode: ParseMode.MarkdownV2);
    }

    public static void send_picture(ITelegramBotClient botClient,string message, int chat_id, string picture){
            botClient.SendPhotoAsync(
                chatId: chat_id,
                photo: picture,
                caption: message,
                parseMode: ParseMode.Html);
    }

}