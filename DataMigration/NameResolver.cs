using System;

namespace DataMigration
{
    public class NameResolver
    {
        public NameResolver(string name)
        {
            if(string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(nameof(name));

            var splitted = name.Split(' ');
            switch (splitted.Length)
            {
                case 1:
                    Firstname = "M.";
                    Lastname = name.ToUpper().Trim();
                    break;
                    
                case 2:
                    if (IsAllUpper(splitted[0]) && IsAllUpper(splitted[1]))
                    {
                        Firstname = splitted[0].Trim();
                        Lastname = splitted[1].Trim();
                    }
                    else if(!IsAllUpper(splitted[0]) && IsAllUpper(splitted[1]))
                    {
                        Firstname = splitted[0].Trim();
                        Lastname = splitted[1].Trim();
                    }
                    else if (IsAllUpper(splitted[0]) && !IsAllUpper(splitted[1]))
                    {
                        Firstname = splitted[1].Trim();
                        Lastname = splitted[0].Trim();
                    }
                    else
                    {
                        Firstname = splitted[0].Trim();
                        Lastname = splitted[1].Trim();
                    }
                    break;

                default:

                    foreach (var s in splitted)
                    {
                        if (!IsAllUpper(s))
                            Firstname += s + " ";
                        else
                        {
                            Lastname += s + " ";
                        }
                    }

                    Firstname = Firstname?.Trim();
                    Lastname = Lastname.Trim();
                    break;
            }
        }

        public string Firstname { get; }
        public string Lastname { get; }

        private bool IsAllUpper(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] != '-' && !Char.IsUpper(input[i]))
                    return false;
            }

            return true;
        }
    }
}