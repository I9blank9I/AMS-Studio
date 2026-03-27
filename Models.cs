using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace ApprenticeManagement;

public class TestResult
{
    public string TestName { get; set; } = string.Empty;
    public double Grade { get; set; }
}

public class Subject
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public List<TestResult> Tests { get; set; } = new();

    [JsonIgnore]
    public double AverageGrade => Tests.Count > 0 ? Math.Round(Tests.Average(t => t.Grade) * 2, MidpointRounding.AwayFromZero) / 2.0 : 0;
}

public class Apprentice
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Department { get; set; } = "IT";
    
    public string PinCode { get; set; } = "1234";
    public string CompanyName { get; set; } = "Not Assigned";
    public string TrainerName { get; set; } = "Not Assigned";
    
    public int SickDays { get; set; } = 0;
    public int CurrentYear { get; set; } = 1; 
    public string DailyPlan { get; set; } = string.Empty;
    public string CareerGoal { get; set; } = "Looking to master my craft.";
    
    // NEW FEATURE: Casino Credits
    public int CasinoCredits { get; set; } = 1000;
    
    public List<Subject> Subjects { get; set; } = new();
    public List<WorkJournal> WorkJournals { get; set; } = new();

    [JsonIgnore]
    public string FullName => $"{FirstName} {LastName}";
    
    [JsonIgnore]
    public double OverallGPA 
    {
        get 
        {
            var activeSubjects = Subjects.Where(s => s.Tests.Count > 0).ToList();
            if (activeSubjects.Count == 0) return 0;
            return Math.Round(activeSubjects.Average(s => s.AverageGrade), 2);
        }
    }

    [JsonIgnore]
    public double AttendanceRate => Math.Max(0, Math.Round(((250.0 - SickDays) / 250.0) * 100, 1));

    [JsonIgnore]
    public bool IsAtRisk => (OverallGPA > 0 && OverallGPA < 4.0) || SickDays > 15;

    [JsonIgnore]
    public string Status 
    {
        get 
        {
            if (IsAtRisk) return "🔴 At Risk";
            if ((OverallGPA > 0 && OverallGPA < 4.5) || SickDays > 8) return "🟡 Warning";
            return "🟢 On Track";
        }
    }
}

public class WorkJournal
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Date { get; set; } = DateTime.Now;
    public string TaskDescription { get; set; } = string.Empty;
    public double HoursWorked { get; set; }
}

public class Company
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
}

public class VocationalTrainer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PinCode { get; set; } = "1234";

    // NEW FEATURE: Casino Credits
    public int CasinoCredits { get; set; } = 1000;

    [JsonIgnore]
    public string FullName => $"{FirstName} {LastName}";
}