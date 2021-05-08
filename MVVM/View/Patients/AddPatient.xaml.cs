using Lab8.MVVM.Model;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Windows;
using System.Windows.Controls;

namespace Lab8.MVVM.View.Patients
{
    public partial class AddPatient : Window
    {
        private Clinic_DatabaseEntities dbContext;
        private ObservableCollection<PatientModel> tableModel;

        public AddPatient(ObservableCollection<PatientModel> tableModel)
        {
            InitializeComponent();

            this.tableModel = tableModel;

            dbContext = new Clinic_DatabaseEntities();
            dbContext.Patients.Load();
        }

        private void mButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            string surname = mFieldSurname.Text;
            string name = mFieldName.Text;
            string lastname = mFieldLastname.Text;

            ComboBoxItem typeItem = (ComboBoxItem)mListGender.SelectedItem;
            string gender = typeItem.Content.ToString();

            string date = mFieldBirthday.Text;

            string address = mFieldAddress.Text;
            string phone = mFieldPhone.Text;

            if (surname == null || surname.Equals(""))
            {
                MessageBox.Show("Укажите фамилию!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (name == null || name.Equals(""))
            {
                MessageBox.Show("Укажите имя!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (address == null || address.Equals(""))
            {
                MessageBox.Show("Укажите адрес проживания!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (phone == null || phone.Equals(""))
            {
                MessageBox.Show("Укажите номер телефона!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DateTime birthday = DateTime.ParseExact(date, "dd.MM.yyyy", null);

            PatientModel patientModel = new PatientModel();
            patientModel.Row_Number = tableModel.Count + 1;
            patientModel.Gender = gender;

            patientModel.Patient_Object = new Patient();
            patientModel.Patient_Object.Surname = surname;
            patientModel.Patient_Object.Name = name;
            patientModel.Patient_Object.Lastname = lastname;

            if (gender.Equals("Мужской"))
                gender = "m";
            else if (gender.Equals("Женский"))
                gender = "f";

            patientModel.Patient_Object.Gender = gender;
            patientModel.Patient_Object.Birthday = birthday;
            patientModel.Patient_Object.Address = address;
            patientModel.Patient_Object.Phone = phone;

            dbContext.Patients.Add(patientModel.Patient_Object);
            dbContext.SaveChanges();

            tableModel.Add(patientModel);

            mFieldSurname.Text = "";
            mFieldName.Text = "";
            mFieldLastname.Text = "";
            mFieldBirthday.Text = "";
            mFieldAddress.Text = "";
            mFieldPhone.Text = "";
            mListGender.SelectedIndex = 0;
        }
    }
}
