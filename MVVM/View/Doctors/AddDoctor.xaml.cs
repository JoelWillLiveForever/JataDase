using System.Windows;
using System.Data.Entity;
using Lab8.MVVM.Model;
using System.Collections.ObjectModel;

namespace Lab8.MVVM.View.Doctors
{
    public partial class AddDoctor : Window
    {
        private Clinic_DatabaseEntities dbContext;
        private ObservableCollection<DoctorModel> tableModel;

        public AddDoctor(ObservableCollection<DoctorModel> tableModel)
        {
            InitializeComponent();

            this.tableModel = tableModel;

            dbContext = new Clinic_DatabaseEntities();
            dbContext.Doctors.Load();
        }

        private void mButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            string surname = mFieldSurname.Text;
            string name = mFieldName.Text;
            string lastname = mFieldLastname.Text;

            if (surname == null || surname.Equals(""))
            {
                MessageBox.Show("Укажите фамилию!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            } else if (name == null || name.Equals(""))
            {
                MessageBox.Show("Укажите имя!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DoctorModel doctorModel = new DoctorModel();
            doctorModel.Row_Number = tableModel.Count + 1;

            doctorModel.Doctor_Object = new Doctor();
            doctorModel.Doctor_Object.Surname = surname;
            doctorModel.Doctor_Object.Name = name;
            doctorModel.Doctor_Object.Lastname = lastname;

            dbContext.Doctors.Add(doctorModel.Doctor_Object);
            dbContext.SaveChanges();

            tableModel.Add(doctorModel);

            mFieldSurname.Text = "";
            mFieldName.Text = "";
            mFieldLastname.Text = "";
        }
    }
}
