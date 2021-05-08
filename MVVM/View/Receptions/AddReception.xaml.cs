using Lab8.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Lab8.MVVM.View.Receptions
{
    public partial class AddReception : Window
    {
        private Clinic_DatabaseEntities dbContext;
        private ObservableCollection<ReceptionModel> tableModel;

        private BindingList<Doctor> doctors;
        private BindingList<Patient> patients;
        private BindingList<Specialty> specialtyes;

        public AddReception(ObservableCollection<ReceptionModel> tableModel)
        {
            InitializeComponent();

            this.tableModel = tableModel;
            dbContext = new Clinic_DatabaseEntities();


            dbContext.Specialties.Load();
            specialtyes = dbContext.Specialties.Local.ToBindingList();

            for (int i = 0; i < specialtyes.Count; i++)
            {
                mListSpecialty.Items.Add(specialtyes[i].Title);
            }

            dbContext.Patients.Load();
            patients = dbContext.Patients.Local.ToBindingList();

            for (int i = 0; i < patients.Count; i++)
            {
                // Добавление короткого имени пациента
                string Surname = patients[i].Surname;
                string Name = patients[i].Name;
                string Lastname = patients[i].Lastname;
                string shortName;

                if (Lastname == null || Lastname.Equals(""))
                    shortName = Surname + " " + Name.Substring(0, 1) + ".";
                else
                    shortName = Surname + " " + Name.Substring(0, 1) + "." + Lastname.Substring(0, 1) + ".";

                mListPatient.Items.Add(shortName);
            }

            dbContext.Doctors.Load();
            doctors = dbContext.Doctors.Local.ToBindingList();

            for (int i = 0; i < doctors.Count; i++)
            {
                // Добавление короткого имени врача
                string Surname = doctors[i].Surname;
                string Name = doctors[i].Name;
                string Lastname = doctors[i].Lastname;
                string shortName;

                if (Lastname == null || Lastname.Equals(""))
                    shortName = Surname + " " + Name.Substring(0, 1) + ".";
                else
                    shortName = Surname + " " + Name.Substring(0, 1) + "." + Lastname.Substring(0, 1) + ".";

                mListDoctor.Items.Add(shortName);
            }

            dbContext.Receptions.Load();
        }

        private void mButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            int specialtyIndex = mListSpecialty.SelectedIndex;
            int doctorIndex = mListDoctor.SelectedIndex;
            int patientIndex = mListPatient.SelectedIndex;

            // Добавление короткого имени врача
            string Surname1 = doctors[doctorIndex].Surname;
            string Name1 = doctors[doctorIndex].Name;
            string Lastname1 = doctors[doctorIndex].Lastname;
            string doctorShortName;

            if (Lastname1 == null || Lastname1.Equals(""))
                doctorShortName = Surname1 + " " + Name1.Substring(0, 1) + ".";
            else
                doctorShortName = Surname1 + " " + Name1.Substring(0, 1) + "." + Lastname1.Substring(0, 1) + ".";

            // Добавление короткого имени пациента
            string Surname2 = patients[patientIndex].Surname;
            string Name2 = patients[patientIndex].Name;
            string Lastname2 = patients[patientIndex].Lastname;
            string patientShortName;

            if (Lastname2 == null || Lastname2.Equals(""))
                patientShortName = Surname2 + " " + Name2.Substring(0, 1) + ".";
            else
                patientShortName = Surname2 + " " + Name2.Substring(0, 1) + "." + Lastname2.Substring(0, 1) + ".";

            // Оставшееся
            string receptionDate = mFieldDate.Text;
            string receptionTime = mFieldTime.Text;
            string receptionType = mFieldType.Text;

            if (receptionDate == null || receptionDate.Equals(""))
            {
                MessageBox.Show("Укажите дату приёма!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (receptionTime == null || receptionTime.Equals(""))
            {
                MessageBox.Show("Укажите время приёма!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (receptionType == null || receptionType.Equals(""))
            {
                MessageBox.Show("Укажите тип приёма!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DateTime date = DateTime.ParseExact(receptionDate, "dd.MM.yyyy", null);
            TimeSpan time = TimeSpan.Parse(receptionTime);

            ReceptionModel receptionModel = new ReceptionModel();
            receptionModel.Row_Number = tableModel.Count + 1;
            receptionModel.DoctorShortName = doctorShortName;
            receptionModel.PatientShortName = patientShortName;

            receptionModel.Reception_Object = new Reception();
            receptionModel.Reception_Object.Specialty_Id = specialtyes[specialtyIndex].Specialty_Id;
            receptionModel.Reception_Object.Doctor_Id = doctors[doctorIndex].Doctor_Id;
            receptionModel.Reception_Object.Patient_Id = patients[patientIndex].Patient_Id;
            receptionModel.Reception_Object.Date = date;
            receptionModel.Reception_Object.Time = time;

            // Преобразование типов
            if (receptionType.Equals("Первичный"))
            {
                receptionModel.Reception_Object.Type = "p";
            } else if (receptionType.Equals("Вторичный"))
            {
                receptionModel.Reception_Object.Type = "s";
            }
            receptionModel.ReceptionType = receptionType;

            tableModel.Add(receptionModel);

            dbContext.Receptions.Add(receptionModel.Reception_Object);
            dbContext.SaveChanges();

            dbContext.Receptions.Attach(receptionModel.Reception_Object);
            dbContext.SaveChanges();

            // Сброс формы
            mListSpecialty.SelectedIndex = 0;
            mListDoctor.SelectedIndex = 0;
            mListPatient.SelectedIndex = 0;

            mFieldDate.Text = "";
            mFieldTime.Text = "";
            mFieldType.Text = "";
        }
    }
}
