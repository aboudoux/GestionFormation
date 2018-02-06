using System;
using System.Linq;
using System.Runtime.CompilerServices;

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

    public static class GuidAssert
    {
        public static void AreNotEmpty(params Guid[] ids)
        {
            if (ids.Any(id => id == Guid.Empty))
                throw new ArgumentException();
        }
    }
}