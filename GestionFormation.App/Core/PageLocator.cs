using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;

namespace GestionFormation.App.Core
{
    public class PageLocator
    {
        private readonly Dictionary<Type, Type> _vmToPageAssociation;

        private PageLocator(Dictionary<Type, Type> vmToPageAssociation)
        {
            _vmToPageAssociation = vmToPageAssociation ?? throw new ArgumentNullException(nameof(vmToPageAssociation));
        }

        public object GetPageFor<T>(T vm) where T : ViewModelBase
        {
            var vmType = typeof(T);
            if (!_vmToPageAssociation.ContainsKey(vmType))
                throw new Exception($"Impossible de trouver le viewModel {vmType.Name}. Vérifiez que ce type à une correspondance dans le PageLocator (bootstrapper)");
            var page = Activator.CreateInstance(_vmToPageAssociation[vmType]);
            AssignBindingContextIfExist(page, vm);
            return page;
        }

        private static void AssignBindingContextIfExist(object obj, ViewModelBase value)
        {
            var bindingContext = obj.GetType().GetProperty("DataContext");
            bindingContext?.SetValue(obj, value);
        }

        public static PageAssociation With()
        {
            return new PageAssociation();
        }

        public class PageAssociation
        {
            private readonly Dictionary<Type, Type> _vmToPageAssociation = new Dictionary<Type, Type>();

            public PageAssociation Element<TViewModel, TPage>() where TViewModel : ViewModelBase
            {
                _vmToPageAssociation.Add(typeof(TViewModel), typeof(TPage));
                return this;
            }

            public PageLocator Build()
            {
                return new PageLocator(_vmToPageAssociation);
            }
        }
    }
}