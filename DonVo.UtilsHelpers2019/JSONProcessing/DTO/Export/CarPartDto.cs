using Newtonsoft.Json;
using System.Collections.Generic;

namespace JSONProcessing.DTO.Export
{
    public class CarPartDto
    {
        public CarPartDto()
        {
            this.Parts = new List<PartDto>();
        }

        [JsonProperty("car")]
        public CarInfoDto Car { get; set; }

        [JsonProperty("parts")]
        public List<PartDto> Parts { get; set; }
    }
}
