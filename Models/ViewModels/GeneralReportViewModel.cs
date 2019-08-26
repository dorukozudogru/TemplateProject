﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TemplateProject.Models.ViewModels
{
    public class GeneralReportViewModel
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Sigorta Şirketi")]
        public string InsuranceCompanyName { get; set; }

        [DisplayName("Poliçe Tipi")]
        public string InsurancePolicyName { get; set; }

        [DisplayName("Nakit/Kredi Kartı")]
        public byte InsurancePaymentType { get; set; }

        [DisplayName("Nakit/Kredi Kartı")]
        public string InsurancePaymentTypeName { get; set; }

        [DisplayName("Toplam Poliçe Sayısı")]
        public int Count { get; set; }
    }
}
