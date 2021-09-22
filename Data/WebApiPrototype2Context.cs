using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApiPrototype2.Model;

namespace WebApiPrototype2.Data
{
    /// <summary>
    /// Clase de contexto de base de datos con la tecnología de
    /// Entity Framework Core y la base de datos del sistema
    /// Web Api Prototype 2.
    /// </summary>
    public class WebApiPrototype2Context : DbContext
    {
        public WebApiPrototype2Context(DbContextOptions<WebApiPrototype2Context> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Entidad País (Country).
            modelBuilder.Entity<Country>(
                co =>
                {
                    co.ToTable("Countries");
                    co.HasKey(c => c.CountryId);
                    co.Property(c => c.CountryId)
                     .HasColumnName("Id")
                     .HasColumnType("int");
                    co.Property(c => c.Uuid)
                     .IsRequired()
                     .ValueGeneratedOnAdd()
                     .HasDefaultValueSql("NEWID()");
                    co.Property(c => c.Name)
                     .HasMaxLength(50)
                     .IsRequired();
                    co.Property(c => c.Population)
                     .HasColumnType("int")
                     .IsRequired();
                    co.HasMany<State>(c => c.States)
                     .WithOne(s => s.Country)
                     .HasForeignKey(s => s.CountryId)
                     .OnDelete(DeleteBehavior.Cascade);
                });
            // Entidad Estado (State).
            modelBuilder.Entity<State>( 
                st => {
                    st.ToTable("States");
                    st.HasKey(s => s.StateId);
                    st.Property(s => s.StateId)
                     .HasColumnName("Id")
                     .HasColumnType("int");
                    st.Property(s => s.Uuid)
                     .IsRequired()
                     .ValueGeneratedOnAdd()
                     .HasDefaultValueSql("NEWID()");
                    st.Property(s => s.Name)
                      .IsRequired()
                      .HasMaxLength(50);
                    st.Property(s => s.Population)
                     .IsRequired()
                     .HasColumnType("int");
                    st.Property(s => s.CountryId)
                     .IsRequired();

            });
            // Entidad Usuario (User).
            modelBuilder.Entity<User>(
                us => {
                    us.ToTable("Users");
                    us.HasKey(u => u.UserId);
                    us.HasAlternateKey(u => u.UserName)
                      .HasName("uq_user_name");
                    us.Property(u => u.UserId)
                      .HasColumnName("Id")
                      .HasColumnType("int");
                    us.Property(u => u.Uuid)
                     .IsRequired()
                     .ValueGeneratedOnAdd()
                     .HasDefaultValueSql("NEWID()");
                    us.Property(u => u.UserName)
                      .IsRequired()
                      .HasMaxLength(20);
                    us.Property(u => u.PasswordHash)
                      .IsRequired();
                    us.Property(u => u.PasswordSalt)
                      .IsRequired();
                    us.Property(u => u.FirstName)
                      .IsRequired()
                      .IsUnicode()
                      .HasMaxLength(50);
                    us.Property(u => u.MiddleName)
                      .IsUnicode()
                      .HasMaxLength(50);
                    us.Property(u => u.LastName)
                      .IsRequired()
                      .IsUnicode()
                      .HasMaxLength(50);
                    us.Property(u => u.Email)
                      .IsRequired()
                      .HasMaxLength(100);
                    us.Property(u => u.RegistrationDate)
                      .HasColumnType("datetime")
                      .HasDefaultValueSql("GETDATE()")
                      .ValueGeneratedOnAdd();
                    us.Property(u => u.LastUpdateDate)
                      .HasColumnType("datetime")
                      .HasDefaultValueSql("GETDATE()")
                      .ValueGeneratedOnAddOrUpdate();
                });

        }

        public DbSet<WebApiPrototype2.Model.Country> Country { get; set; }

        public DbSet<WebApiPrototype2.Model.State> State { get; set; }

        public DbSet<WebApiPrototype2.Model.User> User { get; set; }

    }

}
