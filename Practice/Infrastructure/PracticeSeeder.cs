using AngularNetBase.Practice.Entities.AffirmationValues;
using AngularNetBase.Practice.Entities.DistancedJournals;
using AngularNetBase.Practice.Entities.GrowthMessages;
using AngularNetBase.Practice.Entities.PerspectiveScenarios;
using AngularNetBase.Practice.Entities.Skills;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Infrastructure
{
    public static class PracticeSeeder
    {
        public static async Task SeedAsync(PracticeContext context)
        {
            if (!context.AffirmationValues.Any())
            {
                var value1 = new AffirmationValue(Guid.NewGuid(), "Fokus na ucenje");
                value1.AddStatement(Guid.NewGuid(), "Danas biram da budem otvoren/a za nova znanja.");
                value1.AddStatement(Guid.NewGuid(), "Svaka greška je prilika da naucim nešto novo.");

                var value2 = new AffirmationValue(Guid.NewGuid(), "Samopouzdanje");
                value2.AddStatement(Guid.NewGuid(), "Verujem u proces i svoj napredak.");
                value2.AddStatement(Guid.NewGuid(), "Fokusiram se na ono što mogu da kontrolišem.");

                var value3 = new AffirmationValue(Guid.NewGuid(), "Istrajnost");
                value3.AddStatement(Guid.NewGuid(), "Mali koraci danas donose velike rezultate sutra.");

                context.AffirmationValues.AddRange(value1, value2, value3);
            }

            if (!context.GrowthMessages.Any())
            {
                context.GrowthMessages.AddRange(
                    new GrowthMessage(Guid.NewGuid(), "Odlican izbor! Pravi nacin razmišljanja je pola posla."),
                    new GrowthMessage(Guid.NewGuid(), "Samo napred, svaki trud se racuna."),
                    new GrowthMessage(Guid.NewGuid(), "Spreman/na si za današnje izazove. Srecan rad!")
                );
            }

            if (!context.DistancedJournalChallenges.Any())
            {
                context.DistancedJournalChallenges.AddRange(

                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        "Prisjeti se jedne situacije danas u kojoj si osjetio/la frustraciju. Opiši šta se dogodilo.",
                        "Kako bi tu situaciju opisao/la kao neutralni posmatrac, bez osudivanja?",
                        ChallengeLevel.Easy
                    ),

                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        "Prisjeti se trenutka danas kada si bio/la zadovoljan/na sobom.",
                        "Šta misliš da je doprinijelo tom osjecaju i kako bi to opisao/la nekome sa strane?",
                        ChallengeLevel.Easy
                    ),

                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        "Razmisli o jednoj situaciji danas u kojoj si reagovao/la impulsivno.",
                        "Kako bi ta situacija izgledala iz perspektive nekoga ko te dobro poznaje?",
                        ChallengeLevel.Medium
                    ),

                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        "Prisjeti se razgovora koji ti je danas ostavio snažan utisak.",
                        "Kako bi taj razgovor opisao/la kao scena iz filma u kojoj posmatraš sebe i drugu osobu?",
                        ChallengeLevel.Medium
                    ),

                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        "Prisjeti se situacije u kojoj si osjetio/la nesigurnost.",
                        "Šta bi savjetovao/la prijatelju koji prolazi kroz istu situaciju?",
                        ChallengeLevel.Hard
                    ),

                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        "Razmisli o izazovu koji trenutno imaš u životu.",
                        "Kako bi osoba kojoj se diviš gledala na taj izazov?",
                        ChallengeLevel.Hard
                    )
                );
            }

            if (!context.Skills.Any())
            {
                var emotionRecognition = new Skill(
                    Guid.NewGuid(),
                    "Emotion Recognition",
                    "Recognizing what another person may be feeling based on context and cues.");
                emotionRecognition.AddLevel(1, "Notice", "Identify obvious emotional cues in simple social situations.");
                emotionRecognition.AddLevel(2, "Interpret", "Infer more subtle emotions from mixed cues.");

                var intentionReading = new Skill(
                    Guid.NewGuid(),
                    "Intention Reading",
                    "Inferring what another person might be trying to achieve or communicate.");
                intentionReading.AddLevel(1, "Spot Goals", "Recognize clear intentions in everyday situations.");
                intentionReading.AddLevel(2, "Read Subtext", "Infer indirect or unstated intentions.");

                var perspectiveTaking = new Skill(
                    Guid.NewGuid(),
                    "Perspective Taking",
                    "Considering how the same event may look and feel from another person’s point of view.");
                perspectiveTaking.AddLevel(1, "Shift View", "Describe another person’s likely point of view in simple scenarios.");
                perspectiveTaking.AddLevel(2, "Integrate Context", "Use context, emotions, and motives together when inferring perspective.");

                context.Skills.AddRange(emotionRecognition, intentionReading, perspectiveTaking);
                await context.SaveChangesAsync();
            }

            if (!context.PerspectiveScenarioChallenges.Any())
            {
                var emotionRecognitionSkill = context.Skills.First(x => x.Name == "Emotion Recognition");
                var intentionReadingSkill = context.Skills.First(x => x.Name == "Intention Reading");
                var perspectiveTakingSkill = context.Skills.First(x => x.Name == "Perspective Taking");

                var easyScenario = new PerspectiveScenarioChallenge(
                    Guid.NewGuid(),
                    "Lejla waves to a classmate in the hallway, but he looks away and keeps walking.",
                    "The classmate had just noticed a message that his grandfather was in the hospital and was too distracted to respond.",
                    ChallengeLevel.Easy,
                    new[]
                    {
                        (Guid.NewGuid(), emotionRecognitionSkill.Id, "How do you think the classmate was most likely feeling in that moment?")
                    });

                var mediumScenario = new PerspectiveScenarioChallenge(
                    Guid.NewGuid(),
                    "During a group project, Amar keeps checking the time and briefly interrupts others to ask how much is left to finish.",
                    "Amar was worried because he had to leave early to pick up his younger sister and did not want to let the group down.",
                    ChallengeLevel.Medium,
                    new[]
                    {
                        (Guid.NewGuid(), intentionReadingSkill.Id, "What was Amar most likely trying to accomplish by interrupting the group?"),
                        (Guid.NewGuid(), perspectiveTakingSkill.Id, "How might this situation have looked from Amar’s point of view?")
                    });

                var hardScenario = new PerspectiveScenarioChallenge(
                    Guid.NewGuid(),
                    "A friend responds with short messages all afternoon and declines an invitation to hang out, even though she was enthusiastic yesterday.",
                    "She had argued with her parents before school, felt emotionally drained, and did not have the energy to explain everything yet.",
                    ChallengeLevel.Hard,
                    new[]
                    {
                        (Guid.NewGuid(), emotionRecognitionSkill.Id, "What emotions might your friend have been experiencing?"),
                        (Guid.NewGuid(), intentionReadingSkill.Id, "Why might she have chosen short replies instead of explaining directly?"),
                        (Guid.NewGuid(), perspectiveTakingSkill.Id, "What would this afternoon likely have felt like from her perspective?")
                    });

                context.PerspectiveScenarioChallenges.AddRange(easyScenario, mediumScenario, hardScenario);
            }

            await context.SaveChangesAsync();
        }
    }
}
