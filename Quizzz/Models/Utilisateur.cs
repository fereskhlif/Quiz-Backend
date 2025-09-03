public class Utilisateur
{
    public int Id { get; set; }
    public string NomUtilisateur { get; set; } = null!;
    public string? MotPasseHache { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
    public string? ResetPasswordToken { get; set; }
    public DateTime? ResetTokenExpires { get; set; }


}
