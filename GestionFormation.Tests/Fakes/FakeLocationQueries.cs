using System;
using System.Collections.Generic;
using System.Linq;
using GestionFormation.CoreDomain.Locations.Queries;

namespace GestionFormation.Tests.Fakes
{
    public class FakeLocationQueries : ILocationQueries
    {
        private readonly List<ILocationResult> _locations = new List<ILocationResult>();

        public void Add(string name, string address, int seats)
        {
            _locations.Add(new Result(Guid.NewGuid(), name, address, seats));
        }
        
        public IReadOnlyList<ILocationResult> GetAll()
        {
            return _locations;
        }

        public Guid? GetLocation(string name)
        {
            return _locations.FirstOrDefault(a => a.Name == name)?.LocationId;
        }

        private class Result : ILocationResult
        {
            public Result(Guid locationId, string name, string address, int seats)
            {
                LocationId = locationId;
                Name = name;
                Address = address;
                Seats = seats;
            }
            public Guid LocationId { get;  }
            public string Name { get; }
            public string Address { get;  }
            public int Seats { get; }
        }
    }
}