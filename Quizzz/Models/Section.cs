using System;
using System.Collections.Generic;

namespace Quizzz.Models;

public partial class Section
{
    public int Id { get; set; }

    public string Nom { get; set; } = null!;

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

    public virtual ICollection<Test> Tests { get; set; } = new List<Test>();
    
}
