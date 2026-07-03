namespace BDAplication.Web.Models;

public enum Priority { Low = 1, Medium = 2, High = 3, Critical = 4 }

public class BacklogModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Priority Priority { get; set; }
    public string PriorityLabel { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string UserCreated { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; }
}

public class BacklogRegisterRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Priority Priority { get; set; } = Priority.Medium;
}

public class BacklogListResponse
{
    public IEnumerable<BacklogModel> Items { get; set; } = Enumerable.Empty<BacklogModel>();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class PlannerModel
{
    public int Id { get; set; }
    public int BacklogId { get; set; }
    public string BacklogName { get; set; } = string.Empty;
    public string BacklogDescription { get; set; } = string.Empty;
    public Priority Priority { get; set; }
    public string PriorityLabel { get; set; } = string.Empty;
    public int Day { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public string Notes { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string UserCreated { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; }
}

public class PlannerRegisterRequest
{
    public int BacklogId { get; set; }
    public int Day { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public class PlannerInactiveRequest { public int PlannerId { get; set; } }

public class PlannerListRequest { public int Month { get; set; } public int Year { get; set; } }

public class PlannerListByDayModel
{
    public int Day { get; set; }
    public IEnumerable<PlannerModel> Tasks { get; set; } = Enumerable.Empty<PlannerModel>();
}

public class MovePlannerRequest
{
    public int PlannerId { get; set; }
    public int NewDay { get; set; }
    public int NewMonth { get; set; }
    public int NewYear { get; set; }
}
