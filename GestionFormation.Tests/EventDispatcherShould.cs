using FluentAssertions;
using GestionFormation.Kernel;
using GestionFormation.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GestionFormation.Tests
{
    [TestClass]
    [TestCategory("UnitTests")]
    public class EventDispatcherShould
    {
        [TestMethod]
        public void register_handler_that_handle_single_interface()
        {
            var dispatcher = new EventDispatcher();
            var handler = new TestEventHandler();
            dispatcher.Register(handler);
    
            dispatcher.Dispatch(new DispatcherTestEvent());
            handler.Handled.Should().BeTrue();
        }

        [TestMethod]
        public void register_handler_that_handle_multipple_interfaces()
        {
            var dispatcher = new EventDispatcher();

            var handler = new MultipleEventHandler();
            dispatcher.Register(handler);
            dispatcher.Dispatch(new DispatcherTestEvent2());

            handler.Event1.Should().BeFalse();
            handler.Event2.Should().BeTrue();
        }        
    }

    public class DispatcherTestEvent : FakeDomainEvent
    {       
    }

    public class DispatcherTestEvent2 : FakeDomainEvent
    {

    }

    public class TestEventHandler : IEventHandler<DispatcherTestEvent>
    {
        public bool Handled { get; private set; }

        public void Handle(DispatcherTestEvent @event)
        {
            Handled = true;
        }
    }

    public class MultipleEventHandler : IEventHandler<DispatcherTestEvent>, IEventHandler<DispatcherTestEvent2>
    {
        public bool Event1 { get; private set; }
        public bool Event2 { get; private set; }

        public void Handle(DispatcherTestEvent @event)
        {
            Event1 = true;
        }

        public void Handle(DispatcherTestEvent2 @event)
        {
            Event2 = true;
        }
    }
}