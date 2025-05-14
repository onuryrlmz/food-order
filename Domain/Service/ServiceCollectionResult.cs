using System.Runtime.Serialization;

namespace Domain.Service;

[DataContract(IsReference = true)]
public class ServiceCollectionResult : ServiceResult
{
    [DataMember]
    public object RawData { get; set; }

    [DataMember]
    public long TotalDataCount { get; set; }

    public void SetRawData(object data)
    {
        RawData = data;
    }
}