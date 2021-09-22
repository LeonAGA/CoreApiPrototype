using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Results;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace WebApiPrototype2.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomExceptionFilter : ExceptionFilterAttribute
    {

        #region -- CONSTANTES Y CAMPOS --------------------------------------->
        private readonly ILogger _logger; 
        #endregion

        #region -- CONSTRUCTOR ----------------------------------------------->
        //public CustomExceptionFilter(ILogger logger)
        //{
        //    _logger = logger;
        //} 
        #endregion

        public override void OnException(ExceptionContext context)
        {
            string exceptionMessage = string.Empty;
            if (context.Exception.InnerException == null)
            {
                exceptionMessage = context.Exception.Message;
            }
            else
            {
                exceptionMessage = context.Exception.InnerException.Message;
            }
            //_logger.LogError("Ha ocurrido un error al intentar realizar la acción:" + exceptionMessage);
            context.HttpContext.Response.ContentType = "application/json";
            context.HttpContext.Response.StatusCode = 500;
            //context.Result = 

        }
    }
}
