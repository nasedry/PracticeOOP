using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using ZooSimulator;

namespace ZooSimulator.Tests
{
    public class ZooSimulatorTests
    {
        [Fact]
        public void Animal_Initialization_ShouldSetCorrectBaseProperties()
        {
            var animal = new GenericAnimal("Грім", "Ссавець", "Арктичний біом", 90, 40);

            Assert.Equal("Грім", animal.Name);
            Assert.Equal("Ссавець", animal.Category);
            Assert.Equal("Арктичний біом", animal.Region);
            Assert.Equal(90, animal.Health);
            Assert.Equal(40, animal.Hunger);
        }

        [Fact]
        public void Animal_Hunger_ShouldBeClampedToMaximum100()
        {
            var animal = new GenericAnimal("Коко", "Птах", "Африканський біом", 100, 50);

            animal.Hunger = 150;

            Assert.Equal(100, animal.Hunger);
        }

        [Fact]
        public void Animal_Hunger_ShouldBeClampedToMinimum0()
        {
            var animal = new GenericAnimal("Коко", "Птах", "Африканський біом", 100, 50);

            animal.Hunger = -20;

            Assert.Equal(0, animal.Hunger);
        }

        [Fact]
        public void Animal_Health_ShouldBeClampedToMaximum100()
        {
            var animal = new GenericAnimal("Тор", "Рептилія", "Азіатський біом", 80, 20);

            animal.Health = 200;

            Assert.Equal(100, animal.Health);
        }

        [Fact]
        public void Animal_Health_ShouldBeClampedToMinimum0()
        {
            var animal = new GenericAnimal("Тор", "Рептилія", "Азіатський біом", 80, 20);

            animal.Health = -50;

            Assert.Equal(0, animal.Health);
        }

        [Fact]
        public void CarnivoreFeeding_ShouldAcceptMeatAndFish()
        {
            var strategy = new CarnivoreFeeding();
            var meat = new Food("М'ясо", 20);
            var fish = new Food("Риба", 15);

            var resultMeat = strategy.Feed("Лев", meat);
            var resultFish = strategy.Feed("Лев", fish);

            Assert.Contains("з апетитом з'їв М'ясо", resultMeat);
            Assert.Contains("з апетитом з'їв Риба", resultFish);
        }

        [Fact]
        public void CarnivoreFeeding_ShouldThrowZooException_WhenFoodIsNotMeatOrFish()
        {
            var strategy = new CarnivoreFeeding();
            var grass = new Food("Трава", 10);

            var exception = Assert.Throws<ZooException>(() => strategy.Feed("Тигр", grass));
            Assert.Contains("відмовляється їсти Трава", exception.Message);
        }

        [Fact]
        public void HerbivoreFeeding_ShouldAcceptGrassAndLeaves()
        {
            var strategy = new HerbivoreFeeding();
            var grass = new Food("Трава", 15);

            var result = strategy.Feed("Жираф", grass);

            Assert.Contains("спокійно жує Трава", result);
        }

        [Fact]
        public void HerbivoreFeeding_ShouldThrowZooException_WhenGivenMeatOrFish()
        {
            var strategy = new HerbivoreFeeding();
            var meat = new Food("М'ясо", 30);

            var exception = Assert.Throws<ZooException>(() => strategy.Feed("Коза", meat));
            Assert.Contains("лякається і відмовляється їсти М'ясо", exception.Message);
        }

        [Fact]
        public void OmnivoreFeeding_ShouldAcceptAnyFood()
        {
            var strategy = new OmnivoreFeeding();
            var meat = new Food("М'ясо", 25);
            var banana = new Food("Банан", 15);

            var resultMeat = strategy.Feed("Ведмідь", meat);
            var resultFruit = strategy.Feed("Ведмідь", banana);

            Assert.Contains("із задоволенням з'їв М'ясо", resultMeat);
            Assert.Contains("із задоволенням з'їв Банан", resultFruit);
        }

        [Fact]
        public void Animal_FeedMethod_ShouldThrowZooException_WhenAnimalIsAlreadyFull()
        {
            var animal = new GenericAnimal("Сімба", "Ссавець", "Африканський біом", 100, 0);
            animal.SetFeedingStrategy(new CarnivoreFeeding());
            var food = new Food("М'ясо", 20);

            var exception = Assert.Throws<ZooException>(() => animal.Feed(food));
            Assert.Contains("сита", exception.Message);
        }

        [Fact]
        public void Animal_FeedMethod_ShouldThrowZooException_WhenStrategyNotSet()
        {
            var animal = new GenericAnimal("Бродяга", "Ссавець", "Європейський біом", 100, 50);
            var food = new Food("М'ясо", 20);

            var exception = Assert.Throws<ZooException>(() => animal.Feed(food));
            Assert.Contains("не задано раціон", exception.Message);
        }

