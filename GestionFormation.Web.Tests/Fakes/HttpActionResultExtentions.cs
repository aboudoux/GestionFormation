using System.Web.Http;
using System.Web.Http.Results;

namespace GestionFormation.Web.Tests.Controllers
{
    public static class HttpActionResultExtentions
    {
        public static T TryGetContent<T>(this IHttpActionResult source)
        {
            return !(source is OkNegotiatedContentResult<T> result) ? default(T) : result.Content;
        }
    }
}