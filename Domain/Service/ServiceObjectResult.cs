using System.Runtime.Serialization;

namespace Domain.Service;

[DataContract(IsReference = true)]
public sealed class ServiceObjectResult<TEntity> : ServiceResult
{
    [DataMember]
    public TEntity Data { get; set; }

    public void SetData(TEntity entity)
    {
        Data = entity;
    }
}