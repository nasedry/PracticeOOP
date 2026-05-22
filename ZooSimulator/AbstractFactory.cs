public class AfricanZooFactory : IZooFactory
{
    public Animal CreateAnimal(string name) => new Mammal(name, 100, 30);
    public string CreateEnclosure() => "Савана-вольєр з підігрівом та піском";
    public string CreateStaff(string name) => $"Доглядач африканських тварин {name}";
    public string CreateFood() => "Преміум м'ясо для левів 🥩";
}

public class ArcticZooFactory : IZooFactory
{
    public Animal CreateAnimal(string name) => new Bird(name, 100, 10);
    public string CreateEnclosure() => "Аква-вольєр з льодом та охолодженням";
    public string CreateStaff(string name) => $"Доглядач полярних птахів {name}";
    public string CreateFood() => "Свіжа заморожена риба 🐟";
}