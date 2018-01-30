using System.Linq;

namespace GestionFormation.CoreDomain
{
    public class FullName
    {
        private readonly string _fullName;
        
        public FullName(string lastname, string firstname)
        {
            if (!string.IsNullOrWhiteSpace(firstname))
                _fullName = firstname.First().ToString().ToUpper() + firstname.Substring(1).ToLower();

            if (string.IsNullOrWhiteSpace(lastname)) return;

            if (string.IsNullOrWhiteSpace(_fullName))
                _fullName = lastname.ToUpper();
            else
                _fullName += " " + lastname.ToUpper();
        }

        public override string ToString()
        {
            return _fullName;
        }
    }
}