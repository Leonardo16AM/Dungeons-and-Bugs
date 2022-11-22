using Newtonsoft.Json;

class hero{
    public string name,real_name,historia,picture;
    public int life,strength, agility,mana;
}



class adventure{
    public dynamic file;

    public adventure(string name){
        string json = File.ReadAllText($"adventures/{name}/berserk.json");
        dynamic file = JsonConvert.DeserializeObject(json);
    }
}