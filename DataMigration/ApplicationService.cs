using System;
using System.Linq;
using System.Reflection;
using GestionFormation.Applications;
using GestionFormation.CoreDomain.Users;
using GestionFormation.EventStore;
using GestionFormation.Kernel;

namespace DataMigration
{
    public class ApplicationService
    {
        private readonly UnityIocContainer _ioc = new UnityIocContainer();

        public ApplicationService()
        {
            var eventDispatcher = new EventDispatcher();
            eventDispatcher.AutoRegisterAllEventHandler();
            var eventBus = new EventBus(eventDispatcher, new SqlEventStore(new DomainEventJsonEventSerializer(), new EventStamping(new DataMigrationUser())));

            _ioc.Register(eventBus);
            AutoRegisterQueries(Assembly.GetAssembly(typeof(IRuntimeDependency)));
        }

        public T Command<T>() where T : ActionCommand
        {
            return _ioc.Resolve<T>();
        }

        private void AutoRegisterQueries(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            foreach (var type in assembly.GetAllConcretTypeThatImplementInterface<IRuntimeDependency>())
            {
                var firstInterface = type.GetInterfaces().FirstOrDefault();
                if (firstInterface == null || firstInterface == typeof(IRuntimeDependency))
                    throw new Exception("Impossible de trouver l'interface implémentée par le IRuntimeDependency de type " + type.Name);

                _ioc.Register(firstInterface, Activator.CreateInstance(type));
            }
        }
    }

    public class DataMigrationUser : ILoggedUser
    {
        public DataMigrationUser()
        {
            UserId = Guid.Empty;
            Login = "MIGRATION";
            Nom = "Programme de migration";
            Role = UserRole.Admin;            
        }

        public Guid UserId { get; }
        public string Login { get; }
        public string Nom { get; }
        public string Prenom { get; }
        public UserRole Role { get; }
    }
}