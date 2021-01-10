using System.Globalization;
using System.Collections.Generic;

public class RootObject
{
    public int cardCount { get; set; }
    public List <DeckData> card { get; set; }
}

public class DeckData
{
    public string Id { get; set; }
    public string Name { get; set; }
    public float Damage { get; set; }

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
        return "{\"Id\":\"" + Id + "\", \"Name\":\"" + Name + "\", \"Damage\":" + Damage  + "}";
    }

    public string UserDeckInfo()
    {
        return "Card-ID: " + Id + "\nCard-Name: " + Name + "\nDamage: " + Damage + "\n";
    }
}
