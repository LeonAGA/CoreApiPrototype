using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiPrototype2.Controllers.BaseClasses;
using WebApiPrototype2.Model.Dtos;
using WebApiPrototype2.Model.RepositoryInterfaces;

namespace WebApiPrototype2.Controllers
{
    /// <summary>
    /// Clase de controlador Web API para el registro de usuarios,
    /// EndPoint = /api/registration
    /// </summary>
    public class RegistrationController : BaseApiController
    {
        #region -- CONSTANTES Y CAMPOS --------------------------------------->
        
        private readonly IUserRegistrationRepository _repository;

        #endregion

        #region -- CONSTRUCTOR ----------------------------------------------->
        public RegistrationController(IUserRegistrationRepository repository)
        {
            _repository = repository;
        }
        #endregion

        #region -- ACCIÓN PARA REGISTRAR NUEVOS USUARIOS --------------------->
        /// <summary>
        /// Acción para registrar un nuevo usuario en el sistema.
        /// POST: api/registration
        /// </summary>
        /// <param name="newUser"></param>
        /// <returns></returns>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserDto>> Register(UserRegistrationDto newUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            // Verificar si el nombre de usuario ya existe.
            var userExits = await _repository.UserExists(newUser.UserName);
            if (userExits)
            {
                return BadRequest("El nombre de usuario ya existe en el sistema");
            }
            var result = await _repository.Register(newUser);
            if (!result.ContainsErrors)
            {
                return CreatedAtAction(
                    "Register",
                    new { id = result.Element.UserId },
                    result.Element);
            }
            else
            {
                throw new Exception(result.Messages.FirstOrDefault() + ": " +
                    result.ExceptionContainer.Message);
            }
        } 

        #endregion

    }
}