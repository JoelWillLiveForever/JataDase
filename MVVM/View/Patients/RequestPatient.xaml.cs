using Lab8.MVVM.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Lab8.MVVM.View.Patients
{
    public partial class RequestPatient : Window
    {
        private Clinic_DatabaseEntities dbContext;

        private DataGrid mDataGrid;
        private Button mButton;

        private BindingList<Patient> patients;

        public RequestPatient(DataGrid mDataGrid, Button mButton)
        {
            InitializeComponent();
            this.mDataGrid = mDataGrid;
            this.mButton = mButton;

            dbContext = new Clinic_DatabaseEntities();

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
        }

        private void mButtonRequest_Click(object sender, RoutedEventArgs e)
        {
            mDataGrid.Columns[1].Visibility = Visibility.Collapsed;
            mDataGrid.Columns[2].Visibility = Visibility.Collapsed;
            mDataGrid.Columns[3].Visibility = Visibility.Collapsed;
            mDataGrid.Columns[4].Visibility = Visibility.Collapsed;
            mDataGrid.Columns[5].Visibility = Visibility.Collapsed;
            mDataGrid.Columns[6].Visibility = Visibility.Collapsed;
            mDataGrid.Columns[7].Visibility = Visibility.Collapsed;

            mButton.Content = "Сброс";
            PatientsView.IsRequest = true;

            int patientIndex = mListPatient.SelectedIndex;

            // Смена контента в таблице
            var selectedPatient = patients[patientIndex];

            var countPatients = (from patient in dbContext.Patients
                                 from reception in patient.Receptions
                                 where patient.Patient_Id == selectedPatient.Patient_Id
                                 where patient.Patient_Id == reception.Patient_Id
                                 group patient by patient.Patient_Id into Group
                                 select new
                                 {
                                     Count = Group.Count()
                                 }).ToList();

            mDataGrid.Columns[0] = new DataGridTextColumn
            {
                Header = "Количество посещений для пациента: " + (string)mListPatient.SelectedItem,
                Binding = new Binding("Count"),
                MinWidth = 70,
                IsReadOnly = true,
                ElementStyle = Application.Current.FindResource("ColumnElementTheme") as Style
            };

            mDataGrid.ItemsSource = countPatients;
        }
    }
}