using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiPrototype2.Model.Dtos;
using WebApiPrototype2.Model.Utilities;

namespace WebApiPrototype2.Model.RepositoryInterfaces
{

    /// <summary>
    /// Interfaz de repositorio que determina la funcionalidad
    /// CRUD para entidades de tipo País (Country).
    /// </summary>
    public interface ICountryRepository
    {
        public Task<IEnumerable<CountryDto>> GetCountries(int pageNumber, int pageSize);

        public Task<CountryDto> GetCountry(Guid uuid);

        public Task<GenericContainerWithMessages<CountryDto>> UpdateCountry(Guid uuid, CountryDto country);

        public Task<GenericContainerWithMessages<CountryDto>> InsertCountry(CountryDto country);

        public Task<GenericContainerWithMessages<CountryDto>> DeleteCountry(Guid uuid);

        public Task<bool> CountryExists(string userName);

    }
}
