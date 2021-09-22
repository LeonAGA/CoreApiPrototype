using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiPrototype2.Filters;

namespace WebApiPrototype2.Controllers.BaseClasses
{
    
    /// <summary>
    /// Clase base que contiene toda la funcionalidad común
    /// para lo controladores del API.
    /// </summary>
    [EnableCors("CitlaliCors")]
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class BaseApiController : ControllerBase
    {

    }
}