        [Fact]
        public void Animal_FeedMethod_ShouldDecreaseHungerOnSuccess()
        {
            var animal = new GenericAnimal("Панда", "Ссавець", "Азіатський біом", 100, 60);
            animal.SetFeedingStrategy(new OmnivoreFeeding());
            var food = new Food("Бамбук", 25);

            animal.Feed(food);

            Assert.Equal(35, animal.Hunger);
        }

        [Fact]
        public void AfricanZooFactory_ShouldSetCorrectRegionAndStrategy()
        {
            IZooFactory factory = new AfricanZooFactory();

            var animal = factory.CreateAnimal("Марти", "Ссавець", "2");

            Assert.Equal("Африканський біом", animal.Region);
            Assert.IsType<HerbivoreFeeding>(animal.FeedingStrategy);
        }

        [Fact]
        public void AustralianZooFactory_ShouldSetCorrectRegionAndStrategy()
        {
            IZooFactory factory = new AustralianZooFactory();

            var animal = factory.CreateAnimal("Роо", "Ссавець", "3");

            Assert.Equal("Австралійський біом", animal.Region);
            Assert.IsType<OmnivoreFeeding>(animal.FeedingStrategy);
        }

        [Fact]
        public void ArcticZooFactory_ShouldSetCorrectRegionAndStrategy()
        {
            IZooFactory factory = new ArcticZooFactory();

            var animal = factory.CreateAnimal("Боріс", "Ссавець", "1");

            Assert.Equal("Арктичний біом", animal.Region);
            Assert.IsType<CarnivoreFeeding>(animal.FeedingStrategy);
        }

        [Fact]
        public void Animal_RegisterObserver_ShouldAddObserverToTheList()
        {
            var animal = new GenericAnimal("Рекс", "Ссавець", "Європейський біом", 100, 50);
            var vet = new Veterinarian("Доктор Джон", "Ссавець");

            animal.RegisterObserver(vet);

            Assert.Single(animal.Observers);
            Assert.Contains(vet, animal.Observers);
        }

        [Fact]
        public void Animal_RegisterObserver_ShouldThrowZooException_WhenSpecializationDoesNotMatchCategory()
        {
            var animal = new GenericAnimal("Кеша", "Птах", "Австралійський біом", 100, 30);
            var vet = new Veterinarian("Доктор Сміт", "Рептилія");

            var exception = Assert.Throws<ZooException>(() => animal.RegisterObserver(vet));
            Assert.Contains("Не вдалося призначити лікаря", exception.Message);
        }

        [Fact]
        public void Animal_NotifyObservers_ShouldBeTriggered_WhenHealthDropsBelow30_UsingMock()
        {
            var mockObserver = new Mock<IObserver>();
            mockObserver.Setup(o => o.Specialization).Returns("Ссавець");
            
            var animal = new GenericAnimal("Сімба", "Ссавець", "Африканський біом", 100, 50);
            animal.RegisterObserver(mockObserver.Object);

            animal.Health = 25;

            mockObserver.Verify(o => o.Update("Сімба", "Ссавець", 25), Times.Once);
        }

        [Fact]
        public void Animal_NotifyObservers_ShouldInvokeUpdateOnAnyHealthChange_UsingMock()
        {
            var mockObserver = new Mock<IObserver>();
            mockObserver.Setup(o => o.Specialization).Returns("Ссавець");
            
            var animal = new GenericAnimal("Сімба", "Ссавець", "Африканський біом", 100, 50);
            animal.RegisterObserver(mockObserver.Object);

            animal.Health = 85;

            mockObserver.Verify(o => o.Update("Сімба", "Ссавець", 85), Times.Once); 
        }

        [Fact]
        public void Cache_AddAndGet_ShouldStoreAndRetrieveValueCorrectly()
        {
            var cache = new Cache<string, GenericAnimal>();
            var animal = new GenericAnimal("Бамсі", "Ссавець", "Європейський біом", 100, 40);

            cache.Add("BearKey", animal);
            var retrieved = cache.Get("BearKey");

            Assert.NotNull(retrieved);
            Assert.Equal("Бамсі", retrieved.Name);
        }

        [Fact]
        public void Cache_Get_ShouldReturnDefault_WhenKeyDoesNotExist()
        {
            var cache = new Cache<int, string>();

            var retrieved = cache.Get(999);

            Assert.Null(retrieved);
        }

        [Fact]
        public void Cache_Contains_ShouldReturnTrue_WhenKeyExists()
        {
            var cache = new Cache<string, int>();
            cache.Add("Тест", 42);

            bool exists = cache.Contains("Тест");
            bool notExists = cache.Contains("Відсутній");

            Assert.True(exists);
            Assert.False(notExists);
        }

        [Fact]
        public void Cache_Add_ShouldOverwriteExistingValue_WhenKeyIsRepeated()
        {
            var cache = new Cache<string, string>();
            cache.Add("Регіон", "Африка");

            cache.Add("Регіон", "Азія");
            var value = cache.Get("Регіон");

            Assert.Equal("Азія", value);
        }
    }
}