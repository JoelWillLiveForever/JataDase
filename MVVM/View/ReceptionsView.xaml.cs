using Lab8.MVVM.Model;
using Lab8.MVVM.View.Receptions;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Lab8.MVVM.View
{
    public partial class ReceptionsView : UserControl
    {
        public static bool isRequest { get; set; }

        private Clinic_DatabaseEntities dbContext;
        private ObservableCollection<ReceptionModel> tableModel;

        public ReceptionsView()
        {
            InitializeComponent();

            dbContext = new Clinic_DatabaseEntities();
            dbContext.Receptions.Load();

            var bindingList = dbContext.Receptions.Local.ToBindingList();
            tableModel = new ObservableCollection<ReceptionModel>();

            for (int i = 0; i < bindingList.Count; i++)
            {
                ReceptionModel reception = new ReceptionModel();
                reception.Reception_Object = bindingList[i];

                // Преобразование типов
                if (bindingList[i].Type.Equals("p"))
                {
                    reception.ReceptionType = "Первичный";
                }
                else if (bindingList[i].Type.Equals("s"))
                {
                    reception.ReceptionType = "Вторичный";
                }

                // Дата и время
                reception.Reception_Object.Date = bindingList[i].Date;
                reception.Reception_Object.Time = bindingList[i].Time;

                // Название специальность
                reception.Reception_Object.Specialty.Title = bindingList[i].Specialty.Title;

                // Номер по порядку
                reception.Row_Number = (i + 1);

                // Добавление короткого имени врача
                string Surname = bindingList[i].Doctor.Surname;
                string Name = bindingList[i].Doctor.Name;
                string Lastname = bindingList[i].Doctor.Lastname;

                if (Lastname == null || Lastname.Equals(""))
                    reception.DoctorShortName = Surname + " " + Name.Substring(0, 1) + ".";
                else
                    reception.DoctorShortName = Surname + " " + Name.Substring(0, 1) + "." + Lastname.Substring(0, 1) + ".";

                // Добавление короткого имени пациента
                Surname = bindingList[i].Patient.Surname;
                Name = bindingList[i].Patient.Name;
                Lastname = bindingList[i].Patient.Lastname;

                if (Lastname != null)
                    reception.PatientShortName = Surname + " " + Name.Substring(0, 1) + "." + Lastname.Substring(0, 1) + ".";
                else
                    reception.PatientShortName = Surname + " " + Name.Substring(0, 1) + ".";

                // Добавление объекта в список
                tableModel.Add(reception);
            }

            mReceptionsTable.ItemsSource = tableModel;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            new AddReception(tableModel).ShowDialog();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (mReceptionsTable.SelectedItems.Count < 1)
            {
                MessageBox.Show("Выберите строку для изменения!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (mReceptionsTable.SelectedItems.Count > 1)
            {
                MessageBox.Show("Можно выбрать для изменения не более ОДНОЙ строки за раз!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            new EditReception(tableModel, mReceptionsTable.SelectedIndex).ShowDialog();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (mReceptionsTable.SelectedItems.Count == 1)
            {
                var rObj = tableModel[mReceptionsTable.SelectedIndex];
                var reception = dbContext.Receptions.Single(r => r.Reception_id == rObj.Reception_Object.Reception_id);

                dbContext.Receptions.Remove(reception);
                tableModel.Remove(rObj);
                dbContext.SaveChanges();
            }
            else if (mReceptionsTable.SelectedItems.Count > 1)
            {
                while (mReceptionsTable.SelectedItems.Count > 0)
                {
                    var rObj = tableModel[mReceptionsTable.SelectedIndex];
                    var reception = dbContext.Receptions.Single(r => r.Reception_id == rObj.Reception_Object.Reception_id);

                    dbContext.Receptions.Remove(reception);
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
            if (!isRequest)
            {
                new RequestReception(mReceptionsTable, mRequestButton).ShowDialog();
            }
            else
            {
                mReceptionsTable.ItemsSource = tableModel;
                mRequestButton.Content = "Запрос";
                isRequest = false;
            }
        }
    }
}
