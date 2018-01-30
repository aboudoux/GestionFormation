using System;
using System.Collections.Generic;

namespace GestionFormation.CoreDomain.Seats.Queries
{
    public interface ISeatQueries
    {
        IEnumerable<ISeatResult> GetAll(Guid sessionId);
        IEnumerable<IAgreementSeatResult> GetSeatAgreements(Guid conventionId);
        IEnumerable<ISeatValidatedResult> GetValidatedSeats(Guid sessionId);
        IEnumerable<IListSeat> GetSeatsList();
    }
}