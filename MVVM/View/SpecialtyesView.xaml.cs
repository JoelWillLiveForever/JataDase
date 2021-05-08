using Lab8.MVVM.Model;
using Lab8.MVVM.View.Specialtyes;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Lab8.MVVM.View
{
    public partial class SpecialtyesView : UserControl
    {
        private Clinic_DatabaseEntities dbContext;
        private ObservableCollection<SpecialtyModel> tableModel;

        public SpecialtyesView()
        {
            InitializeComponent();

            dbContext = new Clinic_DatabaseEntities();
            dbContext.Specialties.Load();

            var bindingList = dbContext.Specialties.Local.ToBindingList();
            tableModel = new ObservableCollection<SpecialtyModel>();

            for (int i = 0; i < bindingList.Count; i++)
            {
                SpecialtyModel specialtyModel = new SpecialtyModel();

                specialtyModel.Row_Number = (i + 1);
                specialtyModel.Specialty_Object = bindingList[i];

                tableModel.Add(specialtyModel);
            }

            mSpecialtyesTable.ItemsSource = tableModel;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            new AddSpecialty(tableModel).ShowDialog();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (mSpecialtyesTable.SelectedItems.Count < 1)
            {
                MessageBox.Show("Выберите строку для изменения!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (mSpecialtyesTable.SelectedItems.Count > 1)
            {
                MessageBox.Show("Можно выбрать для изменения не более ОДНОЙ строки за раз!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            new EditSpecialty(tableModel, mSpecialtyesTable.SelectedIndex).ShowDialog();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (mSpecialtyesTable.SelectedItems.Count == 1)
            {
                var specialty = tableModel[mSpecialtyesTable.SelectedIndex].Specialty_Object;
                var receptions = dbContext.Receptions.Where(s => s.Specialty_Id == specialty.Specialty_Id);

                if (receptions != null)
                    dbContext.Receptions.RemoveRange(receptions);

                var rObj = tableModel[mSpecialtyesTable.SelectedIndex];
                var deleteObj = dbContext.Specialties.Single(r => r.Specialty_Id == rObj.Specialty_Object.Specialty_Id);

                dbContext.Specialties.Remove(deleteObj);
                tableModel.Remove(rObj);

                dbContext.SaveChanges();
            }
            else if (mSpecialtyesTable.SelectedItems.Count > 1)
            {
                while (mSpecialtyesTable.SelectedItems.Count > 0)
                {
                    var specialty = tableModel[mSpecialtyesTable.SelectedIndex].Specialty_Object;
                    var receptions = dbContext.Receptions.Where(s => s.Specialty_Id == specialty.Specialty_Id);

                    if (receptions != null)
                        dbContext.Receptions.RemoveRange(receptions);

                    var rObj = tableModel[mSpecialtyesTable.SelectedIndex];
                    var deleteObj = dbContext.Specialties.Single(r => r.Specialty_Id == rObj.Specialty_Object.Specialty_Id);

                    dbContext.Specialties.Remove(deleteObj);
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

        }
    }
}
