using System;
using System.Collections.Generic;
using System.Linq;

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

        public Guid GetId(string key)
        {
            return Mapper.GetId(key);
        }

        public virtual string ConstructKey(string source)
        {
            return source;
        }

        public IEnumerable<Guid> GetAll()
        {
            return Mapper.GetValues();
        }
    }
}