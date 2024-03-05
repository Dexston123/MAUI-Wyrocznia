using Microsoft.Maui.Controls;
using Wyrocznia.Models;
using Wyrocznia.Services;
using System.Linq;

namespace Wyrocznia.Views
{
    public partial class AddStudentPage : ContentPage
    {
        public AddStudentPage()
        {
            InitializeComponent();
        }

        private void OnAddClicked(object sender, System.EventArgs e)
        {
            if (!int.TryParse(JournalNumberEntry.Text, out int journalNumber))
            {
                DisplayAlert("B³¹d", "Numer w dzienniku musi byæ liczb¹.", "OK");
                return;
            }

            var name = NameEntry.Text?.Trim();
            var lastName = LastNameEntry.Text?.Trim();
            var studentClass = ClassEntry.Text?.Trim();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(studentClass))
            {
                DisplayAlert("B³¹d", "Musisz wprowadziæ numer w dzienniku, imiê, nazwisko i klasê ucznia.", "OK");
                return;
            }

            var students = StudentDataService.LoadStudentsList();
            if (students.Any(s => s.JournalNumber == journalNumber && s.Class.Equals(studentClass, System.StringComparison.OrdinalIgnoreCase)))
            {
                DisplayAlert("B³¹d", "Uczeñ z tym numerem w dzienniku ju¿ istnieje w tej klasie.", "OK");
                return;
            }

            students.Add(new Student(journalNumber, name, lastName, studentClass));
            StudentDataService.SaveStudentsList(students);
            DisplayAlert("Dodano", $"Uczeñ {name} {lastName}, numer w dzienniku: {journalNumber}, klasa {studentClass} zosta³ dodany.", "OK");
            JournalNumberEntry.Text = NameEntry.Text = LastNameEntry.Text = ClassEntry.Text = string.Empty;
        }
    }
}
