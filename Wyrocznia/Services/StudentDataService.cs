using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Wyrocznia.Models;

namespace Wyrocznia.Services
{
    public static class StudentDataService
    {
        private static string FolderPath => App.FolderPath;

        public static void SaveStudentsList(IEnumerable<Student> students)
        {
            var classGroups = students.GroupBy(s => s.Class);

            foreach (var group in classGroups)
            {
                string classFilePath = Path.Combine(FolderPath, $"{group.Key}.txt");
                var lines = group.Select(student =>
                    $"{student.JournalNumber}," +
                    $"{student.Name}," +
                    $"{student.LastName}," +
                    $"{student.IsPresent.ToString()}," +
                    $"{student.HasBeenDrawn.ToString()}," +
                    $"{student.DrawnCount}," +
                    $"{student.ColorState}");
                File.WriteAllLines(classFilePath, lines);
            }
        }

        public static List<Student> LoadStudentsList()
        {
            var students = new List<Student>();
            var classFiles = Directory.GetFiles(FolderPath, "*.txt");

            foreach (var classFile in classFiles)
            {
                var classStudents = File.ReadAllLines(classFile)
                                         .Select(line => line.Split(','))
                                         .Select(parts => new Student
                                         {
                                             Class = Path.GetFileNameWithoutExtension(classFile),
                                             JournalNumber = int.Parse(parts[0]),
                                             Name = parts[1],
                                             LastName = parts[2],
                                             IsPresent = bool.Parse(parts[3]),
                                             HasBeenDrawn = bool.Parse(parts[4]),
                                             DrawnCount = int.Parse(parts[5]),
                                             ColorState = parts[6]
                                         });
                students.AddRange(classStudents);
            }

            return students;
        }

        public static IEnumerable<string> LoadClasses()
        {
            var classFiles = Directory.GetFiles(FolderPath, "*.txt");
            return classFiles.Select(Path.GetFileNameWithoutExtension).Distinct().OrderBy(c => c);
        }

        public static List<Student> LoadStudentsByClass(string className)
        {
            string classFilePath = Path.Combine(FolderPath, $"{className}.txt");
            if (!File.Exists(classFilePath)) return new List<Student>();

            var lines = File.ReadAllLines(classFilePath);
            return lines.Select(line => line.Split(','))
                        .Select(parts => new Student
                        {
                            Class = className,
                            JournalNumber = int.Parse(parts[0]),
                            Name = parts[1],
                            LastName = parts[2],
                            IsPresent = bool.Parse(parts[3]),
                            HasBeenDrawn = bool.Parse(parts[4]),
                            DrawnCount = int.Parse(parts[5]),
                            ColorState = parts[6]
                        }).ToList();
        }

        public static Student DrawStudent(List<Student> students, int luckyNumber, bool includeAll = true)
        {
            var filteredStudents = includeAll ? students : students.Where(s => s.IsPresent).ToList();
            filteredStudents = filteredStudents.GroupBy(s => new { s.Class, s.JournalNumber })
                                                .Select(grp => grp.First())
                                                .ToList();

            var rnd = new Random();
            if (filteredStudents.Any())
            {
                var index = rnd.Next(filteredStudents.Count);
                var drawnStudent = filteredStudents[index];
                UpdateStudentState(students, drawnStudent);

                return drawnStudent;
            }

            return null;
        }

        private static void UpdateStudentState(List<Student> students, Student drawnStudent)
        {
            var student = students.FirstOrDefault(s => s.JournalNumber == drawnStudent.JournalNumber && s.Class == drawnStudent.Class);
            if (student != null)
            {
                student.HasBeenDrawn = true;
                student.DrawnCount += 1;
                if (student.DrawnCount >= 4)
                {
                    student.DrawnCount = 0;
                    student.HasBeenDrawn = false;
                }
            }
            SaveStudentsList(students);
        }

        public static void ResetDrawnState()
        {
            var students = LoadStudentsList();
            foreach (var student in students)
            {
                student.HasBeenDrawn = false;
                student.DrawnCount = 0;
            }
            SaveStudentsList(students);
        }

        public static Student LoadStudentByClassAndJournalNumber(string studentClass, int journalNumber)
        {
            return LoadStudentsList().FirstOrDefault(s => s.Class == studentClass && s.JournalNumber == journalNumber);
        }

        public static bool IsJournalNumberUnique(int journalNumber, string studentClass, int originalJournalNumber, string originalClass)
        {
            var students = LoadStudentsList();
            if (studentClass != originalClass || journalNumber != originalJournalNumber)
            {
                return !students.Any(s => s.Class == studentClass && s.JournalNumber == journalNumber);
            }
            return true;
        }

        public static void UpdateStudent(Student updatedStudent, int originalJournalNumber, string originalClass)
        {
            var students = LoadStudentsList();
            if (originalClass != updatedStudent.Class || originalJournalNumber != updatedStudent.JournalNumber)
            {
                DeleteStudent(new Student { Class = originalClass, JournalNumber = originalJournalNumber });
            }

            var existingStudentIndex = students.FindIndex(s => s.Class == updatedStudent.Class && s.JournalNumber == updatedStudent.JournalNumber);
            if (existingStudentIndex != -1)
            {
                students[existingStudentIndex] = updatedStudent;
            }
            else
            {
                students.Add(updatedStudent);
            }

            SaveStudentsList(students);
        }

        public static void DeleteStudent(Student studentToDelete)
        {
            var students = LoadStudentsList();
            var student = students.FirstOrDefault(s => s.Class == studentToDelete.Class && s.JournalNumber == studentToDelete.JournalNumber);
            if (student != null)
            {
                students.Remove(student);
                SaveStudentsList(students);
            }
        }
    }
}