using System;

namespace GestionFormation.Infrastructure
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(Guid id, string tableName) : base($"Impossible de trouver l'entit� {id} dans la table {tableName}")
        {
            
        }
    }
}