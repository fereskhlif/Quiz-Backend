using System.ComponentModel.DataAnnotations.Schema;
using Quizzz.Models;

public class Test
{
    public int ID { get; set; }
    public DateTime Date_Passage { get; set; }
    public int? NoteObtenu { get; set; }
   
    public string Est_reussi { get; set; }

    public int Candidat_ID { get; set; }
    [ForeignKey("Section")]
    public int SectionID { get; set; }
    //public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
    
   // public virtual ICollection<Reponse> Reponses { get; set; } = new List<Reponse>();
 //   public virtual Candidat Candidat { get; set; }
    public virtual Section Section { get; set; }
    public virtual ICollection<QuestionTest> QuestionTests { get; set; } = new List<QuestionTest>();
}

