using Microsoft.Maui.Controls;
using Wyrocznia.Models;
using Wyrocznia.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Wyrocznia.Views
{
    public partial class StudentsListPage : ContentPage
    {
        private ObservableCollection<StudentGroup> studentGroups;

        public StudentsListPage()
        {
            InitializeComponent();
            LoadClasses();
            LoadStudentsGroupedByClass();
            this.Appearing += StudentsListPage_Appearing;
        }

        private void StudentsListPage_Appearing(object sender, EventArgs e)
        {
            LoadStudentsGroupedByClass();
            ClassPicker.SelectedIndex = -1;
            StudentsCollectionView.ItemsSource = null;
        }

        private void LoadClasses()
        {
            var students = StudentDataService.LoadStudentsList();
            var classes = students.Select(s => s.Class).Distinct().OrderBy(c => c).ToList();
            foreach (var studentClass in classes)
            {
                ClassPicker.Items.Add(studentClass);
            }
        }

        private void LoadStudentsGroupedByClass()
        {
            var students = StudentDataService.LoadStudentsList();
            studentGroups = new ObservableCollection<StudentGroup>(
                students.GroupBy(s => s.Class)
                .Select(g => new StudentGroup(g.Key, g.OrderBy(s => s.JournalNumber).ToList()))
                .OrderBy(g => g.Name));

            if (ClassPicker.Items.Any())
            {
                ClassPicker.SelectedIndex = 0;
                UpdateStudentsCollectionView(ClassPicker.Items[0]);
            }
        }

        private void OnClassPickerSelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (ClassPicker.SelectedIndex == -1) return;
            var selectedClass = ClassPicker.Items[ClassPicker.SelectedIndex];
            UpdateStudentsCollectionView(selectedClass);
        }

        private void UpdateStudentsCollectionView(string selectedClass)
        {
            var selectedGroup = studentGroups.FirstOrDefault(g => g.Name == selectedClass);
            if (selectedGroup != null)
            {
                StudentsCollectionView.ItemsSource = new ObservableCollection<Student>(selectedGroup);
            }
            else
            {
                StudentsCollectionView.ItemsSource = new ObservableCollection<Student>();
            }
        }

        private void OnSaveClicked(object sender, System.EventArgs e)
        {
            var students = studentGroups.SelectMany(g => g).ToList();
            StudentDataService.SaveStudentsList(students);
            DisplayAlert("Zapisano", "Lista uczniów zosta³a zapisana.", "OK");
        }
    }

    public class StudentGroup : List<Student>
    {
        public string Name { get; private set; }

        public StudentGroup(string name, List<Student> students) : base(students)
        {
            Name = name;
        }
    }
}