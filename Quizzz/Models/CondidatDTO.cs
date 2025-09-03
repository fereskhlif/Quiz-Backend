public class CondidatDTO
{
    public int Id { get; set; }
    public string Nom { get; set; } = null!;
    //  public ICollection<TestDTO> Tests { get; set; } = new List<TestDTO>();
    public UtilisateurDTO? Utilisateur { get; set; } // nullable
    public List<TestDTO>? Tests { get; set; }
}
