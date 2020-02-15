using System.Collections.Generic;

namespace JSONProcessing.DTO.Import
{
    public class CarImport
    {
        public int Id { get; set; }
        public CarImport()
        {
            this.PartsId = new HashSet<int>();
        }

        public string Make { get; set; }

        public string Model { get; set; }

        public long TravelledDistance { get; set; }

        public HashSet<int> PartsId { get; set; }
    }
}
