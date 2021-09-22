using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiPrototype2.Model.Dtos;
using WebApiPrototype2.Model.Utilities;

namespace WebApiPrototype2.Model.RepositoryInterfaces
{

    /// <summary>
    /// Interfaz de repositorio que determina la funcionalidad
    /// para realizar operaciones de consulta, modificación y
    /// eliminación de entidades de tipo usuario (User).
    /// </summary>
    public interface IUserRepository
    {
        public Task<IEnumerable<UserDto>> GetUsers();

        public Task<UserDto> GetUser(Guid uuid);

        public Task<GenericContainerWithMessages<UserDto>> UpdateUser(Guid uuid, Country country);

        public Task<GenericContainerWithMessages<UserDto>> DeleteUser(Guid uuid);

        public Task<GenericContainerWithMessages<UserTokenDto>> Authenticate(AuthenticationDto user);

    }
}
