using System;

public class Bird : Animal
{
    // Обов'язковий порожній конструктор для XML-серіалізації
    public Bird() : base() 
    { 
    } 

    public Bird(string name) : base(name) 
    { 
    }

    public Bird(string name, int health, int hunger) : base(name, health, hunger) 
    { 
    }

    public override string GetSpeciesType() => "Птах";
    
    public override void MakeSound() => Console.WriteLine($"{Name} каже: Чик-чирик! ");
}