using System;
using GalaSoft.MvvmLight.Messaging;

namespace GestionFormation.Tests.Fakes
{
    public class FakeMessenger : IMessenger
    {
        public void Register<TMessage>(object recipient, Action<TMessage> action)
        {
        }

        public void Register<TMessage>(object recipient, object token, Action<TMessage> action)
        {
        }

        public void Register<TMessage>(object recipient, object token, bool receiveDerivedMessagesToo, Action<TMessage> action)
        {
        }

        public void Register<TMessage>(object recipient, bool receiveDerivedMessagesToo, Action<TMessage> action)
        {
        }

        public void Send<TMessage>(TMessage message)
        {
        }

        public void Send<TMessage, TTarget>(TMessage message)
        {
        }

        public void Send<TMessage>(TMessage message, object token)
        {
        }

        public void Unregister(object recipient)
        {
        }

        public void Unregister<TMessage>(object recipient)
        {
        }

        public void Unregister<TMessage>(object recipient, object token)
        {
        }

        public void Unregister<TMessage>(object recipient, Action<TMessage> action)
        {
            throw new NotImplementedException();
        }

        public void Unregister<TMessage>(object recipient, object token, Action<TMessage> action)
        {
        }
    }
}