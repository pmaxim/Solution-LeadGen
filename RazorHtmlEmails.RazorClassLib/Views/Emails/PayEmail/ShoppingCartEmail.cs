using System.Collections.Generic;

namespace RazorHtmlEmails.RazorClassLib.Views.Emails.PayEmail
{
    public class ShoppingCartEmail
    {
        public string UserName { get; set; }
        public string OrderNumber { get; set; }
        public string OrderDate { get; set; }
        public string OrderTotal { get; set; }
        public string Address { get; set; }
        public List<ShoppingCartEmailItem> List { get; set; } = new List<ShoppingCartEmailItem>();
        public List<ShoppingCartEmailSummaryItem> SummaryList { get; set; } = new List<ShoppingCartEmailSummaryItem>();
    }

    public class ShoppingCartEmailItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Price { get; set; }
        public decimal Qty { get; set; }
        public string Subtotal { get; set; }
        public string Size { get; set; }
        public string EvenTime { get; set; }
        public string EndTime { get; set; }
    }
    public class ShoppingCartEmailSummaryItem
    {
        public string Spelling { get; set; }
        public string Subtotal { get; set; }
        public decimal SubtotalDecimal { get; set; }
        public int Discount { get; set; }
        public decimal Amount { get; set; }
    }
}
