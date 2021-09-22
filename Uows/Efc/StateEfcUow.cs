using AutoMapper;
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
    /// Clase de unidad de trabajo que implementa la funcionalidad para la
    /// funcionalidad CRUD para entidades de tipo Estado (State) por medio
    /// de Entity Framework Core.
    /// </summary>
    public class StateEfcUow : IStateRepository
    {

        #region -- CONSTANTES Y CAMPOS --------------------------------------->

        private readonly WebApiPrototype2Context _context;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        #endregion

        #region -- CONSTRUCTOR ----------------------------------------------->
        public StateEfcUow(
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
        /// Método para obtener el listado completo de países.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<StateDto>> GetStates()
        {
            var states = await _context
                .State
                .ToListAsync();
            return _mapper.Map<IEnumerable<StateDto>>(states);
        }

        /// <summary>
        /// Método para obtener un estado específico en base a su
        /// Uuid.
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public async Task<StateDto> GetState(Guid uuid)
        {
            var state = await _context
                .State
                .Include(s => s.Country)
                .Where(s => s.Uuid == uuid)
                .SingleOrDefaultAsync();
            return _mapper.Map<StateDto>(state);
        }

        #endregion

        #region -- MÉTODO DE ACTUALIZACIÓN ----------------------------------->
        /// <summary>
        /// Método para procesar las modificaciones de entidades de tipo Estado
        /// (State). 
        /// </summary>
        /// <param name="uuid"></param>
        /// <param name="stateDto"></param>
        /// <returns></returns>
        public async Task<GenericContainerWithMessages<StateDto>> UpdateState(Guid uuid, StateDto stateDto)
        {
            // Convertir el DTO a su clase de dominio.
            State state = _mapper.Map<State>(stateDto);
            _context.Entry(state).State = EntityState.Modified;
            GenericContainerWithMessages<StateDto> result =
                new GenericContainerWithMessages<StateDto>();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                await _context.SaveChangesAsync();
                transaction.Commit();
                result.Element = _mapper.Map<StateDto>(state);
                result.ContainsErrors = false;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                string message = "Ha ocurrido un error al intentar " +
                    "modificar el registro del estado.";
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
        /// Método para insertar nuevas entidades de tipo Estado (State).
        /// </summary>
        /// <param name="stateDto"></param>
        /// <returns></returns>
        public async Task<GenericContainerWithMessages<StateDto>> InsertState(StateDto stateDto)
        {
            // Convertir el DTO a su clase de dominio.
            State state = _mapper.Map<State>(stateDto);
            GenericContainerWithMessages<StateDto> result =
                new GenericContainerWithMessages<StateDto>();
            // Comenzar una transacción explicita para aplicar los cambios
            // en la base de datos.
            var transaction = _context.Database.BeginTransaction();
            try
            {
                _context.State.Add(state);
                await _context.SaveChangesAsync();
                transaction.Commit();
                result.ContainsErrors = false;
                result.Element = _mapper.Map<StateDto>(state);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                string message = "Ha ocurrido un error al intentar\n" +
                        "insertar la nueva entidad de tipo Estado.";
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
        /// Método para eliminar entidades de tipo Estado en base a su Uuid.
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public async Task<GenericContainerWithMessages<StateDto>> DeleteState(Guid uuid)
        {
            GenericContainerWithMessages<StateDto> result =
                new GenericContainerWithMessages<StateDto>();
            var state = await _context
               .State
               .Where(s => s.Uuid == uuid)
               .SingleOrDefaultAsync();
            if (state == null)
            {
                string message = "No se ha entontrado el estado " +
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
                    _context.State.Remove(state);
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                    string message = $"El estados con el identificador " +
                        $"{state.Uuid} ha sido eliminado exitosamente";
                    result.Element = _mapper.Map<StateDto>(state);
                    result.Messages.Add(message);
                    result.ContainsErrors = false;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    string message = "Ha ocurrido un error al intentar" +
                            "eliminar el registro del estado.";
                    _logger.LogError(message, ex);
                    result.Messages.Add(message);
                    result.ExceptionContainer = ex;
                    result.ContainsErrors = true;
                }
            }
            return result;
        }

        #endregion

        #region -- MÉTODO PARA DETERMINAR SI EL ESTADO EXISTE ---------------->
        /// <summary>
        /// Método para determinar si el nombre de un estado ya se 
        /// encuentra en uso.
        /// </summary>
        /// <param name="stateName"></param>
        /// <returns></returns>
        public async Task<bool> StateExists(string stateName)
        {
            var state = await _context
                .State
                .Where(s => s.Name.ToLower() == stateName.ToLower())
                .SingleOrDefaultAsync();
            return state != null;
        }

        /// <summary>
        /// Método para determinar si un estado existe en base
        /// a su identificador de sistema.
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
        public async Task<bool> StateExists(int stateId)
        {
            var state = await _context
                .State
                .Where(s => s.StateId == stateId)
                .SingleOrDefaultAsync();
            return state != null;
        }
        #endregion
    }
}
