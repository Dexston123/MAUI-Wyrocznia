using Microsoft.Maui.Controls;
using Wyrocznia.Models;
using Wyrocznia.Services;
using System.Collections.ObjectModel;
using System.Linq;

namespace Wyrocznia.Views
{
    public partial class EditStudentPage : ContentPage
    {
        private ObservableCollection<Student> students = new ObservableCollection<Student>();

        public EditStudentPage()
        {
            InitializeComponent();
            LoadClasses();
            this.Appearing += EditStudentPage_Appearing;
        }

        private void EditStudentPage_Appearing(object sender, EventArgs e)
        {
            ClassPicker.SelectedIndex = -1;
            StudentsCollectionView.ItemsSource = null;
        }

        private void LoadClasses()
        {
            var allStudents = StudentDataService.LoadStudentsList();
            var classes = allStudents.Select(s => s.Class).Distinct().OrderBy(c => c);
            foreach (var studentClass in classes)
            {
                ClassPicker.Items.Add(studentClass);
            }
        }

        private void ClassPicker_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (ClassPicker.SelectedIndex == -1) return;
            var selectedClass = ClassPicker.Items[ClassPicker.SelectedIndex];
            UpdateStudentsList(selectedClass);
        }

        private void UpdateStudentsList(string selectedClass)
        {
            var allStudents = StudentDataService.LoadStudentsList();
            var classStudents = allStudents.Where(s => s.Class == selectedClass).ToList();

            students.Clear();
            foreach (var student in classStudents)
            {
                students.Add(student);
            }
            StudentsCollectionView.ItemsSource = students;
        }

        private async void StudentsCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedStudent = e.CurrentSelection.FirstOrDefault() as Student;
            if (selectedStudent != null)
            {
                var route = $"///studentdetails?Class={Uri.EscapeDataString(selectedStudent.Class)}&JournalNumber={selectedStudent.JournalNumber}";
                await Shell.Current.GoToAsync(route);
            }
        }
    }
}
