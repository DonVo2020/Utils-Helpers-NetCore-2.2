using System;
using System.ComponentModel.DataAnnotations;

namespace AdvancedRelations.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class XorAttribute : ValidationAttribute
    {
        private string xorTargetAttributeName;

        public XorAttribute(string xorTargetAttributeName)
        {
            this.xorTargetAttributeName = xorTargetAttributeName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var targetAttribute = validationContext.ObjectType
                .GetProperty(xorTargetAttributeName)
                .GetValue(validationContext.ObjectInstance);

            if ((targetAttribute == null && value != null) || (targetAttribute != null && value == null))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("The two properties must have opposite values");
        }
    }
}
