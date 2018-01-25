using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using FluentAssertions;
using GestionFormation.CoreDomain;
using GestionFormation.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GestionFormation.Tests
{
    [TestClass]
    [TestCategory("Integrations")]
    public class DocumentRepositoryShould
    {
        [TestMethod]
        public void GenerateCertificatAssiduite()
        {
             var repo = new DocumentRepository(Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\"));
             var doc = repo.CreateCertificatAssiduite(new NomComplet("boudoux", "aurelien"), "DOT SHARK", "SET Niveau I", "Saint PRIEST", 5, new NomComplet("cordier", "fabrice"), DateTime.Now);
            File.Exists(doc).Should().BeTrue();
            //Process.Start(doc);
        }

        [TestMethod]
        public void GenerateDiplome()
        {
            var repo = new DocumentRepository(Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\"));
            var doc = repo.CreateDiplome(new NomComplet("boudoux", "aurelien"), "DOT SHARK", new DateTime(2018, 01, 23), new DateTime(2018, 01, 25), new NomComplet("cordier", "fabrice"));
            File.Exists(doc).Should().BeTrue();
            //Process.Start(doc);
        }

        [TestMethod]
        public void GenerateFeuillepresence()
        {
            var participants = new List<Participant>();
            participants.Add(new Participant("boudoux", "aurelien", "DOT SHARK"));
            participants.Add(new Participant("revel", "alexandre", "TREND"));
            participants.Add(new Participant("Aldebert", "Gregory", "TREND"));
            
            var repo = new DocumentRepository(Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\"));
            var doc = repo.CreateFeuillePresence("SET Niveau I", new DateTime(2018,1,23),3,"Saint PRIEST", new NomComplet("cordier", "fabrice"), participants);
            File.Exists(doc).Should().BeTrue();
            //Process.Start(doc);
        }

        [TestMethod]
        public void GenerateQuestionnaire()
        {
            var repo = new DocumentRepository(Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\"));
            var doc = repo.CreateQuestionnaire(new NomComplet("cordier", "fabrice"), "SET Niveau II");
            File.Exists(doc).Should().BeTrue();
            //Process.Start(doc);
        }
    }
}