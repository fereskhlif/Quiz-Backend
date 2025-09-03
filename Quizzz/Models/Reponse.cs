using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quizzz.Models;

public partial class Reponse
{
    public int ID { get; set; }
   
    public string Test_reponse { get; set; }

    public bool Est_correcte { get; set; }

    public int Question_ID { get; set; }

    public virtual Question Question { get; set; }

}
