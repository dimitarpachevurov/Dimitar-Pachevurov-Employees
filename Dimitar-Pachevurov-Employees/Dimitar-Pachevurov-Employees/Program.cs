using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Enter a filename:");
        string csvFilePath = Console.ReadLine();
        List<EmployeeProject> employeeProjects = LoadDataFromCsv(csvFilePath);

        var longestWorkingPair = FindLongestWorkingPair(employeeProjects);

        Console.WriteLine($"Employee 1: {longestWorkingPair.EmployeeId1}");
        Console.WriteLine($"Employee 2: {longestWorkingPair.EmployeeId2}");
        Console.WriteLine($"Worked together for {longestWorkingPair.DaysWorkedTogether} days.");
    }

    static List<EmployeeProject> LoadDataFromCsv(string filePath)
    {
        var employeeProjects = new List<EmployeeProject>();

        using (var reader = new StreamReader(filePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var parts = line.Split(',');
                if (parts.Length == 4 &&
                    int.TryParse(parts[0], out int empId) &&
                    int.TryParse(parts[1], out int projectId) &&
                    DateTime.TryParseExact(parts[2], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateFrom) &&
                    DateTime.TryParseExact(parts[3], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTo))
                {
                    employeeProjects.Add(new EmployeeProject
                    {
                        EmployeeId = empId,
                        ProjectId = projectId,
                        DateFrom = dateFrom,
                        DateTo = dateTo
                    });
                }
            }
        }

        return employeeProjects;
    }

    static (int EmployeeId1, int EmployeeId2, int DaysWorkedTogether) FindLongestWorkingPair(List<EmployeeProject> employeeProjects)
    {
        var maxDaysWorked = 0;
        var result = (EmployeeId1: 0, EmployeeId2: 0, DaysWorkedTogether: 0);

        for (int i = 0; i < employeeProjects.Count; i++)
        {
            for (int j = i + 1; j < employeeProjects.Count; j++)
            {
                var employee1 = employeeProjects[i];
                var employee2 = employeeProjects[j];

                if (employee1.ProjectId == employee2.ProjectId)
                {
                    var daysWorkedTogether = CalculateDaysWorkedTogether(employee1, employee2);

                    if (daysWorkedTogether > maxDaysWorked)
                    {
                        maxDaysWorked = daysWorkedTogether;
                        result = (employee1.EmployeeId, employee2.EmployeeId, maxDaysWorked);
                    }
                }
            }
        }

        return result;
    }

    static int CalculateDaysWorkedTogether(EmployeeProject project1, EmployeeProject project2)
    {
        var dateFrom1 = project1.DateFrom;
        var dateTo1 = project1.DateTo ?? DateTime.Now;
        var dateFrom2 = project2.DateFrom;
        var dateTo2 = project2.DateTo ?? DateTime.Now;

        var earliestEndDate = dateTo1 < dateTo2 ? dateTo1 : dateTo2;
        var latestStartDate = dateFrom1 > dateFrom2 ? dateFrom1 : dateFrom2;

        if (earliestEndDate >= latestStartDate)
        {
            return (int)(earliestEndDate - latestStartDate).TotalDays;
        }

        return 0;
    }
}
