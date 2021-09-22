using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebApiPrototype2.Data;
using WebApiPrototype2.Model;
using WebApiPrototype2.Model.Dtos;
using WebApiPrototype2.Model.RepositoryInterfaces;
using WebApiPrototype2.Model.Utilities;

namespace WebApiPrototype2.Uows.Efc
{
    /// <summary>
    /// Clase de unidad de trabajo que implementa la funcionalidad
    /// para registrar nuevos usuarios en el sistema.
    /// </summary>
    public class UserRegistrationEfcUow : IUserRegistrationRepository
    {
        #region -- CONSTANTES Y CAMPOS --------------------------------------->
        private readonly WebApiPrototype2Context _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        #endregion

        #region -- CONSTRUCTOR ----------------------------------------------->
        public UserRegistrationEfcUow(
           WebApiPrototype2Context context,
           IMapper mapper,
           ILogger logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }
        #endregion

        #region -- MÉTODO PARA REGISTRAR UN NUEVO USUARIO -------------------->
        /// <summary>
        ///  Método para registrar un nuevo usuario en el sistema.
        /// </summary>
        /// <param name="newUser"></param>
        /// <returns></returns>
        public async Task<GenericContainerWithMessages<UserDto>> Register(UserRegistrationDto newUser)
        {
            GenericContainerWithMessages<UserDto> result =
                new GenericContainerWithMessages<UserDto>();
            // Instanciar la clase de ccódigo de autentificación de mensajes
            // con clave-hash (HMAC) con el algoritmo SHA512.
            using var hmac = new HMACSHA512();
            // Crear el nuevo objeto usuario.
            User user = new User
            {
                UserName = newUser.UserName,
                FirstName = newUser.FirstName,
                MiddleName = newUser.MiddleName,
                LastName = newUser.LastName,
                Email = newUser.Email,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(newUser.Password)),
                PasswordSalt = hmac.Key
            };
            var transaction = _context.Database.BeginTransaction();
            try
            {
                _context.User.Add(user);
                await _context.SaveChangesAsync();
                transaction.Commit();
                result.ContainsErrors = false;
                result.Element = _mapper.Map<User, UserDto>(user);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                string message = "Ha ocurrido un error al intentar " +
                    "registrar al usuario.";
                _logger.LogError(message, ex);
                result.Messages.Add(message);
                result.ContainsErrors = true;
                result.ExceptionContainer = ex;
            }
            return result;
        } 
        #endregion

        #region -- MÉTODO PARA DETERMINAR SI EL USUARIO EXISTE --------------->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<bool> UserExists(string userName)
        {
            var user = await _context
                .User
                .Where(u => u.UserName.ToLower() == userName.ToLower())
                .SingleOrDefaultAsync();
            return user != null;
        } 
        #endregion
    }
}
