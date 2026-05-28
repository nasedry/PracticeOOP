using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            IZooFactory africanFactory = new AfricanZooFactory();
            IZooFactory australianFactory = new AustralianZooFactory();
            IZooFactory asianFactory = new AsianZooFactory();
            IZooFactory europeanFactory = new EuropeanZooFactory();
            IZooFactory arcticFactory = new ArcticZooFactory();
            IZooFactory southAmericanFactory = new SouthAmericanZooFactory();
            
            List<Animal> zooAnimals = new List<Animal>();
            List<IObserver> veterinarians = new List<IObserver>();

            if (File.Exists(SaveFilePath))
            {
                try
                {
                    string jsonInput = File.ReadAllText(SaveFilePath);
                    var importedState = JsonSerializer.Deserialize<ZooStateDto>(jsonInput);

                    if (importedState != null)
                    {
                        if (importedState.Veterinarians != null)
                        {
                            foreach (var vDto in importedState.Veterinarians)
                            {
                                veterinarians.Add(new Veterinarian(vDto.Name, vDto.Specialization));
                            }
                        }

                        if (importedState.Animals != null)
                        {
                            foreach (var dto in importedState.Animals)
                            {
                                string dietChoice = dto.DietType == "Хижак" ? "1" : 
                                                    dto.DietType == "Травоїдна" ? "2" : "3";

                                IZooFactory currentFactory = dto.Region switch
                                {
                                    "Африканський біом" => africanFactory,
                                    "Австралійський біом" => australianFactory,
                                    "Азіатський біом" => asianFactory,
                                    "Європейський біом" => europeanFactory,
                                    "Арктичний біом" => arcticFactory,
                                    "Південноамериканський біом" => southAmericanFactory,
                                    _ => africanFactory
                                };

                                var animal = currentFactory.CreateAnimal(dto.Name, dto.Category, dietChoice);
                                animal.Health = dto.Health;
                                animal.Hunger = dto.Hunger;

                                if (dto.LinkedVets != null)
                                {
                                    foreach (var vetName in dto.LinkedVets)
                                    {
                                        var foundVet = veterinarians.Find(v => v.Name == vetName);
                                        if (foundVet != null)
                                        {
                                            animal.RegisterObserver(foundVet);
                                        }
                                    }
                                }

                                zooAnimals.Add(animal);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Помилка завантаження бази: {ex.Message}");
                    Console.ResetColor();
                    Console.ReadLine();
                }
            }

            if (zooAnimals.Count < 3 || veterinarians.Count == 0)
            {
                zooAnimals.Clear();
                veterinarians.Clear();

                var vet1 = new Veterinarian("Йосип", "Ссавець");
                var vet2 = new Veterinarian("Анна", "Птах");
                var vet3 = new Veterinarian("Максим", "Рептилія");
                var vet4 = new Veterinarian("Олена", "Риба");
                var vet5 = new Veterinarian("Дмитро", "Ссавець");
                var vet6 = new Veterinarian("Марія", "Птах");

                veterinarians.Add(vet1);
                veterinarians.Add(vet2);
                veterinarians.Add(vet3);
                veterinarians.Add(vet4);
                veterinarians.Add(vet5);
                veterinarians.Add(vet6);

                var lion = africanFactory.CreateAnimal("Лев", "Ссавець", "1");
                var kangaroo = australianFactory.CreateAnimal("Кенгуру", "Ссавець", "2");
                var panda = asianFactory.CreateAnimal("Панда", "Ссавець", "2");
                var eagle = europeanFactory.CreateAnimal("Орел", "Птах", "1");
                var penguin = arcticFactory.CreateAnimal("Пінгвін", "Птах", "3");
                var alligator = southAmericanFactory.CreateAnimal("Алігатор", "Рептилія", "1");

                lion.RegisterObserver(vet1);
                kangaroo.RegisterObserver(vet5);
                panda.RegisterObserver(vet1);
                eagle.RegisterObserver(vet2);
                penguin.RegisterObserver(vet6);
                alligator.RegisterObserver(vet3);

                zooAnimals.Add(lion);
                zooAnimals.Add(kangaroo);
                zooAnimals.Add(panda);
                zooAnimals.Add(eagle);
                zooAnimals.Add(penguin);
                zooAnimals.Add(alligator);

                SaveZooData(zooAnimals, veterinarians);
            }

            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(12000); 
                    bool needSave = false;
                    lock (zooAnimals)
                    {
                        foreach (var animal in zooAnimals)
                        {
                            animal.Hunger += 4; 
                            if (animal.Hunger > 75)
                            {
                                animal.Health -= 4; 
                            }
                            needSave = true;
                        }
                    }
                    if (needSave) SaveZooData(zooAnimals, veterinarians);
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
                Console.WriteLine("1. Додати нову тварину через Фабрики Біомів (6 регіонів)");
                Console.WriteLine("2. Додати ветеринара (Спостерігача) та спеціалізацію");
                Console.WriteLine("3. Прив'язати ветеринара до тварини (Контроль сумісності)");
                Console.WriteLine("4. Показати всіх тварин у біомах та їхніх спостерігачів");
                Console.WriteLine("5. Нагодувати тварину за типом раціону");
                Console.WriteLine("6. Змінити стан здоров'я тварини (Стрес-тест для лікаря)");
                Console.WriteLine("7. Видалити тварину із зоопарку");
                Console.WriteLine("8. Редагувати біом тварини");
                Console.WriteLine("9. Показати всіх ветеринарів та їхні спеціалізації");
                Console.WriteLine("0. Вийти з програми");
                Console.WriteLine("=============================================");
                Console.Write("Оберіть дію: ");

                string choice = Console.ReadLine() ?? "";
                if (choice == "0") break;

                switch (choice)
                {
                    case "1":
                        Console.Clear();
                        Console.WriteLine("--- СТВОРЕННЯ ТВАРИНИ ЧЕРЕЗ АБСТРАКТНУ ФАБРИКУ ---");
                        Console.WriteLine("Оберіть географічну фабрику біому:");
                        Console.WriteLine("1. Африканський біом | 2. Австралійський біом | 3. Азіатський біом");
                        Console.WriteLine("4. Європейський біом | 5. Арктичний біом       | 6. Південноамериканський біом");
                        Console.Write("Ваш вибір: ");
                        string factoryChoice = Console.ReadLine() ?? "1";

                        IZooFactory selectedFactory = factoryChoice switch
                        {
                            "2" => australianFactory,
                            "3" => asianFactory,
                            "4" => europeanFactory,
                            "5" => arcticFactory,
                            "6" => southAmericanFactory,
                            _ => africanFactory
                        };

                        Console.Write("\nВведіть назву/вид тварини (наприклад: Лев, Кенгуру, Анаконда): ");
                        string name = Console.ReadLine() ?? "Без назви";

                        Console.WriteLine("\nОберіть категорію об'єкта:");
                        Console.WriteLine("1. Ссавець");
                        Console.WriteLine("2. Птах");
                        Console.WriteLine("3. Рептилія");
                        Console.WriteLine("4. Риба");
                        Console.Write("Ваш вибір (або введіть свій текст): ");
                        string catInput = Console.ReadLine() ?? "1";
                        string category = catInput switch { "1" => "Ссавець", "2" => "Птах", "3" => "Рептилія", "4" => "Риба", _ => catInput };

                        Console.WriteLine("\nТип харчування для фабричного алгоритму:");
                        Console.WriteLine("1. Хижак");
                        Console.WriteLine("2. Травоїдна");
                        Console.WriteLine("3. Всеїдна");
                        Console.Write("Ваш вибір: ");
                        string dietChoice = Console.ReadLine() ?? "3";

                        var newAnimal = selectedFactory.CreateAnimal(name, category, dietChoice);
                        lock (zooAnimals) { zooAnimals.Add(newAnimal); }

                        SaveZooData(zooAnimals, veterinarians);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"\n[Успіх] Фабрика згенерувала об'єкт біому '{newAnimal.Region}': {newAnimal.Category} '{newAnimal.Name}'!");
                        Console.ResetColor();
                        break;

                    case "2":
                        Console.Clear();
                        Console.WriteLine("--- РЕЄСТРАЦІЯ СПОСТЕРІГАЧА (ВЕТЕРИНАРА) ---");
                        Console.Write("Введіть ім'я лікаря (наприклад: Йосип): ");
                        string vetName = Console.ReadLine() ?? "Лікар";
                        
                        Console.WriteLine("\nОберіть категорію тварин, з якими він вміє працювати:");
                        Console.WriteLine("1. Ссавець");
                        Console.WriteLine("2. Птах");
                        Console.WriteLine("3. Рептилія");
                        Console.WriteLine("4. Риба");
                        Console.Write("Ваш вибір: ");
                        string specInput = Console.ReadLine() ?? "1";
                        string specialization = specInput switch { "1" => "Ссавець", "2" => "Птах", "3" => "Рептилія", "4" => "Риба", _ => specInput };

                        veterinarians.Add(new Veterinarian(vetName, specialization));
                        SaveZooData(zooAnimals, veterinarians);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"\n[Успішно] Ветеринара {vetName} додано як експерта по групі: {specialization}!");
                        Console.ResetColor();
                        break;

                    case "3":
                        Console.Clear();
                        Console.WriteLine("--- ПРИВ'ЯЗКА СПОСТЕРІГАЧА ДО ОБ'ЄКТА ---");
                        if (zooAnimals.Count == 0 || veterinarians.Count == 0)
                        {
                            Console.WriteLine("Потрібно мати хоча б одну тварину і одного ветеринара в базі.");
                            break;
                        }

                        Console.WriteLine("Оберіть тварину:");
                        for (int i = 0; i < zooAnimals.Count; i++)
                            Console.WriteLine($"{i + 1}. {zooAnimals[i].Name} ({zooAnimals[i].Category}) — Біом: {zooAnimals[i].Region}");
                        int.TryParse(Console.ReadLine(), out int aIdx);

                        Console.WriteLine("\nОберіть ветеринара:");
                        for (int i = 0; i < veterinarians.Count; i++)
                            Console.WriteLine($"{i + 1}. {veterinarians[i].Name} [Експерт по: {veterinarians[i].Specialization}]");
                        int.TryParse(Console.ReadLine(), out int vIdx);

                        if (aIdx > 0 && aIdx <= zooAnimals.Count && vIdx > 0 && vIdx <= veterinarians.Count)
                        {
                            try
                            {
                                zooAnimals[aIdx - 1].RegisterObserver(veterinarians[vIdx - 1]);
                                SaveZooData(zooAnimals, veterinarians);
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"\n[Паттерн Observer] Успішно! {veterinarians[vIdx - 1].Name} тепер охороняє здоров'я {zooAnimals[aIdx - 1].Name}.");
                                Console.ResetColor();
                            }
                            catch (ZooException ex)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"\n[ПОМИЛКА БЕЗПЕКИ КАТЕГОРІЙ]: {ex.Message}");
                                Console.ResetColor();
                            }
                        }
                        break;

                    case "4":
                        Console.Clear();
                        Console.WriteLine("--- ПОТОЧНИЙ СТАН ЗООПАРКУ ---");
                        lock (zooAnimals)
                        {
                            if (zooAnimals.Count == 0)
                            {
                                Console.WriteLine("Зоопарк порожній.");
                            }
                            else
                            {
                                var sortedAnimals = zooAnimals.OrderBy(a => a.Region).ToList();
                                foreach (var animal in sortedAnimals)
                                {
                                    string dName = animal.FeedingStrategy is CarnivoreFeeding ? "Хижак" : animal.FeedingStrategy is HerbivoreFeeding ? "Травоїдна" : "Всеїдна";
                                    List<string> vNames = new List<string>();
                                    foreach (var o in animal.Observers) vNames.Add(o.Name);
                                    string observersList = vNames.Count > 0 ? string.Join(", ", vNames) : "немає";

                                    Console.WriteLine($"[{animal.Region}] {animal.Name} ({animal.Category}) | Раціон: {dName} | Здоров'я: {animal.Health}% | Голод: {animal.Hunger}% | Лікарі: {observersList}");
                                }
                            }
                        }
                        break;

                    case "5":
                        Console.Clear();
                        Console.WriteLine("--- ГОДУВАННЯ ---");
                        if (zooAnimals.Count == 0) { Console.WriteLine("Спочатку додайте тварин."); break; }
                        
                        Console.WriteLine("Оберіть тварину для годування:");
                        for (int i = 0; i < zooAnimals.Count; i++) Console.WriteLine($"{i + 1}. {zooAnimals[i].Name} [Голод: {zooAnimals[i].Hunger}%]");
                        
                        if (int.TryParse(Console.ReadLine(), out int fIdx) && fIdx > 0 && fIdx <= zooAnimals.Count)
                        {
                            Console.WriteLine("\nОберіть вид корму:");
                            Console.WriteLine("1. М'ясо (+25)");
                            Console.WriteLine("2. Трава (+10)");
                            Console.WriteLine("3. Риба (+20)");
                            Console.WriteLine("4. Фрукти (+15)");
                            Console.Write("Ваш вибір: ");
                            
                            string fChoice = Console.ReadLine() ?? "";
                            Food? food = fChoice switch { "1" => new Food("М'ясо", 25), "2" => new Food("Трава", 10), "3" => new Food("Риба", 20), "4" => new Food("Фрукти", 15), _ => null };
                            if (food != null)
                            {
                                try 
                                { 
                                    zooAnimals[fIdx - 1].Feed(food); 
                                    SaveZooData(zooAnimals, veterinarians);
                                }
                                catch (ZooException ex) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine($"\nВідхилено: {ex.Message}"); Console.ResetColor(); }
                            }
                        }
                        break;

                    case "6":
                        Console.Clear();
                        Console.WriteLine("--- КРИТИЧНИЙ СТРЕС-ТЕСТ ЗДОРОВ'Я ---");
                        if (zooAnimals.Count == 0) { Console.WriteLine("Тварин немає."); break; }
                        for (int i = 0; i < zooAnimals.Count; i++) Console.WriteLine($"{i + 1}. {zooAnimals[i].Name} [Здоров'я: {zooAnimals[i].Health}%]");
                        if (int.TryParse(Console.ReadLine(), out int hIdx) && hIdx > 0 && hIdx <= zooAnimals.Count)
                        {
                            Console.Write("\nВведіть новий рівень здоров'я (0-100) для перевірки реакції лікаря: ");
                            if (int.TryParse(Console.ReadLine(), out int newHp))
                            {
                                zooAnimals[hIdx - 1].Health = newHp;
                                SaveZooData(zooAnimals, veterinarians);
                            }
                        }
                        break;

                    case "7":
                        Console.Clear();
                        Console.WriteLine("--- ВИДАТИ ТВАРИНУ ІЗ ЗOОПАРКУ ---");
                        if (zooAnimals.Count == 0)
                        {
                            Console.WriteLine("У зоопарку зараз немає тварин для видалення.");
                            break;
                        }

                        Console.WriteLine("Оберіть тварину, яку потрібно видалити:");
                        for (int i = 0; i < zooAnimals.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}. {zooAnimals[i].Name} ({zooAnimals[i].Category}) — Біом: {zooAnimals[i].Region}");
                        }
                        Console.Write("Введіть номер (або 0 для скасування): ");
                        
                        if (int.TryParse(Console.ReadLine(), out int deleteIdx) && deleteIdx > 0 && deleteIdx <= zooAnimals.Count)
                        {
                            string removedName = zooAnimals[deleteIdx - 1].Name;
                            
                            lock (zooAnimals)
                            {
                                zooAnimals.RemoveAt(deleteIdx - 1);
                            }

                            SaveZooData(zooAnimals, veterinarians);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"\n[Успішно] Тварину '{removedName}' вилучено із системи зоопарку!");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.WriteLine("\nОперацію скасовано.");
                        }
                        break;

                    case "8":
                        Console.Clear();
                        Console.WriteLine("--- РЕДАГУВАННЯ БІОМУ ТВАРИНИ ---");
                        if (zooAnimals.Count == 0)
                        {
                            Console.WriteLine("У зоопарку зараз немає тварин.");
                            break;
                        }

                        Console.WriteLine("Оберіть тварину для редагування біому:");
                        for (int i = 0; i < zooAnimals.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}. {zooAnimals[i].Name} — Поточний біом: {zooAnimals[i].Region}");
                        }
                        Console.Write("Введіть номер: ");

                        if (int.TryParse(Console.ReadLine(), out int editIdx) && editIdx > 0 && editIdx <= zooAnimals.Count)
                        {
                            Console.WriteLine("\nОберіть новий біом:");
                            Console.WriteLine("1. Африканський біом | 2. Австралійський біом | 3. Азіатський біом");
                            Console.WriteLine("4. Європейський біом | 5. Арктичний біом       | 6. Південноамериканський біом");
                            Console.Write("Ваш вибір: ");
                            
                            string newBiomeChoice = Console.ReadLine() ?? "1";
                            string newBiomeName = newBiomeChoice switch
                            {
                                "2" => "Австралійський біом",
                                "3" => "Азіатський біом",
                                "4" => "Європейський біом",
                                "5" => "Арктичний біом",
                                "6" => "Південноамериканський біом",
                                _ => "Африканський біом"
                            };

                            lock (zooAnimals)
                            {
                                zooAnimals[editIdx - 1].Region = newBiomeName;
                            }

                            SaveZooData(zooAnimals, veterinarians);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"\n[Успішно] Біом для тварини '{zooAnimals[editIdx - 1].Name}' змінено на '{newBiomeName}'!");
                            Console.ResetColor();
                        }
                        break;

                    case "9":
                        Console.Clear();
                        Console.WriteLine("--- СПИСОК ЗАРЕЄСТРОВАНИХ ВЕТЕРИНАРІВ ---");
                        if (veterinarians.Count == 0)
                        {
                            Console.WriteLine("Немає зареєстрованих ветеринарів.");
                        }
                        else
                        {
                            for (int i = 0; i < veterinarians.Count; i++)
                            {
                                Console.WriteLine($"{i + 1}. {veterinarians[i].Name} | Спеціалізація: {veterinarians[i].Specialization}");
                            }
                        }
                        break;
                }
                Console.WriteLine("\nНатисніть Enter для продовження...");
                Console.ReadLine();
            }
        }

        private static void SaveZooData(List<Animal> zooAnimals, List<IObserver> veterinarians)
        {
            var exportState = new ZooStateDto 
            { 
                ZooName = "Глобальний Автоматизований Зоопарк", 
                Animals = new List<AnimalDto>(),
                Veterinarians = new List<VetDto>()
            };

            lock (zooAnimals)
            {
                foreach (var animal in zooAnimals)
                {
                    string dLabel = animal.FeedingStrategy is CarnivoreFeeding ? "Хижак" : 
                                    animal.FeedingStrategy is HerbivoreFeeding ? "Травоїдна" : "Всеїдна";
                    
                    var aDto = new AnimalDto 
                    { 
                        Name = animal.Name, 
                        Category = animal.Category, 
                        Region = animal.Region, 
                        DietType = dLabel, 
                        Health = animal.Health, 
                        Hunger = animal.Hunger,
                        LinkedVets = new List<string>()
                    };

                    foreach (var obs in animal.Observers)
                    {
                        aDto.LinkedVets.Add(obs.Name);
                    }

                    exportState.Animals.Add(aDto);
                }
            }

            foreach (var vet in veterinarians)
            {
                exportState.Veterinarians.Add(new VetDto { Name = vet.Name, Specialization = vet.Specialization });
            }

            try
            {
                string json = JsonSerializer.Serialize(exportState, new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) });
                File.WriteAllText(SaveFilePath, json);
            }
            catch { }
        }
    }
}