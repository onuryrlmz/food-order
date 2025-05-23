using System.Net;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

namespace Infrastructure.Adapters.AwsS3;

public class AwsS3Base
{
    private readonly string _accessKey;
    private readonly string _bucketName;
    private readonly string _domainUrl;
    private readonly string _secretKey;
    private readonly string _serviceUrl;

    public AwsS3Base(string domainUrl, string serviceUrl, string bucketName, string accessKey, string secretKey)
    {
        _domainUrl = domainUrl;
        _serviceUrl = serviceUrl;
        _bucketName = bucketName;
        _accessKey = accessKey;
        _secretKey = secretKey;
    }

    public string? UploadBase64(string base64, string fileName, string? folderName = null)
    {
        var credentials = new BasicAWSCredentials(_accessKey, _secretKey);
        var s3Client = new AmazonS3Client(credentials, new AmazonS3Config
        {
            ServiceURL = _serviceUrl
        });

        var bytes = Convert.FromBase64String(base64);
        var ms = new MemoryStream(bytes);
        var request = new PutObjectRequest
        {
            CannedACL = S3CannedACL.PublicRead,
            BucketName = _bucketName,
            InputStream = ms,
            Key = string.IsNullOrEmpty(folderName) ? $@"{fileName}" : $@"{folderName}/{fileName}",
            DisablePayloadSigning = true
        };

        var result = Task.Run(() => s3Client.PutObjectAsync(request)).Result;
        if (result == null) return null;
        if (result.HttpStatusCode == HttpStatusCode.OK)
            return string.IsNullOrEmpty(folderName) ? $@"{_domainUrl}/{fileName}" : $@"{_domainUrl}/{folderName}/{fileName}";
        return null;
    }

    public string? Upload(string path, string? folderName = null)
    {
        var credentials = new BasicAWSCredentials(_accessKey, _secretKey);
        var s3Client = new AmazonS3Client(credentials, new AmazonS3Config
        {
            ServiceURL = _serviceUrl
        });

        var file = new FileInfo(path);
        var request = new PutObjectRequest
        {
            CannedACL = S3CannedACL.PublicRead,
            BucketName = _bucketName,
            FilePath = path,
            Key = string.IsNullOrEmpty(folderName) ? $@"{file.Name}" : $@"{folderName}/{file.Name}",
            DisablePayloadSigning = true,
        };

        var result = Task.Run(() => s3Client.PutObjectAsync(request)).Result;
        if (result == null) return null;
        if (result.HttpStatusCode == HttpStatusCode.OK)
            return string.IsNullOrEmpty(folderName) ? $@"{_domainUrl}/{file.Name}" : $@"{_domainUrl}/{folderName}/{file.Name}";
        return null;
    }
}