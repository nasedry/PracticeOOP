using System;
using System.Collections.Generic;

namespace ZooSimulator
{
    public class ZooException : Exception
    {
        public ZooException(string message) : base(message) { }
    }

    public abstract class Animal
    {
        private int _health;
        private int _hunger;
        private readonly List<IObserver> _observers = new List<IObserver>();
        private readonly object _lock = new object();

        public string Name { get; set; }
        public IFeedingStrategy? FeedingStrategy { get; set; }
        public string Category { get; set; }

        public int Hunger
        {
            get { lock (_lock) return _hunger; }
            set { lock (_lock) _hunger = Math.Clamp(value, 0, 100); }
        }

        public int Health
        {
            get { lock (_lock) return _health; }
            set
            {
                lock (_lock) _health = Math.Clamp(value, 0, 100);
                NotifyObservers();
            }
        }

        protected Animal(string name, string category, int health, int hunger)
        {
            Name = name;
            Category = category;
            _health = health;
            _hunger = hunger;
        }

        public void SetFeedingStrategy(IFeedingStrategy strategy) => FeedingStrategy = strategy;
        public void RegisterObserver(IObserver observer) => _observers.Add(observer);
        
        private void NotifyObservers()
        {
            foreach (var observer in _observers)
            {
                observer.Update(Name, _health);
            }
        }

        public void Feed(Food food)
        {
            if (Hunger <= 0)
                throw new ZooException($"Тварина {Name} абсолютно сита. Годування скасовано!");

            if (FeedingStrategy == null)
                throw new ZooException($"Для тварини {Name} не задано раціон!");

            string resultMessage = FeedingStrategy.Feed(Name, food);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"   {resultMessage}");
            Console.ResetColor();
            
            Hunger -= food.NutritionalValue;
            Console.WriteLine($"   Поточний рівень голоду для {Name}: {Hunger}%");
        }
    }

    public class GenericAnimal : Animal 
    { 
        public GenericAnimal(string name, string category, int health, int hunger) : base(name, category, health, hunger) { } 
    }

    public interface IZooFactory
    {
        Animal CreateCustomAnimal(string name, string category, string dietChoice);
    }

    public class UniversalZooFactory : IZooFactory
    {
        public Animal CreateCustomAnimal(string name, string category, string dietChoice)
        {
            var animal = new GenericAnimal(name, category, 100, 30);
            
            if (dietChoice == "1")
                animal.SetFeedingStrategy(new CarnivoreFeeding());
            else if (dietChoice == "2")
                animal.SetFeedingStrategy(new HerbivoreFeeding());
            else
                animal.SetFeedingStrategy(new OmnivoreFeeding());

            return animal;
        }
    }
}