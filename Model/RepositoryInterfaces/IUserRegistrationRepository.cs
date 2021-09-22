using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiPrototype2.Model.Dtos;
using WebApiPrototype2.Model.Utilities;

namespace WebApiPrototype2.Model.RepositoryInterfaces
{

    /// Interfaz de repositorio que determina la funcionalidad
    /// para realizar el registro de nuevos usuarios en el
    /// sistema.
    public interface IUserRegistrationRepository
    {

        public Task<GenericContainerWithMessages<UserDto>> 
            Register(UserRegistrationDto newUser);

        public Task<bool> UserExists(string userName);

    }
}
