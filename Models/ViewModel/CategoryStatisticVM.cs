namespace _25DH110651_MyStore.Models.ViewModel
{
    public class CategoryStatisticVM
    {
        public string CategoryName { get; set; }
        public int Quantity { get; set; }
        public decimal? MaxPrice { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? AvgPrice { get; set; }
    }
}