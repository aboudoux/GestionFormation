using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace GestionFormation.Kernel
{
    public class DomainEventTypeBinder : ISerializationBinder
    {
        private readonly List<Type> _knownTypes = new List<Type>();

        public DomainEventTypeBinder(Assembly assemblyToScan = null)
        {
            ScanAssemblyorRegisterAllDomainEvents(assemblyToScan ?? Assembly.GetExecutingAssembly());
        }

        private void ScanAssemblyorRegisterAllDomainEvents(Assembly assemblyToScan)
        {
            foreach (var type in assemblyToScan.GetAllConcretTypeThatImplementInterface<IDomainEvent>())
                _knownTypes.Add(type);
        }

        public Type BindToType(string assemblyName, string typeName)
        {
            return _knownTypes.SingleOrDefault(t => t.Name == typeName);
        }

        public void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = null;
            typeName = serializedType.Name;
        }
    }
}