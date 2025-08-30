using System.Collections.ObjectModel;

namespace DentalCareSystem.ViewModel
{
    public class GenericDataGridViewModel<T>
    {
        public IList<T> Objects = new List<T>();
        public ObservableCollection<T> Items { get; set; }
        public T SelectedItem { get; set; }
    }
}