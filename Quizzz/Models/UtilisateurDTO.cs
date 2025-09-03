public class UtilisateurDTO
{
    public int Id { get; set; }
    public string NomUtilisateur { get; set; }
    public string MotPasseHache { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public UtilisateurDTO Utilisateur { get; set; }
}
