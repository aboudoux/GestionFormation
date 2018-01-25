using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
using GestionFormation.CoreDomain.Formations.Queries;

namespace GestionFormation.Web.Controllers
{
    [RoutePrefix("api/v1/formation/query")]
    public class FormationQueryController : ExtendedApiController
    {
        private readonly IFormationQueries _queries;

        public FormationQueryController(IFormationQueries queries)
        {
            _queries = queries ?? throw new ArgumentNullException(nameof(queries));
        }

        [Route("retrieve"), HttpGet]
        public IHttpActionResult GetAllFormations()
        {
            return Run(() => _queries.GetAll().ToList());
        }
    }
}
