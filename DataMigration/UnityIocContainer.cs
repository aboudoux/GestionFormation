using System;
using System.Collections.Generic;
using System.Linq;
using Unity;
using Unity.Resolution;

namespace DataMigration
{
    public class UnityIocContainer
    {
        private readonly UnityContainer _ioc = new UnityContainer();
        public void Register<T>(T instance)
        {
            _ioc.RegisterInstance(instance);
        }

        public void Register(Type t, object instance)
        {
            _ioc.RegisterInstance(t, instance);
        }

        public T Resolve<T>(params object[] parameters)
        {
            return parameters?.Length > 0 ? _ioc.Resolve<T>(GetParameters<T>(parameters)) : _ioc.Resolve<T>();
        }

        private static ResolverOverride[] GetParameters<T>(object[] parameters)
        {
            var constructorParameters = typeof(T).GetConstructors().First().GetParameters();
            var overrides = new List<ResolverOverride>();

            foreach (var parameter in parameters)
            {
                var param = constructorParameters.FirstOrDefault(a => a.ParameterType.IsInstanceOfType(parameter));
                if (param == null)
                    throw new Exception($"IOC error : the parameter of type {parameter.GetType()} is not supported by the first constructor of {typeof(T).Name}");
                overrides.Add(new ParameterOverride(param.Name, parameter));
            }

            return overrides.ToArray();
        }
    }
}