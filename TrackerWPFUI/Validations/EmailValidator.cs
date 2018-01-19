using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TrackerWPFUI.Validations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class EmailValidator : RegularExpressionAttribute, IValidationControl
    {
        public const string EmailValidationExpression = @"([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)";

        public EmailValidator()
            : base(string.Format("^{0}$", EmailValidationExpression))
        {

        }

        public bool ValidateWhileDisabled { get; set; }
        public string GuardProperty { get; set; }
    }
}
