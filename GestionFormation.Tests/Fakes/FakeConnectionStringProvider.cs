using System;
using GestionFormation.Infrastructure;

namespace GestionFormation.Tests.Fakes
{
    public class FakeConnectionStringProvider : IConnectionStringProvider
    {
        public FakeConnectionStringProvider()
        {
            
        }

        public FakeConnectionStringProvider(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; private set; }

        public void Write(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public string Read()
        {
            return ConnectionString;
        }
    }
}