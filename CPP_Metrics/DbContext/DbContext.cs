
using Microsoft.EntityFrameworkCore;



namespace CPP_Metrics.DatabaseContext
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
        // Справочник метрик
        public DbSet<MetricDirectory> MetricDirectory { get; set; }
        public DbSet<LevelMetric> LevelsMetric { get; set; }
        public DbContextMetrics()
        {
            Database.EnsureCreated();
        }

        public int? GetIdMetric(string name)
        {
            return MetricDirectory.SingleOrDefaultAsync(x => x.Name == name)?.Result?.ID;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("host=localhost;Port=5432;Database=metrics;Username=postgres;Password=123;Pooling=false;");
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>().HasData(
                new Project { ID = 1, Name = "test" }
                );

            modelBuilder.Entity<Solution>().HasData(
                new Solution { ID = 1,Date = DateTime.UtcNow,ProjectID = 1  }
                );

            modelBuilder.Entity<LevelMetric>().HasData(
                new LevelMetric { ID = 1, Name = "file" },
                new LevelMetric { ID = 2, Name = "class" },
                new LevelMetric { ID = 3, Name = "function" },
                new LevelMetric { ID = 4, Name = "category" }

                );

            modelBuilder.Entity<MetricDirectory>().HasData(
                // SLOC
                new MetricDirectory { ID = 1, Name = "LOC", LevelMetricID = 1  },
                new MetricDirectory { ID = 2, Name = "LOCo", LevelMetricID = 1 },
                new MetricDirectory { ID = 3, Name = "LOE", LevelMetricID = 1 },
                new MetricDirectory {ID = 4, Name = "PC", LevelMetricID = 1 },
                new MetricDirectory {ID = 5, Name = "PE", LevelMetricID = 1 },
                // Cyclomatic
                new MetricDirectory {ID = 6, Name = "Cyclomatic", LevelMetricID = 3 },
                // DIT
                new MetricDirectory {ID = 7, Name = "DIT", LevelMetricID = 2 },
                // Abstraction
                new MetricDirectory {ID = 8, Name = "A", LevelMetricID = 4 },
                // CBO
                new MetricDirectory {ID = 9, Name = "CBO", LevelMetricID = 2 },
                // CA CE
                new MetricDirectory { ID = 10, Name = "CA", LevelMetricID = 4 },
                new MetricDirectory { ID = 11, Name = "CE", LevelMetricID = 4 },
                // Instability
                new MetricDirectory { ID = 12, Name = "I", LevelMetricID = 4 }
  
                );
           
        }


    }
}
