using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace fa_api.Models
{
    [Table("FA_WR_SignalLevel")]
    public partial class FaWrSignalLevel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(10)]
        public string SignalLevel { get; set; }
        [StringLength(20)]
        public string SeverityCode { get; set; }
        [StringLength(100)]
        public string AreaDesc { get; set; }
        [StringLength(50)]
        public string County { get; set; }
        [StringLength(100)]
        public string AlertIdentifier { get; set; }
        [StringLength(500)]
        public string Headline { get; set; }
        public string Description { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? EffectiveTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ExpiresTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime RecordTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateTime { get; set; }
        [StringLength(50)]
        public string DataSource { get; set; }
        [Required]
        public bool? IsActive { get; set; }
        [StringLength(500)]
        public string Remarks { get; set; }
    }
}
