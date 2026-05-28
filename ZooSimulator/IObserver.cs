using System;

namespace ZooSimulator
{
    public interface IObserver
    {
        string Name { get; set; }
        string Specialization { get; set; }
        void Update(string animalName, string animalCategory, int health);
    }

    public class Veterinarian : IObserver
    {
        public string Name { get; set; }
        public string Specialization { get; set; }

        public Veterinarian(string name, string specialization)
        {
            Name = name;
            Specialization = specialization;
        }

        public void Update(string animalName, string animalCategory, int health)
        {
            if (health < 30)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n   [ТРИВОГА] Ветеринар {Name} (Спеціаліст по: {Specialization}): Здоров'я тварини {animalName} ({animalCategory}) критичне: {health}%! Терміново виїжджаю!");
                Console.ResetColor();
            }
        }
    }
}