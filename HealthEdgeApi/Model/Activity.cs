namespace HealthEdgeApi.Model
{
    public class Activity
    {
        public string ItemName { get; set; }

        /// <summary>
        /// This assume that any change on the quantity is caused by a high demand
        /// </summary>
        public ushort Count { get; set; }
    }
}
