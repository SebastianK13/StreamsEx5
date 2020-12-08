using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zadanie_5
{
    class Program
    {
        static void Main(string[] args)
        {
            string mainPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

            TableCreator tableCreator = new TableCreator();
            var students = tableCreator.GetData(mainPath);
            tableCreator.CreateTableFile(mainPath, students);
            ResultsFileCreator fileCreator = new ResultsFileCreator();
            fileCreator.CreateResultsFile(mainPath, students);
        }
    }
    public class ResultsFileCreator
    {
        public void CreateResultsFile(string mainPath, List<Student> students)
        {
            string outputPath = Path.Combine(mainPath, "OutputFiles");

            if (!File.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            using (StreamWriter file = File.CreateText(Path.Combine(outputPath, "tabelka.txt")))
            {
                file.WriteLine("{0,14}{1,14}{2,14}{3,14}",
                    "Numer", "Imie", "Nazwisko", "Średnia");
                foreach (var s in students)
                {
                    file.WriteLine("{0,14}{1,14}{2,14}{3,14}",
                    s.Id, s.Surname, s.Name, CountAvg(s.SchoolSubjects));
                }
            }
        }
        private string CountAvg(List<SchoolSubject> schoolSubjects)
        {
            int summary = 0;
            int temp = 0;
            int divider = 0;
         
            foreach(var s in schoolSubjects)
            {
                if (Int32.TryParse(s.Mark, out temp))
                {
                    summary += temp;
                    divider++;
                }
            }

            double avgString = (summary / (double)divider);

            return String.Format("{0:F2}", avgString);
        }
    }
    public class TableCreator
    {
        private List<string> TableHeader { get; set; }

        public List<Student> GetData(string mainPath)
        {
            string inputPath = Path.Combine(mainPath, "Results", "wyniki.csv");
            List<Student> students = new List<Student>();

            if (File.Exists(inputPath))
            {
                //microsoft docs ISO 8859-2 Central European; Central European (ISO) - code 28592
                string[] schemaLines = File.ReadAllLines(inputPath, Encoding.GetEncoding(28592));
                string[] subjectsNames = schemaLines[0].Split(';').Skip(3).ToArray();

                SetColumnNames(schemaLines[0].Split(';').ToArray());

                for (int i = 1; i < schemaLines.Length; i++)
                {
                    students.Add(new Student(schemaLines[i].Split(';'), subjectsNames));
                }
            }

            return students;
        }
        private void SetColumnNames(string[] fileHeader)
        {
            TableHeader = new List<string>();
            foreach(var h in fileHeader)
            {
                TableHeader.Add(h);
            }
        }
        public void CreateTableFile(string mainPath, List<Student> students)
        {
            string outputPath = Path.Combine(mainPath, "OutputFiles");

            if (!File.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
            using (StreamWriter file = File.CreateText(Path.Combine(outputPath, "tabelka.txt")))
            {
                file.WriteLine("{0,14}{1,14}{2,14}{3,14}{4,14}{5,14}{6,14}",
                    TableHeader[0], TableHeader[1], TableHeader[2], TableHeader[3],
                    TableHeader[4], TableHeader[5], TableHeader[6]);
                foreach (var s in students)
                {
                    file.WriteLine("{0,14}{1,14}{2,14}{3,14}{4,14}{5,14}{6,14}",
                    s.Id, s.Surname, s.Name, s.SchoolSubjects[0].Mark, s.SchoolSubjects[1].Mark,
                    s.SchoolSubjects[2].Mark, s.SchoolSubjects[3].Mark);
                }
            }
        }
    }
    public class Student
    {
        public Student(string[] studentInfo, string[] subjectsNames)
        {
            List<SchoolSubject> schoolSubjects = new List<SchoolSubject>();
            string[] marks = studentInfo.Skip(3).ToArray();
            Id = Int32.Parse(studentInfo[0]);
            Surname = studentInfo[1];
            Name = studentInfo[2];
            for (int i = 0; i < subjectsNames.Length; i++)
            {
                schoolSubjects.Add(new SchoolSubject
                {
                    SchoolSubjectName = subjectsNames[i],
                    Mark = marks[i]
                });
            }
            SchoolSubjects = schoolSubjects;
        }
        public int Id { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public List<SchoolSubject> SchoolSubjects { get; set; }
    }
    public class SchoolSubject
    {
        public string SchoolSubjectName { get; set; }
        public string Mark { get; set; }
    }
}
