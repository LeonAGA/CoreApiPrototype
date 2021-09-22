using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiPrototype2.Model.Dtos;
using WebApiPrototype2.Model.Utilities;

namespace WebApiPrototype2.Model.RepositoryInterfaces
{
    
    /// <summary>
    /// Interfaz de repositorio que determina la funcionalidad
    /// CRUD para entidades de tipo Estado (State).
    /// </summary>
    public interface IStateRepository
    {

        public Task<IEnumerable<StateDto>> GetStates();

        public Task<StateDto> GetState(Guid uuid);

        public Task<GenericContainerWithMessages<StateDto>> UpdateState(Guid uuid, StateDto stateDto);

        public Task<GenericContainerWithMessages<StateDto>> InsertState(StateDto stateDto);

        public Task<GenericContainerWithMessages<StateDto>> DeleteState(Guid uuid);

        public Task<bool> StateExists(string stateName);
        
        public Task<bool> StateExists(int stateId);
    }
}
