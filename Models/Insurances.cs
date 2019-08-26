﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TemplateProject.Models
{
    public class Insurances
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Customer))]
        public int CustomerId { get; set; }
        public virtual Customers Customer { get; set; }

        [Required]
        [ForeignKey(nameof(CarModel))]
        public int CarModelId { get; set; }
        public virtual CarModels CarModel { get; set; }

        [Required]
        [DisplayName("Plaka")]
        public string LicencePlate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayName("Poliçe Başlama Tarihi")]
        public DateTime InsuranceStartDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayName("Poliçe Bitiş Tarihi")]
        public DateTime InsuranceFinishDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Poliçe Bitiş Tarihi E-Posta Atılma Tarihi")]
        public DateTime? InsuranceLastMailDate { get; set; }

        [Required]
        [DisplayName("Poliçe Tutarı")]
        public double InsuranceAmount { get; set; }
        [NotMapped]
        [DisplayName("Toplam Poliçe Tutarı")]
        public double InsuranceAmountTotal { get; set; }
        [DisplayName("Prim")]
        public double InsuranceBonus { get; set; }
        [NotMapped]
        [DisplayName("Toplam Prim")]
        public double InsuranceBonusTotal { get; set; }
        [Required]
        [DisplayName("Sıfır/Yenileme")]
        public byte InsuranceType { get; set; }
        [NotMapped]
        [DisplayName("Sıfır/Yenileme")]
        public string InsuranceTypeName { get; set; }

        public enum InsuranceTypeEnum
        {
            [Display(Name = "SIFIR")]
            NEW = 0,
            [Display(Name = "YENİLEME")]
            RENEWAL = 1,
            [Display(Name = "2. EL")]
            USED_CAR = 2
        }

        [Required]
        [DisplayName("Nakit/Kredi Kartı")]
        public byte InsurancePaymentType { get; set; }
        [NotMapped]
        [DisplayName("Nakit/Kredi Kartı")]
        public string InsurancePaymentTypeName { get; set; }

        public enum InsurancePaymentTypeEnum
        {
            [Display(Name = "NAKİT")]
            CASH = 0,
            [Display(Name = "KREDİ KARTI")]
            CREDIT_CARD = 1,
            [Display(Name = "BEDELSİZ")]
            UNPAID = 2
        }

        [DisplayName("Aktif mi?")]
        public bool IsActive { get; set; }

        [DisplayName("Oluşturulma Tarihi")]
        public DateTime CreatedAt { get; set; }
        [DisplayName("Oluşturan Kişi")]
        public string CreatedBy { get; set; }
        [DisplayName("Güncellenme Tarihi")]
        public DateTime? UpdatedAt { get; set; }
        [DisplayName("Güncelleyen Kişi")]
        public string UpdatedBy { get; set; }
        [DisplayName("Silinme Tarihi")]
        public DateTime? DeletedAt { get; set; }
        [DisplayName("Silen Kişi")]
        public string DeletedBy { get; set; }
    }
}