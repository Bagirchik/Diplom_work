using System;
using System.Collections.Generic;

namespace Diplom_Work;

public partial class Ticket
{
    public int TicketId { get; set; }

    public int UserId { get; set; }

    public int ShowId { get; set; }

    public decimal Price { get; set; }

    public DateTime PurchaseDate { get; set; }

    public int SeatNumber { get; set; }

    public virtual Show Show { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
