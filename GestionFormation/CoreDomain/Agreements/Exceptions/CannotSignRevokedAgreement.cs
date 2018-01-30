using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Agreements.Exceptions
{
    public class CannotSignRevokedAgreement : DomainException
    {
        public CannotSignRevokedAgreement() : base("Vous ne pouvez pas signer une convention révoquée")
        {
        }
    }
}
