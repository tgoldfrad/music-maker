using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Music.Core.IRepositories;
using Music.Core.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Music.Service
{
    public class AWSService:IAWSService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly IConfiguration _configuration;
        private readonly string _bucketName;

        public AWSService(IConfiguration configuration)
        {
            _configuration = configuration;
            var accessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
            var secretKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");
            var region = RegionEndpoint.USEast1;
            _bucketName = _configuration["AWS:BucketName"];
            _s3Client = new AmazonS3Client(accessKey, secretKey, region);
        }

        public async Task<string> GetPreSignedUrlAsync(int userId, string fileName,string contentType)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = $"{userId}/{fileName}",
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddMinutes(15),
                ContentType = contentType
            };
            return await Task.FromResult(_s3Client.GetPreSignedURL(request));
        }

        public async Task<string> GetDownloadUrlAsync(int userId, string fileName)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = $"{userId}/{fileName}",
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.AddDays(120),
            };
            return await Task.FromResult(_s3Client.GetPreSignedURL(request));
        }


    }
}
