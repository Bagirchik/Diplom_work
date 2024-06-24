using System;
using System.Collections.Generic;

namespace Diplom_Work;

public partial class Show
{
    public int ShowId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int SeatCount { get; set; }

    public DateTime StartDate { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
