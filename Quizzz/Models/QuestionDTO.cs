using System.Text.Json.Serialization;

public class QuestionDTO
{
    public int Id { get; set; }

    [JsonPropertyName("enonce")]
    public string Enonce { get; set; }

   
    public int Section_Id { get; set; }

  

    [JsonPropertyName("sectionNom")]
    public string? SectionNom { get; set; }

    [JsonPropertyName("reponses")]
    public List<ReponseDTO> Reponses { get; set; }
    public string? BonneReponse { get; set; }
    public string? ReponseUtilisateur { get; set; }
}
