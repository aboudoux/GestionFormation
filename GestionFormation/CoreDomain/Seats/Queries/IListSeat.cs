using System;

namespace GestionFormation.CoreDomain.Seats.Queries
{
    public interface IListSeat
    {       
        SeatStatus SeatStatus { get;  }
        string Company { get;  }
        string TraineeLastname { get; }
        string TraineeFirstname { get;  }

        string TrainerLastname { get;  }
        string TrainerFirstname { get;  }

        string Training { get;  }
        DateTime SessionStart { get;  }
        int Duration { get;  }

        string AgreementNumber { get;  }

        string ContactLastname { get;  }
        string Contactfirstname { get;  }
        string Telephone { get;  }
        string Email { get;  }
    }
}