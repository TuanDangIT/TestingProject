using FluentValidation;

namespace ResultPatternTesting
{
    public class FluentValidationSettings
    {
        public static void UnableLanguageManager()
        {
            ValidatorOptions.Global.LanguageManager.Enabled = false;
        }
    }
}
