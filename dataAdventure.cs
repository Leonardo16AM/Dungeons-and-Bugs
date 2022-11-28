using Newtonsoft.Json;
public class DataAdventure{
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
}