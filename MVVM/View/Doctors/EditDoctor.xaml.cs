using Lab8.MVVM.Model;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;

namespace Lab8.MVVM.View.Doctors
{
    public partial class EditDoctor : Window
    {
        private Clinic_DatabaseEntities dbContext;
        private ObservableCollection<DoctorModel> tableModel;
        private int index;

        public EditDoctor(ObservableCollection<DoctorModel> tableModel, int index)
        {
            InitializeComponent();
            this.tableModel = tableModel;
            this.index = index;

            mFieldSurname.Text = tableModel[index].Doctor_Object.Surname;
            mFieldName.Text = tableModel[index].Doctor_Object.Name;
            mFieldLastname.Text = tableModel[index].Doctor_Object.Lastname;

            dbContext = new Clinic_DatabaseEntities();
            dbContext.Doctors.Load();
        }

        private void mButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            string surname = mFieldSurname.Text;
            string name = mFieldName.Text;
            string lastname = mFieldLastname.Text;

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

            // Сохранение экземпляра таблицы в переменной
            Doctor doctor = tableModel[index].Doctor_Object;
            doctor.Surname = surname;
            doctor.Name = name;
            doctor.Lastname = lastname;

            int rowNum = tableModel[index].Row_Number;

            // Удаление старой записи
            tableModel.RemoveAt(index);

            // Создание обновленной модели
            DoctorModel doctorModel = new DoctorModel();
            doctorModel.Row_Number = rowNum;
            doctorModel.Doctor_Object = doctor;

            // Запись обновленной модели в GUI таблицу
            tableModel.Insert(index, doctorModel);

            // Выборка записи таблицы из БД
            var doctorBD = dbContext.Doctors.First(d => d.Doctor_Id == doctorModel.Doctor_Object.Doctor_Id);

            // Изменение информации в строке
            doctorBD.Surname = surname;
            doctorBD.Name = name;
            doctorBD.Lastname = lastname;

            // Сохранение изменений в БД
            dbContext.SaveChanges();
        }
    }
}
