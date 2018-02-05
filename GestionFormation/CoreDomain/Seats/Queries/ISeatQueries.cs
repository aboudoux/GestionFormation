using System;
using System.Collections.Generic;

namespace GestionFormation.CoreDomain.Seats.Queries
{
    public interface ISeatQueries
    {
        IEnumerable<ISeatResult> GetAll(Guid sessionId);
        ISeatResult GetSeat(Guid seatId);
        IEnumerable<IAgreementSeatResult> GetSeatAgreements(Guid agreementId);
        IEnumerable<ISeatValidatedResult> GetValidatedSeats(Guid sessionId);
        IEnumerable<IListSeat> GetSeatsList();
    }
}