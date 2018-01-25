using System;
using GestionFormation.CoreDomain.Formateurs;
using GestionFormation.CoreDomain.Formateurs.Exceptions;
using GestionFormation.CoreDomain.Lieux;
using GestionFormation.CoreDomain.Lieux.Exceptions;
using GestionFormation.CoreDomain.Sessions;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Sessions
{
    public class PlanSession : ActionCommand
    {
        public PlanSession(EventBus eventBus) : base(eventBus)
        { 
        }
        public Session Execute(Guid formationId, DateTime debut, int durée, int nombrePlace, Guid? lieuId, Guid? formateurId)
        {
            Formateur formateur = null;
            if (formateurId.HasValue)
            {
                formateur = GetAggregate<Formateur>(formateurId.Value);
                if( formateur == null)
                    throw new FormateurNotExistsException();
                formateur.Assign(debut, durée);
            }

            Lieu lieu = null;
            if (lieuId.HasValue)
            {
                lieu = GetAggregate<Lieu>(lieuId.Value);
                if (lieu == null)
                    throw new LieuNotExistsException();
                lieu.Assign(debut, durée);
            }


            var session = Session.Plan(formationId, debut, durée, nombrePlace, lieuId, formateurId);
            PublishUncommitedEvents(formateur, lieu, session);

            return session;
        }
    }    
    
}