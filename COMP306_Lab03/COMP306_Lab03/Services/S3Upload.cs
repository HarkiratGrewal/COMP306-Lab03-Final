using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

namespace COMP306_Lab03.Services
{
    public class S3Upload
    {
        private const string bucketName = "comp306-lab03";
        private const string accessKey = "AKIAIYDXPTXS5C5H44QA";
        private const string secretKey = "4FhhdkOyeM1LFBHEA0+Dl4Q53N7PgXnbmXEgjoxv";

        // Specify your bucket region here
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast1;
        private static IAmazonS3 s3Client;


        public async Task<bool> UploadDocument(Stream stream, string fileName)
        {
            try
            {
                s3Client = new AmazonS3Client(accessKey, secretKey, bucketRegion);
                TransferUtility transferUtility = new TransferUtility(s3Client);
                //Set partSize to 5 MB
                long partSize = 5 * 1024 * 1024;
                TransferUtilityUploadRequest transferUtilityUploadRequest = new TransferUtilityUploadRequest
                {
                    BucketName = bucketName,
                    InputStream = stream,
                    StorageClass = S3StorageClass.Standard,
                    CannedACL = S3CannedACL.PublicReadWrite,
                    PartSize = partSize,
                    Key = fileName
                };

                await transferUtility.UploadAsync(transferUtilityUploadRequest);
                transferUtility.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                //Log exception
                return false;
            }
        }

        public async Task<bool> DownloadDocument(string fileName)
        {
            try
            {
                string directoryPath = @"D:\AWSDocuments\";
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                s3Client = new AmazonS3Client(accessKey, secretKey, bucketRegion);
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = fileName,
                };

                using (GetObjectResponse response = await s3Client.GetObjectAsync(request))
                {
                    string filePath = directoryPath + fileName;
                    await response.WriteResponseStreamToFileAsync(filePath, false, CancellationToken.None);
                    return true;
                }
            }
            catch (Exception ex)
            {
                //Log exception
            }
            return false;
        }
    }
}
