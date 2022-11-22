using Newtonsoft.Json;


class adventure{
    public string adv;
    public dynamic file { get; set; }
    public string json { get; set; }
    
    public adventure(string name){
        adv=name;
        json = File.ReadAllText($"adventures/{name}/berserk.json");
        file = JsonConvert.DeserializeObject(json);
    }
}