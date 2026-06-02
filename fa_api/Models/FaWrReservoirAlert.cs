using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace fa_api.Models
{
    [Table("FA_WR_ReservoirAlert")]
    public partial class FaWrReservoirAlert
    {
        [Key]
        public long Id { get; set; }
        [Required]
        [StringLength(20)]
        public string ReservoirName { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal? LowLevelPercentage { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal? MiddleLevelPercentage { get; set; }
        [Column(TypeName = "decimal(10, 6)")]
        public decimal? Longitude { get; set; }
        [Column(TypeName = "decimal(10, 6)")]
        public decimal? Latitude { get; set; }
        public DateTime CreateTime { get; set; }
        [Required]
        [StringLength(20)]
        public string CreateUserNo { get; set; }
        public DateTime? UpdateTime { get; set; }
        [StringLength(20)]
        public string UpdateUserNo { get; set; }
    }
}
