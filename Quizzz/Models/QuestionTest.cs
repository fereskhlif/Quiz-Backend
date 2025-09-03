using System.ComponentModel.DataAnnotations.Schema;
using Quizzz.Models;

public class QuestionTest
{
    public int QuestionID { get; set; }
    public int TestID { get; set; }

    public Question Question { get; set; }
    public Test Test { get; set; }
    public int? ReponseChoisieId { get; set; }
    public Reponse ReponseChoisie { get; set; }
}
