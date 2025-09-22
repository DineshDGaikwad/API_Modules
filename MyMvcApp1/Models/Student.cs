using System;
using System.Collections.Generic;

namespace MyMvcApp1.Models;

public partial class Student
{
    public int StudId { get; set; }

    public string? StudName { get; set; }

    public int? DeptId { get; set; }

    public virtual Department? Dept { get; set; }
}
