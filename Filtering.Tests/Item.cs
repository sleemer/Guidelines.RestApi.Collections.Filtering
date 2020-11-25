using System;

namespace Guidelines.RestApi.Collections.Filtering
{
    public sealed class Item
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal? Value { get; set; }
        public bool IsEnabled { get; set; }
    }
}