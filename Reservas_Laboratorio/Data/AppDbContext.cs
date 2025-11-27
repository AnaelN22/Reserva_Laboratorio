using Microsoft.EntityFrameworkCore;
using Reservas_Laboratorio.Models;

namespace Reservas_Laboratorio.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        //public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Role> Roles { get; set; }
        public DbSet<Usuario> Users { get; set; }
        public DbSet<Laboratorio> Laboratorios { get; set; }
        public DbSet<Reserva> Reservas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "Usuario" }
            );

            base.OnModelCreating(modelBuilder);
            // Configuracion de propiedades y relaciones
            //modelBuilder.Entity<Reserva>(entity =>
            //{
            //    entity.HasKey(r => r.Id);
            //    entity.HasOne(r => r.Usuario).WithMany().HasForeignKey(r => r.UsuarioId).OnDelete(DeleteBehavior.Restrict);
            //    entity.HasOne(r => r.Lab).WithMany(l => l.Reservas).HasForeignKey(r => r.LabId).OnDelete(DeleteBehavior.Cascade);

            //    // Asegurar que Fecha sea solo fecha en la DB (ej. en SQL Server usar date)
            //    entity.Property(r => r.Fecha).HasColumnType("date");
            //    // HoraInicio/HoraFin puede ser time (si quieres hora sin fecha)
            //    entity.Property(r => r.HoraInicio).HasColumnType("time");
            //    entity.Property(r => r.HoraFin).HasColumnType("time");
            //});

        }
    }
}
