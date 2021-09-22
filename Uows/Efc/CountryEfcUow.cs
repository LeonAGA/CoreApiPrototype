using AutoMapper;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiPrototype2.Data;
using WebApiPrototype2.Model;
using WebApiPrototype2.Model.Dtos;
using WebApiPrototype2.Model.RepositoryInterfaces;
using WebApiPrototype2.Model.Utilities;

namespace WebApiPrototype2.Uows.Efc
{
    /// <summary>
    /// Clase de unidad de trabajo para la implementación de la funcionalidad
    /// CRUD para las entidades de países por medio de Entity Framework Core.
    /// </summary>
    public class CountryEfcUow : ICountryRepository
    {
        #region -- CONSTANTES Y CAMPOS --------------------------------------->

        private readonly WebApiPrototype2Context _context;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly int INSERT_INT_INIT_VALUE = 0;

        #endregion

        #region -- CONSTRUCTOR ----------------------------------------------->
        public CountryEfcUow(
            WebApiPrototype2Context context,
            ILogger logger,
            IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }
        #endregion

        #region -- MÉTODOS DE CONSULTA --------------------------------------->
        /// <summary>
        /// Método para obtener el listado completo de países
        /// con sus respectivas dependencias.
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<IEnumerable<CountryDto>>
            GetCountries(int pageNumber = 0, int pageSize = 0)
        {
            try
            {
                if (pageNumber == 0 || pageSize == 0)
                {
                    var countries = await _context
                    .Country
                    .OrderBy(c => c.Name)
                    .ToListAsync();
                    return _mapper.Map<IEnumerable<CountryDto>>(countries);
                }
                else
                {
                    var countries = await _context
                    .Country
                    .OrderBy(c => c.Name)
                    .Skip(pageSize * (pageNumber - 1))
                    .Take(pageSize)
                    .ToListAsync();
                    return _mapper.Map<IEnumerable<CountryDto>>(countries);
                }
            }
            catch (Exception ex)
            {
                if (ex is SqlException)
                {
                    // Verificar el código de error.
                    Console.WriteLine(ex.GetType());
                }
                    
                
                throw;
            }
        }

        /// <summary>
        /// Método para obtener un país específico en base a su
        /// Uuid.
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public async Task<CountryDto> GetCountry(Guid uuid)
        {
            var country = await _context
                .Country
                .Include(c => c.States)
                .Where(ct => ct.Uuid == uuid)
                .SingleOrDefaultAsync();
            return _mapper.Map<CountryDto>(country);
        }
        #endregion

        #region -- MÉTODO DE ACTUALIZACIÓN ----------------------------------->
        // Método para procesar las modificaciones en las entidades de países
        // incluyendo las operaciones CRUD de sus dependencias.
        public async Task<GenericContainerWithMessages<CountryDto>>
            UpdateCountry(Guid uuid, CountryDto countryDto)
        {
            // Convertir el DTO a su clase de dominio.
            Country country = _mapper.Map<Country>(countryDto);
            //_context.Country.Update(country);
            // Agregar los estados no existentes en la entidad.
            foreach (var state in country.States)
            {
                // Verificar si el estado ya existe
                // en la base de datos.
                if (state.StateId.Equals(INSERT_INT_INIT_VALUE))
                {
                    _context.Entry(state).State = EntityState.Added;
                }
                else
                {
                    _context.Entry(state).State = EntityState.Modified;
                }
            }
            // Obtener la información actual de los estados del país en modo 
            // de solo lectura.
            List<State> currentStates = await _context
                .State
                .Where(s => s.CountryId == country.CountryId)
                .ToListAsync();
            // Determinar si existen estados que deban ser eliminados
            List<int> deletedStates = currentStates
                .Select(ds => ds.StateId)
                .Except(
                    country
                    .States
                    .Select(s => s.StateId)).ToList();
            // Eliminar todos aquellos registros de estados que se hayan
            // removido desde el FrontEnd.
            foreach (int stateId in deletedStates)
            {
                State deletedState = _context.State.Find(stateId);
                _context.Entry(deletedState).State = EntityState.Deleted;
            }
            _context.Entry(country).State = EntityState.Modified;
            GenericContainerWithMessages<CountryDto> result =
                new GenericContainerWithMessages<CountryDto>();
            // Comenzar una transacción explicita para aplicar los cambios
            // en la base de datos.
            var transaction = _context.Database.BeginTransaction();
            try
            {
                await _context.SaveChangesAsync();
                transaction.Commit();
                result.Element = _mapper.Map<CountryDto>(country);
                result.ContainsErrors = false;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                string message = "Ha ocurrido un error al intentar " +
                    "modifica rel registro del país.";
                _logger.LogError(message, ex);
                result.ContainsErrors = true;
                result.ExceptionContainer = ex;
                result.Messages.Add(message);
            }
            return result;
        }
        #endregion

