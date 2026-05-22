using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ZooSimulator
{
    // ======================================================================
    // 1. КАСТОМНІ ВИНЯТКИ ТА ПОЛІТИКА БЕЗПЕКИ (Завдання 5)
    // ======================================================================
    public class ZooException : Exception
    {
        public ZooException(string message) : base(message) { }
    }

    // ======================================================================
    // 2. ПАТЕРН STRATEGY — РЕЖИМИ ГОДУВАННЯ (Завдання 4)
    // ======================================================================
    public interface IFeedingStrategy
    {
        string Feed(string animalName, Food food);
    }

    public class CarnivoreFeeding : IFeedingStrategy
    {
        public string Feed(string animalName, Food food)
        {
            if (food.Type.ToLower() != "м'ясо")
                throw new ZooException($"Хижак {animalName} відмовляється їсти {food.Type}! Йому потрібне лише м'ясо.");
            return $"{animalName} з апетитом з'їв м'ясо (поживність: +{food.NutritionalValue}).";
        }
    }

    public class HerbivoreFeeding : IFeedingStrategy
    {
        public string Feed(string animalName, Food food)
        {
            if (food.Type.ToLower() == "м'ясо")
                throw new ZooException($"Травоїдна тварина {animalName} лякається і відмовляється їсти м'ясо!");
            return $"{animalName} спокійно жує {food.Type} (поживність: +{food.NutritionalValue}).";
        }
    }

    // ======================================================================
    // 3. ПАТЕРН OBSERVER — МОНІТОРИНГ СТАНУ ЗДОРОВ'Я (Завдання 4)
    // ======================================================================
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
                Console.WriteLine($"   [OBSERVER ТРИВОГА] Ветеринар {Name}: Здоров'я {animalName} критичне ({health}%)! Терміново виїжджаю з ліками!");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine($"   [OBSERVER ЛОГ] Ветеринар {Name}: Оновлено стан {animalName}. Поточне здоров'я: {health}%.");
            }
        }
    }

    // ======================================================================
    // 4. БАЗОВІ СУТНОСТІ ЗООПАРКУ (Завдання 2)
    // ======================================================================
    public class Food
    {
        public string Type { get; set; }
        public int NutritionalValue { get; set; }
        public Food(string type, int value) { Type = type; NutritionalValue = value; }
    }

    public class Employee
    {
        public string Name { get; set; }
        public string Position { get; set; }
        public Employee(string name, string position) { Name = name; Position = position; }
    }

    public abstract class Animal
    {
        private int _health;
        private readonly List<IObserver> _observers = new List<IObserver>();

        public string Name { get; set; }
        public int Hunger { get; set; } // 0 - сита, 100 - дуже голодна
        public IFeedingStrategy? FeedingStrategy { get; set; }

        public int Health
        {
            get => _health;
            set
            {
                _health = value;
                NotifyObservers();
            }
        }

        protected Animal(string name, int health, int hunger)
        {
            Name = name;
            _health = health;
            Hunger = hunger;
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
                throw new ZooException($"Тварина {Name} абсолютно сита (Hunger = 0). Годування скасовано для запобігання перевитрати кормів!");

            if (FeedingStrategy == null)
                throw new ZooException($"Для тварини {Name} не задано стратегію годування!");

            // Викликаємо алгоритм стратегії
            string resultMessage = FeedingStrategy.Feed(Name, food);
            Console.WriteLine($"   [Strategy] {resultMessage}");
            
            // Зменшуємо рівень голоду після їжі
            Hunger = Math.Max(0, Hunger - food.NutritionalValue);
            Console.WriteLine($"   -> Поточний рівень голоду для {Name}: {Hunger}%");
        }
    }

    // Конкретні сутності тварин (Спадкоємці)
    public class Mammal : Animal
    {
        public Mammal(string name, int health, int hunger) : base(name, health, hunger) { }
    }

    public class Bird : Animal
    {
        public class Иird : Animal { public Иird(string name, int health, int hunger) : base(name, health, hunger) { } }
        public Bird(string name, int health, int hunger) : base(name, health, hunger) { }
    }

    // Сутність Вольєр
    public class Enclosure
    {
        public string Name { get; set; }
        public string EnvironmentType { get; set; }
        public List<Animal> Animals { get; set; } = new List<Animal>();

        public Enclosure(string name, string envType) { Name = name; EnvironmentType = envType; }
    }

    // ======================================================================
    // 5. ПАТЕРН ABSTRACT FACTORY — РОДИНИ ТВАРІН ТА СЕРЕДОВИЩА (Завдання 3)
    // ======================================================================
    public interface IZooFactory
    {
        Animal CreateAnimal(string name);
        Enclosure CreateEnclosure(string name);
    }

    // Африканська родина (Хижаки)
    public class AfricanZooFactory : IZooFactory
    {
        public Animal CreateAnimal(string name)
        {
            var lion = new Mammal(name, 100, 60);
            lion.SetFeedingStrategy(new CarnivoreFeeding()); // Автоматично призначаємо стратегію хижака
            return lion;
        }

        public Enclosure CreateEnclosure(string name)
        {
            return new Enclosure(name, "Африканська Саванна");
        }
    }

    // Австралійська родина (Травоїдні)
    public class AustralianZooFactory : IZooFactory
    {
        public Animal CreateAnimal(string name)
        {
            var kangaroo = new Mammal(name, 100, 40);
            kangaroo.SetFeedingStrategy(new HerbivoreFeeding()); // Автоматично призначаємо стратегію травоїдного
            return kangaroo;
        }

        public Enclosure CreateEnclosure(string name)
        {
            return new Enclosure(name, "Евкаліптовий Ліс");
        }
    }

    // ======================================================================
    // 6. СПЕЦІАЛІЗОВАНІ DTO КЛАСИ ДЛЯ БЕЗПЕЧНОЇ СЕРІАЛІЗАЦІЇ (Завдання 7)
    // ======================================================================
    public class AnimalDto
    {
        public required string Name { get; set; }
        public required string Type { get; set; }
        public int Health { get; set; }
        public int Hunger { get; set; }
    }

    public class ZooStateDto
    {
        public required string ZooName { get; set; }
        public List<AnimalDto> Animals { get; set; } = new List<AnimalDto>();
    }

    // ======================================================================
    // 7. ГОЛОВНИЙ КЛАС ПРОГРАМИ (НАУЧНИЙ СЦЕНАРІЙ ДЛЯ ЗАХИСТУ)
    // ======================================================================
    class Program
    {
        static void Main(string[] args)
        {
            // Налаштування кодування для коректного виведення літер І, Є, Ї
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("==================================================================");
            Console.WriteLine("     СИМУЛЯТОР ЗООПАРКУ — ДЕМОНСТРАЦІЯ УСІХ ЗАВДАНЬ ПРАКТИКИ     ");
            Console.WriteLine("                 Студентка: Середа Анастасія                      ");
            Console.WriteLine("==================================================================\n");
            Console.ResetColor();

            // ------------------------------------------------------------------
            // ЕТАП 1 & 2: Робота Абстрактної Фабрики та Створення Сутностей
            // ------------------------------------------------------------------
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[ЕТАП 1] Демонстрація Патерну 'Abstract Factory' (Завдання 2, 3)");
            Console.ResetColor();

            IZooFactory africanFactory = new AfricanZooFactory();
            Enclosure savanna = africanFactory.CreateEnclosure("Вольєр №1 (Південний сектор)");
            Animal simba = africanFactory.CreateAnimal("Лев Сімба");
            savanna.Animals.Add(simba);

            Employee keeper = new Employee("Олексій", "Доглядач зони Саванни");

            Console.WriteLine($"-> Створено вольєр з біомом: '{savanna.EnvironmentType}'");
            Console.WriteLine($"-> Фабрика згенерувала тварину: '{simba.Name}' з базовим голодом {simba.Hunger}%");
            Console.WriteLine($"-> Призначено працівника: {keeper.Name} на посаду [{keeper.Position}]");

            // ------------------------------------------------------------------
            // ЕТАП 3: Тестування Патерну Спостерігач (Observer)
            // ------------------------------------------------------------------
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n[ЕТАП 2] Демонстрація Патерну 'Observer' — Моніторинг здоров'я (Завдання 4)");
            Console.ResetColor();

            Veterinarian vet = new Veterinarian("Доктор Дмитро");
            simba.RegisterObserver(vet); // Підписка ветеринара на стан Лева
            Console.WriteLine($"-> Ветеринар {vet.Name} успішно підписався на сповіщення від {simba.Name}.");

            Console.WriteLine("...Штучно знижуємо здоров'я лева до стабільного рівня (75%)...");
            simba.Health = 75;

            Console.WriteLine("...КРИТИЧНЕ ПАДІННЯ ЗДОРОВ'Я ЛЕВА ДО 15%...");
            simba.Health = 15; // Спрацьовує логіка тривоги всередині сетера властивості

            // ------------------------------------------------------------------
            // ЕТАП 4: Робота Патерну Стратегія та Обробка Кастомних Винятків
            // ------------------------------------------------------------------
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n[ЕТАП 3] Демонстрація Патерну 'Strategy' та обробки помилок (Завдання 4, 5)");
            Console.ResetColor();

            Food meat = new Food("М'ясо", 25);
            Food grass = new Food("Трава", 10);

            // 1. Успішний кейс годування за стратегією CarnivoreFeeding
            Console.WriteLine("Сценарій А: Правильне годування хижої тварини:");
            simba.Feed(meat);

            // 2. Кейс із винятком: спроба дати леву траву
            try
            {
                Console.WriteLine("\nСценарій Б: Спроба нагодувати Лева травою (Помилка стратегії):");
                simba.Feed(grass);
            }
            catch (ZooException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"   [ПЕРЕХОПЛЕНО ПОМИЛКУ БІЗНЕС-ЛОГІКИ]: {ex.Message}");
                Console.ResetColor();
                Console.WriteLine("   -> Політика Retry: Корм повернуто на склад, тварина не постраждала.");
            }

            // 3. Кейс із винятком: захист від перевитрати їжі (годування абсолютно ситої тварини)
            try
            {
                Console.WriteLine("\nСценарій В: Годування ситої тварини (Голод = 0):");
                simba.Hunger = 0;
                simba.Feed(meat);
            }
            catch (ZooException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"   [ПЕРЕХОПЛЕНО ПОМИЛКУ БІЗНЕС-ЛОГІКИ]: {ex.Message}");
                Console.ResetColor();
            }

            // ------------------------------------------------------------------
            // ЕТАП 5: Фінальна серіалізація в JSON через розділення моделей DTO
            // ------------------------------------------------------------------
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n[ЕТАП 4] Серіалізація даних у JSON через об'єкти DTO (Завдання 7)");
            Console.ResetColor();

            // Створюємо стан для експорту (мапимо доменні об'єкти в безпечні DTO моделі)
            var currentZooState = new ZooStateDto
            {
                ZooName = "Центральний Симулятор Зоопарку РФКІТ",
                Animals = new List<AnimalDto>
                {
                    new AnimalDto { Name = simba.Name, Type = "Mammal", Health = simba.Health, Hunger = simba.Hunger },
                    new AnimalDto { Name = "Кеша (Папуга)", Type = "Bird", Health = 90, Hunger = 15 }
                }
            };

            var serializerOptions = new JsonSerializerOptions { WriteIndented = true };

            try
            {
                string jsonOutput = JsonSerializer.Serialize(currentZooState, serializerOptions);
                File.WriteAllText("zoo_state.json", jsonOutput);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("-> Стан успішно записано та структуровано у файл 'zoo_state.json'!");
                Console.ResetColor();

                Console.WriteLine("\nВміст файлу zoo_state.json для звіту:");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(jsonOutput);
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при роботі з диском: {ex.Message}");
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n==================================================================");
            Console.WriteLine("   ДЕМОНСТРАЦІЮ ЗАВЕРШЕНО. УСІ КОМПОНЕНТИ ПРАЦЮЮТЬ ЯК ОДНЕ ЦІЛЕ!  ");
            Console.WriteLine("==================================================================");
            Console.ResetColor();
        }
    }
}