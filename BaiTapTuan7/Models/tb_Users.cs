//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class tb_Users
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Usertype { get; set; }
        public Nullable<bool> Block { get; set; }
        public Nullable<System.DateTime> RegisterDate { get; set; }
        public string ResetPasswordCode { get; set; }

        public string AddUserError;
    }
}
