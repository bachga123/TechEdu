namespace Model.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_CTS
    {
        public int? ClassId { get; set; }

        public int? StudentId { get; set; }

        public int? TeacherId { get; set; }

        [Key]
        [StringLength(50)]
        public string Details { get; set; }
    }
}
