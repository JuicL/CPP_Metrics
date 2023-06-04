
using Microsoft.EntityFrameworkCore;



namespace CPP_Metrics.DbContextMetrics
{

    public class Project
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }

    }

    public class Solution
    {
        public int ID { get; set; }
        public int ProjectID { get; set; }
        public DateTime Date { get; set; }
        public Project Project { get; set; }
    }

    public class MetricValue
    {
        public int ID { get; set; }
        //Название файла
        public string FileName { get; set; }
        // Название объекта
        public string ObjectName { get; set; }
        public int MetricDirectoryID { get; set; }
        public int SolutionID { get; set; }
        public decimal Value { get; set; }
        public Solution Solution { get; set; }
        public MetricDirectory MetricDirectory { get; set; }
    }

    public class MetricDirectory
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public int LevelMetricID {get;set; }

        public LevelMetric LevelMetric { get; set; }

    }

    public class LevelMetric
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
    

    public class DbContextMetrics : DbContext
    {
        public DbSet<Project> Projects { get; set; }
        public DbSet<Solution> Solutions { get; set; }
        public DbSet<MetricValue> MetricValues { get; set; }
        public DbSet<MetricDirectory> MetricDirectory { get; set; }
        public DbSet<LevelMetric> LevelsMetric { get; set; }
        public DbContextMetrics()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("host=db;Port=5432;Database=studentdb;Username=postgres;Password=123");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Group>().HasData(
            //    new Group { GroupId = 1, Name = "12" },
            //     new Group { GroupId = 2, Name = "13" }
            //);
            
        }


    }
}
