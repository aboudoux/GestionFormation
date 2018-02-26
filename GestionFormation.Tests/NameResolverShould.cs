using DataMigration;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GestionFormation.Tests
{
    [TestClass]
    [TestCategory("UnitTests")]
    public class NameResolverShould
    {
        [DataTestMethod]
        [DataRow("David LUCAS", "David", "LUCAS")]
        [DataRow("Maxime GRAVALON-THERMI AUTOMATION", "Maxime", "GRAVALON-THERMI AUTOMATION")]
        [DataRow("Jean-Michel FAUCHIER", "Jean-Michel", "FAUCHIER")]
        [DataRow("Manuel DA SYLVA", "Manuel", "DA SYLVA")]
        [DataRow("Marc ROBLOT COULANGES", "Marc", "ROBLOT COULANGES")]
        [DataRow("Grégory GUILLEMET  ( stagiaire)", "Grégory ( stagiaire)", "GUILLEMET")]
        [DataRow("Christophe VAZ DA COSTA - SOCIETE ATP", "Christophe", "VAZ DA COSTA - SOCIETE ATP")]
        [DataRow("Sébastien METAYER - EXPANSCIENCE", "Sébastien", "METAYER - EXPANSCIENCE")]
        [DataRow("DURADE Fabien", "Fabien", "DURADE")]
        [DataRow("ROMEO VALENTE", "ROMEO", "VALENTE")]
        [DataRow("MARATZU Didier", "Didier", "MARATZU")]
        [DataRow("BASTIE", "M.", "BASTIE")]                
        [DataRow("BOISSET", "M.", "BOISSET")]
        [DataRow("EL BACHAOUI Mohammed", "Mohammed", "EL BACHAOUI")]
        [DataRow("DI RIENZO Lydie", "Lydie", "DI RIENZO")]
        [DataRow("Christophe Rabaron", "Christophe", "Rabaron")]
        [DataRow("KOITA Djibril", "Djibril", "KOITA")]
        [DataRow("Mr JOUNIAUX", "Mr", "JOUNIAUX")]
        [DataRow("LILIAN LACROUTE", "LILIAN", "LACROUTE")]
        [DataRow("LAURENT GEOFFROY", "LAURENT", "GEOFFROY")]
        [DataRow("Mohammed EL BACHAOUI", "Mohammed", "EL BACHAOUI")]
        public void TestNameResolution(string name, string expectedFirstname, string expectedLastname)
        {
            var resolvedName = new NameResolver(name);

            resolvedName.Firstname.Should().Be(expectedFirstname);
            resolvedName.Lastname.Should().Be(expectedLastname);
        }
    }
}