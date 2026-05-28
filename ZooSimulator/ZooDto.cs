using System.Collections.Generic;

namespace ZooSimulator
{
    public class VetDto
    {
        public required string Name { get; set; }
        public required string Specialization { get; set; }
    }

    public class AnimalDto
    {
        public required string Name { get; set; }
        public required string Category { get; set; }
        public required string Region { get; set; }
        public required string DietType { get; set; }
        public int Health { get; set; }
        public int Hunger { get; set; }
        public List<string> LinkedVets { get; set; } = new List<string>(); // Список імен закріплених лікарів
    }

    public class ZooStateDto
    {
        public required string ZooName { get; set; }
        public List<AnimalDto> Animals { get; set; } = new List<AnimalDto>();
        public List<VetDto> Veterinarians { get; set; } = new List<VetDto>();
    }
}