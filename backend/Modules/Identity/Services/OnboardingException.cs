namespace AngularNetBase.Identity.Services;

public class OnboardingException : Exception
{
    public string Code { get; }

    public OnboardingException(string code, string message) : base(message)
    {
        Code = code;
    }
}
