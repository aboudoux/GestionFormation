using System;
using System.Linq;
using System.Reflection;
using GestionFormation.App.Core;
using GestionFormation.Applications;
using GestionFormation.EventStore;
using GestionFormation.Kernel;
using GestionFormation.Tests.Fakes;

namespace GestionFormation.Tests.Tools
{
    public class SqlTestApplicationService
    {
        private readonly IIocContainer _ioc = new UnityIocContainer();
        
        public SqlTestApplicationService()
        {
            var eventDispatcher = new EventDispatcher();
            eventDispatcher.AutoRegisterAllEventHandler();
            var eventBus = new EventBus(eventDispatcher, new SqlEventStore(new DomainEventJsonEventSerializer(), new FakeEventStamping()));

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
                    throw new Exception("Impossible de trouver l'interface implémentée par le runtimeQueries de type " + type.Name);

                _ioc.Register(firstInterface, Activator.CreateInstance(type));
            }
        }
    }
}
