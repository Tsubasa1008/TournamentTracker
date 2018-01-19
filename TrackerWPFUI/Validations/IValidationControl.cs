using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerWPFUI.Validations
{
    public interface IValidationControl
    {
        bool ValidateWhileDisabled { get; set; }

        string GuardProperty { get; set; }
    }
}
