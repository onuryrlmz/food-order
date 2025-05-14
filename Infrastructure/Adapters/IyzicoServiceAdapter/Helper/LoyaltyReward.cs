namespace Infrastructure.Adapters.IyzicoServiceAdapter.Helper;

public class LoyaltyReward : RequestStringConvertible
{
    public string RewardAmount { get; set; }
    public int RewardUsage { get; set; }

    public virtual string ToPKIRequestString()
    {
        return ToStringRequestBuilder.NewInstance()
            .Append("rewardAmount", RewardAmount)
            .Append("rewardUsage", RewardUsage)
            .GetRequestString();
    }
}