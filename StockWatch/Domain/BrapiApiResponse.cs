namespace StockWatch.Domain {
    public class BrapiApiResponse {
        public List<BrapiStockResults> Results { get; set; }
    }

    public class BrapiStockResults {
        public float RegularMarketPrice { get; set; }
    }
}
