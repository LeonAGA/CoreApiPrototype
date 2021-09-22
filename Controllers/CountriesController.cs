using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiPrototype2.Controllers.BaseClasses;
using WebApiPrototype2.Filters;
using WebApiPrototype2.Model;
using WebApiPrototype2.Model.Dtos;
using WebApiPrototype2.Model.RepositoryInterfaces;
using WebApiPrototype2.Tools;

namespace WebApiPrototype2.Controllers
{
    /// <summary>
    /// Clase de controlador Web API para las entidades de tipo País (Country).
    /// EndPoint: /api/countries
    /// </summary>
    [CustomExceptionFilter]
    public class CountriesController : BaseApiController
    {
        #region -- CONSTANTES Y CAMPOS --------------------------------------->

        private readonly ICountryRepository _repository;

        #endregion

        #region -- CONSTRUCTOR ----------------------------------------------->
        public CountriesController(ICountryRepository repository)
        {
            _repository = repository;
        }
        #endregion

        #region -- ACCIONES GET ---------------------------------------------->
        /// <summary>
        /// Acción para obtener el listado completo de países.
        /// GET: api/countries
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<CountryDto>>> 
            GetCountry([FromQuery] int pageNumber = 0, [FromQuery] int pageSize = 0 )
        {
            // Procesar los encabezados de la solicitud para obtener la
            // el token JWT de los encabezados.
            string jwt = JwtTools.ExtractJwtFromHeader(Request.Headers);
            string payload = JwtTools.GetJwtPayload(jwt);
            return Ok(await _repository.GetCountries(pageNumber, pageSize));
        }

        /// <summary>
        /// Acción para obtener un país específico en base a su Uuid.
        /// GET: api/countries/2e840bb2-f22f-4a40-91b1-5ed2ff23ec80
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [HttpGet("{uuid}", Name = nameof(GetCountry))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<CountryDto>> GetCountry(Guid uuid)
        {
            //CountryDto countryDto = await _repository.GetCountry(uuid);
            //if (countryDto == null)
            //{
            //    return NotFound();
            //}
            //return Ok(CreateLinksForCountry(countryDto));
            var countryTask = _repository.GetCountry(uuid);
            await Task.Delay(1000);
            CountryDto countryDto = await countryTask;
            if (countryDto == null)
            {
                return NotFound();
            }
            return Ok(CreateLinksForCountry(countryDto));

        }
        #endregion

        #region -- ACCIÓN PUT ------------------------------------------------>
        /// <summary>
        /// Acción para actualizar la información de un país específico
        /// junto con sus dependencias en base a su Uuid.
        /// PUT: api/countries/2e840bb2-f22f-4a40-91b1-5ed2ff23ec80
        /// </summary>
        /// <param name="uuid"></param>
        /// <param name="countryDto"></param>
        /// <returns></returns>
        [HttpPut("{uuid}", Name = nameof(PutCountry))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<CountryDto>> 
            PutCountry(Guid uuid, CountryDto countryDto)
        {
            // Validar el estado del modelo.
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (uuid != countryDto.Uuid)
            {
                return BadRequest();
            }
            var result = await _repository.UpdateCountry(uuid, countryDto);
            if (!result.ContainsErrors)
            {
                return Ok(result.Element);
            }
            else
            {
                throw new Exception(result.Messages.FirstOrDefault() + ": " +
                    result.ExceptionContainer.Message);
            }
        }
        #endregion

        #region -- ACCIÓN POST ----------------------------------------------->
        /// <summary>
        /// Acción para insertar una entidad país junto con sus 
        /// respectivas dependencias.
        /// POST: api/countries
        /// </summary>
        /// <param name="countryDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<CountryDto>> 
            PostCountry(CountryDto countryDto)
        {
            // Validar el estado del modelo.
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            // Verificar que el nombre del país no esté en uso.
            var countryExists = await _repository.CountryExists(countryDto.Name);
            if (countryExists)
            {
                return BadRequest("El nombre del país ya existe en el sistema");
            }
            var result = await _repository.InsertCountry(countryDto);
            if (!result.ContainsErrors)
            {
                return CreatedAtAction(
                    "PostCountry", 
                    new { id = countryDto.CountryId }, 
                    result.Element);
            }
            else
            {
                throw new Exception(result.Messages.FirstOrDefault() + ": " +
                    result.ExceptionContainer.Message);
            }
        }
        #endregion

        #region -- ACCIÓN DELETE --------------------------------------------->
        /// <summary>
        /// Acción para eliminar una entidad de tipo país junto con todas 
        /// sus dependencias en base a su Uuid.
        /// DELETE: api/countries/6a54475f-b662-49d8-a863-ce953128eb3e
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [HttpDelete("{uuid}", Name = nameof(DeleteCountry))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<CountryDto>> DeleteCountry(Guid uuid)
        {
            var result = await _repository.DeleteCountry(uuid);
            if (!result.ContainsErrors)
            {
                return NoContent();
            }
            else
            {
                // Verificar si existe una excepción.
                if (result.ExceptionContainer == null)
                {
                    return NotFound(result.Messages.FirstOrDefault());
                }
                else
                {
                    return Conflict(result.Messages.FirstOrDefault());
                }
            }
        }
        #endregion

        #region -- MÉTODO PARA LA CREACIÓN DE ENLACES HATEOAS ---------------->
        /// <summary>
        /// Método para generar enlaces de tipo HATEOAS que le proporcionarán
        /// la funcionalidad RESTful completa al API.
        /// </summary>
        /// <param name="countryDto"></param>
        /// <returns></returns>
        private CountryDto CreateLinksForCountry(CountryDto countryDto)
        {

            var countyUuid = new { uuid = countryDto.Uuid };
            // Crear los enlaces para este recurso.
            // GET.
            countryDto.Links.Add(
                new LinkDto(this.Url.Link(nameof(this.GetCountry), countyUuid),
                    "self",
                    "GET"));
            // PUT.
            countryDto.Links.Add(
                new LinkDto(this.Url.Link(nameof(this.PutCountry), countyUuid),
                    "self",
                    "PUT"));
            // DELETE.
            countryDto.Links.Add(
                new LinkDto(this.Url.Link(nameof(this.DeleteCountry), countyUuid),
                    "self",
                    "DELETE"));
            return countryDto;
        }
        #endregion

    }
}
