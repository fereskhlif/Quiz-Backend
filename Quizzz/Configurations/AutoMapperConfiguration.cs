using AutoMapper;
using Quizzz.Models;

namespace Quizzz.Configurations
{
    public class AutoMapperConfiguration : Profile
    {
        public AutoMapperConfiguration() {
            CreateMap<UtilisateurDTO,Utilisateur>().ReverseMap()
                .ForMember(n=>n.MotPasseHache,opt=>opt.Ignore());
          /*  CreateMap<Candidat, CondidatDTO>()
    .ForMember(dest => dest.Tests, opt => opt.MapFrom(src => src.Tests))
    .ForMember(dest => dest.Utilisateur, opt => opt.MapFrom(src => src.Utilisateur)).ReverseMap();
          */
            CreateMap<Utilisateur, UtilisateurDTO>();
            CreateMap<Test, TestDTO>()
           .ForMember(dest => dest.Est_reussi, opt => opt.MapFrom(src =>
            src.Est_reussi == "1" ? true :
            src.Est_reussi == "0" ? false :
            (bool?)null
        ));

            CreateMap<TestDTO, Test>()
           .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.Id))
           .ForMember(dest => dest.Date_Passage, opt => opt.MapFrom(src => src.DateDePassage.ToDateTime(TimeOnly.MinValue)))
           .ForMember(dest => dest.SectionID, opt => opt.MapFrom(src => src.SectionId))
           .ForMember(dest => dest.Candidat_ID, opt => opt.Ignore()) 
           .ForMember(dest => dest.Section, opt => opt.Ignore())
          // .ForMember(dest => dest.Candidat, opt => opt.Ignore()) 
           .ForMember(dest => dest.NoteObtenu, opt => opt.Ignore())
           .ForMember(dest => dest.Est_reussi, opt => opt.Ignore());
            CreateMap<Question, QuestionDTO>()
            .ForMember(dest => dest.SectionNom, opt => opt.MapFrom(src => src.Section != null ? src.Section.Nom : string.Empty))
            .ForMember(dest => dest.Section_Id, opt => opt.MapFrom(src => src.Section_ID));
            CreateMap<ReponseDTO, Reponse>()
            .ForMember(dest => dest.Test_reponse, opt => opt.MapFrom(src => src.Texte))
            .ForMember(dest => dest.Est_correcte, opt => opt.MapFrom(src => src.EstCorrecte));
             CreateMap<QuestionDTO, Question>()
            .ForMember(dest => dest.Section_ID, opt => opt.MapFrom(src => src.Section_Id))
            .ForMember(dest => dest.Reponses, opt => opt.MapFrom(src => src.Reponses))
            .ForMember(dest => dest.Section, opt => opt.Ignore());


        }

    }
}
