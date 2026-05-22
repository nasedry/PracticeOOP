public interface IZooFactory
{
    Animal CreateAnimal(string name);
    string CreateEnclosure();
    string CreateStaff(string name);
    string CreateFood();
}