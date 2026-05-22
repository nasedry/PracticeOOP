using System;

public class Mammal : Animal
{
    // Обов'язковий порожній конструктор для XML-серіалізації
    public Mammal() : base() 
    { 
    } 

    public Mammal(string name) : base(name) 
    { 
    }

    public Mammal(string name, int health, int hunger) : base(name, health, hunger) 
    { 
    }

    public override string GetSpeciesType() => "Ссавець";
    
    public override void MakeSound() => Console.WriteLine($"{Name} каже: Ррррр! ");
}