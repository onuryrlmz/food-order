using Base.Constant;

namespace Infrastructure.Adapters.AwsS3;

public class AwsS3ServiceAdapter
{
    private readonly AwsS3Base _awsS3 = new(
        Global.Configuration?.GetSection("AwsS3:Domain")?.Value ?? string.Empty,
        Global.Configuration?.GetSection("AwsS3:Endpoint")?.Value ?? string.Empty,
        Global.Configuration?.GetSection("AwsS3:BucketName")?.Value ?? string.Empty,
        Global.Configuration?.GetSection("AwsS3:AccessKey")?.Value ?? string.Empty,
        Global.Configuration?.GetSection("AwsS3:SecretKey")?.Value ?? string.Empty
    );

    public string? UploadFileAsync(string filePath, string folderName)
    {
        return _awsS3.Upload(filePath, folderName);
    }
}