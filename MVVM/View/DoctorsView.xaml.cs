using Lab8.MVVM.Model;
using Lab8.MVVM.View.Doctors;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Lab8.MVVM.View
{
    public partial class DoctorsView : UserControl
    {
        public static bool isRequest { get; set; }

        private Clinic_DatabaseEntities dbContext;
        private ObservableCollection<DoctorModel> tableModel;

        public DoctorsView()
        {
            InitializeComponent();

            isRequest = false;

            dbContext = new Clinic_DatabaseEntities();
            dbContext.Doctors.Load();

            var bindingList = dbContext.Doctors.Local.ToBindingList();
            tableModel = new ObservableCollection<DoctorModel>();

            for (int i = 0; i < bindingList.Count; i++)
            {
                DoctorModel doctorModel = new DoctorModel();

                doctorModel.Row_Number = (i + 1);
                doctorModel.Doctor_Object = bindingList[i];

                tableModel.Add(doctorModel);
            }

            mDoctorsTable.ItemsSource = tableModel;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            new AddDoctor(tableModel).ShowDialog();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (mDoctorsTable.SelectedItems.Count < 1)
            {
                MessageBox.Show("Выберите строку для изменения!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            } else if (mDoctorsTable.SelectedItems.Count > 1)
            {
                MessageBox.Show("Можно выбрать для изменения не более ОДНОЙ строки за раз!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            new EditDoctor(tableModel, mDoctorsTable.SelectedIndex).ShowDialog();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (mDoctorsTable.SelectedItems.Count == 0)
            {
                MessageBox.Show("Выберите строку для удаления!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            while (mDoctorsTable.SelectedItems.Count > 0)
            {
                var doctor = tableModel[mDoctorsTable.SelectedIndex].Doctor_Object;
                var receptions = dbContext.Receptions.Where(d => d.Doctor_Id == doctor.Doctor_Id);

                if (receptions != null)
                    dbContext.Receptions.RemoveRange(receptions);

                var rObj = tableModel[mDoctorsTable.SelectedIndex];
                var deleteObj = dbContext.Doctors.Single(r => r.Doctor_Id == rObj.Doctor_Object.Doctor_Id);

                dbContext.Doctors.Remove(deleteObj);
                tableModel.Remove(rObj);
            }
            dbContext.SaveChanges();
        }

        private void mRequestButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isRequest)
            {
                new RequestDoctor(mDoctorsTable, mRequestButton).ShowDialog();
            } else
            {
                mDoctorsTable.Columns[1].Visibility = Visibility.Visible;
                mDoctorsTable.Columns[2].Visibility = Visibility.Visible;
                mDoctorsTable.Columns[3].Visibility = Visibility.Visible;

                mDoctorsTable.Columns[0] = new DataGridTextColumn
                {
                    Header = "№ п/п",
                    Binding = new Binding("Row_Number"),
                    IsReadOnly = true,
                    ElementStyle = Application.Current.FindResource("ColumnElementTheme") as Style,
                    MinWidth = 70,
                    MaxWidth = 105
                };

                mDoctorsTable.ItemsSource = tableModel;
                mRequestButton.Content = "Запрос";
                isRequest = false;
            }
        }
    }
}
