using System;
using System.Collections.Generic;
using System.Xml.Serialization;

[XmlInclude(typeof(Mammal))]
[XmlInclude(typeof(Bird))]
public abstract class Animal
{
    private string _name = string.Empty;
    
    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Ім'я не може бути порожнім!");
            _name = value;
        }
    }

    public int Health { get; set; }
    public int Hunger { get; set; }

    [XmlIgnore] // Серіалізатор не повинен зберігати підписників
    private readonly List<IHealthObserver> _observers = new List<IHealthObserver>();

    public Animal() { } // Порожній конструктор обов'язковий для серіалізації

    public Animal(string name)
    {
        Name = name;
        Health = 100;
        Hunger = 0;
    }

    public Animal(string name, int health, int hunger)
    {
        Name = name;
        Health = health;
        Hunger = hunger;
    }

    public void RegisterObserver(IHealthObserver observer)
    {
        _observers.Add(observer);
    }

    public void ChangeHealth(int newHealth)
    {
        Health = newHealth;
        foreach (var observer in _observers)
        {
            observer.Update(this);
        }
    }

    public abstract string GetSpeciesType();
    public virtual void MakeSound() => Console.WriteLine("Тварина видає звук.");
}