using System;
using System.Collections.Generic;

namespace MyMvcApp1.Models;

public partial class Course
{
    public int CourseId { get; set; }

    public string? CourseName { get; set; }

    public int? DeptId { get; set; }

    public int? TId { get; set; }

    public virtual Department? Dept { get; set; }

    public virtual Teacher? TIdNavigation { get; set; }
}
