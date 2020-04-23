using Conclave.Models;
using Conclave.Services;
using Conclave.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Conclave.Controllers.User
{
    [Route("user/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private IConfiguration _config { get; set; }
        private readonly RedisClient _cache;
        public PostsController(IConfiguration configuration, RedisClient cache)
        {
            _config = configuration;
            _cache = cache;
        }

        // GET: api/Post
        [HttpGet]
        public IActionResult Get(int offset = -1, int limit = -1)
        {
            int resHttpStatusCode = 400;
            string resMsg = "Bad request";
            var res = new List<dynamic>();
            try
            {
                if (offset != -1 || limit != -1)
                {
                    using (var context = new ConclaveDbContext())
                    {
                        var postList = (from post in context.Post join user in context.UserSocial on post.UserSocialId equals user.Id select new { id = post.Id, username = user.UserName, date = post.Date, text = post.Text, postid = post.Id, postmedia = (post.Media == "N" ? "No" : "Yes") });
                        if (offset != -1)
                        {
                            postList = postList.Skip(offset);
                        }

                        if (limit != -1)
                        {
                            postList = postList.Take(limit);
                        }
                        foreach (var rec in postList)
                        {
                            if (rec.postmedia == "Yes")
                            {
                                var pathPrefix = Request.Scheme + "://" + Request.Host;
                                var mediaList = (from m in context.PostMedia where m.PostId == rec.postid select new { path = pathPrefix + m.Path, type = m.Filetype }).ToList();
                                res.Add(new { id = rec.id, username = rec.username, date = rec.date, text = rec.text, media = mediaList });
                            }
                            else
                            {
                                res.Add(new { id = rec.id, username = rec.username, date = rec.date, text = rec.text });
                            }
                        }
                        resHttpStatusCode = 200;
                        resMsg = "NA";
                    }
                }
                else
                {
                    string allPosts;
                    if (_cache.KeyExists("post-get-all") && _cache.StringGet("posts-get-all-isnew") == "n")
                    {
                        allPosts = _cache.StringGet("post-get-all");
                        JArray obj = (JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(allPosts);
                        foreach (var item in obj)
                        {
                            res.Add(item);
                        }
                        //res = obj.ToList();
                        resHttpStatusCode = 200;
                    }
                    else
                    {
                        using (var context = new ConclaveDbContext())
                        {
                            var postList = (from post in context.Post join user in context.UserSocial on post.UserSocialId equals user.Id select new { id = post.Id, username = user.UserName, date = post.Date, text = post.Text, postid = post.Id, postmedia = (post.Media == "N" ? "No" : "Yes") });

                            foreach (var rec in postList)
                            {
                                if (rec.postmedia == "Yes")
                                {
                                    var pathPrefix = Request.Scheme + "://" + Request.Host;
                                    var mediaList = (from m in context.PostMedia where m.PostId == rec.postid select new { path = pathPrefix + m.Path, type = m.Filetype }).ToList();
                                    res.Add(new { id = rec.id, username = rec.username, date = rec.date, text = rec.text, media = mediaList });
                                }
                                else
                                {
                                    res.Add(new { id = rec.id, username = rec.username, date = rec.date, text = rec.text });
                                }
                            }
                        }
                        allPosts = Newtonsoft.Json.JsonConvert.SerializeObject(res);
                        _cache.StringSet("post-get-all", allPosts);
                        _cache.StringSet("posts-get-all-isnew", "n");
                        resHttpStatusCode = 200;
                    }
                }
            }
            catch (Exception e)
            {
                CLogger.Log(e);
                resHttpStatusCode = 500;
            }
            HttpContext.Response.StatusCode = resHttpStatusCode;
            dynamic response = new ExpandoObject();
            if (resHttpStatusCode == 200)
            {
                response.success = "true";
                response.posts = res;
            }
            else
            {
                response.success = "false";
                response.msg = resMsg;
            }

            return new JsonResult(response);
        }

        // GET: api/Post/5
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(int id)
        {
            dynamic response = new { success = "false", msg = "NA" };
            int resHttpStatusCode = 400;
            try
            {
                using (var context = new ConclaveDbContext())
                {
                    var res = context.Post.Where(x => x.Id == id).FirstOrDefault();
                    if (res != null)
                    {
                        response = new { success = "true", text = res.Text };
                        if (res.Media == "Y")
                        {
                            var mediaList = (from ml in context.PostMedia where ml.PostId == res.Id select new { path = ml.Path, type = ml.Filetype }).ToList();
                            response = new { success = "true", text = res.Text, attachments = mediaList };
                        }
                        resHttpStatusCode = 200;
                    }
                }
            }
            catch (Exception e)
            {
                resHttpStatusCode = 500;
                CLogger.Log(e);
                response = new { success = "false", msg = "An error has occured" };
            }
            HttpContext.Response.StatusCode = resHttpStatusCode;
            return new JsonResult(response);
        }

        [HttpPost]
        [DisableRequestSizeLimit, RequestFormLimits(MultipartBodyLengthLimit = Int32.MaxValue, ValueLengthLimit = Int32.MaxValue)]
        public async Task<IActionResult> Post()
        {
            int resHttpStatusCode = 400;
            string resMsg = "Bad request";
            try
            {
                var uploadPath = _config["AppConfig:Storage:Uploads"];
                if (!string.IsNullOrEmpty(Request.Form["text"].ToString()))
                {
                    Post post = new Post();
                    post.UserSocialId = AuthToken.GetUserSocialId(HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1]);
                    post.Text = Request.Form["text"].ToString();

                    List<Tuple<string, string>> attachmentList = new List<Tuple<string, string>>();
                    foreach (var formFile in Request.Form.Files)
                    {
                        if (formFile.Length > 0)
                        {
                            var filetype = formFile.ContentType.Split("/")[0];
                            var filename = Guid.NewGuid().ToString() + DateTime.UtcNow.ToString("yyyyMMddHHmmsss") + formFile.FileName.Substring(formFile.FileName.LastIndexOf(".", System.StringComparison.Ordinal));
                            var filePath = Path.Combine(uploadPath, filename);

                            using (var stream = System.IO.File.Create(filePath))
                            {
                                await formFile.CopyToAsync(stream).ConfigureAwait(false);
                                attachmentList.Add(Tuple.Create(filetype, "/" + uploadPath + filename));
                            }
                        }
                    }
                    post.Media = attachmentList.Count() > 0 ? "Y" : "N";
                    using (var ctx = new ConclaveDbContext())
                    {
                        ctx.Post.Add(post);
                        ctx.SaveChanges();
                        var postid = post.Id;
                        foreach (var (filetype, filepath) in attachmentList)
                        {
                            PostMedia pm = new PostMedia();
                            pm.PostId = postid;
                            pm.Path = filepath;
                            pm.Filetype = filetype;
                            ctx.PostMedia.Add(pm);
                            ctx.SaveChanges();
                        }
                    }
                    _cache.StringSet("posts-get-all-isnew", "y");
                    resHttpStatusCode = 200;
                    resMsg = "Post addedd successfully";
                }

            }
            catch (Exception e)
            {
                resHttpStatusCode = 500;
                resMsg = "An internal error has occurred";
                CLogger.Log(e);
            }
            HttpContext.Response.StatusCode = resHttpStatusCode;
            return new JsonResult(new
            {
                success = (resHttpStatusCode == 200 ? "true" : "false"),
                msg = resMsg
            }); ;

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id)
        {
            int resHttpStatusCode = 400;
            string resMsg = "Bad request";
            try
            {
                using (var ctx = new ConclaveDbContext())
                {
                    Post post = (from p in ctx.Post where p.Id == id select p).FirstOrDefault();
                    if (post != null)
                    {
                        var uploadPath = _config["AppConfig:Storage:Uploads"];

                        if (Request.Form["text"].ToString() != "")
                        {
                            post.Text = Request.Form["text"].ToString();
                        }

                        if (Request.Form.Files.Count > 0)
                        {
                            List<Tuple<string, string>> attachmentList = new List<Tuple<string, string>>();
                            foreach (var formFile in Request.Form.Files)
                            {
                                if (formFile.Length > 0)
                                {
                                    var filetype = formFile.ContentType.Split("/")[0];
                                    var filename = Guid.NewGuid().ToString() + DateTime.UtcNow.ToString("yyyyMMddHHmmsss") + formFile.FileName.Substring(formFile.FileName.LastIndexOf(".", System.StringComparison.Ordinal));
                                    var filePath = Path.Combine(uploadPath, filename);
                                    var origin = Request.Scheme + "://" + Request.Host;

                                    using (var stream = System.IO.File.Create(filePath))
                                    {
                                        await formFile.CopyToAsync(stream).ConfigureAwait(false);
                                        attachmentList.Add(Tuple.Create(filetype, origin + "/" + uploadPath + filename));
                                    }
                                }
                            }
                            post.Media = "Y";
                            ctx.PostMedia.RemoveRange(ctx.PostMedia.Where(x => x.PostId == id));
                        }
                        ctx.SaveChanges();
                        _cache.StringSet("posts-get-all-isnew", "y");
                        resHttpStatusCode = 200;
                        resMsg = "Post updated successfully";
                    }
                }
            }
            catch (Exception e)
            {
                CLogger.Log(e);
                resHttpStatusCode = 500;
                resMsg = "An internal error has occurred";
            }

            HttpContext.Response.StatusCode = resHttpStatusCode;
            return new JsonResult(new
            {
                success = (resHttpStatusCode == 200 ? "true" : "false"),
                msg = resMsg
            });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            int resHttpStatusCode = 400;
            string resMsg = "Bad request";
            try
            {
                using (var ctx = new ConclaveDbContext())
                {
                    var itemToRemove = ctx.Post.SingleOrDefault(x => x.Id == id);
                    if (itemToRemove != null)
                    {
                        if (itemToRemove.Media == "Y")
                        {
                            var q = (from item in ctx.PostMedia where item.PostId == id select new { id = item.Id }).ToList();
                            foreach (var item in q)
                            {
                                PostMedia pm = new PostMedia() { Id = item.id };
                                ctx.PostMedia.Attach(pm);
                                ctx.PostMedia.Remove(pm);
                            }
                            ctx.SaveChanges();
                        }
                        ctx.Post.Attach(itemToRemove);
                        ctx.Post.Remove(itemToRemove);
                        ctx.SaveChanges();
                        _cache.StringSet("posts-get-all-isnew", "y");
                        resHttpStatusCode = 200;
                    }
                }
            }
            catch (Exception e)
            {
                CLogger.Log(e);
                resHttpStatusCode = 500;
            }

            HttpContext.Response.StatusCode = resHttpStatusCode;
            return new JsonResult(new
            {
                success = (resHttpStatusCode == 200 ? "true" : "false"),
                msg = resMsg
            });
        }
    }
}
