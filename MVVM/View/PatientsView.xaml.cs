using Lab8.MVVM.Model;
using Lab8.MVVM.View.Patients;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Lab8.MVVM.View
{
    public partial class PatientsView : UserControl
    {
        public static bool IsRequest { get; set; }

        private Clinic_DatabaseEntities dbContext;
        private ObservableCollection<PatientModel> tableModel;

        public PatientsView()
        {
            InitializeComponent();

            dbContext = new Clinic_DatabaseEntities();
            dbContext.Patients.Load();

            var bindingList = dbContext.Patients.Local.ToBindingList();
            tableModel = new ObservableCollection<PatientModel>();

            for (int i = 0; i < bindingList.Count; i++)
            {
                PatientModel patientModel = new PatientModel();
                patientModel.Patient_Object = bindingList[i];

                // Преобразование гендеров
                if (bindingList[i].Gender.Equals("m"))
                {
                    patientModel.Gender = "Мужской";
                } else if (bindingList[i].Gender.Equals("f"))
                {
                    patientModel.Gender = "Женский";
                }

                patientModel.Row_Number = (i + 1);

                tableModel.Add(patientModel);
            }

            mPatientsTable.ItemsSource = tableModel;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            new AddPatient(tableModel).ShowDialog();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (mPatientsTable.SelectedItems.Count < 1)
            {
                MessageBox.Show("Выберите строку для изменения!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (mPatientsTable.SelectedItems.Count > 1)
            {
                MessageBox.Show("Можно выбрать для изменения не более ОДНОЙ строки за раз!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            new EditPatient(tableModel, mPatientsTable.SelectedIndex).ShowDialog();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (mPatientsTable.SelectedItems.Count == 1)
            {
                var patient = tableModel[mPatientsTable.SelectedIndex].Patient_Object;
                var receptions = dbContext.Receptions.Where(p => p.Patient_Id == patient.Patient_Id);

                if (receptions != null)
                    dbContext.Receptions.RemoveRange(receptions);

                var rObj = tableModel[mPatientsTable.SelectedIndex];
                var deleteObj = dbContext.Patients.Single(r => r.Patient_Id == rObj.Patient_Object.Patient_Id);

                dbContext.Patients.Remove(deleteObj);
                tableModel.Remove(rObj);

                dbContext.SaveChanges();
            }
            else if (mPatientsTable.SelectedItems.Count > 1)
            {
                while (mPatientsTable.SelectedItems.Count > 0)
                {
                    var patient = tableModel[mPatientsTable.SelectedIndex].Patient_Object;
                    var receptions = dbContext.Receptions.Where(p => p.Patient_Id == patient.Patient_Id);

                    if (receptions != null)
                        dbContext.Receptions.RemoveRange(receptions);

                    var rObj = tableModel[mPatientsTable.SelectedIndex];
                    var deleteObj = dbContext.Patients.Single(r => r.Patient_Id == rObj.Patient_Object.Patient_Id);

                    dbContext.Patients.Remove(deleteObj);
                    tableModel.Remove(rObj);
                }
                dbContext.SaveChanges();
            }
            else
            {
                MessageBox.Show("Выберите строку для удаления!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Request_Click(object sender, RoutedEventArgs e)
        {
            if (!IsRequest)
            {
                new RequestPatient(mPatientsTable, mRequestButton).ShowDialog();
            } else
            {
                mPatientsTable.Columns[1].Visibility = Visibility.Visible;
                mPatientsTable.Columns[2].Visibility = Visibility.Visible;
                mPatientsTable.Columns[3].Visibility = Visibility.Visible;
                mPatientsTable.Columns[4].Visibility = Visibility.Visible;
                mPatientsTable.Columns[5].Visibility = Visibility.Visible;
                mPatientsTable.Columns[6].Visibility = Visibility.Visible;
                mPatientsTable.Columns[7].Visibility = Visibility.Visible;

                mPatientsTable.Columns[0] = new DataGridTextColumn
                {
                    Header = "№ п/п",
                    Binding = new Binding("Row_Number"),
                    MinWidth = 70,
                    MaxWidth = 105,
                    IsReadOnly = true,
                    ElementStyle = Application.Current.FindResource("ColumnElementTheme") as Style
                };

                mRequestButton.Content = "Запрос";
                mPatientsTable.ItemsSource = tableModel;
                IsRequest = false;
            }
        }
    }
}
