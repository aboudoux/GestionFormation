using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GestionFormation.Kernel;

namespace GestionFormation.Web.Controllers
{
    [RoutePrefix("api/v1/session/command")]
    public class SessionCommandController : ExtendedApiController
    {
        private readonly EventBus _eventBus;

        public SessionCommandController(EventBus eventBus)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

       /* [Route("plan"), HttpPost]
        public IHttpActionResult Planifier([FromBody] SessionData data )
        {
            //return Run(()=>new PlanSession(_eventBus).Execute(data.FormationId, data.DateDebut, data.NbrJour, data.NbrPlaces, data.Lieu, data.FormateurId ));
        }*/
    }

    public class SessionData
    {
        public Guid FormationId { get; set; }
        public DateTime DateDebut { get; set; }
        public int NbrJour { get; set; }
        public int NbrPlaces { get; set; }
        public string Lieu { get; set; }
        public string Formateur { get; set; }

    }
}
