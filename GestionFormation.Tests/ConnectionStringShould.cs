using System;
using FluentAssertions;
using GestionFormation.Infrastructure;
using GestionFormation.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GestionFormation.Tests
{
    [TestClass]
    public class ConnectionStringShould
    {
        [TestMethod]
        public void throw_error_if_no_connection_string_found()
        {
            var fake = new FakeConnectionStringProvider();
            var cs = new ConnectionString(fake);

            Action action = () => cs.GetConnectionString();
            action.ShouldThrow<Exception>();
        }

        [TestMethod]
        public void write_encrypted_connection_in_file_if_no_salt_detected()
        {
            var fake = new FakeConnectionStringProvider("Data Source=158.138.54.60;Initial Catalog=GestionFormation;User ID=sa;Password=123456");
            var cs = new ConnectionString(fake);

            var result = cs.GetConnectionString();
            result.Should().Be("Data Source=158.138.54.60;Initial Catalog=GestionFormation;User ID=sa;Password=123456");
            fake.ConnectionString.Should().Contain("secret:");
        }

        [TestMethod]
        public void get_decrypted_password_if_connection_string_salted()
        {
            var fake = new FakeConnectionStringProvider("data source=158.138.54.60;initial catalog=GestionFormation;user id=sa;password=\"secret:H8X8H7qBpO+L8gPmJmHTAQ==\"");
            var cs = new ConnectionString(fake);

            var result = cs.GetConnectionString();
            result.ToLower().Should().Be("data source=158.138.54.60;initial catalog=gestionformation;user id=sa;password=123456");
            fake.ConnectionString.Should().Contain("secret:");
        }

        [TestMethod]
        public void get_connection_string_from_settings()
        {
            var provider = new AppConfigConnectionStringProvider();
            var connectionString = provider.Read();
            connectionString.Should().NotBeNullOrEmpty();
        }      
    }
}