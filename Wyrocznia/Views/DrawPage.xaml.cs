using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Wyrocznia.Models;
using Wyrocznia.Services;

namespace Wyrocznia.Views
{
    public partial class DrawPage : ContentPage
    {
        private ObservableCollection<Student> studentsCollection;
        private ObservableCollection<Student> drawnStudentsCollection;
        private int luckyNumber;
        private readonly Random random = new Random();

        public DrawPage()
        {
            InitializeComponent();
            InitializeCollections();
            LoadClasses();
            NewLuckyNumber();
            this.Appearing += DrawPage_Appearing;
        }

        private void DrawPage_Appearing(object sender, EventArgs e)
        {
            LoadClasses();
            NewLuckyNumber();
            ClassPicker.SelectedIndex = -1;
            StudentsCollectionView.ItemsSource = null;
            DrawnStudentsCollectionView.ItemsSource = null;
        }

        private void InitializeCollections()
        {
            studentsCollection = new ObservableCollection<Student>();
            drawnStudentsCollection = new ObservableCollection<Student>();
        }

        private void LoadClasses()
        {
            var classes = StudentDataService.LoadClasses();
            ClassPicker.Items.Clear();
            foreach (var studentClass in classes)
            {
                ClassPicker.Items.Add(studentClass);
            }
        }

        private void OnClassPickerSelectedIndexChanged(object sender, EventArgs e)
        {
            if (ClassPicker.SelectedIndex == -1) return;

            var selectedClass = ClassPicker.Items[ClassPicker.SelectedIndex];
            LoadStudents(selectedClass);
        }

        private void LoadStudents(string selectedClass)
        {
            var students = StudentDataService.LoadStudentsByClass(selectedClass);
            studentsCollection.Clear();
            drawnStudentsCollection.Clear();

            foreach (var student in students)
            {
                UpdateStudentColorState(student);

                studentsCollection.Add(student);

                if (student.HasBeenDrawn && student.DrawnCount < 4)
                    drawnStudentsCollection.Add(student);
            }

            StudentsCollectionView.ItemsSource = studentsCollection;
            DrawnStudentsCollectionView.ItemsSource = drawnStudentsCollection;
        }

        private void UpdateStudentColorState(Student student)
        {
            if (student.JournalNumber == luckyNumber)
            {
                student.ColorState = "Gold";
            }
            else if (student.HasBeenDrawn || (!student.IsPresent && DrawModeSwitch.IsToggled))
            {
                student.ColorState = "Red";
            }
            else if (!student.IsPresent && !DrawModeSwitch.IsToggled)
            {
                student.ColorState = "Green";
            }
            else if (student.IsPresent || !DrawModeSwitch.IsToggled)
            {
                student.ColorState = "Green";
            }
            else
            {
                student.ColorState = "Default";
            }
        }

        private void NewLuckyNumber()
        {
            luckyNumber = random.Next(0, 36);
            LuckyNumberLabel.Text = $"Szczêœliwy numer: {luckyNumber}";
            UpdateColorsForAllStudents();
        }

        private void OnNewLuckyNumberClicked(object sender, EventArgs e)
        {
            NewLuckyNumber();
        }

        private void OnDrawClicked(object sender, EventArgs e)
        {
            var studentsToDrawFrom = DrawModeSwitch.IsToggled
                ? studentsCollection.Where(s => s.IsPresent && !s.HasBeenDrawn && s.JournalNumber != luckyNumber)
                : studentsCollection.Where(s => !s.HasBeenDrawn && s.JournalNumber != luckyNumber);

            if (!studentsToDrawFrom.Any())
            {
                DisplayAlert("Informacja", "Nie ma ¿adnych uczniów do losowania w tej klasie lub wszyscy byli ju¿ wylosowani.", "OK");
                return;
            }

            var drawnStudent = studentsToDrawFrom.ElementAt(random.Next(studentsToDrawFrom.Count()));
            drawnStudent.HasBeenDrawn = true;
            drawnStudentsCollection.Add(drawnStudent);

            foreach (var student in studentsCollection.Where(s => s.HasBeenDrawn))
            {
                student.DrawnCount += 1;

                if (student.DrawnCount >= 4)
                {
                    student.HasBeenDrawn = false;
                    student.DrawnCount = 0;
                    drawnStudentsCollection.Remove(student);
                }
            }

            StudentDataService.SaveStudentsList(studentsCollection.ToList());

            LoadStudents(ClassPicker.Items[ClassPicker.SelectedIndex]);

            DrawnStudentLabel.Text = $"Wylosowany uczeñ: {drawnStudent.Name} {drawnStudent.LastName}";
        }

        private void DrawModeSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            UpdateColorsForAllStudents();
        }

        private void UpdateColorsForAllStudents()
        {
            foreach (var student in studentsCollection)
            {
                UpdateStudentColorState(student);
            }

            StudentsCollectionView.ItemsSource = null;
            StudentsCollectionView.ItemsSource = studentsCollection;
        }
    }
}