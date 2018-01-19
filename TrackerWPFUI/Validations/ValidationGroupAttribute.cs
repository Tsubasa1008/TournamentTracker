using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerWPFUI.Validations
{
    public class ValidationGroupAttribute : Attribute
    {
        public const string DefaultGroupName = "Default";

        public ValidationGroupAttribute()
            : base()
        {
            GroupName = DefaultGroupName;
            IncludeInErrorsValidation = true;
        }

        public string GroupName { get; set; }
        public bool IncludeInErrorsValidation { get; set; }
    }
}
