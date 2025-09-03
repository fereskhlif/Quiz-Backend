using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quizzz.Models
{
    public partial class Question
    {
        public int ID { get; set; }
        public string Enonce { get; set; } = null!;
        public int Section_ID { get; set; }  
       

        public virtual Section Section { get; set; } = null!;

        public virtual ICollection<Reponse> Reponses { get; set; } = new List<Reponse>();
        public virtual ICollection<QuestionTest> QuestionTests { get; set; } = new List<QuestionTest>();
    }
}
