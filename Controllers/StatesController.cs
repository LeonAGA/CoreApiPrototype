using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiPrototype2.Controllers.BaseClasses;
using WebApiPrototype2.Data;
using WebApiPrototype2.Model;
using WebApiPrototype2.Model.Dtos;
using WebApiPrototype2.Model.RepositoryInterfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace WebApiPrototype2.Controllers
{

    /// <summary>
    /// Clase de controlador Web API para las entidades de tipo Estado (State).
    /// EndPoint: /api/states
    /// </summary>
    public class StatesController : BaseApiController
    {

        #region -- CONSTANTES Y CAMPOS --------------------------------------->

        private readonly IStateRepository _repository;
        private readonly ILogger _logger;

        #endregion

        #region -- CONSTRUCTOR ----------------------------------------------->
        public StatesController(IStateRepository repository, ILogger logger)
        {
            _repository = repository;
            _logger = logger;
        }
        #endregion

        #region -- ACCIONES GET ---------------------------------------------->
        /// <summary>
        /// Acción para obtener el listado completo de países.
        /// GET: api/states
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StateDto>>> GetStates()
        {
            return Ok(await _repository.GetStates());
        }

        /// <summary>
        /// Acción para obtener un estado específico en base a su Uuid.
        /// GET: api/states/2e840bb2-f22f-4a40-91b1-5ed2ff23ec80
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [HttpGet("{uuid}", Name = nameof(GetState))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<StateDto>> GetState(Guid uuid)
        {
            StateDto stateDto = await _repository.GetState(uuid);
            if (stateDto == null)
            {
                return NotFound();
            }
            return Ok(this.CreateLinksForState(stateDto));
        }
        #endregion

        #region -- ACCIÓN PUT ------------------------------------------------>
        /// <summary>
        /// Acción para actualizar la información de un estado específico
        /// en base a su Uuid.
        /// PUT: api/states/2e840bb2-f22f-4a40-91b1-5ed2ff23ec80
        /// </summary>
        /// <param name="uuid"></param>
        /// <param name="stateDto"></param>
        /// <returns></returns>
        [HttpPut("{uuid}", Name = nameof(PutState))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StateDto>>
            PutState(Guid uuid, StateDto stateDto)
        {
            try
            {
                // Validar el estado del modelo.
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                if (uuid.ToString() != stateDto.Uuid)
                {
                    return BadRequest();
                }
                // Verifica que el estado que se está intentando modificar exista 
                // en la base de datos.
                if (!await _repository.StateExists(stateDto.StateId))
                {
                    var error = new
                    {
                        message = "El estado que está intentando modificar no existe en el sistema",
                        arguments = new { Uuid = uuid, stateDto = stateDto }
                    };
                    _logger.LogError(JsonConvert.SerializeObject(error));
                    return BadRequest(new { error = error });
                }
                var result = await _repository.UpdateState(uuid, stateDto);
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
            catch (Exception ex)
            {
                var controllerException = JsonConvert.SerializeObject(new
                {
                    error = new
                    {
                        customMessage = "Ha ocurrido un error al intentar actualizar el registro del estado.",
                        arguments = new { Uuid = uuid, stateDto = stateDto },
                        exceptionMessage = ex.Message,
                        stackTrace = ex.StackTrace
                    }
                });
                _logger.LogError(controllerException);
                throw new Exception(controllerException);
            }
        }
        #endregion

        #region -- ACCIÓN POST ----------------------------------------------->
        /// <summary>
        /// Acción para insertar una entidad estado.
        /// POST: api/states
        /// </summary>
        /// <param name="stateDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<StateDto>> PostState(StateDto stateDto)
        {
            // Validar el estado del modelo.
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            // Verificar que el nombre del país no esté en uso.
            var stateExists = await _repository.StateExists(stateDto.Name);
            if (stateExists)
            {
                return BadRequest("El nombre del estado ya existe en el sistema");
            }
            var result = await _repository.InsertState(stateDto);
            if (!result.ContainsErrors)
            {
                return CreatedAtAction(
                    "PostState",
                    new { id = stateDto.StateId },
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
        /// Acción para eliminar una entidad de tipo estado en base a su Uuid.
        /// DELETE: api/states/2e840bb2-f22f-4a40-91b1-5ed2ff23ec80
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [HttpDelete("{uuid}", Name = nameof(DeleteState))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<StateDto>> DeleteState(Guid uuid)
        {
            var result = await _repository.DeleteState(uuid);
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
        /// <param name="stateDto"></param>
        /// <returns></returns>
        private StateDto CreateLinksForState(StateDto stateDto)
        {

            var stateUuid = new { uuid = stateDto.Uuid };
            // Crear los enlaces para este recurso.
            // GET.
            stateDto.Links.Add(
                new LinkDto(this.Url.Link(nameof(this.GetState), stateUuid),
                    "self",
                    "GET"));
            // PUT.
            stateDto.Links.Add(
                new LinkDto(this.Url.Link(nameof(this.PutState), stateUuid),
                    "self",
                    "PUT"));
            // DELETE.
            stateDto.Links.Add(
                new LinkDto(this.Url.Link(nameof(this.DeleteState), stateUuid),
                    "self",
                    "DELETE"));
            return stateDto;
        } 
        #endregion
    }
}
