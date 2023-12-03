using System.Runtime.Serialization;

namespace NegotiationsApi.Models
{
    public class NegotiationModel
    {
        public enum NegotiationStatus
        {
            [EnumMember(Value = "Pending")]
            Pending,

            [EnumMember(Value = "Accepted")]
            Accepted,

            [EnumMember(Value = "Rejected")]
            Rejected
        }
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int CustomerId { get; set; }
        public decimal? ProposedPrice { get; set; }
        public int AttemptsLeft { get; set; } = 3;
        public NegotiationStatus Status { get; set; }
    }
}
