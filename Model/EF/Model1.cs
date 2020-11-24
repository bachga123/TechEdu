using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Model.EF
{
    public partial class Model1 : DbContext
    {
        public Model1()
            : base("name=Model1")
        {
        }

        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<tb_Class> tb_Class { get; set; }
        public virtual DbSet<tb_Course> tb_Course { get; set; }
        public virtual DbSet<tb_CTS> tb_CTS { get; set; }
        public virtual DbSet<tb_Roles> tb_Roles { get; set; }
        public virtual DbSet<tb_Student> tb_Student { get; set; }
        public virtual DbSet<tb_Teacher> tb_Teacher { get; set; }
        public virtual DbSet<tb_Users> tb_Users { get; set; }
        public virtual DbSet<tb_UserType> tb_UserType { get; set; }
        public virtual DbSet<tb_UserType_Roles> tb_UserType_Roles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tb_CTS>()
                .Property(e => e.Details)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Roles>()
                .Property(e => e.RoleId)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Student>()
                .Property(e => e.Gmail)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Teacher>()
                .Property(e => e.Gmail)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Teacher>()
                .Property(e => e.PlaceOfBirth)
                .IsFixedLength();

            modelBuilder.Entity<tb_Users>()
                .Property(e => e.Username)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Users>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Users>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Users>()
                .Property(e => e.Usertype)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Users>()
                .Property(e => e.ResetPasswordCode)
                .IsFixedLength();

            modelBuilder.Entity<tb_UserType>()
                .Property(e => e.UserTypeId)
                .IsUnicode(false);

            modelBuilder.Entity<tb_UserType_Roles>()
                .Property(e => e.UserTypeId)
                .IsUnicode(false);

            modelBuilder.Entity<tb_UserType_Roles>()
                .Property(e => e.RoleId)
                .IsUnicode(false);
        }
    }
}
