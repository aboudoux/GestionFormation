using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using GestionFormation.App.Views.Seats;
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
        public void GenerateCertificatOfAttendance()
        {
            var repo = new DocumentCreator(Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\"));
            var doc = repo.CreateCertificateOfAttendance(new FullName("boudoux", "aurelien"), "DOT SHARK", "SET Niveau I", "Saint PRIEST", 5, new FullName("cordier", "fabrice"), DateTime.Now);
            File.Exists(doc).Should().BeTrue();
            //Process.Start(doc);
        }

        [TestMethod]
        public void GenerateDegree()
        {
            var repo = new DocumentCreator(Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\"));
            var doc = repo.CreateDegree(new FullName("boudoux", "aurelien"), "DOT SHARK", new DateTime(2018, 01, 23), new DateTime(2018, 01, 25), new FullName("cordier", "fabrice"));
            File.Exists(doc).Should().BeTrue();
            //Process.Start(doc);
        }

        [TestMethod]
        public void GenerateTimesheet()
        {
            var attendees = new List<Attendee>();
            attendees.Add(new Attendee(new FullName("boudoux", "aurelien"), "DOT SHARK"));
            attendees.Add(new Attendee(new FullName("revel", "alexandre"), "TREND"));
            attendees.Add(new Attendee(new FullName("Aldebert", "Gregory"), "TREND"));
            
            var repo = new DocumentCreator(Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\"));
            var doc = repo.CreateTimesheet("SET Niveau I", new DateTime(2018,1,23),3,"Saint PRIEST", new FullName("cordier", "fabrice"), attendees);
            File.Exists(doc).Should().BeTrue();
            //Process.Start(doc);
        }

        [TestMethod]
        public void GenerateSurvey()
        {
            var repo = new DocumentCreator(Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\"));
            var doc = repo.CreateSurvey(new FullName("cordier", "fabrice"), "SET Niveau II");
            File.Exists(doc).Should().BeTrue();
            //Process.Start(doc);
        }

        [TestMethod]
        public void GenerateFreeAgreement()
        {
            var attendees = new List<Attendee>();
            attendees.Add(new Attendee(new FullName("boudoux", "aurelien"), "DOT SHARK"));
            attendees.Add(new Attendee(new FullName("revel", "alexandre"), "TREND"));
            attendees.Add(new Attendee(new FullName("Aldebert", "Gregory"), "TREND"));

            var repo = new DocumentCreator(Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\"));
            var doc = repo.CreateFreeAgreement("2018 6001 T", "DOT SHARK", "111 rue francis de pressensé", "69100", "VILLEURBANNE", new FullName("boudoux", "aurelien"), "SET Niveau IV", new DateTime(2018,1,23), 3, "Saint PRIEST", attendees   );
            //Process.Start(doc);
        }

        [TestMethod]
        public void GenerateDetailedPaidAgreement()
        {
            var attendees = new List<Attendee>();
            attendees.Add(new Attendee(new FullName("boudoux", "aurelien"), "DOT SHARK"));
            attendees.Add(new Attendee(new FullName("revel", "alexandre"), "TREND"));
            attendees.Add(new Attendee(new FullName("Aldebert", "Gregory"), "TREND"));

            var repo = new DocumentCreator(Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\"));
            var doc = repo.CreatePaidAgreement("2018 6001 T", "DOT SHARK", "111 rue francis de pressensé", "69100", "VILLEURBANNE", new FullName("boudoux", "aurelien"), "SET Niveau IV", new DateTime(2018, 1, 23), 3, "Saint PRIEST", attendees, AgreementPriceType.DetailedPrice, 500);
            //Process.Start(doc);
        }

        [TestMethod]
        public void GeneratePackagePaidAgreement()
        {
            var attendees = new List<Attendee>();
            attendees.Add(new Attendee(new FullName("boudoux", "aurelien"), "DOT SHARK"));
            attendees.Add(new Attendee(new FullName("revel", "alexandre"), "TREND"));
            attendees.Add(new Attendee(new FullName("Aldebert", "Gregory"), "TREND"));

            var repo = new DocumentCreator(Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\"));
            var doc = repo.CreatePaidAgreement("2018 6001 T", "DOT SHARK", "111 rue francis de pressensé", "69100", "VILLEURBANNE", new FullName("boudoux", "aurelien"), "SET Niveau IV", new DateTime(2018, 1, 23), 3, "Saint PRIEST", attendees, AgreementPriceType.PackagePrice, 3500);
            //Process.Start(doc);
        }

        [TestMethod]
        public void GenerateFirstPage()
        {
            var repo = new DocumentCreator(Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\"));

            var signature = "Mélanie DUCOURTIOUX - yolo " + Environment.NewLine +
                            "Service Formation" + Environment.NewLine +
                            "Tél. : 04.37.54 13 60" + Environment.NewLine +
                            "Fax: 04 72 50 97 93" + Environment.NewLine;

            var doc = repo.CreateFirstPage("SET Niveau I", DateTime.Now, "DOT SHARK",new FullName("boudoux", "aurelien"), "111 rue francis de pressensé", "69100", "Villeurbanne", signature);
            //Process.Start(doc);
        }
    }
}