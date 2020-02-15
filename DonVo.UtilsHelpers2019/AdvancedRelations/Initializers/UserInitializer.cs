using AdvancedRelations.Models;

namespace AdvancedRelations.Initializers
{
    internal class UserInitializer
    {
        public static User[] GetUsers()
        {
            User[] users = new User[]
            {
                new User() { FirstName = "Georgi",LastName = "Ivanov", Email = "georgiivanov@abv.bg",Password = "gosh1234"},
                new User() { FirstName = "Ivan",LastName = "Terziev", Email = "IvanIvan@abv.bg",Password = "iv1234"},
                new User() { FirstName = "Petar",LastName = "Petrov", Email = "PetarPetar@abv.bg",Password = "pt1234"}
            };

            return users;
        }
    }
}
