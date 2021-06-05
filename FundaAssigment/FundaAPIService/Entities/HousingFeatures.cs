namespace FundaAssigment.FundaAPIService.Entities
{
    public enum HousingFeature
    {
        Garden
    }

    static class Extensions
    {
        public static string ToApiName(this HousingFeature housingFeature)
        {
            switch (housingFeature)
            {
                case HousingFeature.Garden: return "tuin";
            }

            return string.Empty;
        }
    }
}
