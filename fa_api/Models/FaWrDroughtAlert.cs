using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace fa_api.Models
{
    [Table("FA_WR_DroughtAlert")]
    public partial class FaWrDroughtAlert
    {
        [Key]
        public long Id { get; set; }
        [Required]
        [StringLength(20)]
        public string AreaName { get; set; }
        [StringLength(10)]
        public string Severity { get; set; }
        public DateTime CreateTime { get; set; }
        [Required]
        [StringLength(20)]
        public string CreateUserNo { get; set; }
        public DateTime? UpdateTime { get; set; }
        [StringLength(20)]
        public string UpdateUserNo { get; set; }
    }
}
