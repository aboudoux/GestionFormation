using System;

namespace GestionFormation.App.Core
{
    public interface IIocContainer
    {
        void Register<T>(T instance);
        void Register(Type t, object instance);
        T Resolve<T>(params object[] parameters);
    }
}