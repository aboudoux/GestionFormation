using System;

namespace DataMigration
{
    public class Name
    {
        public Name(string name)
        {
            if(string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(nameof(name));

            name = TrimAfterChar(name, '/');

            var splitted = name.Split(new[]{" "}, StringSplitOptions.RemoveEmptyEntries);
            if (splitted.Length == 1)
            {
                splitted = name.Split('.');
                splitted[0] = splitted[0] + ".";
            }

            switch (splitted.Length)
            {
                case 1:
                    Firstname = "NC";
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

                    if (AllWordAreLower(name))
                    {
                        var isFirst = true;
                        foreach (var s in splitted)
                        {
                            if (isFirst)
                            {
                                Firstname = s;
                                isFirst = false;
                            }
                            else
                            {
                                Lastname += s + " ";
                            }
                        }
                    }
                    else
                    {
                        foreach (var s in splitted)
                        {
                            if (s == "M.") continue;

                            if (!IsAllUpper(s))
                                Firstname += s + " ";
                            else
                            {
                                Lastname += s + " ";
                            }
                        }
                    }

                    if (string.IsNullOrWhiteSpace(Firstname))
                        Firstname = "NC";
                    if(string.IsNullOrWhiteSpace(Lastname))
                        throw new Exception("Le nom n'est pas renseigné pour " + name);

                    Firstname = Firstname.Trim();
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

        public bool AllWordAreLower(string input)
        {
            foreach (var s in input.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (IsAllUpper(s))
                    return false;
            }

            return true;
        }

        private string TrimAfterChar(string source, char c)
        {
            var result = source.Split(c);
            return result[0].Trim();
        }

        public override string ToString()
        {
            return Firstname + " " + Lastname;
        }
    }
}