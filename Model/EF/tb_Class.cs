namespace Model.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_Class
    {
        [Key]
        public int ClassId { get; set; }

        [StringLength(50)]
        public string ClassName { get; set; }

        public int? YearOfClass { get; set; }
    }
}
