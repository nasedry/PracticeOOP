using System.Collections.Generic;

namespace ZooSimulator
{
    public class AnimalDto
    {
        public required string Name { get; set; }
        public required string Category { get; set; }
        public required string DietType { get; set; }
        public int Health { get; set; }
        public int Hunger { get; set; }
    }

    public class ZooStateDto
    {
        public required string ZooName { get; set; }
        public List<AnimalDto> Animals { get; set; } = new List<AnimalDto>();
    }
}