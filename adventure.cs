using Newtonsoft.Json;

class hero{
    public string name,real_name,historia,picture;
    public int life,strength, agility,mana;
}



class adventure{
    public string adv;
    public dynamic file { get; set; }
    public string json { get; set; }
    
    public adventure(string name){
        adv=name;
        json = File.ReadAllText($"adventures/{name}/berserk.json");
        file = JsonConvert.DeserializeObject(json);
        Console.WriteLine(file.heroes[0][1]);
    }
}