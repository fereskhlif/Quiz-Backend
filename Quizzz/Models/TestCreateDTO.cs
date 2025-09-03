public class TestCreateDTO
{
    public DateTime Date_Passage { get; set; }
    public int? NoteObtenu { get; set; }
    public string Est_reussi { get; set; }
    public int Candidat_ID { get; set; }
    public int SectionID { get; set; }

    public List<int> QuestionIDs { get; set; } = new(); // 🔁 ajout utile
}