        #region -- MÉTODO DE INSERCIÓN --------------------------------------->
        /// <summary>
        /// Método para insertar nuevas entidades de tipo País (Country).
        /// </summary>
        /// <param name="countryDto"></param>
        /// <returns></returns>
        public async Task<GenericContainerWithMessages<CountryDto>>
            InsertCountry(CountryDto countryDto)
        {
            // Convertir el DTO a su clase de dominio.
            Country country = _mapper.Map<Country>(countryDto);
            GenericContainerWithMessages<CountryDto> result =
                new GenericContainerWithMessages<CountryDto>();
            // Comenzar una transacción explicita para aplicar los cambios
            // en la base de datos.
            var transaction = _context.Database.BeginTransaction();
            try
            {
                _context.Country.Add(country);
                await _context.SaveChangesAsync();
                transaction.Commit();
                result.ContainsErrors = false;
                result.Element = _mapper.Map<CountryDto>(country);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                string message = "Ha ocurrido un error al intentar\n" +
                        "insertar la nueva entidad de tipo País.";
                _logger.LogError(message, ex);
                result.Messages.Add(message);
                result.ExceptionContainer = ex;
                result.ContainsErrors = true;
            }
            return result;
        }
        #endregion

        #region -- MÉTODO DE ELIMINACIÓN ------------------------------------->
        /// <summary>
        /// Método para eliminar entidades de tipo País en base a su Uuid.
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public async Task<GenericContainerWithMessages<CountryDto>>
            DeleteCountry(Guid uuid)
        {
            GenericContainerWithMessages<CountryDto> result =
                new GenericContainerWithMessages<CountryDto>();
            var country = await _context
               .Country
               .Where(c => c.Uuid == uuid)
               .SingleOrDefaultAsync();
            if (country == null)
            {
                string message = "No se ha entontrado el país " +
                    "con el Uuid proporcionado.";
                result.ContainsErrors = true;
                result.Messages.Add(message);
            }
            else
            {
                // Comenzar una transacción explicita para aplicar los cambios
                // en la base de datos.
                var transaction = _context.Database.BeginTransaction();
                try
                {
                    _context.Country.Remove(country);
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                    string message = $"El registro del país con el identificador " +
                        $"{country.Uuid} ha sido eliminado exitosamente";
                    result.Element = _mapper.Map<CountryDto>(country);
                    result.Messages.Add(message);
                    result.ContainsErrors = false;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    string message = "Ha ocurrido un error al intentar" +
                            "eliminar el registro del país.";
                    _logger.LogError(message, ex);
                    result.Messages.Add(message);
                    result.ExceptionContainer = ex;
                    result.ContainsErrors = true;
                }
            }
            return result;
        }
        #endregion

        #region -- MÉTODO PARA DETERMINAR SI EL PAÍS EXISTE ------------------>
        /// <summary>
        /// Método para determinar si el nombre de un país ya se 
        /// encuentra en uso.
        /// </summary>
        /// <param name="countryName"></param>
        /// <returns></returns>
        public async Task<bool> CountryExists(string countryName)
        {
            var country = await _context
                .Country
                .Where(c => c.Name.ToLower() == countryName.ToLower())
                .SingleOrDefaultAsync();
            return country != null;
        }
        #endregion

    }
}
