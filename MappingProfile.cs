using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiPrototype2.Model;
using WebApiPrototype2.Model.Dtos;

namespace WebApiPrototype2
{
    /// <summary>
    /// Clase para controlar el mapero entre clases de dominio
    /// y de objetos de trasferencia de datos (DTOs).
    /// </summary>
    public class MappingProfile : Profile
    {

        public MappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
            CreateMap<Country, CountryDto>();
            CreateMap<CountryDto, Country>();
            CreateMap<State, StateDto>();
            CreateMap<StateDto, State>();
        }

    }
}
