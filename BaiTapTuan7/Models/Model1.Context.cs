﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BaiTapTuan7.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class TechEduEntities : DbContext
    {
        public TechEduEntities()
            : base("name=TechEduEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<tb_Class> tb_Class { get; set; }
        public virtual DbSet<tb_CTS> tb_CTS { get; set; }
        public virtual DbSet<tb_Roles> tb_Roles { get; set; }
        public virtual DbSet<tb_Student> tb_Student { get; set; }
        public virtual DbSet<tb_Teacher> tb_Teacher { get; set; }
        public virtual DbSet<tb_UserType> tb_UserType { get; set; }
        public virtual DbSet<tb_UserType_Roles> tb_UserType_Roles { get; set; }
        public virtual DbSet<tb_Users> tb_Users { get; set; }

    }
}
