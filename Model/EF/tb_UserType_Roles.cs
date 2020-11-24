namespace Model.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_UserType_Roles
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(50)]
        public string UserTypeId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string RoleId { get; set; }

        [StringLength(150)]
        public string Details { get; set; }
    }
}
