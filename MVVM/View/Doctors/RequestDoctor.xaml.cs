using Lab8.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Lab8.MVVM.View.Doctors
{
    public partial class RequestDoctor : Window
    {
        private Clinic_DatabaseEntities dbContext;

        private DataGrid mDataGrid;
        private Button mButton;

        private BindingList<Doctor> doctors;
        private BindingList<Specialty> specialtyes;

        public RequestDoctor(DataGrid mDataGrid, Button mButton)
        {
            InitializeComponent();
            this.mDataGrid = mDataGrid;
            this.mButton = mButton;

            dbContext = new Clinic_DatabaseEntities();
            dbContext.Specialties.Load();

            specialtyes = dbContext.Specialties.Local.ToBindingList();

            for (int i = 0; i < specialtyes.Count; i++)
            {
                mChangeList.Items.Add(specialtyes[i].Title);
            }

            dbContext.Doctors.Load();
            doctors = dbContext.Doctors.Local.ToBindingList();
        }

        private void mButtonRequest_Click(object sender, RoutedEventArgs e)
        {
            // Смена состояния и текста кнопки
            mButton.Content = "Сброс";
            DoctorsView.isRequest = true;
            int currentIndex = mChangeList.SelectedIndex;

            if (mListAttribute.SelectedIndex == 0)
            {
                // Смена контента в таблице
                var specialty = specialtyes[currentIndex];

                //    .Select(r => r);
                var doctors = (from doctor in dbContext.Doctors
                               from reception in doctor.Receptions
                               where doctor.Doctor_Id == reception.Doctor_Id
                               where reception.Specialty_Id == specialty.Specialty_Id
                               select doctor).ToList();

                var doctorsModels = new BindingList<DoctorModel>();
                int row = 1;

                foreach (var item in doctors)
                {
                    DoctorModel doctorModel = new DoctorModel();
                    doctorModel.Row_Number = row++;
                    doctorModel.Doctor_Object = item;
                    doctorsModels.Add(doctorModel);
                }

                mDataGrid.ItemsSource = doctorsModels;
            } else if (mListAttribute.SelectedIndex == 1)
            {
                var selectedDoctor = doctors[currentIndex];

                var receptions = (from doctor in dbContext.Doctors
                                  from reception in doctor.Receptions
                                  where doctor.Doctor_Id == selectedDoctor.Doctor_Id
                                  where doctor.Doctor_Id == reception.Doctor_Id
                                  select reception).ToList();

                Console.WriteLine("Count = " + receptions.Count);

                double avg = 0;
                for (int i = 0; i <= 52; i++)
                {
                    int count = 0;
                    foreach (var item in receptions)
                    {
                        // week
                        int weekNum = item.Date.DayOfYear / 7;
                        if (i == weekNum)
                        {
                            count++;
                        }
                    }

                    avg += count;
                }

                avg /= 52.0;

                mDataGrid.Columns[1].Visibility = Visibility.Collapsed;
                mDataGrid.Columns[2].Visibility = Visibility.Collapsed;
                mDataGrid.Columns[3].Visibility = Visibility.Collapsed;

                List<Count> localList = new List<Count>();
                Count countObj = new Count();
                countObj.avg = avg;
                localList.Add(countObj);

                mDataGrid.Columns[0] = new DataGridTextColumn
                {
                    Header = "Среднее количество посещений к врачу \"" + (string)mChangeList.SelectedItem + "\" в неделю",
                    Binding = new Binding("avg"),
                    IsReadOnly = true,
                    ElementStyle = Application.Current.FindResource("ColumnElementTheme") as Style,
                    MinWidth = 100
                };

                mDataGrid.ItemsSource = localList;
            }
        }

        class Count
        {
            public double avg { get; set; }
        }

        private void mListAttribute_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mListAttribute.SelectedIndex == 0)
            {
                if (mChangeTextBlock != null)
                    mChangeTextBlock.Text = "Специальность:";

                if (mChangeList != null)
                {
                    mChangeList.Items.Clear();

                    for (int i = 0; i < specialtyes.Count; i++)
                    {
                        mChangeList.Items.Add(specialtyes[i].Title);
                    }

                    mChangeList.SelectedIndex = 0;
                }

            } else if (mListAttribute.SelectedIndex == 1)
            {
                if (mChangeTextBlock != null)
                    mChangeTextBlock.Text = "Врач:";

                if (mChangeList != null)
                {
                    mChangeList.Items.Clear();

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

                        mChangeList.Items.Add(shortName);
                    }

                    mChangeList.SelectedIndex = 0;
                }
            }
        }
    }
}
