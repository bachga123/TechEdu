namespace Model.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_UserType
    {
        [Key]
        [StringLength(50)]
        public string UserTypeId { get; set; }

        [StringLength(50)]
        public string UserTypeName { get; set; }
    }
}
