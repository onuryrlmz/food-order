using System.Collections;
using System.Runtime.Serialization;

namespace Domain.Service;

[DataContract(IsReference = true)]
public class ServiceCollectionResult<TEntity> : ServiceCollectionResult
{
    private bool _HasData;

    [DataMember]
    public new List<TEntity> Data { get; set; }

    [DataMember]
    public bool HasData
    {
        get
        {
            if (Data != null && Data.Count > 0)
                _HasData = true;
            return _HasData;
        }
        set => _HasData = value;
    }

    public void SetData(int totalCount, List<TEntity> list)
    {
        SetData(Convert.ToInt64(totalCount), list);
    }

    public void SetData(long totalCount, IList list)
    {
        if (list is List<TEntity>)
        {
            SetData(totalCount, list as List<TEntity>);
        }
        else
        {
            TotalDataCount = totalCount;
            base.Data = list;
            if (!HasFailed)
                HasFailed = false;
        }
    }

    public void SetData(long totalCount, List<TEntity> list)
    {
        TotalDataCount = totalCount;
        Data = list;
        if (!HasFailed)
            HasFailed = false;
    }

    public void SetData(List<TEntity> list)
    {
        Data = list;
        if (list != null)
            TotalDataCount = list.Count;
        if (!HasFailed)
            HasFailed = false;
    }
}