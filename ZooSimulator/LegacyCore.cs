using System;

namespace ZooSimulator
{
   
    public class IntensiveFeeding : IFeedingStrategy
    {
        // Виправлено: метод тепер повертає string, як вимагає інтерфейс IFeedingStrategy
        public string Feed(string dietType, Food food)
        {
            return $"[Стратегія: Посилене харчування] Оброблено тип раціону {dietType}.";
        }

        public void ExecuteFeeding(Animal animal)
        {
            animal.Hunger = Math.Max(0, animal.Hunger - 40);
            animal.Health = Math.Min(100, animal.Health + 15);
            Console.WriteLine($"[Стратегія: Посилене харчування] {animal.Name} активно набирає сили.");
        }
    }

    public class DietaryFeeding : IFeedingStrategy
    {
        // Виправлено: метод тепер повертає string, як вимагає інтерфейс IFeedingStrategy
        public string Feed(string dietType, Food food)
        {
            return $"[Стратегія: Дієтичне харчування] Оброблено тип раціону {dietType}.";
        }

        // Старий метод для сумісності з текстом звіту
        public void ExecuteFeeding(Animal animal)
        {
            animal.Hunger = Math.Max(0, animal.Hunger - 15);
            animal.Health = Math.Min(100, animal.Health + 5);
            Console.WriteLine($"[Стратегія: Дієтичне харчування] {animal.Name} на дієті.");
        }
    }

    public interface IHealthObserver
    {
        void Update(Animal animal);
    }

    public class Vet : IHealthObserver, IObserver
    {
        public string Name { get; set; }
        public string Specialization { get; set; }

        public Vet(string name, string specialization)
        {
            Name = name;
            Specialization = specialization;
        }

        public void Update(string animalName, string condition, int healthValue)
        {
            if (healthValue < 30)
            {
                Console.WriteLine($"[Медична допомога] {Name} помітив критичний стан {animalName} ({condition}) та надав допомогу.");
            }
        }

        public void Update(Animal animal)
        {
            if (animal.Health < 30 || animal.Hunger > 70)
            {
                animal.Health = Math.Min(100, animal.Health + 25);
                Console.WriteLine($"[Медична допомога] {Name} провів терапію для {animal.Name}.");
            }
        }
    }

    public interface IAnimalFactory
    {
        Animal CreateAnimal(string name, string category, string dietChoice);
    }

    public class LegacyAfricanZooFactory : IAnimalFactory
    {
        public Animal CreateAnimal(string name, string category, string dietChoice)
        {
            IZooFactory internalFactory = new AfricanZooFactory();
            return internalFactory.CreateAnimal(name, category, dietChoice);
        }
    }
}