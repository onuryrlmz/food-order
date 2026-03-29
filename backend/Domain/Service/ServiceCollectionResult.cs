using System.Runtime.Serialization;

namespace Domain.Service;

[DataContract(IsReference = true)]
public class ServiceCollectionResult : ServiceResult
{
    [DataMember]
    public object Data { get; set; }

    [DataMember]
    public long TotalDataCount { get; set; }

    public void SetData(object data)
    {
        Data = data;
    }
}