using System;

namespace GestionFormation.Kernel
{
    public interface IAssignable
    {
        void Assign(DateTime debutSession, int durée);
        void UnAssign(DateTime debut, int durée);
        void ChangeAssignation(DateTime oldDateDebut, int oldDurée, DateTime newDateDebut, int newDurée);
    }
}