using Conclave.Models;
using Conclave.Models.api;
using Conclave.Services;
using Conclave.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace Conclave.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class AccessController : ControllerBase
    {
        private readonly RedisClient _cache;

        public AccessController(RedisClient cache)
        {
            _cache = cache;
        }


        public IActionResult Renew([FromBody] AccessTokens reqData)
        {
            int resHttpStatusCode = 400;
            dynamic response = new ExpandoObject();
            response.success = "false";
            try
            {
                if (reqData != null && AuthToken.VerifyRefreshToken(reqData.refreshToken, reqData.accessToken))
                {
                    var value = _cache.StringGet(reqData.accessToken);
                    _cache.KeyDelete(reqData.accessToken);
                    var newAccessToken = AuthToken.RefreshAccessToken(reqData.accessToken);
                    _cache.StringSet(newAccessToken, value);
                    resHttpStatusCode = 200;
                    response.success = "true";
                    response.accessToken = newAccessToken;
                }
                else
                {
                    response.msg = "Invalid request";
                }
            }
            catch (Exception e)
            {
                CLogger.Log(e);
                resHttpStatusCode = 500;
                response.msg = "Internal server error";
            }
            HttpContext.Response.StatusCode = resHttpStatusCode;
            return new JsonResult(response);
        }

        public async Task<IActionResult> Login([FromBody] LoginInput reqData)
        {
            int resHttpStatusCode = 400;
            Dictionary<string, string> res = new Dictionary<string, string>();
            Func<string, Task<bool>> authverifier = null;
            string authverifierToken = "";
            switch (reqData.provider)
            {
                case "GOOGLE":
                    authverifier = AuthToken.VerifyGoogleOauthV2Token;
                    authverifierToken = reqData.idToken;
                    break;
                case "FACEBOOK":
                    authverifier = AuthToken.VerifyFacebookOauthToken;
                    authverifierToken = reqData.authToken;
                    break;
            }
            if (authverifier != null && await authverifier(authverifierToken).ConfigureAwait(true))
            {
                int userSocialId = 0;
                using (var ctx = new ConclaveDbContext())
                {
                    var entity = from u in ctx.UserSocial where u.Email == reqData.email && u.Provider == reqData.provider select u;
                    if (entity.FirstOrDefault() != null)
                    {
                        userSocialId = entity.First().Id;
                    }
                    else
                    {
                        UserSocial user = new UserSocial()
                        {
                            Provider = reqData.provider,
                            Email = reqData.email,
                            UserName = reqData.name
                        };
                        ctx.Add(user);
                        ctx.SaveChanges();
                        userSocialId = user.Id;
                    }
                }

                string accessToken = AuthToken.GetNewAccessToken(userSocialId);
                string refreshToken = AuthToken.GetNewRefreshToken(userSocialId);
                res.Add("success", "true");
                res.Add("email", reqData.email);
                res.Add("accessToken", accessToken);
                res.Add("refreshToken", refreshToken);
                _cache.StringSet(accessToken, userSocialId.ToString());
                _cache.StringSet(refreshToken, userSocialId.ToString());
                resHttpStatusCode = 200;
            }
            else
            {
                res.Add("success", "false");
                res.Add("msg", "Invalid Token");
            }
            return StatusCode(resHttpStatusCode, JsonConvert.SerializeObject(res));
        }

        public ActionResult Logout([FromBody] AccessTokens reqData)
        {
            int resHttpStatusCode = 400;
            if (!(string.IsNullOrEmpty(reqData.accessToken) || string.IsNullOrEmpty(reqData.refreshToken)) && _cache.KeyExists(reqData.accessToken) && _cache.KeyExists(reqData.refreshToken))
            {
                _cache.KeyDelete(reqData.accessToken);
                _cache.KeyDelete(reqData.refreshToken);
                resHttpStatusCode = 200;
            }
            HttpContext.Response.StatusCode = resHttpStatusCode;
            return new JsonResult(new
            {
                success = resHttpStatusCode == 200 ? "true" : "false"
            });
        }
    }
}