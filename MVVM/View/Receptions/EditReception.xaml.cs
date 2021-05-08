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
    public partial class EditReception : Window
    {
        private Clinic_DatabaseEntities dbContext;
        private ObservableCollection<ReceptionModel> tableModel;

        private BindingList<Doctor> doctors;
        private BindingList<Patient> patients;
        private BindingList<Specialty> specialtyes;

        private int index;

        public EditReception(ObservableCollection<ReceptionModel> tableModel, int index)
        {
            InitializeComponent();

            this.tableModel = tableModel;
            this.index = index;

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

            int specialtyIndex = -1;
            for (int i = 0; i < specialtyes.Count; i++)
            {
                if (specialtyes[i].Specialty_Id == tableModel[index].Reception_Object.Specialty_Id)
                {
                    specialtyIndex = i;
                    break;
                }
            }

            if (specialtyIndex == -1)
            {
                MessageBox.Show("Критическая ошибка! Специальность не найдена!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int doctorIndex = -1;
            for (int i = 0; i < doctors.Count; i++)
            {
                if (doctors[i].Doctor_Id == tableModel[index].Reception_Object.Doctor_Id)
                {
                    doctorIndex = i;
                    break;
                }
            }

            if (doctorIndex == -1)
            {
                MessageBox.Show("Критическая ошибка! Доктор не найден!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int patientIndex = -1;
            for (int i = 0; i < patients.Count; i++)
            {
                if (patients[i].Patient_Id == tableModel[index].Reception_Object.Patient_Id)
                {
                    patientIndex = i;
                    break;
                }
            }

            if (patientIndex == -1)
            {
                MessageBox.Show("Критическая ошибка! Пациент не найден!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            mListSpecialty.SelectedIndex = specialtyIndex;
            mListDoctor.SelectedIndex = doctorIndex;
            mListPatient.SelectedIndex = patientIndex;

            mFieldDate.Text = tableModel[index].Reception_Object.Date.ToString("dd.MM.yyyy");
            mFieldTime.Text = tableModel[index].Reception_Object.Time.ToString(@"hh\:mm");
            mFieldType.Text = tableModel[index].ReceptionType;
        }

        private void mButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            int specialtyIndex = mListSpecialty.SelectedIndex;
            int doctorIndex = mListDoctor.SelectedIndex;
            int patientIndex = mListPatient.SelectedIndex;

            // Обновление короткого имени врача
            string Surname1 = doctors[doctorIndex].Surname;
            string Name1 = doctors[doctorIndex].Name;
            string Lastname1 = doctors[doctorIndex].Lastname;
            string doctorShortName;

            if (Lastname1 == null || Lastname1.Equals(""))
                doctorShortName = Surname1 + " " + Name1.Substring(0, 1) + ".";
            else
                doctorShortName = Surname1 + " " + Name1.Substring(0, 1) + "." + Lastname1.Substring(0, 1) + ".";

            // Обновление короткого имени пациента
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

            // Сохранение экземпляра таблицы в переменной
            Reception reception = tableModel[index].Reception_Object;
            reception.Specialty_Id = specialtyes[specialtyIndex].Specialty_Id;
            reception.Doctor_Id = doctors[doctorIndex].Doctor_Id;
            reception.Patient_Id = patients[patientIndex].Patient_Id;
            reception.Date = date;
            reception.Time = time;

            // Преобразование типов
            if (receptionType.Equals("Первичный"))
            {
                reception.Type = "p";
            }
            else if (receptionType.Equals("Вторичный"))
            {
                reception.Type = "s";
            }

            int rowNum = tableModel[index].Row_Number;

            // Удаление старой записи
            tableModel.RemoveAt(index);

            // Создание обновленной модели
            ReceptionModel receptionModel = new ReceptionModel();
            receptionModel.Row_Number = rowNum;
            receptionModel.Reception_Object = reception;
            receptionModel.DoctorShortName = doctorShortName;
            receptionModel.PatientShortName = patientShortName;
            receptionModel.ReceptionType = receptionType;

            // Запись обновленной модели в GUI таблицу
            tableModel.Insert(index, receptionModel);

            // Выборка записи таблицы из БД
            var receptionBD = dbContext.Receptions.First(r => r.Reception_id == receptionModel.Reception_Object.Reception_id);

            // Изменение информации в строке
            receptionBD.Specialty_Id = receptionModel.Reception_Object.Specialty_Id;
            receptionBD.Doctor_Id = receptionModel.Reception_Object.Doctor_Id;
            receptionBD.Patient_Id = receptionModel.Reception_Object.Patient_Id;
            receptionBD.Date = receptionModel.Reception_Object.Date;
            receptionBD.Time = receptionModel.Reception_Object.Time;
            receptionBD.Type = receptionModel.Reception_Object.Type;

            // Сохранение изменений в БД
            dbContext.SaveChanges();
        }
    }
}
