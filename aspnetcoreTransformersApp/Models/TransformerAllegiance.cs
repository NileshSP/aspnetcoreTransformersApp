using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace aspnetcoreTransformersApp.Models
{
    public class TransformerAllegiance
    {
        [Key]
        public int TransformerAllegianceId { get; set; }

        [Required(ErrorMessage = "AllegianceName is required")]
        [MinLength(5, ErrorMessage = "AllegianceName should be atleast 5 characters")]
        public string AllegianceName { get; set; }

        public Transformer Transformer { get; set; }
    }

    public static class TransformersEnums
    {
        public enum TransformerAllegiances
        {
            Autobot = 0,
            Decepticon = 1
        }

        public static List<TransformerAllegiance> TransformerAllegiancesList()
        {
            return Enum.GetValues(typeof(TransformersEnums.TransformerAllegiances))
                    .Cast<TransformersEnums.TransformerAllegiances>()
                    .Select(item => new TransformerAllegiance() { AllegianceName = item.ToString() })
                    .ToList();
        }
    }
}
