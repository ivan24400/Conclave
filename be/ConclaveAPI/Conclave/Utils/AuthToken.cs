using Conclave.Services;
using Google.Apis.Auth;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace Conclave.Utils
{
    /* 
     * A refresh-token based authentication scheme.
     */
    internal static class AuthToken
    {
        private const int accessTTL = 5 * 60; // 5 minutes
        private const int refreshTTL = 180 * 24 * 60 * 60; // 180 days
        internal static string GetNewAccessToken(int userSocialId)
        {
            if(userSocialId < 0)
            {
                throw new ArgumentException("Invalid user social id");
            }

            StringBuilder token = new StringBuilder(Guid.NewGuid().ToString());
            token.Append("@");
            token.Append(userSocialId.ToString());
            token.Append("@");
            token.Append(DateTime.UtcNow.AddSeconds(accessTTL).ToString("yyyy,MM,dd,HH,mm,ss"));
            return Crypto.Encrypt(token.ToString());
        }

        internal static string RefreshAccessToken(string oldToken)
        {
            string oldTokenDecrypted = Crypto.Decrypt(oldToken);
            string userSocialId = oldTokenDecrypted.Split("@")[1];
            return GetNewAccessToken(Convert.ToInt32(userSocialId));
        }

        internal static int GetUserSocialId(string token)
        {
            string tokenDecrypted = Crypto.Decrypt(token);
            return Convert.ToInt32(tokenDecrypted.Split("@")[1]);
        }

        internal static bool VerifyAccessToken(string token)
        {
            bool IsValid = false;
            try
            {
                Guid tg;
                string TokenDecrypted = Crypto.Decrypt(token);
                if (Guid.TryParse(TokenDecrypted.Split("@")[0], out tg))
                {
                    int[] dts = TokenDecrypted.Split("@")[2].Split(",").Select(val => Convert.ToInt32(val)).ToArray();
                    DateTime dt = new DateTime(dts[0], dts[1], dts[2], dts[3], dts[4], dts[5], DateTimeKind.Utc);
                    if (DateTime.Compare(dt, DateTime.UtcNow) >= 0)
                    {
                        IsValid = true;
                    }
                }
            }
            catch (Exception e)
            {
                CLogger.Log(e);
            }
            return IsValid;
        }

        internal static string GetNewRefreshToken(int userSocialId)
        {
            if (userSocialId < 0)
            {
                throw new ArgumentException("Invalid user social id");
            }
            StringBuilder token = new StringBuilder(Guid.NewGuid().ToString());
            token.Append("@");
            token.Append(userSocialId.ToString());
            token.Append("@");
            token.Append(DateTime.UtcNow.AddSeconds(refreshTTL).ToString("yyyy,MM,dd,HH,mm,ss"));
            return Crypto.Encrypt(token.ToString());
        }

        internal static bool VerifyRefreshToken(string refreshToken, string accessToken)
        {
            bool IsValid = false;
            try
            {
                Guid tg;
                string rtokenDecrypted = Crypto.Decrypt(refreshToken);
                string atokenDecrypted = Crypto.Decrypt(accessToken);
                if (rtokenDecrypted.Split("@")[1] == atokenDecrypted.Split("@")[1] && Guid.TryParse(rtokenDecrypted.Split("@")[0], out tg))
                {
                    int[] dts = rtokenDecrypted.Split("@")[2].Split(",").Select(val => Convert.ToInt32(val)).ToArray();
                    DateTime dt = new DateTime(dts[0], dts[1], dts[2], dts[3], dts[4], dts[5], DateTimeKind.Utc);
                    if (DateTime.Compare(dt, DateTime.UtcNow) >= 0)
                    {
                        IsValid = true;
                    }
                }
            }
            catch (Exception e)
            {
                CLogger.Log(e);
            }
            return IsValid;
        }

        internal async static Task<bool> VerifyGoogleOauthV2Token(string idToken)
        {
            bool isValid = false;
            try
            {
                var validPayload = await GoogleJsonWebSignature.ValidateAsync(idToken);
                isValid = true;

            }
            catch (Exception e)
            {
                CLogger.Log(e);
            }
            return isValid;
        }

        internal async static Task<bool> VerifyFacebookOauthToken(string token)
        {
            bool isValid = false;
            try
            {
                HttpResponseMessage response = await HttpRequest.Get().GetAsync("https://graph.facebook.com/me?access_token=" + token);
                if (response.IsSuccessStatusCode)
                {
                    isValid = true;
                }
            }
            catch (Exception e)
            {
                CLogger.Log(e);
            }

            return isValid;
        }
    }
}
