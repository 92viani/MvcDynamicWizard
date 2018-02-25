namespace Mvc.DynamicWizard.ViewModels
{
    using System;

    [Serializable]
    public class WizardValidationResult
    {
        // Summary:
        //     Gets or sets the name of the member.
        //
        // Returns:
        //     The name of the member.
        public string MemberName { get; set; }
        //
        // Summary:
        //     Gets or sets the validation result message.
        //
        // Returns:
        //     The validation result message.
        public string Message { get; set; }
    }
}
