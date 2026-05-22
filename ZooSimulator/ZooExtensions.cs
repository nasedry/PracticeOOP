using System;
using System.Collections.Generic;
using System.Linq;

public static class ZooExtensions
{
    public static double GetAverageHealth(this IEnumerable<Animal> animals)
    {
        if (!animals.Any()) return 0;
        return animals.Average(a => a.Health);
    }
    public static void PrintGroupedBySpecies(this IEnumerable<Animal> animals)
    {
        var groups = animals.GroupBy(a => a.GetSpeciesType());

        foreach (var group in groups)
        {
            Console.WriteLine($"--- Група: {group.Key} ---");
            foreach (var animal in group)
            {
                Console.WriteLine($"  * {animal.Name} (Голод: {animal.Hunger}%)");
            }
        }
    }
}