namespace Model.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_Teacher
    {
        [Key]
        public int TeacherId { get; set; }

        public int? UserId { get; set; }

        [StringLength(50)]
        public string TeacherFirstName { get; set; }

        [StringLength(50)]
        public string TeacherLastName { get; set; }

        [StringLength(100)]
        public string Gmail { get; set; }

        public int? PhoneNumber { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [StringLength(10)]
        public string PlaceOfBirth { get; set; }

        public byte[] Images { get; set; }
    }
}
