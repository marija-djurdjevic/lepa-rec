using AngularNetBase.Practice.Dtos.GrowthMessages;
using AngularNetBase.Practice.Entities.AffirmationValues;
using AngularNetBase.Practice.Entities.GrowthMessages;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Services
{
    public class GrowthMessageService : IGrowthMessageService
    {
        private static readonly IReadOnlyDictionary<Guid, string> SkillEndMessagePrefixesSr =
            new Dictionary<Guid, string>
            {
                [Guid.Parse("a1111111-1111-1111-1111-111111111111")] = "U zadatku ste pisali o sopstvenom iskustvu u trećem licu. Istraživanja pokazuju da kada situaciju sagledavamo kao da nije naša, naš mozak je drugačije \"obrađuje\" i vidimo je objektivnije. Kroz ovu vežbu stičete sposobnost da se u svakodnevnoj interakciji distancirate od trenutnog afekta i vidite svet jasnije.",
                [Guid.Parse("a2222222-2222-2222-2222-222222222222")] = "U zadatku ste zamišljali kako će ova situacija izgledati za mesec ili godinu. Istraživanja pokazuju da nam istupanje iz perspektive ovog trenutka i posmatranje stvari sa vremenske distance daje mogućnost da prepoznamo šta je važno i šta će zaista imati uticaja na naš život dugoročno. Tako smanjujemo trenutni teret i povećavamo šansu da postupimo mudro.",
                [Guid.Parse("a3333333-3333-3333-3333-333333333333")] = "U zadatku ste sagledali situaciju iz perspektive neutralnog posmatrača. Istraživanja pokazuju da ovakav pristup problematičnoj situaciji smanjuje upliv ličnih emocija i daje vam mogućnost da stvari vidite jasnije.",
                [Guid.Parse("a4444444-4444-4444-4444-444444444444")] = "U zadatku ste se postavili u ulogu savetnika nekoga do koga vam je stalo. Istraživanja pokazuju da drugima delimo kvalitetnije savete nego sebi, jer nismo emotivno uloženi na isti način. Ovom vežbom trenirate da sebi delite bolje savete i da ih lakše prihvatate.",
                [Guid.Parse("a5555555-5555-5555-5555-555555555555")] = "U zadatku ste ispitivali šta zaista znate, šta pretpostavljate i šta ne znate. Istraživanja pokazuju da ljudi koji mogu da osete granice svog znanja bolje raspoznaju dobre argumente od loših, otvoreniji su za ispravku i bolje se snalaze u razgovoru s drugima.",
                [Guid.Parse("a6666666-6666-6666-6666-666666666666")] = "U zadatku ste razmatrali kako jedna situacija može na više načina da se razvije u zavisnosti od okolnosti i ponašanja osoba u datoj situaciji. Istraživanja pokazuju da ova svest, da ništa nije suđeno i da mogu stvari da se odviju drugačiji čak i kada su se u prošlosti odvile na određen način, omogućuju čoveku da se bolje prilagodi svetu i utiče na razvoj situacije.",
                [Guid.Parse("a7777777-7777-7777-7777-777777777777")] = "U zadatku ste svesno zamišljali kako druga osoba vidi, oseća i razume situaciju. Istraživanja pokazuju da ovakav fokusiran napor, čak i kad traje samo nekoliko minuta, smanjuje predrasude i osuđivanje i pomaže vam da se približite ljudima.",
                [Guid.Parse("a8888888-8888-8888-8888-888888888888")] = "U zadatku ste pokušali da različite i suprotstavljene tačke gledišta spojite u nešto koherentno. Niste tražili samo kompromis, već rešenje koje ozbiljno uzima u obzir šta svaka strana zaista hoće. Istraživanja pokazuju da kratke vežbe ovog tipa povećavaju otvorenost prema drugima i spremnost da pronađemo najbolja rešenja koja koriste svima.",
                [Guid.Parse("a9999999-9999-9999-9999-999999999999")] = "U zadatku ste primetili da vaše viđenje situacije nije objektivan opis stvarnosti, već interpretacija oblikovana vašim iskustvom, znanjem i trenutnim emocijama. Istraživanja pokazuju da svi imamo sklonost da naš pogled doživljavamo kao direktan uvid u \"kako stvari jesu\", dok drugima pripisujemo pristranost. Prepoznavanje okvira je korak izvan te zamke.",
            };
        private static readonly IReadOnlyDictionary<Guid, string> SkillEndMessagePrefixesEn =
            new Dictionary<Guid, string>
            {
                [Guid.Parse("a1111111-1111-1111-1111-111111111111")] = "In this exercise, you wrote about your own experience in the third person. Research shows that when we view a situation as if it were not our own, our brain processes it differently and we see it more objectively. Through this practice, you build the ability to distance yourself from immediate emotional intensity in everyday interactions and see things more clearly.",
                [Guid.Parse("a2222222-2222-2222-2222-222222222222")] = "In this exercise, you imagined how this situation might look in a month or a year. Research shows that stepping out of the present-moment perspective and observing events from a time distance helps us recognize what truly matters and what will have a real long-term impact on our life. This reduces immediate burden and increases the chance of acting wisely.",
                [Guid.Parse("a3333333-3333-3333-3333-333333333333")] = "In this exercise, you viewed the situation from the perspective of a neutral observer. Research shows this approach reduces the influence of personal emotions in difficult situations and helps you see things more clearly.",
                [Guid.Parse("a4444444-4444-4444-4444-444444444444")] = "In this exercise, you put yourself in the role of advisor to someone you care about. Research shows we often give better advice to others than to ourselves because we are not emotionally invested in the same way. This practice trains you to give yourself better advice and accept it more easily.",
                [Guid.Parse("a5555555-5555-5555-5555-555555555555")] = "In this exercise, you examined what you truly know, what you assume, and what you do not know. Research shows that people who can sense the limits of their knowledge distinguish strong arguments from weak ones more effectively, are more open to correction, and navigate conversations better.",
                [Guid.Parse("a6666666-6666-6666-6666-666666666666")] = "In this exercise, you considered how one situation can unfold in multiple ways depending on circumstances and people’s behavior. Research shows this awareness, that outcomes are not fixed and can unfold differently even when history points one way, helps people adapt better and influence how situations develop.",
                [Guid.Parse("a7777777-7777-7777-7777-777777777777")] = "In this exercise, you intentionally imagined how another person sees, feels, and understands the situation. Research shows that even a few minutes of this focused effort can reduce prejudice and judgment and help you connect with people.",
                [Guid.Parse("a8888888-8888-8888-8888-888888888888")] = "In this exercise, you tried to combine different and opposing viewpoints into something coherent. You were not looking for compromise alone, but for a solution that seriously accounts for what each side truly wants. Research shows brief practices like this increase openness to others and readiness to find better solutions that benefit everyone.",
                [Guid.Parse("a9999999-9999-9999-9999-999999999999")] = "In this exercise, you noticed that your view of the situation is not an objective description of reality, but an interpretation shaped by your experience, knowledge, and current emotions. Research shows we all tend to treat our own view as direct access to \"how things are\" while attributing bias to others. Recognizing the frame is a step beyond that trap.",
            };

        private readonly IGrowthMessageRepository _repository;
        private readonly IAffirmationValueRepository _affirmationValueRepository;

        public GrowthMessageService(
            IGrowthMessageRepository repository,
            IAffirmationValueRepository affirmationValueRepository)
        {
            _repository = repository;
            _affirmationValueRepository = affirmationValueRepository;
        }

        public async Task<Guid> CreateMessageAsync(CreateGrowthMessageDto dto, CancellationToken cancellationToken = default)
        {
            var message = new GrowthMessage(Guid.NewGuid(), dto.Text, dto.Type, true, null, dto.TextEn, dto.SkillId);

            await _repository.AddAsync(message, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return message.Id;
        }

        public async Task ToggleMessageStatusAsync(Guid id, bool activate, CancellationToken cancellationToken = default)
        {
            var message = await _repository.GetByIdAsync(id, cancellationToken)
                ?? throw new InvalidOperationException($"Growth message sa ID-jem {id} nije pronaden.");

            if (activate)
                message.Activate();
            else
                message.Deactivate();

            await _repository.UpdateAsync(message, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
        }

        public async Task<GrowthMessageDto> GetRandomMessageAsync(
            GrowthMessageType type,
            Guid? selectedStatementId = null,
            IReadOnlyCollection<Guid>? developedSkillIds = null,
            string? language = null,
            CancellationToken cancellationToken = default)
        {
            var message = await SelectMessageAsync(type, selectedStatementId, developedSkillIds, cancellationToken)
                ?? throw new InvalidOperationException("Nema aktivnih poruka ohrabrenja u bazi.");

            var localized = SelectLocalized(message.Text, message.TextEn, language);
            var composed = ComposeSkillEndMessage(type, message.SkillId, localized, language);

            return new GrowthMessageDto(message.Id, composed);
        }

        private async Task<GrowthMessage?> SelectMessageAsync(
            GrowthMessageType type,
            Guid? selectedStatementId,
            IReadOnlyCollection<Guid>? developedSkillIds,
            CancellationToken cancellationToken)
        {
            if (type == GrowthMessageType.End)
            {
                var selectedSkillMessage = await SelectMessageForDevelopedSkillsAsync(
                    developedSkillIds,
                    cancellationToken);

                if (selectedSkillMessage is not null)
                    return selectedSkillMessage;

                return await _repository.GetRandomActiveMessageAsync(type, cancellationToken);
            }

            if (type != GrowthMessageType.Begin || !selectedStatementId.HasValue || selectedStatementId.Value == Guid.Empty)
            {
                return await _repository.GetRandomActiveMessageAsync(type, cancellationToken);
            }

            var statement = await _affirmationValueRepository.GetStatementByIdAsync(selectedStatementId.Value, cancellationToken);
            if (statement is null)
            {
                return await _repository.GetRandomActiveMessageAsync(type, cancellationToken);
            }

            var preferGeneric = Random.Shared.Next(4) == 0; // 25%

            if (preferGeneric)
            {
                var generic = await _repository.GetRandomActiveMessageWithoutAffirmationValueAsync(type, cancellationToken);
                if (generic is not null)
                    return generic;

                return await _repository.GetRandomActiveMessageByAffirmationValueAsync(type, statement.AffirmationValueId, cancellationToken);
            }

            var matched = await _repository.GetRandomActiveMessageByAffirmationValueAsync(type, statement.AffirmationValueId, cancellationToken);
            if (matched is not null)
                return matched;

            return await _repository.GetRandomActiveMessageWithoutAffirmationValueAsync(type, cancellationToken);
        }

        private async Task<GrowthMessage?> SelectMessageForDevelopedSkillsAsync(
            IReadOnlyCollection<Guid>? developedSkillIds,
            CancellationToken cancellationToken)
        {
            if (developedSkillIds is null || developedSkillIds.Count == 0)
                return null;

            var distinctSkillIds = developedSkillIds
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList();

            if (distinctSkillIds.Count == 0)
                return null;

            var randomSkillId = distinctSkillIds[Random.Shared.Next(distinctSkillIds.Count)];

            return await _repository.GetRandomActiveMessageBySkillAsync(
                GrowthMessageType.End,
                randomSkillId,
                cancellationToken);
        }

        private static string SelectLocalized(string sr, string? en, string? language)
        {
            var isEnglish = !string.IsNullOrWhiteSpace(language)
                && language.StartsWith("en", StringComparison.OrdinalIgnoreCase);

            if (isEnglish && !string.IsNullOrWhiteSpace(en))
                return en;

            return sr;
        }

        private static string ComposeSkillEndMessage(
            GrowthMessageType type,
            Guid? skillId,
            string messageBody,
            string? language)
        {
            if (type != GrowthMessageType.End || !skillId.HasValue)
                return messageBody;

            var isEnglish = !string.IsNullOrWhiteSpace(language)
                && language.StartsWith("en", StringComparison.OrdinalIgnoreCase);

            var prefixes = isEnglish ? SkillEndMessagePrefixesEn : SkillEndMessagePrefixesSr;

            if (!prefixes.TryGetValue(skillId.Value, out var prefix))
                return messageBody;

            return $"{prefix} {messageBody}";
        }
    }
}
