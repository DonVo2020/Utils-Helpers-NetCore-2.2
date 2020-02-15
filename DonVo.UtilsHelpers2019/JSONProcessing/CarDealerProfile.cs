using AutoMapper;
using JSONProcessing.DTO.Export;
using JSONProcessing.DTO.Import;
using JSONProcessing.Models;
using System.Linq;

namespace JSONProcessing
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            CreateMap<CarImport, Car>()
                .ForMember(c => c.PartCars,
                y => y.MapFrom(s => s.PartsId.Select(id => new PartCar() { PartId = id })));

            CreateMap<Customer, CustomerDto>();

            CreateMap<Car, CarDto>();

            CreateMap<Supplier, LocalSuppliersDto>()
                .ForMember(x => x.PartsCount, y => y.MapFrom(s => s.Parts.Count));

            CreateMap<Car, CarInfoDto>();
        }
    }
}
