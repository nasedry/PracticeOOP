using Xunit;
using Moq;
using System;

public class ZooTests
{
    [Fact]
    public void Animal_ShouldThrowException_WhenNameIsEmpty()
    {
        Assert.Throws<ArgumentException>(() => new Mammal(""));
    }

    [Fact]
    public void Animal_ShouldSetCorrectInitialParameters()
    {
        var lion = new Mammal("Сімба", 90, 35);

        Assert.Equal("Сімба", lion.Name);
        Assert.Equal(90, lion.Health);
        Assert.Equal(35, lion.Hunger);
    }

    [Fact]
    public void AfricanZooFactory_ShouldCreateMammalWithCorrectType()
    {
        IZooFactory factory = new AfricanZooFactory();

        Animal animal = factory.CreateAnimal("Алекс");

        Assert.NotNull(animal);
        Assert.Equal("Ссавець", animal.GetSpeciesType());
        Assert.IsType<Mammal>(animal);
    }

    [Fact]
    public void Strategy_ShouldThrowZooException_WhenAnimalIsAlreadyFull()
    {
        Animal fullLion = new Mammal("Лев", 100, 0);
        IFeedingStrategy diet = new CarnivoreFeeding();

        Assert.Throws<ZooException>(() => diet.ExecuteFeeding(fullLion));
    }

    [Fact]
    public void Observer_ShouldBeTriggered_WhenHealthDrops()
    {
        var mockObserver = new Mock<IHealthObserver>();
        Animal bear = new Mammal("Балу", 100, 20);
        
        bear.RegisterObserver(mockObserver.Object);

        bear.ChangeHealth(45);

        mockObserver.Verify(obs => obs.Update(bear), Times.Once);
    }

    [Fact]
    public void Observer_ShouldReceiveUpdate_OnAnyHealthChange()
    {
        var mockObserver = new Mock<IHealthObserver>();
        Animal eagle = new Bird("Кеша", 100, 10);
        eagle.RegisterObserver(mockObserver.Object);

        eagle.ChangeHealth(90);

        mockObserver.Verify(obs => obs.Update(eagle), Times.Once);
    }
}