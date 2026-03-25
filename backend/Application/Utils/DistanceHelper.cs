namespace Application.Utils;

public static class DistanceHelper
{
    /// <summary>
    /// Calculates the distance between two coordinates in kilometers using the Haversine formula.
    /// </summary>
    public static decimal CalculateDistanceKm(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
    {
        var R = 6371m; // Earth's radius in kilometers
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);

        var a = (decimal)Math.Sin((double)dLat / 2) * (decimal)Math.Sin((double)dLat / 2) +
                (decimal)Math.Cos((double)ToRadians(lat1)) * (decimal)Math.Cos((double)ToRadians(lat2)) *
                (decimal)Math.Sin((double)dLon / 2) * (decimal)Math.Sin((double)dLon / 2);

        var c = 2 * (decimal)Math.Atan2(Math.Sqrt((double)a), Math.Sqrt((double)(1 - a)));
        return R * c;
    }

    private static decimal ToRadians(decimal angle)
    {
        return angle * (decimal)Math.PI / 180m;
    }
}