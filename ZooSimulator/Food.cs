namespace ZooSimulator
{
    public class Food
    {
        public string Type { get; set; }
        public int NutritionalValue { get; set; }
        public Food(string type, int value) { Type = type; NutritionalValue = value; }
    }
}