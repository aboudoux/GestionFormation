using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Xpf.Docking;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GestionFormation.Applications;
using GestionFormation.EventStore;
using GestionFormation.Kernel;

namespace GestionFormation.App.Core
{
    public class ApplicationService : IApplicationService
    {
        private readonly DocumentGroup _documentGroup;
        private readonly PageLocator _pageLocator;
        private readonly IIocContainer _ioc = new UnityIocContainer();
        public ILoggedUser LoggedUser { get; set; } = new LocalApplicationUser();

        public ApplicationService(PageLocator pageLocator, DocumentGroup documentGroup, EventBus eventBus, IMessenger messenger)
        {
            _documentGroup = documentGroup;
            _pageLocator = pageLocator ?? throw new ArgumentNullException(nameof(pageLocator));

            _ioc.Register<IApplicationService>(this);
            _ioc.Register(messenger);
            _ioc.Register(eventBus);
        }

        public void AutoRegisterSimpleDependencies(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            foreach (var type in assembly.GetAllConcretTypeThatImplementInterface<IRuntimeDependency>())
            {
                var firstInterface = type.GetInterfaces().FirstOrDefault();
                if( firstInterface == null || firstInterface == typeof(IRuntimeDependency))
                    throw new Exception("Impossible de trouver l'interface implémentée par le runtimeQueries de type " + type.Name);                               

                _ioc.Register(firstInterface, Activator.CreateInstance(type));
            }
        }

        public T OpenDocument<T>(params object[] injectionParameters) where T : ViewModelBase
        {
            var result = Open<T>(injectionParameters);
            var document = result.page as DocumentPanel;
            _documentGroup.Add(document);
            document?.ActivateCommand.Execute(null);
            return result.vm;
        }

        public async Task<T> OpenPopup<T>(params object[] injectionParameters) where T : ViewModelBase, IPopupVm
        {
            var result = Open<T>(injectionParameters);            
            var window = result.page as Window;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            result.vm.OnClose += (sender, args) => window.Close();
            await result.vm.Init();
            window?.ShowDialog();            
            return result.vm;
        }

        public T Command<T>() where T : ActionCommand
        {
            return _ioc.Resolve<T>();
        }
      
        private (T vm, object page) Open<T>(params object[] injectionParameters) where T : ViewModelBase
        {
            var vm = _ioc.Resolve<T>(injectionParameters);
            var page = _pageLocator.GetPageFor(vm);           
            return (vm, page);
        }
    }
}