using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace GestionFormation.App.Views.Listers.Bases
{
    public class StatisticWindowVm : ViewModelBase
    {
        private ObservableCollection<object> _items;

        public StatisticWindowVm(string title)
        {
            Title = "Statistiques de " + title;
        }

        public string Title { get; }

        public ObservableCollection<object> Items
        {
            get => _items;
            set { Set(()=>Items, ref _items, value); }
        }
    }
}