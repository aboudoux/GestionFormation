using System;

namespace DataMigration.Creators
{
    public abstract class Creator
    {
        protected readonly Mapper Mapper = new Mapper();
        protected readonly ApplicationService App;

        protected Creator(ApplicationService applicationService)
        {
            App = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
        }
    }
}