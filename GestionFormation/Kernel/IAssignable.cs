using System;

namespace GestionFormation.Kernel
{
    public interface IAssignable
    {
        void Assign(DateTime sessionStart, int duration);
        void UnAssign(DateTime sessionStart, int duration);
        void ChangeAssignation(DateTime oldSessionStart, int oldDuration, DateTime newSessionStart, int newDuration);
    }
}