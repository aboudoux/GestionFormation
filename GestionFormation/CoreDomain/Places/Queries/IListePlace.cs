using System;

namespace GestionFormation.CoreDomain.Places.Queries
{
    public interface IListePlace
    {       
        PlaceStatus EtatPlace { get;  }
        string Societe { get;  }
        string StagiaireNom { get; }
        string StagiairePrenom { get;  }

        string FormateurNom { get;  }
        string FormateurPrenom { get;  }

        string Formation { get;  }
        DateTime DateDebut { get;  }
        int Duree { get;  }

        string NumeroConvention { get;  }

        string ContactNom { get;  }
        string ContactPrenom { get;  }
        string Telephone { get;  }
        string Email { get;  }
    }
}