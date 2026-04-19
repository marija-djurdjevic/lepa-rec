using AngularNetBase.Practice.Entities.DistancedJournals;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Dtos.DistancedJournals
{
public record DistancedJournalChallengeDto(
    Guid Id,
    string Content,
    string FollowUpQuestion,
    ChallengeLevel ChallengeLevel,
    Guid? SkillId);
}
