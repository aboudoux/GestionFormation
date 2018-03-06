using System;
using FluentAssertions;
using GestionFormation.Applications.Locations;
using GestionFormation.Applications.Locations.Exceptions;
using GestionFormation.Tests.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GestionFormation.Tests.Applications
{
    [TestClass]
    [TestCategory("IntegrationTests")]
    public class LocationCommandShould
    {
        private static SqlTestApplicationService _service;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            _service = new SqlTestApplicationService();
        }

        [TestMethod]        
        public void not_create_location_whith_same_name()
        {
            var locationName = "LOCATION_" + Guid.NewGuid();
            _service.Command<CreateLocation>().Execute(locationName, "", 10);

            Action action = ()=> _service.Command<CreateLocation>().Execute(locationName, "", 10);
            action.ShouldThrow<LocationAlreadyExistsException>();
        }

        [TestMethod]        
        public void create_location_with_same_name_if_other_is_disabled()
        {
            var locationName = "LOCATION_" + Guid.NewGuid();
            var location = _service.Command<CreateLocation>().Execute(locationName, "", 10);    
            _service.Command<DisableLocation>().Execute(location.AggregateId);

            Action action = () => _service.Command<CreateLocation>().Execute(locationName, "", 8);
            action.ShouldNotThrow<LocationAlreadyExistsException>();
        }
    }
}
