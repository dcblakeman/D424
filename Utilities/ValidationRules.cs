using System.Net.Mail;
using System.Text.RegularExpressions;

namespace C_971.Utilities;

public static class ValidationRules
{
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
    private static readonly Regex PhoneRegex = new(@"^(\+[1-9]\d{10,14}|[1-9]\d{2}-\d{3}-\d{4})$", RegexOptions.Compiled);

    public static bool IsValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email) || !EmailRegex.IsMatch(email))
        {
            return false;
        }

        try
        {
            MailAddress address = new(email);
            return address.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsValidPhone(string? phone)
    {
        return !string.IsNullOrWhiteSpace(phone) && PhoneRegex.IsMatch(phone.Trim());
    }

    public static bool IsValidDateRange(DateTime startDate, DateTime endDate)
    {
        return endDate > startDate;
    }
}
