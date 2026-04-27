using C_971.Models;

namespace C_971.Tests.Models;

public class UserTests
{
    [Fact]
    public void DefaultConstructor_SetsIdToZero()
    {
        var user = new User();
        Assert.Equal(0, user.Id);
    }

    [Fact]
    public void DefaultConstructor_SetsEmailToEmptyString()
    {
        var user = new User();
        Assert.Equal(string.Empty, user.Email);
    }

    [Fact]
    public void DefaultConstructor_SetsFirstNameToEmptyString()
    {
        var user = new User();
        Assert.Equal(string.Empty, user.FirstName);
    }

    [Fact]
    public void DefaultConstructor_SetsLastNameToEmptyString()
    {
        var user = new User();
        Assert.Equal(string.Empty, user.LastName);
    }

    [Fact]
    public void DefaultConstructor_SetsHashedPasswordToEmptyString()
    {
        var user = new User();
        Assert.Equal(string.Empty, user.HashedPassword);
    }

    [Fact]
    public void CanSetEmail()
    {
        var user = new User { Email = "student@university.edu" };
        Assert.Equal("student@university.edu", user.Email);
    }

    [Fact]
    public void CanSetFirstName()
    {
        var user = new User { FirstName = "John" };
        Assert.Equal("John", user.FirstName);
    }

    [Fact]
    public void CanSetLastName()
    {
        var user = new User { LastName = "Doe" };
        Assert.Equal("Doe", user.LastName);
    }

    [Fact]
    public void CanSetHashedPassword()
    {
        var user = new User { HashedPassword = "$2a$10$hashedvalue" };
        Assert.Equal("$2a$10$hashedvalue", user.HashedPassword);
    }

    [Fact]
    public void CanSetId()
    {
        var user = new User { Id = 5 };
        Assert.Equal(5, user.Id);
    }

    [Fact]
    public void CanSetAllProperties()
    {
        var user = new User
        {
            Id = 1,
            Email = "alice@example.com",
            FirstName = "Alice",
            LastName = "Johnson",
            HashedPassword = "$2a$10$abcdefghijklmnopqrstuuVVWXYZ1234567890"
        };

        Assert.Equal(1, user.Id);
        Assert.Equal("alice@example.com", user.Email);
        Assert.Equal("Alice", user.FirstName);
        Assert.Equal("Johnson", user.LastName);
        Assert.Equal("$2a$10$abcdefghijklmnopqrstuuVVWXYZ1234567890", user.HashedPassword);
    }
}
