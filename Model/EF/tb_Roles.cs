namespace Model.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_Roles
    {
        [Key]
        [StringLength(50)]
        public string RoleId { get; set; }

        [StringLength(50)]
        public string RoleName { get; set; }
    }
}
