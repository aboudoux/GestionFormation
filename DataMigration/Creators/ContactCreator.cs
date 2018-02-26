namespace DataMigration.Creators
{
    public class ContactCreator : Creator
    {
        public ContactCreator(ApplicationService applicationService) : base(applicationService)
        {
        }

        public void Create(string contact, string email, string telephone, string companyName)
        {
            if(string.IsNullOrWhiteSpace(companyName)) return;
            if(string.IsNullOrWhiteSpace(contact)) return;


        }
    }
}