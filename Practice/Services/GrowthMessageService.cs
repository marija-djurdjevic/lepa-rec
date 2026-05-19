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
        private static readonly IReadOnlyDictionary<Guid, string> SkillEndMessagePrefixes =
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
            var composed = ComposeSkillEndMessage(type, message.SkillId, localized);

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
            string messageBody)
        {
            if (type != GrowthMessageType.End || !skillId.HasValue)
                return messageBody;

            if (!SkillEndMessagePrefixes.TryGetValue(skillId.Value, out var prefix))
                return messageBody;

            return $"{prefix} {messageBody}";
        }
    }
}
