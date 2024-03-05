namespace Wyrocznia.Models
{
    public class Student
    {
        public int JournalNumber { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Class { get; set; }
        public bool IsPresent { get; set; } = true;
        public bool HasBeenDrawn { get; set; } = false;
        public int DrawnCount { get; set; } = 0;
        public string ColorState { get; set; } = "Default";

        public Student() { }

        public Student(int journalNumber, string name, string lastName, string studentClass, bool isPresent = true)
        {
            JournalNumber = journalNumber;
            Name = name;
            LastName = lastName;
            Class = studentClass;
            IsPresent = isPresent;
        }
    }
}
