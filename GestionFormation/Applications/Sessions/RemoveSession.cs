using System;
using GestionFormation.CoreDomain.Formateurs;
using GestionFormation.CoreDomain.Formateurs.Exceptions;
using GestionFormation.CoreDomain.Lieux;
using GestionFormation.CoreDomain.Lieux.Exceptions;
using GestionFormation.CoreDomain.Sessions;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Sessions
{
    public class RemoveSession : ActionCommand
    {
        public RemoveSession(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid sessionId)
        {
            Formateur formateur = null;
            var session = GetAggregate<Session>(sessionId);

            if (session.FormateurId.HasValue)
            {
                formateur = GetAggregate<Formateur>(session.FormateurId.Value);
                if( formateur == null )
                    throw new FormateurNotExistsException();
                formateur.UnAssign(session.DateDebut, session.Durée);
            }

            Lieu lieu = null;
            if (session.LieuId.HasValue)
            {
                lieu = GetAggregate<Lieu>(session.LieuId.Value);
                if( lieu == null )
                    throw  new LieuNotExistsException();
                lieu.UnAssign(session.DateDebut, session.Durée);
            }

            session.Delete();
            PublishUncommitedEvents(formateur, lieu, session);
        }
    }
}