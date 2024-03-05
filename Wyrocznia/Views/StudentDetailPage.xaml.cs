using Microsoft.Maui.Controls;
using System;
using Wyrocznia.Models;
using Wyrocznia.Services;

namespace Wyrocznia.Views
{
    [QueryProperty(nameof(JournalNumber), "JournalNumber")]
    [QueryProperty(nameof(Class), "Class")]
    public partial class StudentDetailsPage : ContentPage
    {
        private Student originalStudent;
        private int _journalNumber;
        private string _class;

        public string Class
        {
            set
            {
                _class = Uri.UnescapeDataString(value);
                LoadStudentData();
            }
        }

        public int JournalNumber
        {
            set
            {
                _journalNumber = value;
                LoadStudentData();
            }
        }

        public StudentDetailsPage()
        {
            InitializeComponent();
        }

        private void LoadStudentData()
        {
            originalStudent = StudentDataService.LoadStudentByClassAndJournalNumber(_class, _journalNumber);
            if (originalStudent != null)
            {
                NameEntry.Text = originalStudent.Name;
                LastNameEntry.Text = originalStudent.LastName;
                JournalNumberEntry.Text = originalStudent.JournalNumber.ToString();
                ClassEntry.Text = originalStudent.Class;
                IsPresentSwitch.IsToggled = originalStudent.IsPresent;
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            try
            {
                var updatedStudent = new Student
                {
                    Name = NameEntry.Text,
                    LastName = LastNameEntry.Text,
                    JournalNumber = int.Parse(JournalNumberEntry.Text),
                    Class = ClassEntry.Text,
                    IsPresent = IsPresentSwitch.IsToggled
                };

                if (!StudentDataService.IsJournalNumberUnique(updatedStudent.JournalNumber, updatedStudent.Class, originalStudent.JournalNumber, originalStudent.Class))
                {
                    await DisplayAlert("B³¹d", "Inny uczeñ posiada ju¿ ten numer w dzienniku w tej klasie.", "OK");
                    return;
                }

                StudentDataService.UpdateStudent(updatedStudent, originalStudent.JournalNumber, originalStudent.Class);
                await DisplayAlert("Sukces", "Dane ucznia zosta³y zaktualizowane.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            catch (FormatException)
            {
                await DisplayAlert("B³¹d", "Numer w dzienniku musi byæ liczb¹.", "OK");
            }
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            bool isConfirmed = await DisplayAlert("Potwierdzenie", "Czy na pewno chcesz usun¹æ tego ucznia?", "Tak", "Nie");
            if (isConfirmed)
            {
                StudentDataService.DeleteStudent(originalStudent);
                await DisplayAlert("Usuniêto", "Uczeñ zosta³ usuniêty.", "OK");
                await Shell.Current.GoToAsync("//editstudents");
            }
        }
        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//editstudents");
        }
    }
}
