using System;

public class CarnivoreFeeding : IFeedingStrategy
{
    public void ExecuteFeeding(Animal animal)
    {
        if (animal.Hunger == 0)
            throw new ZooException($"[Помилка БГ] {animal.Name} не хоче їсти м'ясо, бо вже ситий!");

        animal.MakeSound();
        Console.WriteLine($"Годуємо хижака {animal.Name} свіжим м'ясом 🥩");
    }
}

public class HerbivoreFeeding : IFeedingStrategy
{
    public void ExecuteFeeding(Animal animal)
    {
        if (animal.Hunger == 0)
            throw new ZooException($"[Помилка БГ] {animal.Name} не хоче їсти траву, бо вже ситий!");

        animal.MakeSound();
        Console.WriteLine($"Годуємо травоїдного {animal.Name} свіжим сіном 🌿");
    }
}