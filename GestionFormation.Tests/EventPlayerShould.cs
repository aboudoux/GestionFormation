using FluentAssertions;
using GestionFormation.Kernel;
using GestionFormation.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GestionFormation.Tests
{
    [TestClass]
    [TestCategory("UnitTests")]
    public class EventPlayerShould
    {
        [TestMethod]
        public void apply_method_for_one_event()
        {
            var raised = false;

            var player = new EventPlayer();
            player.Add<TestEvent1>(a=> { raised = true; });
            player.Apply(new TestEvent1());

            raised.Should().BeTrue();
        }

        [TestMethod]
        public void apply_method_for_two_events()
        {
            var raised1 = false;
            var raised2 = false;

            var player = new EventPlayer();
            player.Add<TestEvent1>(a => { raised1 = true; })
                  .Add<TestEvent2>(a=>raised2 = true);

            player.Apply(new TestEvent1());
            raised1.Should().BeTrue();
            raised2.Should().BeFalse();

            player.Apply(new TestEvent2());
            raised2.Should().BeTrue();
        }

        [TestMethod]
        public void dont_raise_event_for_unregistered_event()
        {
            var raised1 = false;

            var player = new EventPlayer();
            player.Add<TestEvent1>(a => { raised1 = true; });

            player.Apply(new TestEvent2());
            raised1.Should().BeFalse();
        }

        [TestMethod]
        public void deal_with_event_member_when_applying()
        {
            var expectedResult = string.Empty;

            var player = new EventPlayer();
            player.Add<TestEvent1>(a => { expectedResult = a.Test; });

            var ev = new TestEvent1(){ Test = "ESSAI"};
            player.Apply(ev);

            expectedResult.Should().Be("ESSAI");
        }

        public class TestEvent1 : FakeDomainEvent
        {
            public string Test { get; set; }
          
        }

        public class TestEvent2 : FakeDomainEvent
        {
        }
    }
}