using FluentAssertions;
using GestionFormation.CoreDomain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GestionFormation.Tests
{
    [TestClass]
    [TestCategory("UnitTests")]
    public class FullNameShould
    {
        [DataTestMethod]
        [DataRow("boudoux", "aurelien", "Aurelien BOUDOUX")]
        [DataRow("", "aurelien", "Aurelien")]
        [DataRow("boudoux", "", "BOUDOUX")]
        [DataRow("boudoux", "a", "A BOUDOUX")]
        public void GenerateFirstnameAndLastname(string lastname, string firstname, string expected)
        {
            var nomComplet = new FullName(lastname, firstname);
            nomComplet.ToString().Should().Be(expected);
        }
    }
}