using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BookingApp.Model;
using BookingApp.Services;

namespace BookingApp.ViewModel
{
    public class HotelsViewModel : BaseViewModel
    {
        private readonly HotelService _hotelService;

        // Svi hoteli iz servisa (osnovna lista)
        private List<Hotel> _allHotels = new List<Hotel>();

        // Kolekcija koja se binduje na DataGrid
        public ObservableCollection<Hotel> Hotels { get; } = new ObservableCollection<Hotel>();

        // Opcije sortiranja koje prikazujemo u ComboBox-u
        public List<string> SortOptions { get; } =
            new List<string> { "No sort", "Stars ascending", "Stars descending" };

        private string _selectedSortOption;
        public string SelectedSortOption
        {
            get => _selectedSortOption;
            set
            {
                if (_selectedSortOption != value)
                {
                    _selectedSortOption = value;
                    OnPropertyChanged();
                    ApplySort();
                }
            }
        }

        public HotelsViewModel(HotelService hotelService)
        {
            _hotelService = hotelService;
            LoadHotels();

            // podrazumevano bez sortiranja
            SelectedSortOption = SortOptions[0];
        }

        private void LoadHotels()
        {
            _allHotels = _hotelService.GetAll() ?? new List<Hotel>();
            RefreshCollection(_allHotels);
        }

        private void ApplySort()
        {
            IEnumerable<Hotel> sorted = _allHotels;

            switch (SelectedSortOption)
            {
                case "Stars ascending":
                    sorted = _allHotels.OrderBy(h => h.Stars);
                    break;

                case "Stars descending":
                    sorted = _allHotels.OrderByDescending(h => h.Stars);
                    break;

                case "No sort":
                default:
                    sorted = _allHotels;
                    break;
            }

            RefreshCollection(sorted.ToList());
        }

        private void RefreshCollection(List<Hotel> hotels)
        {
            Hotels.Clear();
            foreach (var h in hotels)
            {
                Hotels.Add(h);
            }
        }
    }
}