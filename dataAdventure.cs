using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

public static class DataAdventure{
    public static Dictionary<string, dynamic> Adventures = new Dictionary<string, dynamic>();
    
    public static void loadData(){
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
    
    public static void printAllAdventures(ITelegramBotClient botClient,long chatId, long reply=-1){
        if(Adventures.Count()==0){
            tlg.send_message(botClient, (int)chatId, "No hay aventuras disponibles.",(int)reply);
            return;
        }
        string s="Aventuras Disponibles: \n";
        int counter=1;
        foreach(KeyValuePair<string, dynamic> adv in Adventures){
            s+= $"{counter.ToString()} - {adv.Key} \n";
        }
        tlg.send_message(botClient, (int)chatId, s, (int)reply);;
    }

}