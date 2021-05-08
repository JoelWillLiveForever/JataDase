using Lab8.Core;

namespace Lab8.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {
        public RelayCommand DoctorsViewCommand { get; set; }
        public RelayCommand SpecialtyesViewCommand { get; set; }
        public RelayCommand PatientsViewCommand { get; set; }
        public RelayCommand ReceptionsViewCommand { get; set; }

        public DoctorsViewModel DoctorsVM { get; set; }
        public SpecialtyesViewModel SpecialtyesVM { get; set; }
        public PatientsViewModel PatientsVM { get; set; }
        public ReceptionsViewModel ReceptionsVM { get; set; }

        private object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            DoctorsVM = new DoctorsViewModel();
            SpecialtyesVM = new SpecialtyesViewModel();
            PatientsVM = new PatientsViewModel();
            ReceptionsVM = new ReceptionsViewModel();

            CurrentView = ReceptionsVM;

            DoctorsViewCommand = new RelayCommand(o =>
            {
                CurrentView = DoctorsVM;
            });

            SpecialtyesViewCommand = new RelayCommand(o => 
            {
                CurrentView = SpecialtyesVM;
            });

            PatientsViewCommand = new RelayCommand(o =>
            {
                CurrentView = PatientsVM;
            });

            ReceptionsViewCommand = new RelayCommand(o =>
            {
                CurrentView = ReceptionsVM;
            });
        }
    }
}
