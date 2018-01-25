using System;

namespace GestionFormation.Kernel
{
    public interface IAssignable
    {
        void Assign(DateTime debutSession, int dur�e);
        void UnAssign(DateTime debut, int dur�e);
        void ChangeAssignation(DateTime oldDateDebut, int oldDur�e, DateTime newDateDebut, int newDur�e);
    }
}