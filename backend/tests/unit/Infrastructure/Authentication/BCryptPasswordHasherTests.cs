using EduConnect.Infrastructure.Authentication;
using Shouldly;

namespace EduConnect.Unit.Infrastructure.Authentication;

public class BCryptPasswordHasherTests
{
    [Fact]
    public void Hash_ReturnsBCryptHashThatVerifiesPassword()
    {
        const string password = "secret";
        var hasher = new BCryptPasswordHasher();

        var hash = hasher.Hash(password);

        hash.ShouldNotBe(password);
        BCrypt.Net.BCrypt.Verify(password, hash).ShouldBeTrue();
    }
}
