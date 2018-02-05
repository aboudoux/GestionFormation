using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GestionFormation.App.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GestionFormation.Tests
{
    [TestClass]
    [TestCategory("UnitTests")]
    public class RelayCommandAsyncShould
    {
        [TestMethod]
        public async Task block_until_await_not_finished()
        {
            var count = 0;
            var currentValue = 0;

            var command = new RelayCommandAsync(async () =>
            {
                await Task.Run(() => currentValue = ++count);
                await Task.Delay(500);
            });

            var t1 = command.ExecuteAsync();
            var t2 = command.ExecuteAsync();

            await Task.WhenAll(t1, t2);

            currentValue.Should().Be(1);
        }
    }
}