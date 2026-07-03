namespace Infrastructure.Adapters.AwsS3;

public interface IAwsS3ServiceAdapter
{
    string? UploadFile(string filePath, string folderName);
    Task<string?> UploadBytesAsync(byte[] content, string fileName, string folderName, string contentType = "application/json");
    // Verilen public URL'e karşılık gelen S3/R2 objesini siler. Orphan obje bırakmamak için kullanılır.
    Task<bool> DeleteFileAsync(string fileUrl);
}