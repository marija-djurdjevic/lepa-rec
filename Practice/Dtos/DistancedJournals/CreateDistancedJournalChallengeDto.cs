using AngularNetBase.Practice.Entities.DistancedJournals;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Dtos.DistancedJournals
{
public record CreateDistancedJournalChallengeDto(
    string Content,
    string FollowUpQuestion,
    ChallengeLevel ChallengeLevel,
    Guid? SkillId = null,
    string? ContentEn = null,
    string? FollowUpQuestionEn = null);
}
