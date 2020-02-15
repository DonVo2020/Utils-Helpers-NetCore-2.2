using AutoMapper;
using XMLProcessing.Dtos.Export;
using XMLProcessing.Dtos.Import;
using XMLProcessing.Models;

namespace XMLProcessing
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            this.CreateMap<ImportSuppliersDto, Supplier>();

            this.CreateMap<ImportPartDto, Part>();

            this.CreateMap<ImportCustomerDto, Customer>();

            this.CreateMap<ImportSaleDto, Sale>();

            this.CreateMap<ImportCarDto, Car>();

            this.CreateMap<Car, CarInfoDto>();

            this.CreateMap<Car, CarSaleDto>();
        }
    }
}
