using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using BookingApp.Model;
using BookingApp.Services;

namespace BookingApp.ViewModel
{
    public class SearchHotelsViewModel : BaseViewModel
    {
        public bool IsApartmentSearch => SelectedSearchType == "By apartments";

        private readonly HotelService _hotelService;

        public ObservableCollection<Hotel> Hotels { get; } = new ObservableCollection<Hotel>();

        public List<string> SearchTypes { get; } =
            new List<string> { "By name", "By year", "By stars", "By apartments" };

        private string _selectedSearchType;
        public string SelectedSearchType
        {
            get => _selectedSearchType;
            set
            {
                if (_selectedSearchType != value)
                {
                    _selectedSearchType = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsApartmentSearch));
                }
            }
        }

        public bool IsRoomsOnly => SelectedApartmentSearchMode == "Rooms only";
        public bool IsGuestsOnly => SelectedApartmentSearchMode == "Guests only";
        public bool IsRoomsAndGuests => SelectedApartmentSearchMode == "Rooms + Guests";

        public List<string> ApartmentSearchModes { get; } = new List<string> { "Rooms only", "Guests only", "Rooms + Guests" };

        private string _selectedApartmentSearchMode;
        public string SelectedApartmentSearchMode
        {
            get => _selectedApartmentSearchMode;
            set
            {
                if (_selectedApartmentSearchMode != value)
                {
                    _selectedApartmentSearchMode = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsRoomsOnly));
                    OnPropertyChanged(nameof(IsGuestsOnly));
                    OnPropertyChanged(nameof(IsRoomsAndGuests));
                }
            }
        }

        // Za name/year/stars pretragu
        private string _queryText;
        public string QueryText
        {
            get => _queryText;
            set
            {
                if (_queryText != value)
                {
                    _queryText = value;
                    OnPropertyChanged();
                }
            }
        }

        // Za pretragu po apartmanima
        private string _roomsText;
        public string RoomsText
        {
            get => _roomsText;
            set
            {
                if (_roomsText != value)
                {
                    _roomsText = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _guestsText;
        public string GuestsText
        {
            get => _guestsText;
            set
            {
                if (_guestsText != value)
                {
                    _guestsText = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<string> ApartmentOperatorOptions { get; } = new List<string> { "AND", "OR" };

        private string _selectedApartmentOperator;
        public string SelectedApartmentOperator
        {
            get => _selectedApartmentOperator;
            set
            {
                if (_selectedApartmentOperator != value)
                {
                    _selectedApartmentOperator = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand SearchCommand { get; }

        public SearchHotelsViewModel(HotelService hotelService)
        {
            _hotelService = hotelService;
            SelectedSearchType = SearchTypes[0];
            SelectedApartmentSearchMode = ApartmentSearchModes[0];
            SelectedApartmentOperator = ApartmentOperatorOptions[0]; // "None"

            SearchCommand = new RelayCommand(_ => ExecuteSearch());
        }

        private void ExecuteSearch()
        {
            ErrorMessage = string.Empty;
            Hotels.Clear();

            try
            {
                List<Hotel> result = null;

                switch (SelectedSearchType)
                {
                    case "By name":
                        if (string.IsNullOrWhiteSpace(QueryText))
                        {
                            ErrorMessage = "Please enter name part.";
                            return;
                        }
                        result = _hotelService.SearchByName(QueryText);
                        break;

                    case "By year":
                        if (!int.TryParse(QueryText, out int year))
                        {
                            ErrorMessage = "Year must be a number.";
                            return;
                        }
                        result = _hotelService.SearchByYear(year);
                        break;

                    case "By stars":
                        if (!int.TryParse(QueryText, out int stars))
                        {
                            ErrorMessage = "Stars must be a number.";
                            return;
                        }
                        result = _hotelService.SearchByStars(stars);
                        break;

                    case "By apartments":
                        // Reset error
                        ErrorMessage = string.Empty;

                        int? rooms = null;
                        int? guests = null;

                        if (IsRoomsOnly || IsRoomsAndGuests)
                        {
                            if (string.IsNullOrWhiteSpace(RoomsText))
                            {
                                ErrorMessage = "Enter rooms.";
                                return;
                            }
                            if (!int.TryParse(RoomsText, out int r))
                            {
                                ErrorMessage = "Rooms must be a number.";
                                return;
                            }
                            rooms = r;
                        }

                        if (IsGuestsOnly || IsRoomsAndGuests)
                        {
                            if (string.IsNullOrWhiteSpace(GuestsText))
                            {
                                ErrorMessage = "Enter guests.";
                                return;
                            }
                            if (!int.TryParse(GuestsText, out int g))
                            {
                                ErrorMessage = "Guests must be a number.";
                                return;
                            }
                            guests = g;
                        }

                        string logicalOp = "&"; // default

                        if (IsRoomsOnly)
                        {
                            logicalOp = "&";
                        }
                        else if (IsGuestsOnly)
                        {
                            logicalOp = "&";
                        }
                        else if (IsRoomsAndGuests)
                        {
                            if (SelectedApartmentOperator == "OR")
                            {
                                logicalOp = "|"; // barem jedan uslov
                            }
                            else
                            {
                                logicalOp = "&"; // oba uslova
                            }
                        }

                        result = _hotelService.SearchByApartments(rooms, guests, logicalOp);
                        break;
                }

                if (result != null)
                {
                    foreach (var h in result)
                        Hotels.Add(h);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
        }
    }
}
