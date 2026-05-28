using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace ZooSimulator
{
    class Program
    {
        private const string SaveFilePath = "zoo_state.json";

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            IZooFactory zooFactory = new UniversalZooFactory();
            List<Animal> zooAnimals = new List<Animal>();
            Veterinarian activeVet = new Veterinarian("Доктор Дмитро");

            if (File.Exists(SaveFilePath))
            {
                try
                {
                    string jsonInput = File.ReadAllText(SaveFilePath);
                    var importedState = JsonSerializer.Deserialize<ZooStateDto>(jsonInput);

                    if (importedState != null && importedState.Animals != null)
                    {
                        foreach (var dto in importedState.Animals)
                        {
                            string dietChoice = dto.DietType == "Хижак" ? "1" : 
                                                dto.DietType == "Травоїдна" ? "2" : "3";

                            var animal = zooFactory.CreateCustomAnimal(dto.Name, dto.Category, dietChoice);
                            animal.Health = dto.Health;
                            animal.Hunger = dto.Hunger;
                            animal.RegisterObserver(activeVet);

                            zooAnimals.Add(animal);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Помилка автоматичного завантаження даних: {ex.Message}");
                    Console.ResetColor();
                    Console.WriteLine("Натисніть Enter, щоб продовжити з порожнім зоопарком...");
                    Console.ReadLine();
                }
            }

            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(10000); 
                    lock (zooAnimals)
                    {
                        foreach (var animal in zooAnimals)
                        {
                            animal.Hunger += 5; 
                            if (animal.Hunger > 80)
                            {
                                animal.Health -= 5; 
                            }
                        }
                    }
                }
            });

            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("=============================================");
                Console.WriteLine("    ІНТЕРАКТИВНИЙ СИМУЛЯТОР ЗООПАРКУ        ");
                Console.WriteLine("=============================================");
                Console.ResetColor();
                Console.WriteLine("1. Додати нову тварину (Будь-який тип та раціон)");
                Console.WriteLine("2. Показати всіх тварин та їхній стан (Оновлюється наживо)");
                Console.WriteLine("3. Нагодувати тварину");
                Console.WriteLine("4. Змінити стан здоров'я тварини");
                Console.WriteLine("5. Зберегти поточний стан зоопарку");
                Console.WriteLine("0. Вийти з програми");
                Console.WriteLine("=============================================");
                Console.Write("Оберіть дію: ");

                string choice = Console.ReadLine() ?? "";
                if (choice == "0") break;

                switch (choice)
                {
                    case "1":
                        Console.Clear();
                        Console.WriteLine("--- ДОДАВАННЯ ТВАРИНИ ---");
                        Console.Write("Введіть назву/вид тварини (наприклад: Лев, Кенгуру, Слон): ");
                        string name = Console.ReadLine() ?? "Без назви";
                        if (string.IsNullOrWhiteSpace(name)) name = "Без назви";

                        Console.WriteLine("\nОберіть категорію тварини:");
                        Console.WriteLine("1. Ссавець");
                        Console.WriteLine("2. Птах");
                        Console.WriteLine("3. Рептилія");
                        Console.WriteLine("4. Риба");
                        Console.WriteLine("5. Амфібія");
                        Console.Write("Ваш вибір (або введіть свій варіант тексту): ");
                        string categoryInput = Console.ReadLine() ?? "Інше";
                        
                        string category = categoryInput switch
                        {
                            "1" => "Ссавець",
                            "2" => "Птах",
                            "3" => "Рептилія",
                            "4" => "Риба",
                            "5" => "Амфібія",
                            _ => string.IsNullOrWhiteSpace(categoryInput) ? "Інше" : categoryInput
                        };

                        Console.WriteLine("\nЧим харчується тварина (Тип раціону):");
                        Console.WriteLine("1. М'ясо / Риба (Хижак)");
                        Console.WriteLine("2. Рослини / Трава (Травоїдна)");
                        Console.WriteLine("3. Всеїдна (Їсть усе)");
                        Console.Write("Ваш вибір: ");
                        string dietChoice = Console.ReadLine() ?? "3";

                        var newAnimal = zooFactory.CreateCustomAnimal(name, category, dietChoice);
                        newAnimal.RegisterObserver(activeVet);
                        
                        lock (zooAnimals)
                        {
                            zooAnimals.Add(newAnimal);
                        }

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"\n[Успішно] {newAnimal.Category} '{newAnimal.Name}' додано до системи зоопарку!");
                        Console.ResetColor();
                        break;

                    case "2":
                        Console.Clear();
                        Console.WriteLine("--- СПИСОК ТВАРИН У ЗООПАРКУ ---");
                        lock (zooAnimals)
                        {
                            if (zooAnimals.Count == 0)
                            {
                                Console.WriteLine("Зоопарк порожній.");
                            }
                            else
                            {
                                foreach (var animal in zooAnimals)
                                {
                                    string dietName = animal.FeedingStrategy is CarnivoreFeeding ? "Хижак" : 
                                                      animal.FeedingStrategy is HerbivoreFeeding ? "Травоїдна" : "Всеїдна";
                                    Console.WriteLine($"Тварина: {animal.Name} | Категорія: {animal.Category} | Раціон: {dietName} | Здоров'я: {animal.Health}% | Голод: {animal.Hunger}%");
                                }
                            }
                        }
                        break;

                    case "3":
                        Console.Clear();
                        Console.WriteLine("--- КЕРУВАННЯ ГОДУВАННЯМ ---");
                        lock (zooAnimals)
                        {
                            if (zooAnimals.Count == 0)
                            {
                                Console.WriteLine("У зоопарку немає тварин.");
                                break;
                            }

                            for (int i = 0; i < zooAnimals.Count; i++)
                            {
                                Console.WriteLine($"{i + 1}. {zooAnimals[i].Name} [Голод: {zooAnimals[i].Hunger}%]");
                            }
                        }
                        Console.Write("Оберіть номер тварини: ");
                        string animalInput = Console.ReadLine() ?? "";
                        if (int.TryParse(animalInput, out int animalIndex) && animalIndex > 0 && animalIndex <= zooAnimals.Count)
                        {
                            Animal targetAnimal = zooAnimals[animalIndex - 1];

                            Console.WriteLine("\nОберіть тип корму:");
                            Console.WriteLine("1. Свіже м'ясо (Поживність +25)");
                            Console.WriteLine("2. Зелена трава (Поживність +10)");
                            Console.WriteLine("3. Морська риба (Поживність +20)");
                            Console.WriteLine("4. Стиглі фрукти (Поживність +15)");
                            Console.Write("Ваш вибір: ");
                            string foodChoice = Console.ReadLine() ?? "";

                            Food? selectedFood = null;
                            if (foodChoice == "1") selectedFood = new Food("М'ясо", 25);
                            else if (foodChoice == "2") selectedFood = new Food("Трава", 10);
                            else if (foodChoice == "3") selectedFood = new Food("Риба", 20);
                            else if (foodChoice == "4") selectedFood = new Food("Фрукти", 15);

                            if (selectedFood != null)
                            {
                                try
                                {
                                    targetAnimal.Feed(selectedFood);
                                }
                                catch (ZooException ex)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine($"\n[Відхилено]: {ex.Message}");
                                    Console.ResetColor();
                                }
                            }
                        }
                        break;

                    case "4":
                        Console.Clear();
                        Console.WriteLine("--- МОНІТОРИНГ ЗДОРОВ'Я ---");
                        lock (zooAnimals)
                        {
                            if (zooAnimals.Count == 0)
                            {
                                Console.WriteLine("У зоопарку немає тварин.");
                                break;
                            }

                            for (int i = 0; i < zooAnimals.Count; i++)
                            {
                                Console.WriteLine($"{i + 1}. {zooAnimals[i].Name} [Поточне здоров'я: {zooAnimals[i].Health}%]");
                            }
                        }
                        Console.Write("Оберіть номер тварини: ");
                        string hpInput = Console.ReadLine() ?? "";
                        if (int.TryParse(hpInput, out int hpIndex) && hpIndex > 0 && hpIndex <= zooAnimals.Count)
                        {
                            Console.Write("Введіть новий рівень здоров'я (0-100): ");
                            string newHpInput = Console.ReadLine() ?? "";
                            if (int.TryParse(newHpInput, out int newHp))
                            {
                                Console.WriteLine("\nОновлення інформації...");
                                zooAnimals[hpIndex - 1].Health = newHp;
                            }
                        }
                        break;

                    case "5":
                        Console.Clear();
                        Console.WriteLine("--- ЗБЕРЕЖЕННЯ СТАНУ ЗООПАРКУ ---");

                        var exportState = new ZooStateDto
                        {
                            ZooName = "Універсальний Симулятор Зоопарку",
                            Animals = new List<AnimalDto>()
                        };

                        lock (zooAnimals)
                        {
                            foreach (var animal in zooAnimals)
                            {
                                string dietLabel = animal.FeedingStrategy is CarnivoreFeeding ? "Хижак" : 
                                                   animal.FeedingStrategy is HerbivoreFeeding ? "Травоїдна" : "Всеїдна";
                                exportState.Animals.Add(new AnimalDto
                                {
                                    Name = animal.Name,
                                    Category = animal.Category,
                                    DietType = dietLabel,
                                    Health = animal.Health,
                                    Hunger = animal.Hunger
                                });
                            }
                        }

                        var serializerOptions = new JsonSerializerOptions 
                        { 
                            WriteIndented = true,
                            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
                        };

                        try
                        {
                            string jsonOutput = JsonSerializer.Serialize(exportState, serializerOptions);
                            File.WriteAllText(SaveFilePath, jsonOutput);

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("-> Дані успішно збережено у файл 'zoo_state.json'!");
                            Console.ResetColor();
                            Console.WriteLine("\nВміст збереженого файлу:\n");
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.WriteLine(jsonOutput);
                            Console.ResetColor();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Помилка запису файлу: {ex.Message}");
                        }
                        break;
                }

                Console.WriteLine("\nНатисніть Enter для продовження...");
                Console.ReadLine();
            }
        }
    }
}