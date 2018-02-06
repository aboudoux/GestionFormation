using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DevExpress.Xpf.Docking;
using FluentAssertions;
using GestionFormation.App.Core;
using GestionFormation.Applications;
using GestionFormation.CoreDomain;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;
using GestionFormation.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GestionFormation.Tests
{
    [TestClass]
    [TestCategory("UnitTests")]
    public class LearningTests
    {
        [TestMethod]
        public void retrieve_generic_interfaces_from_reflexion()
        {
            var handler = new GenericHandler();
            var handlers = handler.GetType().GetInterfaces().Where(a => a.IsGenericType && a.GetGenericTypeDefinition() == typeof(IEventHandler<>)).ToList();
            handlers.Should().HaveCount(1);
        }

        [TestMethod]
        public void test_auto_register()
        {
            var dispatcher = new EventDispatcher();
            dispatcher.AutoRegisterAllEventHandler();            
        }

        [TestMethod]
        public void test_domain_event_equality_with_null_value()
        {
            var ev1 = new EventWithNullValue(Guid.NewGuid(), 1, null);
            var ev2 = new EventWithNullValue(Guid.NewGuid(), 1, null);
            ev1.Equals(ev2).Should().BeTrue();            
        }

        [TestMethod]
        public void test_auto_register_queries()
        {
            var applicationService = new ApplicationService(PageLocator.With().Build(), new DocumentGroup(), new EventBus(new EventDispatcher(), new FakeEventStore()), new FakeMessenger());
            applicationService.AutoRegisterSimpleDependencies(Assembly.GetExecutingAssembly());
            applicationService.Command<TestCommand>().Execute().Should().Be("{43B94367-F832-42F7-B4E0-18FC065E4915}");
        }

        [TestMethod]
        public void test_password_hash()
        {
            var hashedPassword = "123456".GetHash();
            hashedPassword.Should().Be("7C4A8D09CA3762AF61E59520943DC26494F8941B");
        }

        [TestMethod]
        [Ignore]
        public void test_open_outlook_email()
        {
            ComputerService service = new ComputerService();
            service.OpenMailInOutlook("test", "cect est un test", new List<MailAttachement>(){ new MailAttachement(@"C:\Users\H264376\AppData\Local\Temp\2c147147-1058-410c-81ec-f7f512196820.rtf", "convention") });
        }
    }

    public class TestCommand : ActionCommand
    {
        private readonly ITestQueries _testQueries;

        public TestCommand(EventBus eventBus, ITestQueries testQueries) : base(eventBus)
        {
            _testQueries = testQueries ?? throw new ArgumentNullException(nameof(testQueries));
        }

        public string Execute()
        {
            return _testQueries.Get();
        }
    }

    public interface ITestQueries
    {
        string Get();
    }

    public class TestQueries : ITestQueries, IRuntimeDependency
    {
        public string Get()
        {
            return "{43B94367-F832-42F7-B4E0-18FC065E4915}";
        }
    }


    public class GenericHandler : IEventHandler<LocalEvent>
    {
        public void Handle(LocalEvent @event)
        {
            throw new System.NotImplementedException();
        }
    }

    public class EventWithNullValue : DomainEvent
    {
        public Guid? Test { get; }

        public EventWithNullValue(Guid aggregateId, int sequence, Guid? test) : base(aggregateId, sequence)
        {
            Test = test;
        }

        protected override string Description { get; }
    }

    public class LocalEvent : IDomainEvent
    {
        public Guid AggregateId { get; }
        public int Sequence { get; }
    }
}