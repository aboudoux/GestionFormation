using System.Collections.Generic;
using System.Linq;
using GestionFormation.CoreDomain.Trainers.Queries;
using GestionFormation.EventStore;
using GestionFormation.Kernel;

namespace GestionFormation.Infrastructure.Trainers.Queries
{
    public class TrainerSqlQueries : ITrainerQueries, IRuntimeDependency
    {
        public IReadOnlyList<ITrainerResult> GetAll()
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return context.Trainers.ToList().Select(a=>new TrainerResult(a)).ToList();
            }
        }

        public bool Exists(string lastname, string firstname)
        {
            var lastnameLower = lastname.ToLower();
            var firstnameLower = firstname.ToLower();

            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return context.Trainers.Any(a => a.Firstname.ToLower() == firstnameLower && a.Lastname.ToLower() == lastnameLower);
            }
        }
    }
}