using MongoDB.Bson;

namespace MongoHelper.xUnitTests
{
    public class Person
    {
        public ObjectId Id { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

        public Person(string name, int age)
        {
            Name = name;
            Age = age;
        }

        public override string ToString() => $"Id:{Id}\tName:{Name}\tAge:{Age}";
    }
}
