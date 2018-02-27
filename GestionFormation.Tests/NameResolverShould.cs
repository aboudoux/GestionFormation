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
        [DataRow("BASTIE", "NC", "BASTIE")]                
        [DataRow("BOISSET", "NC", "BOISSET")]
        [DataRow("EL BACHAOUI Mohammed", "Mohammed", "EL BACHAOUI")]
        [DataRow("DI RIENZO Lydie", "Lydie", "DI RIENZO")]
        [DataRow("Christophe Rabaron", "Christophe", "Rabaron")]
        [DataRow("KOITA Djibril", "Djibril", "KOITA")]
        [DataRow("Mr JOUNIAUX", "Mr", "JOUNIAUX")]
        [DataRow("LILIAN LACROUTE", "LILIAN", "LACROUTE")]
        [DataRow("LAURENT GEOFFROY", "LAURENT", "GEOFFROY")]
        [DataRow("Mohammed EL BACHAOUI", "Mohammed", "EL BACHAOUI")]
        [DataRow("F.Cordier", "F.", "Cordier")]
        [DataRow("A.Coulon", "A.", "Coulon")]
        [DataRow("Ortino", "NC", "ORTINO")]
        [DataRow("Sébastien BARRERO / ABST", "Sébastien", "BARRERO")]
        [DataRow("Georges GONAN / ABST", "Georges", "GONAN")]
        [DataRow("Jordan DE LAULANIE DE SAINTE CROIX", "Jordan", "DE LAULANIE DE SAINTE CROIX")]
        [DataRow("M. TERRIER Mathieu", "Mathieu", "TERRIER")]
        [DataRow("M. Pascal ANDUZE", "Pascal", "ANDUZE")]
        [DataRow("EN ATTENTE NOM", "NC", "EN ATTENTE NOM")]
        [DataRow("Said El hich", "Said", "El hich")]
        [DataRow("Jean Yves  Badole", "Jean", "Yves Badole")]
        public void TestNameResolution(string name, string expectedFirstname, string expectedLastname)
        {
            var resolvedName = new Name(name);

            resolvedName.Firstname.Should().Be(expectedFirstname);
            resolvedName.Lastname.Should().Be(expectedLastname);
        }
    }
}