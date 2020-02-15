using System.Collections.Generic;

namespace ConfigurationHelperCore
{
    public class Class
    {
        public string ClassName { get; set; }

        public Person Master { get; set; }

        public IEnumerable<Person> Students { get; set; }
    }
}
