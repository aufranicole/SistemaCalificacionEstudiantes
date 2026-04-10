using System.Data.Entity;
using SistemaCalificacionEstudiantes.Models;

namespace SistemaCalificacionEstudiantes.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("name=SistemaCalificacionEstudiantesDB")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<AppDbContext>());
        }

        public DbSet<Estudiante> Estudiantes { get; set; }
        public DbSet<Materia> Materias { get; set; }
        public DbSet<Calificacion> Calificaciones { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Calificacion>()
                .HasRequired(c => c.Estudiante)
                .WithMany(e => e.Calificaciones)
                .HasForeignKey(c => c.EstudianteID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Calificacion>()
                .HasRequired(c => c.Materia)
                .WithMany(m => m.Calificaciones)
                .HasForeignKey(c => c.MateriaID)
                .WillCascadeOnDelete(false);

            // le pongo precision a los decimales para que no haya problemas con sql
            modelBuilder.Entity<Calificacion>()
                .Property(c => c.Calificacion1).HasPrecision(5, 2);
            modelBuilder.Entity<Calificacion>()
                .Property(c => c.Calificacion2).HasPrecision(5, 2);
            modelBuilder.Entity<Calificacion>()
                .Property(c => c.Calificacion3).HasPrecision(5, 2);
            modelBuilder.Entity<Calificacion>()
                .Property(c => c.Calificacion4).HasPrecision(5, 2);
            modelBuilder.Entity<Calificacion>()
                .Property(c => c.Examen).HasPrecision(5, 2);
            modelBuilder.Entity<Calificacion>()
                .Property(c => c.TotalCalificacionDB).HasPrecision(5, 2);
        }
    }
}
