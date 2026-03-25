namespace Infrastructure.Adapters.AwsS3;

public interface IAwsS3ServiceAdapter
{
    string? UploadFile(string filePath, string folderName);
    Task<string?> UploadBytesAsync(byte[] content, string fileName, string folderName, string contentType = "application/json");
}