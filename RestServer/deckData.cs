using System.Globalization;
using System.Collections.Generic;

public class RootObject
{
    public int resultCount { get; set; }
    public List <deckData> results { get; set; }
}

public class deckData
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int Damage { get; set; }

    public string GetId()
    {
        return Id;
    }
    public string GetName()
    {
        return Name;
    }
    public string GetDamage()
    {
        return Damage.ToString();
    }

    public string GetDeckInfo()
    {
        return "Id: " + Id + " Name: " + Name + " Damage: " + Damage;
    }
}
