using System;
using GestionFormation.CoreDomain.Trainers.Queries;
using GestionFormation.Infrastructure.Trainers.Projections;

namespace GestionFormation.Infrastructure.Trainers.Queries
{
    public class TrainerResult : ITrainerResult
    {
        public TrainerResult(TrainerSqlEntity entity)
        {
            Id = entity.TrainerId;
            Lastname = entity.Lastname;
            Firstname = entity.Firstname;
            Email = entity.Email;
        }
        public Guid Id { get; }
        public string Lastname { get; }
        public string Firstname { get; }
        public string Email { get; }
    }
}