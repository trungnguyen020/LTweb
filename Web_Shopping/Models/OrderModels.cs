namespace Web_Shopping.Models
{
    public class OrderModels
    {
        public int Id { get; set; }
        public string OrderCode { get; set; }
        public string UserName { get; set; }
        public DateTime CreateDate { get; set; }
        public int Status { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
    }
}
