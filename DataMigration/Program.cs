using System;
using System.Collections.Generic;
using System.Data;
using DataMigration.Creators;
using GestionFormation.Applications.Locations;
using GestionFormation.Applications.Trainers;
using GestionFormation.Applications.Trainings;

namespace DataMigration
{
    class Program
    {
        static void Main()
        {
            var app = new ApplicationService();

            var training = new TrainingCreator(app);
            var location = new LocationCreator(app);
            var trainer = new TrainerCreator(app);
            var company = new CompanyCreator(app);
            var student = new StudentCreator(app);
            var contact = new ContactCreator(app, company);
            var session = new SessionCreator(app, training, trainer, location, student, company);

            var reader = new MsAccessReader("c:\\temp\\Database1.mdb");
            var lineCount = 1;

            foreach (DataRow row in reader.GetRows("Formation"))
            {
                Console.Write($"Traitement de la ligne {lineCount++}\r");
                training.Create(row["Formation"].ToString());
                location.Create(row["Lieu"].ToString());
                trainer.Create(row["Formateur"].ToString());
                company.Create(row["Societe"].ToString(), row["Adresse"].ToString(), row["CP"].ToString(), row["Ville"].ToString());
                student.Create(row["Stagiaire"].ToString());
                contact.Create(row["Contact"].ToString(), row["Email"].ToString(), row["Telephone"].ToString(), row["Societe"].ToString());
                session.Create(DateTime.Parse(row["DateFormation"].ToString()), int.Parse(row["NbJour"].ToString()), row["Formation"].ToString(), row["Formateur"].ToString(), row["Lieu"].ToString(), row["Stagiaire"].ToString(), row["Societe"].ToString());
            }

        /*    DisableAll("Formations", training.GetAll(), id => app.Command<DisableTraining>().Execute(id));
            DisableAll("Lieux", location.GetAll(), id => app.Command<DisableLocation>().Execute(id));
            DisableAll("Formateur", trainer.GetAll(), id => app.Command<DisableTrainer>().Execute(id));*/

            Console.WriteLine("\r\nImport terminé !");
            Console.ReadKey();
        }

        private static void DisableAll(string label, IEnumerable<Guid> ids, Action<Guid> action)
        {
            var i = 1;
            Console.WriteLine("\r\nDesactivation des " + label);
            foreach (var guid in ids)
            {
                Console.Write($"Traitement de la ligne {i++}\r");
                action(guid);
            }
        }        
    }
}
