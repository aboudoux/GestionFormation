using System;
using System.Web.Http;

namespace GestionFormation.Web.Controllers
{
    public abstract class ExtendedApiController : ApiController
    {
        protected IHttpActionResult Run<T>(Func<T> func)
        {
            try
            {                
                return Ok(func());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        protected IHttpActionResult Run(Action action)
        {
            try
            {
                action();
                return Ok();
            }
            catch (Exception e)
            {
                var currentException = e;
                while (currentException.InnerException != null)                
                    currentException = currentException.InnerException;
                
                return BadRequest(currentException.Message);
            }
        }
    }
}