namespace Model.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_Course
    {
        [Key]
        public int Course_Id { get; set; }

        [StringLength(50)]
        public string Course_Name { get; set; }

        public byte[] Images { get; set; }

        [StringLength(150)]
        public string Details { get; set; }
    }
}
