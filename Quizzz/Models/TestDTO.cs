public class TestDTO
{
    public int Id { get; set; }
    public DateOnly DateDePassage { get; set; }
    public int SectionId { get; set; }
    public int CandidatId { get; set; }

    public int? NoteObtenu { get; set; }      
    public bool? Est_reussi { get; set; }     
    public List<QuestionDTO> Questions { get; set; }

}
