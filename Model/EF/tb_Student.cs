namespace Model.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_Student
    {
        [Key]
        public int StudentId { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        public int? UserId { get; set; }

        [StringLength(50)]
        public string Gmail { get; set; }

        public int? PhoneNumber { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [StringLength(150)]
        public string PlaceOfBirth { get; set; }

        public byte[] Images { get; set; }
    }
}
