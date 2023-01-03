using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

public class DataAdventure{
    public static Dictionary<string, dynamic> Adventures = new Dictionary<string, dynamic>();
    IClient Client;
    
    public DataAdventure(IClient client){
        Client=client;
        loadData();
    }
    public void loadData(){
        string[] paths= Directory.GetFiles("adventures//", "*.json",SearchOption.AllDirectories);
        for(int i=0;i<paths.Length;i++){
            string json = File.ReadAllText(paths[i]);
            dynamic file = JsonConvert.DeserializeObject(json);
            if(file!=null){
                string token = Convert.ToString(file?.token);
                Adventures.Add(token,file);
            }
        }
            
    }
    
    public void printAllAdventures(int chatId, long reply=-1){
        if(Adventures.Count()==0){
            Client.notify(
                (int)chatId,
                new ClientParams("No hay aventuras disponibles.", rM: (int)reply)
            );
            return;
        }

        string s="Aventuras Disponibles: \n";
        int counter=0;
        InlineKeyboardButton[] payload= new InlineKeyboardButton[Adventures.Count()];
        foreach(KeyValuePair<string, dynamic> adv in Adventures){
            payload[counter]= InlineKeyboardButton.WithCallbackData(
                text: $"{(counter+1).ToString()} - {adv.Value.name} \n", 
                callbackData: $"/new_adv {adv.Key}"
            );
            counter++;
        }

        Client.notify(
            (int)chatId,
            new ClientParams(
                s,
                rS: new InlineKeyboardMarkup(payload),
                rM: reply==-1? null: (int)reply
            )
        );
    }

}