namespace StockWatch.Domain {
    public class Stock {
        public string Name { get; set; }
        public float SellingValue { get; set; }
        public float AcquisitionValue { get; set; }

        public Stock(string name, float sellingValue, float acquisitionValue) {
            Name = name;
            SellingValue = sellingValue;
            AcquisitionValue = acquisitionValue;
        }
    }
}
