using FluentAssertions;
using GestionFormation.CoreDomain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GestionFormation.Tests
{
    [TestClass]
    [TestCategory("UnitTests")]
    public class NomCompletShould
    {
        [DataTestMethod]
        [DataRow("boudoux", "aurelien", "Aurelien BOUDOUX")]
        [DataRow("", "aurelien", "Aurelien")]
        [DataRow("boudoux", "", "BOUDOUX")]
        [DataRow("boudoux", "a", "A BOUDOUX")]
        public void GeneratePrenomNom(string nom, string prenom, string expected)
        {
            var nomComplet = new FullName(nom, prenom);
            nomComplet.ToString().Should().Be(expected);
        }
    }
}