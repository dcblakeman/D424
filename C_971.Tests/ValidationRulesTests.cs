using C_971.Utilities;

namespace C_971.Tests;

[TestClass]
public class ValidationRulesTests
{
    [TestMethod]
    public void IsValidEmail_ReturnsTrue_ForStandardEmail()
    {
        bool result = ValidationRules.IsValidEmail("student@example.com");

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsValidEmail_ReturnsFalse_ForMalformedEmail()
    {
        bool result = ValidationRules.IsValidEmail("studentexample.com");

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IsValidPhone_ReturnsTrue_ForDashedPhoneNumber()
    {
        bool result = ValidationRules.IsValidPhone("555-123-4567");

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsValidPhone_ReturnsFalse_ForUnsupportedPhoneFormat()
    {
        bool result = ValidationRules.IsValidPhone("(555) 123-4567");

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IsValidDateRange_ReturnsTrue_WhenEndDateIsAfterStartDate()
    {
        DateTime startDate = new(2026, 4, 1);
        DateTime endDate = new(2026, 5, 1);

        bool result = ValidationRules.IsValidDateRange(startDate, endDate);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsValidDateRange_ReturnsFalse_WhenEndDateIsNotAfterStartDate()
    {
        DateTime startDate = new(2026, 4, 1);
        DateTime endDate = new(2026, 4, 1);

        bool result = ValidationRules.IsValidDateRange(startDate, endDate);

        Assert.IsFalse(result);
    }
}
