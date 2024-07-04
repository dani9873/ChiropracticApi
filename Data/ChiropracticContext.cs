using Microsoft.EntityFrameworkCore;
using ChiropracticApi.Models;

namespace ChiropracticApi.Data
{
   public class ChiropracticContext : DbContext
{
    public ChiropracticContext(DbContextOptions<ChiropracticContext> options) : base(options)
    {
    }

    public required DbSet<User> User { get; set; }
    public required DbSet<Role> Role { get; set; }
    public required DbSet<Service> Service { get; set; }
    public required DbSet<Image> Image { get; set; }
    public required DbSet<Appointment> Appointment { get; set; }
    public required DbSet<UserAppointment> User_Appointment { get; set; }
    public required DbSet<AppointmentHistory> appointment_history { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configurar claves primarias y relaciones
        modelBuilder.Entity<User>().HasKey(u => u.IdUsuario);
        modelBuilder.Entity<Role>().HasKey(r => r.IdRole);
        modelBuilder.Entity<Service>().HasKey(s => s.IdService);
        modelBuilder.Entity<Image>().HasKey(i => i.IdImage);
        modelBuilder.Entity<Appointment>().HasKey(a => a.IdAppointment);
        modelBuilder.Entity<UserAppointment>().HasKey(ua => ua.IdUser_Appointment);
        modelBuilder.Entity<AppointmentHistory>().HasKey(ah => ah.Idappointment_history);

        modelBuilder.Entity<User>()
            .HasMany(r => r.UserAppointments)
            .WithOne(u => u.User)
            .HasForeignKey(u => u.User_idusuario)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Role>()
            .HasMany(r => r.Users)
            .WithOne(u => u.Role)
            .HasForeignKey(u => u.Role_idrole)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Image>()
            .HasOne(i => i.Service)
            .WithMany(s => s.Images)
            .HasForeignKey(i => i.Service_idservice)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Appointment>()
            .HasMany(a => a.appointment_history)
            .WithOne(ah => ah.Appointment)
            .HasForeignKey(ah => ah.Appointment_idappointment)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Appointment>()
            .HasMany(a => a.UserAppointments)
            .WithOne(ua => ua.Appointment)
            .HasForeignKey(ua => ua.Appointment_idappointment)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserAppointment>()
            .HasOne(ua => ua.User)
            .WithMany(u => u.UserAppointments)
            .HasForeignKey(ua => ua.User_idusuario)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserAppointment>()
            .HasOne(ua => ua.Appointment)
            .WithMany(a => a.UserAppointments)
            .HasForeignKey(ua => ua.Appointment_idappointment)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserAppointment>()
            .HasOne(ua => ua.Service)
            .WithMany(s => s.UserAppointments)
            .HasForeignKey(ua => ua.Service_idservice)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AppointmentHistory>()
            .HasOne(ah => ah.Appointment)
            .WithMany(a => a.appointment_history)
            .HasForeignKey(ah => ah.Appointment_idappointment)
            .OnDelete(DeleteBehavior.Cascade);

        // Datos semilla
        modelBuilder.Entity<Role>().HasData(
            new Role { IdRole = 2, description_role = "Admin" },
            new Role { IdRole = 3, description_role = "User" }
        );
    }
}
}
