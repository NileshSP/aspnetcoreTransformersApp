using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace aspnetcoreTransformersApp.Models
{
    public class Transformer : IValidatableObject
    {
        [Key]
        public int TransformerId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MinLength(5, ErrorMessage = "Name should be atleast 5 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Strength is required")]
        public int Strength { get; set; }

        [Required(ErrorMessage = "Intelligence is required")]
        public int Intelligence { get; set; }

        [Required(ErrorMessage = "Speed is required")]
        public int Speed { get; set; }

        [Required(ErrorMessage = "Endurance is required")]
        public int Endurance { get; set; }

        [Required(ErrorMessage = "Rank is required")]
        public int Rank { get; set; }

        [Required(ErrorMessage = "Courage is required")]
        public int Courage { get; set; }

        [Required(ErrorMessage = "Firepower is required")]
        public int Firepower { get; set; }

        [Required(ErrorMessage = "Skill is required")]
        public int Skill { get; set; }

        [Required(ErrorMessage = "AllegianceId is required")]
        public int? AllegianceId { get; set; }
        public List<TransformerAllegiance> Allegiance { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Name.ToLower().Trim() == "string")
            {
                yield return new ValidationResult($"Transformer name cannot be {Name}", new[] { "Name" });
            }

            if (AllegianceId == null)
            {
                yield return new ValidationResult($"AllegianceId value is required", new[] { "AllegianceId" });
            }
            else if (AllegianceId == 0)
            {
                yield return new ValidationResult($"AllegianceId value should be greater than {AllegianceId}", new[] { "AllegianceId" });
            }
        }
    }

    public class TransformerScore
    {
        public int TransformerId { get; set; }
        public int Score { get; set; }
    }
}
