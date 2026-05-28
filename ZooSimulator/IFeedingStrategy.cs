using System;

namespace ZooSimulator
{
    public interface IFeedingStrategy
    {
        string Feed(string animalName, Food food);
    }

    public class CarnivoreFeeding : IFeedingStrategy
    {
        public string Feed(string animalName, Food food)
        {
            if (food.Type.ToLower() != "м'ясо" && food.Type.ToLower() != "риба")
                throw new ZooException($"Хижак {animalName} відмовляється їсти {food.Type}! Йому потрібне м'ясо або риба.");
            return $"{animalName} з апетитом з'їв {food.Type} (поживність: +{food.NutritionalValue}).";
        }
    }

    public class HerbivoreFeeding : IFeedingStrategy
    {
        public string Feed(string animalName, Food food)
        {
            if (food.Type.ToLower() == "м'ясо" || food.Type.ToLower() == "риба")
                throw new ZooException($"Травоїдна тварина {animalName} лякається і відмовляється їсти {food.Type}!");
            return $"{animalName} спокійно жує {food.Type} (поживність: +{food.NutritionalValue}).";
        }
    }

    public class OmnivoreFeeding : IFeedingStrategy
    {
        public string Feed(string animalName, Food food)
        {
            return $"{animalName} із задоволенням з'їв {food.Type} (поживність: +{food.NutritionalValue}).";
        }
    }
}