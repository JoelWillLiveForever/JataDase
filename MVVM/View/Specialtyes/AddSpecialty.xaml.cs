using Lab8.MVVM.Model;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Windows;

namespace Lab8.MVVM.View.Specialtyes
{
    public partial class AddSpecialty : Window
    {
        private Clinic_DatabaseEntities dbContext;
        private ObservableCollection<SpecialtyModel> tableModel;
        public AddSpecialty(ObservableCollection<SpecialtyModel> tableModel)
        {
            InitializeComponent();

            this.tableModel = tableModel;

            dbContext = new Clinic_DatabaseEntities();
            dbContext.Doctors.Load();
        }

        private void mButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            string title = mFieldTitle.Text;

            if (title == null || title.Equals(""))
            {
                MessageBox.Show("Укажите название специальности!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            SpecialtyModel specialtyModel = new SpecialtyModel();
            specialtyModel.Row_Number = tableModel.Count + 1;

            specialtyModel.Specialty_Object = new Specialty();
            specialtyModel.Specialty_Object.Title = title;

            dbContext.Specialties.Add(specialtyModel.Specialty_Object);
            dbContext.SaveChanges();

            tableModel.Add(specialtyModel);

            mFieldTitle.Text = "";
        }
    }
}
