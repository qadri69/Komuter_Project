namespace ktm_project.Models
{
    public class FareTableViewModel
    {
        public IDictionary<int, string> Stations { get; set; }
        public double[,] Rates { get; set; }
        public string SelectedFareType { get; set; }

        public double GetDiscountedRate(int start, int end)
        {
            double rate = Rates[start, end];
            switch (SelectedFareType)
            {
                case "Senior":
                    return rate * 0.70; // 30% discount
                case "Disabled":
                    return rate * 0.65; // 35% discount
                case "Student":
                    return rate * 0.75; // 25% discount
                default:
                    return rate; // No discount
            }
        }

        public List<string> FareTypes { get; } = new List<string> { "Regular", "Senior", "Disabled", "Student" };
    }
}
