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
    string? FollowUpQuestionEn = null,
    string? Theme = null,
    string? OpeningQuestion = null,
    string? ReflectionQuestion = null,
    DistancedJournalVariant Variant = DistancedJournalVariant.A,
    DistancedJournalPhase Phase = DistancedJournalPhase.Single,
    Guid? FollowUpSkillId = null,
    Guid? ReflectionSkillId = null,
    string? OpeningQuestionEn = null,
    string? ReflectionQuestionEn = null);
}
