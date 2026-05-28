using System;

namespace ZooSimulator
{
    public interface IObserver
    {
        void Update(string animalName, int health);
    }

    public class Veterinarian : IObserver
    {
        public string Name { get; set; }
        public Veterinarian(string name) { Name = name; }

        public void Update(string animalName, int health)
        {
            if (health < 30)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n   [УВАГА] Ветеринар {Name}: Здоров'я {animalName} критичне ({health}%)! Виїжджаю для надання допомоги.");
                Console.ResetColor();
            }
        }
    }
}