using DAKKN.Domain.Common;

namespace DAKKN.Domain.Entities
{
    public class SupportFAQ : BaseEntity<Guid>
    {
        public string Question { get; set; } = string.Empty;
        public string ArQuestion { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public string ArAnswer { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsPublished { get; set; } = true;

        public SupportFAQCategory Category { get; set; } = null!;

        public static SupportFAQ Create(string question, string arQuestion, string answer, string arAnswer,
            Guid categoryId, int displayOrder = 0)
        {
            return new SupportFAQ
            {
                Id = Guid.NewGuid(),
                Question = question,
                ArQuestion = arQuestion,
                Answer = answer,
                ArAnswer = arAnswer,
                CategoryId = categoryId,
                DisplayOrder = displayOrder,
                IsPublished = true,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
        }
    }
}
