using Lab8.MVVM.Model;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Lab8.MVVM.View.Patients
{
    public partial class EditPatient : Window
    {
        private Clinic_DatabaseEntities dbContext;
        private ObservableCollection<PatientModel> tableModel;
        private int index;

        public EditPatient(ObservableCollection<PatientModel> tableModel, int index)
        {
            InitializeComponent();
            this.tableModel = tableModel;
            this.index = index;

            mFieldSurname.Text = tableModel[index].Patient_Object.Surname;
            mFieldName.Text = tableModel[index].Patient_Object.Name;
            mFieldLastname.Text = tableModel[index].Patient_Object.Lastname;

            string gender = tableModel[index].Gender;
            if (gender.Equals("Мужской"))
            {
                mListGender.SelectedIndex = 0;
            }
            else if (gender.Equals("Женский"))
            {
                mListGender.SelectedIndex = 1;
            }

            mFieldBirthday.Text = tableModel[index].Patient_Object.Birthday.ToString("dd.MM.yyyy");
            mFieldAddress.Text = tableModel[index].Patient_Object.Address;
            mFieldPhone.Text = tableModel[index].Patient_Object.Phone;

            dbContext = new Clinic_DatabaseEntities();
            dbContext.Patients.Load();
        }

        private void mButtonEdit_Click(object sender, RoutedEventArgs e)
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

            // Сохранение экземпляра таблицы в переменной
            Patient patient = tableModel[index].Patient_Object;
            patient.Surname = surname;
            patient.Name = name;
            patient.Lastname = lastname;

            if (gender.Equals("Мужской"))
            {
                patient.Gender = "m";
            }
            else if (gender.Equals("Женский"))
            {
                patient.Gender = "f";
            }

            patient.Birthday = birthday;
            patient.Address = address;
            patient.Phone = phone;

            int rowNum = tableModel[index].Row_Number;

            // Удаление старой записи
            tableModel.RemoveAt(index);

            // Создание обновленной модели
            PatientModel patientModel = new PatientModel();
            patientModel.Row_Number = rowNum;
            patientModel.Gender = gender;
            patientModel.Patient_Object = patient;

            // Запись обновленной модели в GUI таблицу
            tableModel.Insert(index, patientModel);

            // Выборка записи таблицы из БД
            var patientBD = dbContext.Patients.First(p => p.Patient_Id == patientModel.Patient_Object.Patient_Id);

            // Изменение информации в строке
            patientBD.Surname = surname;
            patientBD.Name = name;
            patientBD.Lastname = lastname;

            if (gender.Equals("Мужской"))
            {
                patientBD.Gender = "m";
            }
            else if (gender.Equals("Женский"))
            {
                patientBD.Gender = "f";
            }

            patientBD.Birthday = birthday;
            patientBD.Address = address;
            patientBD.Phone = phone;

            // Сохранение изменений в БД
            dbContext.SaveChanges();
        }
    }
}
