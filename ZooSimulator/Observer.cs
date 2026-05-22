using System;

public class VetObserver : IHealthObserver
{
    private string _name;

    public VetObserver(string name)
    {
        _name = name;
    }

    public void Update(Animal animal)
    {
        if (animal.Health < 50)
        {
            Console.WriteLine($"[ОБСЕРВЕР] Ветеринар {_name}: УВАГА! Тварина {animal.Name} має критичне здоров'я ({animal.Health}%)!");
        }
    }
}