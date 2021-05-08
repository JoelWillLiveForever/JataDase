using Lab8.MVVM.Model;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;

namespace Lab8.MVVM.View.Specialtyes
{
    public partial class EditSpecialty : Window
    {
        private Clinic_DatabaseEntities dbContext;
        private ObservableCollection<SpecialtyModel> tableModel;
        private int index;

        public EditSpecialty(ObservableCollection<SpecialtyModel> tableModel, int index)
        {
            InitializeComponent();
            this.tableModel = tableModel;
            this.index = index;

            mFieldTitle.Text = tableModel[index].Specialty_Object.Title;

            dbContext = new Clinic_DatabaseEntities();
            dbContext.Doctors.Load();
        }

        private void mButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            string title = mFieldTitle.Text;

            if (title == null || title.Equals(""))
            {
                MessageBox.Show("Укажите фамилию!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Сохранение экземпляра таблицы в переменной
            Specialty specialty = tableModel[index].Specialty_Object;
            specialty.Title = title;

            int rowNum = tableModel[index].Row_Number;

            // Удаление старой записи
            tableModel.RemoveAt(index);

            // Создание обновленной модели
            SpecialtyModel specialtyModel = new SpecialtyModel();
            specialtyModel.Row_Number = rowNum;
            specialtyModel.Specialty_Object = specialty;

            // Запись обновленной модели в GUI таблицу
            tableModel.Insert(index, specialtyModel);

            // Выборка записи таблицы из БД
            var specialtyBD = dbContext.Specialties.First(d => d.Specialty_Id == specialtyModel.Specialty_Object.Specialty_Id);

            // Изменение информации в строке
            specialtyBD.Title = title;

            // Сохранение изменений в БД
            dbContext.SaveChanges();
        }
    }
}
