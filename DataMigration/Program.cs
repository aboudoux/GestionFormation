using System;
using System.Data;
using DataMigration.Creators;

namespace DataMigration
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new ApplicationService();

            var training = new TrainingCreator(app);
            var location = new LocationCreator(app);
            var trainer = new TrainerCreator(app);
            var company = new CompanyCreator(app);
            var student = new StudentCreator(app);
            var contact = new ContactCreator(app, company);

            var reader = new MsAccessReader("d:\\formation\\Gestion Formation_2014.mdb");
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
            }

            Console.WriteLine("\r\nImport terminé !");
            Console.ReadKey();
        }
    }
}
