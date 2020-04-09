using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NF.Samples.AWS_S3.MVC_Core.Models;

namespace NF.Samples.AWS_S3.MVC_Core.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #region Amazon
        string AWS_accessKey = "xxxxxxx";
        string AWS_secretKey = "xxxxxxxxxxxxxx";
        string AWS_bucketName = "my-uswest";
        string AWS_defaultFolder = "MyTest_Folder";
        protected async Task<string> UploadFileToAWSAsync(IFormFile file, string subFolder = "")
        {
            var result = "";
            try
            {
                var s3Client = new AmazonS3Client(AWS_accessKey, AWS_secretKey, Amazon.RegionEndpoint.USWest2);
                var bucketName = AWS_bucketName;
                var keyName = AWS_defaultFolder;
                if (!string.IsNullOrEmpty(subFolder))
                    keyName = keyName + "/" + subFolder.Trim();
                keyName = keyName + "/" + file.FileName;

                var fs = file.OpenReadStream();
                var request = new Amazon.S3.Model.PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName,
                    InputStream = fs,
                    ContentType = file.ContentType,
                    CannedACL = S3CannedACL.PublicRead
                };
                await s3Client.PutObjectAsync(request);
                
                result = string.Format("http://{0}.s3.amazonaws.com/{1}", bucketName, keyName);
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
        #endregion
    }
}
