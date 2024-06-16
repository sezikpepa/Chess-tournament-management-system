using System.ComponentModel.DataAnnotations;
using System.Text;
using TournamentLibrary.AnnotationsTranslations;

namespace TournamentLibrary.Participants
{
    public class TournamentPlayer : IParticipant
    {
        public string Id { get; set; }

        [Required(ErrorMessageResourceName = "firstNameRequired", ErrorMessageResourceType = typeof(AnnotationTranslations))]
        [StringLength(50, MinimumLength = 1, ErrorMessageResourceName = "firstNameLength", ErrorMessageResourceType = typeof(AnnotationTranslations))]
        public string FirstName { get; set; }

        [StringLength(50, MinimumLength = 0, ErrorMessageResourceName = "middleNameLength", ErrorMessageResourceType = typeof(AnnotationTranslations))]
        public string? MiddleName { get; set; }

        [Required(ErrorMessageResourceName = "lastNameRequired", ErrorMessageResourceType = typeof(AnnotationTranslations))]
        [StringLength(50, MinimumLength = 1, ErrorMessageResourceName = "lastNameLength", ErrorMessageResourceType = typeof(AnnotationTranslations))]
        public string LastName { get; set; }

        public string DisplayName => GetDisplayName();

        private string GetDisplayName()
        {
			StringBuilder result = new(FirstName);
            if(MiddleName is not null)
            {
                result.Append(" " + MiddleName);
            }
            result.Append(" " + LastName);
			return result.ToString();
		}


		public override bool Equals(object? obj)
		{
			if(obj is TournamentPlayer other)
            {
                return other.Id == Id; 
            }
            return false;
		}

		public override int GetHashCode()
		{
            return HashCode.Combine(Id);
		}
	}
}
