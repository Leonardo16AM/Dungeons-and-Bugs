
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

    public static void notify_members(ITelegramBotClient botClient,List<int>chat_ids,string message){
        foreach(int chat_id in chat_ids){
            send_message(botClient, chat_id,message);
        }
    }

    public static void notify_admins(ITelegramBotClient botClient,string message){
        send_message(botClient, 789850916,message);
        send_message(botClient, 639646249,message);
    }
}