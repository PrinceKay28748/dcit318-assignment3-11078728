using System;
using System.Collections.Generic;
using System.IO;

public class Student
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public int Score { get; set; }

    public Student(int id, string fullName, int score)
    {
        Id = id;
        FullName = fullName;
        Score = score;
    }

    public string GetGrade()
    {
        if (Score >= 80 && Score <= 100) return "A";
        if (Score >= 70) return "B";
        if (Score >= 60) return "C";
        if (Score >= 50) return "D";
        return "F";
    }
}

public class InvalidScoreFormatException : Exception
{
    public InvalidScoreFormatException(string message) : base(message) { }
}

public class MissingFieldException : Exception
{
    public MissingFieldException(string message) : base(message) { }
}


public class StudentResultProcessor
{
    public List<Student> ReadStudentsFromFile(string inputFilePath)
    {
        List<Student> students = new List<Student>();

        using (StreamReader reader = new StreamReader(inputFilePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var parts = line.Split(',');

                if (parts.Length != 3)
                    throw new MissingFieldException($"Line is missing fields: \"{line}\"");

                if (!int.TryParse(parts[0].Trim(), out int id))
                    throw new InvalidScoreFormatException($"Invalid ID format: \"{parts[0]}\"");

                string fullName = parts[1].Trim();

                if (!int.TryParse(parts[2].Trim(), out int score))
                    throw new InvalidScoreFormatException($"Invalid score format: \"{parts[2]}\"");

                students.Add(new Student(id, fullName, score));
            }
        }

        return students;
    }

    public void WriteReportToFile(List<Student> students, string outputFilePath)
    {
        using (StreamWriter writer = new StreamWriter(outputFilePath))
        {
            foreach (var student in students)
            {
                string report = $"{student.FullName} (ID: {student.Id}): Score = {student.Score}, Grade = {student.GetGrade()}";
                writer.WriteLine(report);
            }
        }
    }
}

public class Program
{
    public static void Main()
    {
        string inputFilePath = @"C:\Users\HP\source\repos\students.txt";
        string outputFilePath = @"C:\Users\HP\source\repos\report.txt";


        StudentResultProcessor processor = new StudentResultProcessor();

        try
        {
            var students = processor.ReadStudentsFromFile(inputFilePath);
            processor.WriteReportToFile(students, outputFilePath);
            Console.WriteLine("Report generated successfully!");
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("Input file not found.");
        }
        catch (InvalidScoreFormatException ex)
        {
            Console.WriteLine("Invalid score error: " + ex.Message);
        }
        catch (MissingFieldException ex)
        {
            Console.WriteLine("Missing field error: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Unexpected error: " + ex.Message);
        }
    }
}

