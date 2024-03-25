using ConsoleTables;
using Microsoft.EntityFrameworkCore;

namespace CPP_Metrics.DatabaseContext
{
    public static class Repository
    {
        public static void CreateProjects(string name)
        {
            using (var db = new DbContextMetrics())
            {
                var project = new Project() { Name = name, Date = DateTime.UtcNow };
                db.Projects.Add(project);
                db.SaveChanges();
                Console.WriteLine($"Created project succes id {project.ID}");
            }
        }
        public static void GetProjects(string? name)
        {
            using (var db = new DbContextMetrics())
            {
                var table = new ConsoleTable("Id", "Name", "Date");
                var projects = db.Projects.ToList();
                if (name is not null)
                    projects = projects.Where(p => p.Name == name).ToList();

                if (projects is null)
                {
                    Console.WriteLine("Not find projecs");
                    return;
                }

                foreach (var item in projects)
                {
                    table.AddRow(item.ID, item.Name, item.Date);
                }

                table.Write();
            }
        }
        public static void GetSolution(int projectId)
        {
            using (var db = new DbContextMetrics())
            {
                var table = new ConsoleTable("Id", "Date", "ProjectName");
                var solutions = db.Solutions.Include(x => x.Project).Where(x => x.ProjectID == projectId).ToList();
                if (solutions is null)
                {
                    Console.WriteLine("Not find solutions");
                    return;
                }
                foreach (var item in solutions)
                {
                    table.AddRow(item.ID, item.Date, item.Project.Name);
                }

                table.Write();
            }
        }
        public static void GetMetrics(int solutionId)
        {
            using (var db = new DbContextMetrics())
            {
                var table = new ConsoleTable("Id", "Metric", "FileName", "ObjectName", "Value");
                var metricValues = db.MetricValues.Include(x => x.MetricDirectory).ThenInclude(x => x.LevelMetric).Where(x => x.SolutionID == solutionId).ToList();
                if (metricValues is null)
                {
                    Console.WriteLine("Not find metricsValues");
                    return;
                }
                foreach (var item in metricValues)
                {
                    table.AddRow(item.ID, item.MetricDirectory.Name, item.FileName, item.ObjectName, item.Value);
                }

                table.Write();
            }
        }
        public static void UpdateProject(string name, string newName)
        {
            using (var db = new DbContextMetrics())
            {
                var project = db.Projects.FirstOrDefault(x => x.Name == name);
                if (project is null)
                {
                    Console.WriteLine("Error! Update project name was not found");
                    return;
                }
                project.Name = newName;
                db.SaveChanges();
            }
        }
        public static void DeleteProject(string name)
        {
            using (var db = new DbContextMetrics())
            {
                var project = db.Projects.FirstOrDefault(x => x.Name == name);
                if (project is null)
                {
                    Console.WriteLine("Error! Delete project name was not found");
                    return;
                }
                db.Projects.Remove(project);
                db.SaveChanges();
            }
        }
    }
}
