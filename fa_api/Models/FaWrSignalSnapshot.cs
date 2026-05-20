using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace fa_api.Models
{
    [Table("FA_WR_WaterSignalSnapshot")]
    public partial class FaWrSignalSnapshot
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(36)]
        public string BatchId { get; set; }
        [Required]
        [StringLength(100)]
        public string AreaName { get; set; }
        [Required]
        [StringLength(10)]
        public string SignalLevel { get; set; }
        public int SupplyStatus { get; set; }
        [StringLength(50)]
        public string StatusDescription { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime RecordTime { get; set; }
    }
}
