using Xunit;
using Conclave.Utils;
using System;

namespace Conclave.Tests
{
    public class Authentication
    {
        [Fact]
        public void IsAccessTokenValid()
        {
            string accessToken = AuthToken.GetNewAccessToken(1);

            Assert.True(AuthToken.VerifyAccessToken(accessToken));
        }
    }
}
