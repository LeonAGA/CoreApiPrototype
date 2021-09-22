using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiPrototype2.Controllers.BaseClasses;
using WebApiPrototype2.Model.Dtos;
using WebApiPrototype2.Model.RepositoryInterfaces;
using WebApiPrototype2.Model.Utilities;

namespace WebApiPrototype2.Controllers
{
    /// <summary>
    /// Clase de controlador Web API para las entidades de tipo Usuario (User).
    /// </summary>
    public class UsersController : BaseApiController
    {

        #region -- CONSTANTES ------------------------------------------------>
        
        private readonly IUserRepository _repository;

        #endregion

        #region -- CONSTRUCTOR ----------------------------------------------->
        public UsersController(IUserRepository repository)
        {
            _repository = repository;
        }
        #endregion

        #region -- ACCIONES GET ---------------------------------------------->
        /// <summary>
        /// Acción para obtener el listado completo de usuarios.
        /// GET: api/users/
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            return Ok(await _repository.GetUsers());
        }

        /// <summary>
        /// Acción para obtener la información de un usuario específico
        /// en base a su Uuid.
        /// GET: api/users/6a54475f-b662-49d8-a863-ce953128eb3e
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [HttpGet("{uuid}", Name = nameof(GetUser))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUser(Guid uuid)
        {
            UserDto userDto = await _repository.GetUser(uuid);
            if (userDto == null)
            {
                return NotFound();
            }
            return Ok(this.CreateLinksForUser(userDto));
        }
        #endregion

        #region -- ACCIONES DE AUTENTICACIÓN --------------------------------->
        /// <summary>
        /// Acción para validar el acceso de los usuarios a la información
        /// del API.
        /// POST: api/users/authenticate
        /// </summary>
        /// <param name="authentication"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("authenticate",Name = nameof(Authenticate))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserTokenDto>> 
            Authenticate(AuthenticationDto authentication)
        {
            var result = await _repository.Authenticate(authentication);
            if (!result.ContainsErrors)
            {
                return Ok(result.Element);
            }
            else
            {
                return Unauthorized(result.Messages.FirstOrDefault());
            }
        }
        #endregion

        #region -- MÉTODO PARA LA CREACIÓN DE ENLACES HATEOAS ---------------->
        /// <summary>
        /// Método para generar enlaces de tipo HATEOAS que le proporcionarán
        /// la funcionalidad RESTful completa al API.
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        private UserDto CreateLinksForUser(UserDto userDto)
        {

            var stateUuid = new { uuid = userDto.Uuid };
            // Crear los enlaces para este recurso.
            // GET.
            userDto.Links.Add(
                new LinkDto(this.Url.Link(nameof(this.GetUser), stateUuid),
                    "self",
                    "GET"));
            // POST.
            userDto.Links.Add(
                new LinkDto(this.Url.Link(nameof(this.Authenticate), null),
                    "self",
                    "POST"));
            return userDto;
        }
        #endregion
    }

}
