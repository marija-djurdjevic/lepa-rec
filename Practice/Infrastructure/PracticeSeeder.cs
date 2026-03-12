using AngularNetBase.Practice.Entities.AffirmationValues;
using AngularNetBase.Practice.Entities.GrowthMessages;
using AngularNetBase.Practice.Entities.DistancedJournals;
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
                var value1 = new AffirmationValue(Guid.NewGuid(), "Fokus na učenje");
                value1.AddStatement(Guid.NewGuid(), "Danas biram da budem otvoren/a za nova znanja.");
                value1.AddStatement(Guid.NewGuid(), "Svaka greška je prilika da naučim nešto novo.");

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
                    new GrowthMessage(Guid.NewGuid(), "Odličan izbor! Pravi način razmišljanja je pola posla."),
                    new GrowthMessage(Guid.NewGuid(), "Samo napred, svaki trud se računa."),
                    new GrowthMessage(Guid.NewGuid(), "Spreman/na si za današnje izazove. Srećan rad!")
                );
            }

            if (!context.DistancedJournalChallenges.Any())
            {
                context.DistancedJournalChallenges.AddRange(

                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        "Prisjeti se jedne situacije danas u kojoj si osjetio/la frustraciju. Opiši šta se dogodilo.",
                        "Kako bi tu situaciju opisao/la kao neutralni posmatrač, bez osuđivanja?",
                        ChallengeLevel.Easy
                    ),

                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        "Prisjeti se trenutka danas kada si bio/la zadovoljan/na sobom.",
                        "Šta misliš da je doprinijelo tom osjećaju i kako bi to opisao/la nekome sa strane?",
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

            await context.SaveChangesAsync();
        }
    }
}
