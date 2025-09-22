using System;
using System.Collections.Generic;

namespace MyMvcApp1.Models;

public partial class Teacher
{
    public int TId { get; set; }

    public string? TName { get; set; }

    public int? DeptId { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual Department? Dept { get; set; }
}
