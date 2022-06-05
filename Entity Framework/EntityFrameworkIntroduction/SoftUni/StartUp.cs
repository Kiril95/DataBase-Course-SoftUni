﻿using Microsoft.EntityFrameworkCore;
using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            // I refactored everything and made it asynchronous but... the Judge system did not like it :(
            var db = new SoftUniContext();

            var exe = RemoveTown(db);
            Console.WriteLine(exe);
        }

        public static string GetEmployeesFullInformation(SoftUniContext context) // Task - 0.3
        {
            var result = string.Empty;
            var employees = context.Employees.ToArray();

            foreach (var person in employees)
            {
                result += $"{person.FirstName} {person.LastName} {person.MiddleName} {person.JobTitle} {person.Salary:f2}\n";
            }
            return result;
        }

        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context) // Task - 0.4
        {
            StringBuilder sb = new StringBuilder();
            var employees = context.Employees
                .Where(x => x.Salary > 50000)
                .Select(x => new { x.FirstName, x.Salary })
                .OrderBy(x => x.FirstName)
                .ToArray();

            foreach (var person in employees)
            {
                sb.AppendLine($"{person.FirstName} - {person.Salary:f2}");
            }
            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context) // Task - 0.5
        {
            StringBuilder sb = new StringBuilder();
            var employees = context.Employees
                .Where(x => x.Department.Name == "Research and Development")
                .Select(x => new
                {
                    firstName = x.FirstName,
                    lastName = x.LastName,
                    deppName = x.Department.Name,
                    salary = x.Salary,
                })
                .OrderBy(x => x.salary)
                .ThenByDescending(x => x.firstName)
                .ToArray();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.firstName} {e.lastName} from {e.deppName} - ${e.salary:f2}");
            }
            return sb.ToString().TrimEnd();
        }

        public static string AddNewAddressToEmployee(SoftUniContext context) // Task - 0.6
        {
            var nakovTheBoss = context.Employees.First(x => x.LastName == "Nakov");
            var nakovNewHome = new Address
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };
            context.Addresses.Add(nakovNewHome);
            nakovTheBoss.Address = nakovNewHome;

            context.SaveChanges();


            StringBuilder sb = new StringBuilder();
            var employees = context.Employees
                .OrderByDescending(x => x.AddressId)
                .Take(10)
                .Select(x => x.Address.AddressText)
                .ToArray();

            return string.Join('\n', employees);
        }

        public static string GetEmployeesInPeriod(SoftUniContext context) // Task - 0.7
        {
            StringBuilder sb = new StringBuilder();
            var employees = context.Employees
                .Where(e => e.EmployeesProjects.Any(ep => ep.Project.StartDate.Year >= 2001 && ep.Project.StartDate.Year <= 2003))
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    managerFName = x.Manager.FirstName,
                    managerLName = x.Manager.LastName,
                    Projects = x.EmployeesProjects.Select(ep => new
                    {
                        projName = ep.Project.Name,
                        projStart = ep.Project.StartDate,
                        projEnd = ep.Project.EndDate
                    })
                })
                .Take(10)
                .ToArray();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - Manager: {e.managerFName} {e.managerLName}");

                foreach (var em in e.Projects)
                {
                    var projEnd = em.projEnd.HasValue ? em.projEnd.Value.ToString("M/d/yyyy h:mm:ss tt") : "not finished";

                    sb.AppendLine($"--{em.projName} - {em.projStart.ToString("M/d/yyyy h:mm:ss tt")} - {projEnd}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetAddressesByTown(SoftUniContext context) // Task - 0.8
        {
            StringBuilder sb = new StringBuilder();
            var employees = context.Addresses
                .Select(x => new
                {
                    Address = x.AddressText,
                    Town = x.Town.Name,
                    Count = x.Employees.Count()
                })
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.Town)
                .ThenBy(x => x.Address)
                .Take(10)
                .ToArray();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.Address}, {e.Town} - {e.Count} employees");
            }
            return sb.ToString().TrimEnd();
        }

        public static string GetEmployee147(SoftUniContext context)  // Task - 0.9
        {
            StringBuilder sb = new StringBuilder();
            var employees = context.Employees
                .Where(x => x.EmployeeId == 147)
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    x.JobTitle,
                    Projects = x.EmployeesProjects
                    .OrderBy(ep => ep.Project.Name)
                    .Select(ep => new
                    {
                        projName = ep.Project.Name,
                    })
                })
                .ToArray();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle}");

                foreach (var em in e.Projects)
                {
                    sb.AppendLine($"{em.projName}");
                }
            }
            return sb.ToString().TrimEnd();
        }

        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)  // Task - 10
        {
            StringBuilder sb = new StringBuilder();
            var employees = context.Departments
                .Where(x => x.Employees.Count() > 5)
                .OrderBy(x => x.Employees.Count())
                .ThenBy(x => x.Name)
                .Select(x => new
                {
                    DeppName = x.Name,
                    ManagerFName = x.Manager.FirstName,
                    ManageLName = x.Manager.LastName,
                    Employees = x.Employees.Select(e => new
                    {
                        EmpFName = e.FirstName,
                        EmpLName = e.LastName,
                        JobTitle = e.JobTitle
                    })
                    .OrderBy(e => e.EmpFName)
                    .ThenBy(e => e.EmpLName)
                })
                .ToArray();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.DeppName} - {e.ManagerFName} {e.ManageLName}");

                foreach (var em in e.Employees)
                {
                    sb.AppendLine($"{em.EmpFName} {em.EmpLName} - {em.JobTitle}");
                }
            }
            return sb.ToString().TrimEnd();
        }

        public static string GetLatestProjects(SoftUniContext context)  // Task - 11
        {
            StringBuilder sb = new StringBuilder();
            var employees = context.Projects
                .OrderByDescending(x => x.StartDate)
                .Take(10)
                .OrderBy(x => x.Name)
                .Select(x => new
                {
                    x.Name,
                    x.Description,
                    x.StartDate
                })
                .ToArray();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.Name}\n{e.Description}\n{e.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)}");
            }
            return sb.ToString().TrimEnd();
        }

        public static string IncreaseSalaries(SoftUniContext context)  // Task - 12
        {
            StringBuilder sb = new StringBuilder();
            var employees = context.Employees
                .Where(x => x.Department.Name == "Engineering" || x.Department.Name == "Tool Design" || x.Department.Name == "Marketing" || x.Department.Name == "Information Services")
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.Salary
                })
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToArray();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} (${e.Salary * (decimal)1.12:f2})");
            }
            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)  // Task - 13
        {
            StringBuilder sb = new StringBuilder();
            var employees = context.Employees
                .Where(x => x.FirstName.StartsWith("Sa"))
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    e.Salary
                })
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToArray();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:f2})");
            }
            return sb.ToString().TrimEnd();
        }

        public static string DeleteProjectById(SoftUniContext context)   // Task - 14
        {
            var targetProject = context.Projects.FirstOrDefault(x => x.ProjectId == 2);
            var projects = context.EmployeesProjects.Where(x => x.ProjectId == 2).ToArray();

            foreach (var project in projects)
            {
                context.EmployeesProjects.Remove(project);
            }

            context.Projects.Remove(targetProject);
            context.SaveChanges();

            StringBuilder sb = new StringBuilder();
            var tenProjects = context.Projects.Take(10).ToArray();

            foreach (var project in tenProjects)
            {
                sb.AppendLine(project.Name);
            }
            return sb.ToString().TrimEnd();
        }

        public static string RemoveTown(SoftUniContext context)   // Task - 15
        {
            var targetTown = context.Towns.FirstOrDefault(x => x.Name == "Seattle");
            var employeesFromSeattle = context.Employees
                .Where(x => x.Address.TownId == targetTown.TownId)
                .ToArray();

            foreach (var e in employeesFromSeattle)
            {
                e.AddressId = null;
            }

            var addressesInSeattle = context.Addresses
                .Where(x => x.Town.Name == "Seattle")
                .ToArray();
            var totalAddresses = addressesInSeattle.Count();

            foreach (var address in addressesInSeattle)
            {
                context.Addresses.Remove(address);
            }

            context.Towns.Remove(targetTown);
            context.SaveChanges();

            return $"{totalAddresses} addresses in Seattle were deleted";
        }
    }
}