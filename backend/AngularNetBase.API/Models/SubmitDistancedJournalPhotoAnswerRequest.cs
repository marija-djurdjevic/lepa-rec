using Microsoft.AspNetCore.Http;

namespace AngularNetBase.API.Models
{
    public class SubmitDistancedJournalPhotoAnswerRequest
    {
        public Guid ExerciseId { get; set; }
        public DateTime SessionDate { get; set; }
        public string? MainAnswer { get; set; }
        public string? FollowUpAnswer { get; set; }
        public string? Reflection { get; set; }
        public List<IFormFile> Photos { get; set; } = new();
    }
}
