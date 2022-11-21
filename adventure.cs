using Newtonsoft.Json;

class hero{
    public string name,real_name,historia,picture;
    public int life,strength, agility,mana;
}



class adventure{
    public string name,full_name;

    public adventure(string name){
        // string json = File.ReadAllText($"adventures/{name}");
        // dynamic file = JsonConvert.DeserializeObject(json);
        // full_name=file.name;
    }
}