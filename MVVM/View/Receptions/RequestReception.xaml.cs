using Lab8.MVVM.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Lab8.MVVM.View.Receptions
{
    public partial class RequestReception : Window
    {
        private Clinic_DatabaseEntities dbContext;

        private DataGrid mDataGrid;
        private Button mButton;

        public RequestReception(DataGrid mDataGrid, Button mButton)
        {
            InitializeComponent();
            this.mDataGrid = mDataGrid;
            this.mButton = mButton;

            dbContext = new Clinic_DatabaseEntities();
            dbContext.Receptions.Load();
        }

        private void mButtonRequest_Click(object sender, RoutedEventArgs e)
        {
            // Смена состояния и текста кнопки
            mButton.Content = "Сброс";
            ReceptionsView.isRequest = true;

            string receptionDate = mFieldDate.Text;
            DateTime date = DateTime.ParseExact(receptionDate, "dd.MM.yyyy", null);

            var receptions = (from reception in dbContext.Receptions
                              where reception.Date == date
                              select reception).ToList();

            var receptionsModels = new BindingList<ReceptionModel>();
            int row = 1;

            foreach (var item in receptions)
            {

                // Добавление короткого имени врача
                string Surname1 = item.Doctor.Surname;
                string Name1 = item.Doctor.Name;
                string Lastname1 = item.Doctor.Lastname;
                string doctorShortName;

                if (Lastname1 == null || Lastname1.Equals(""))
                    doctorShortName = Surname1 + " " + Name1.Substring(0, 1) + ".";
                else
                    doctorShortName = Surname1 + " " + Name1.Substring(0, 1) + "." + Lastname1.Substring(0, 1) + ".";

                // Добавление короткого имени пациента
                string Surname2 = item.Patient.Surname;
                string Name2 = item.Patient.Name;
                string Lastname2 = item.Patient.Lastname;
                string patientShortName;

                if (Lastname2 == null || Lastname2.Equals(""))
                    patientShortName = Surname2 + " " + Name2.Substring(0, 1) + ".";
                else
                    patientShortName = Surname2 + " " + Name2.Substring(0, 1) + "." + Lastname2.Substring(0, 1) + ".";

                ReceptionModel receptionModel = new ReceptionModel();
                receptionModel.Row_Number = row++;
                receptionModel.Reception_Object = item;
                receptionModel.ReceptionType = (item.Type == "p") ? "Первичный" : "Вторичный";
                receptionModel.DoctorShortName = doctorShortName;
                receptionModel.PatientShortName = patientShortName;
                receptionsModels.Add(receptionModel);
            }

            mDataGrid.ItemsSource = receptionsModels;
        }
    }
}
