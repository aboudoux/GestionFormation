using System;

namespace GestionFormation.Kernel
{
    public static class GuidExtentions
    {
        public static void EnsureNotEmpty(this Guid id, string memberName)
        {
            if(id == Guid.Empty)
                throw new ArgumentException(memberName);
        }
    }
}