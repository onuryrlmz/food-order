using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Base.Constant;
using System.Net;

namespace Infrastructure.Adapters.AwsS3;

public class AwsS3ServiceAdapter : IAwsS3ServiceAdapter
{
    private readonly string _domain;
    private readonly string _endpoint;
    private readonly string _bucketName;
    private readonly string _accessKey;
    private readonly string _secretKey;

    public AwsS3ServiceAdapter()
    {
        _domain = Global.Configuration?.GetSection("AwsS3:Domain")?.Value ?? string.Empty;
        _endpoint = Global.Configuration?.GetSection("AwsS3:Endpoint")?.Value ?? string.Empty;
        _bucketName = Global.Configuration?.GetSection("AwsS3:BucketName")?.Value ?? string.Empty;
        _accessKey = Global.Configuration?.GetSection("AwsS3:AccessKey")?.Value ?? string.Empty;
        _secretKey = Global.Configuration?.GetSection("AwsS3:SecretKey")?.Value ?? string.Empty;
    }

    public string? UploadFile(string filePath, string folderName)
    {
        var awsS3 = CreateBase();
        return awsS3.Upload(filePath, folderName);
    }

    public async Task<string?> UploadBytesAsync(byte[] content, string fileName, string folderName, string contentType = "application/json")
    {
        var credentials = new BasicAWSCredentials(_accessKey, _secretKey);
        var s3Client = new AmazonS3Client(credentials, new AmazonS3Config
        {
            ServiceURL = _endpoint
        });

        var key = string.IsNullOrEmpty(folderName) ? fileName : $"{folderName}/{fileName}";

        using var ms = new MemoryStream(content);
        var request = new PutObjectRequest
        {
            CannedACL = S3CannedACL.PublicRead,
            BucketName = _bucketName,
            InputStream = ms,
            Key = key,
            ContentType = contentType,
            DisablePayloadSigning = true
        };

        var result = await s3Client.PutObjectAsync(request);
        if (result?.HttpStatusCode == HttpStatusCode.OK)
            return $"{_domain}/{key}";

        return null;
    }

    private AwsS3Base CreateBase() => new(_domain, _endpoint, _bucketName, _accessKey, _secretKey);
}
