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
            context.GrowthMessages.RemoveRange(context.GrowthMessages);
            context.AffirmationValues.RemoveRange(context.AffirmationValues);
            await context.SaveChangesAsync();

            var affirmationData = new (int LegacyId, string Name, string[] Statements)[]
            {
                (-1, "Empatija", new[]
                {
                    "Danas pokušavam da vidim svet tuđim očima.",
                }),
                (-2, "Poniznost", new[]
                {
                    "Ne moram da budem u pravu da bih bio/la vredna.",
                }),
                (-3, "Povezanost", new[]
                {
                    "Danas tražim ono što me povezuje s nekim ko je drugačiji od mene.",
                }),
                (-4, "Radoznalost", new[]
                {
                    "Danas biram da pitam umesto da pretpostavim.",
                    "Svaka osoba ima perspektivu koju ja nisam razmatrao/la.",
                    "Radoznalost prema drugima počinje jednim iskrenim pitanjem.",
                    "Danas tražim priču iza mišljenja.",
                }),
                (-5, "Autonomija", new[]
                {
                    "Biram da rastem zato što to želim, ne zato što moram.",
                    "Moj razvoj pripada meni i ja određujem ritam.",
                    "Danas radim ono što je u skladu s mojim vrednostima.",
                    "Ne moram da se složim da bih razumeo/la.",
                    "Biram da budem otvoren/a jer sam to odlučio/la, ne jer se to očekuje.",
                }),
                (-6, "Pravičnost", new[]
                {
                    "Kontekst u kome neko živi oblikuje ono što može da postigne.",
                    "Danas se pitam šta bih ja uradio/la na tuđem mestu, s tuđim resursima i ograničenjima.",
                    "Razumevanje počinje kad uvažim da su startne pozicije različite.",
                    "Svačija realnost je oblikovana prilikama koje je imao.",
                    "Danas biram da ne sudim pre nego što razumem pozadinu.",
                    "Pravičnost je kad vidim celu sliku, ne samo svoj deo.",
                }),
                (-7, "Velikodušnost", new[]
                {
                    "Danas biram da pretpostavim dobru nameru.",
                    "Danas tumačim tuđe reči blagonaklono dok ne saznam više.",
                    "Jedna velikodušna misao može da promeni ceo utisak o nekome.",
                    "Biram da vidim osobu iza postupka.",
                    "Nekad je najmudrije dati nekome drugu šansu.",
                    "Danas biram razumevanje pre reagovanja.",
                    "Lako je osuditi, a vredno je pokušati da razumem.",
                }),
                (-8, "Hrabrost", new[]
                {
                    "Danas biram da ostanem otvoren/a čak i kad je nelagodno.",
                    "Danas ne bežim od mišljenja koje me uznemirava.",
                    "Rast počinje tamo gde prestaje zona komfora.",
                    "Biram da saslušam ono što je teško čuti.",
                    "Snaga je u tome da izdržim nesigurnost bez da odustanem.",
                    "Danas se ne branim, već slušam.",
                    "Hrabrost je priznati sebi da sam možda pogrešio/la.",
                    "Jedan neugodan razgovor danas može otvoriti nova vrata sutra.",
                }),
                (-9, "Mudrost", new[]
                {
                    "Danas biram da sagledam celu sliku pre nego što zaključim.",
                    "Pre nego što odlučim, pitam se šta bih savetovao/la prijatelju.",
                    "Danas biram da razmišljam sporije i šire.",
                    "Situacija izgleda drugačije kad je pogledam iz daljine.",
                    "Danas tražim ono što mi nije očigledno na prvi pogled.",
                    "Žuran odgovor retko je mudar odgovor.",
                    "Biram da zadržim više mogućnosti otvorenim pre nego što zaključim.",
                    "Mudrost je znati da moja perspektiva nije jedina koja važi.",
                    "Danas gledam svoje probleme kao da pomažem nekom drugom.",
                }),
                (-10, "Rast", new[]
                {
                    "Mali koraci danas donose velike rezultate sutra.",
                    "Svaka greška je prilika da naučim nešto novo.",
                    "Danas vežbam veštinu koju juče nisam imao/la.",
                    "Trud danas gradi sposobnost za sutra.",
                    "Danas biram napredak, ne savršenstvo.",
                    "Ono što mi je teško sada, biće lakše s vežbom.",
                    "Fokusiram se na proces, ne na rezultat.",
                    "Svaki pokušaj me čini boljim/om nego juče.",
                    "Razvoj je niz malih odluka, ne jedan veliki skok.",
                    "Danas biram da pokušam, čak i kad nisam siguran/na.",
                }),
            };

            var affirmationIdsByLegacy = new Dictionary<int, Guid>();
            foreach (var entry in affirmationData)
            {
                var affirmationId = Guid.NewGuid();
                var value = new AffirmationValue(affirmationId, entry.Name);
                foreach (var statement in entry.Statements)
                {
                    value.AddStatement(Guid.NewGuid(), statement);
                }
                context.AffirmationValues.Add(value);
                affirmationIdsByLegacy[entry.LegacyId] = affirmationId;
            }

            var beginMessages = new (string Text, int? LegacyAffirmationId)[]
            {
                ("Vaša pažnja je najvredniji resurs koji imate. Uložite je u sledećih par minuta.", null),
                ("Danas imate priliku da vežbate nešto što većina ljudi nikad ne vežba svesno.", null),
                ("Sve što vam treba za ovaj izazov već nosite sa sobom.", null),
                ("Ono što vas čeka traži samo jednu stvar: iskrenu nameru da pokušate.", null),
                ("Svaki put kad pokušate da razumete nekoga, vaš svet postaje veći.", -1),
                ("Pre nego što počnete, pitajte se kako se druga osoba oseća.", -1),
                ("Fokusirajte se na ono što osoba oseća, ne samo na ono što je rekla.", -1),
                ("Vaš trud danas menja način na koji ćete sutra videti ljude oko sebe.", -1),
                ("Ne postoji pogrešan odgovor, već samo vaša spremnost da razmislite.", -2),
                ("Dopustite sebi da ne znate sve. Tu nastaje novo razumevanje.", -2),
                ("Jedna suprotna misao može vam dati više nego deset potvrda vaše.", -2),
                ("Spremni ste. Ne morate biti savršeni, dovoljno je da ste prisutni.", -2),
                ("Razumevanje koje sada gradite čini vaše odnose dubljim, jedan po jedan.", -3),
                ("Povezanost počinje kada pokušate da razumete drugog. To je ono što upravo radite.", -3),
                ("Predstojeća vežba tiho gradi mostove ka ljudima oko vas.", -3),
                ("Danas vežbate nešto što vas čini boljim sagovornikom, kolegom i prijateljem.", -3),
                ("Svaki pogled na svet krije nešto što niste videli. Ovo su minuti da to potražite.", -4),
                ("Jedno iskreno pitanje danas vredi više od deset pretpostavki.", -4),
                ("Ono što vam nije jasno kod drugih je mesto gde učenje počinje.", -4),
                ("Ova vežba počinje interesovanjem za ono što ne vidite na prvi pogled.", -4),
                ("Nema žurbe. Uzmite vremena koliko vam treba i budite iskreni prema sebi.", -5),
                ("Danas birate da se potrudite oko nečega što je zaista važno.", -5),
                ("Danas radite nešto retko: svesno birate da vidite šire.", -5),
                ("Iza svakog ponašanja stoji kontekst koji još ne poznajete.", -6),
                ("Ne kreću svi ljudi sa istog mesta. U toj spoznaji počinje razumevanje.", -6),
                ("Ova vežba vas poziva da vidite okolnosti, ne samo postupke.", -6),
                ("Pitajte se šta biste uradili u tuđim okolnostima, sa tuđim ograničenjima.", -6),
                ("Danas probajte da pretpostavite dobru nameru pre nego što procenite.", -7),
                ("Iza nesporazuma je češće umor ili nespretnost, a ne zloba. Krenite odatle.", -7),
                ("Svako zaslužuje šansu da bude shvaćen.", -7),
                ("Pokušaj da razumete je velikodušniji od žurbe da osudite.", -7),
                ("Nelagodnost koju osetite tokom vežbe je znak da učite.", -8),
                ("Hrabrost je ostati prisutan kad bi bilo lakše napustiti razgovor.", -8),
                ("Možda ćete primetiti nešto neprijatno o sebi. To je znak da vežba radi.", -8),
                ("Iskoristite sledećih nekoliko minuta da vidite svet malo šire.", -9),
                ("Perspektiva se širi jednim pokušajem u trenutku. Ovo je vaš trenutak.", -9),
                ("Počnite polako. Razumevanje nije trka, već praksa.", -9),
                ("Nekoliko minuta fokusa danas gradi veštinu koja traje.", -10),
                ("Čak i malo vežbanja daje velike rezultate na duže staze.", -10),
                ("Ovaj izazov je mali, ali ono što gradite njime nije.", -10),
                ("Svaki izazov koji rešite dodaje sloj razumevanja koji niste imali juče.", -10),
                ("Niko ne vidi rezultate ove vežbe odmah, ali svi ih osete vremenom.", -10),
                ("Nekoliko minuta pažnje danas vredi više nego sat rasejanog truda.", null),
                ("Ne morate imati odgovore. Dovoljno je da ste spremni da razmislite.", null),
                ("Ovo što vas čeka ne zahteva pripremu, samo iskrenost prema sebi.", null),
                ("Započnite bez očekivanja. Ono što treba da dođe, doći će kroz sam pokušaj.", null),
            };
            var endMessages = new[]
            {
                "Upravo ste uradili neÅ¡to Å¡to veÄ‡ina ljudi danas nije: svesno ste pokuÅ¡ali da vidite Å¡ire.",
                "Ovo Å¡to ste upravo zavrÅ¡ili ne daje rezultate odmah, ali ih daje sigurno.",
                "Svaki put kad proÄ‘ete kroz ovaj proces, malo lakÅ¡e razumete ljude oko sebe.",
                "MoÅ¾da ne oseÄ‡ate razliku danas, ali ona se upravo desila.",
                "Ono Å¡to ste upravo veÅ¾bali Äini vas boljim u svakom razgovoru koji vas Äeka.",
                "Malo ko danas svesno veÅ¾ba razumevanje drugih. Vi jeste.",
                "Rezultati ove veÅ¾be se ne mere brojevima, nego kvalitetom vaÅ¡ih buduÄ‡ih razgovora.",
                "Upravo ste uloÅ¾ili u veÅ¡tinu koja tiho menja sve vaÅ¡e odnose.",
                "Svaka sesija vas pomera napred, Äak i kad to ne izgleda tako.",
                "Ovo nije bio lak zadatak i baÅ¡ zato vredi Å¡to ste ga zavrÅ¡ili.",
                "Upravo ste trenirali miÅ¡iÄ‡ koji veÄ‡ina ljudi ne zna da ima.",
                "VaÅ¡ trud danas ima efekat koji Ä‡ete videti u nedeljama i mesecima koji dolaze.",
                "Ne morate savrÅ¡eno da uradite svaki izazov. Dovoljno je da ste ga proÅ¡li iskreno.",
                "Ono Å¡to gradite ovim veÅ¾bama menja kako sluÅ¡ate, kako gledate i kako razumete.",
                "Danas ste izabrali da se potrudite oko neÄeg teÅ¡kog. To zasluÅ¾uje poÅ¡tovanje.",
                "Perspektiva nije neÅ¡to Å¡to se stekne odjednom. Gradi se upravo ovako.",
                "Upravo ste zavrÅ¡ili neÅ¡to Äemu se veÄ‡ina odraslih nikad ne vraÄ‡a svesno.",
                "Razumevanje se razvija u tiÅ¡ini, ali se primeÄ‡uje u svakom odnosu.",
                "Vi ste danas veÅ¾bali. To je jedini korak koji je bio potreban.",
                "Svaka zavrÅ¡ena sesija je dokaz da vam je stalo, a to je veÄ‡ mnogo.",
            };

            foreach (var message in beginMessages)
            {
                Guid? affirmationValueId = null;
                if (message.LegacyAffirmationId.HasValue
                    && affirmationIdsByLegacy.TryGetValue(message.LegacyAffirmationId.Value, out var mappedAffirmationId))
                {
                    affirmationValueId = mappedAffirmationId;
                }

                context.GrowthMessages.Add(new GrowthMessage(
                    Guid.NewGuid(),
                    message.Text,
                    GrowthMessageType.Begin,
                    true,
                    affirmationValueId));
            }
            foreach (var message in endMessages)
            {
                context.GrowthMessages.Add(new GrowthMessage(Guid.NewGuid(), message, GrowthMessageType.End));
            }

            var requiredSkills = new (Guid Id, string Name, string Description)[]
            {
                (Guid.Parse("a1111111-1111-1111-1111-111111111111"), "Narrative distancing", "Actively shifting the linguistic frame when reflecting on one's own experience using third-person pronouns and one's own name rather than \"I\" and \"me.\""),
                (Guid.Parse("a2222222-2222-2222-2222-222222222222"), "Temporal distancing", "Projecting a current situation forward in time and reasoning from the perspective of a future self (e.g., one or five years out)."),
                (Guid.Parse("a3333333-3333-3333-3333-333333333333"), "Observer framing", "Mentally stepping outside one's own body and viewing the current situation from an external visual vantage (e.g., the \"fly on the wall\" or \"scene from across the room\" perspective). Distinct from narrative distancing (linguistic) and advisor simulation (analogical): this one is specifically a visual-spatial re-framing."),
                (Guid.Parse("a4444444-4444-4444-4444-444444444444"), "Advisor simulation", "Treating one's own situation as though it were a close friend's and generating the advice one would give to that friend. The self's situation is substituted with a structurally identical other's, which sidesteps the ego-protective reasoning that distorts self-directed judgment."),
                (Guid.Parse("a5555555-5555-5555-5555-555555555555"), "Epistemic calibration", "The ability to epistemically map what one does and does not know, held without ego-threat. Calibrated recognition that one's knowledge, beliefs, and judgments are limited and potentially wrong, paired with non-defensive openness to correction."),
                (Guid.Parse("a6666666-6666-6666-6666-666666666666"), "Prospective reasoning", "The ability to consider multiple ways an interpersonal situation may unfold. Recognition that situations, relationships, and people are not fixed. Recognition that current conflicts may resolve, current alliances may fray, and the meaning of an event shifts with time and context."),
                (Guid.Parse("a7777777-7777-7777-7777-777777777777"), "Viewpoint simulation", "Active consideration of how others involved in a situation see, feel, and reason through it."),
                (Guid.Parse("a8888888-8888-8888-8888-888888888888"), "Perspective integration", "Active effort to synthesize competing viewpoints into a coherent, higher-order understanding and a resolution that addresses the core interests behind each position, and not a mere compromise."),
            };

            foreach (var skill in requiredSkills)
            {
                if (!context.Skills.Any(x => x.Id == skill.Id))
                {
                    context.Skills.Add(new Skill(skill.Id, skill.Name, skill.Description));
                }
            }

            if (!context.DistancedJournalChallenges.Any())
            {
                var distancedJournalSkillMap = new Dictionary<int, Guid>
                {
                    [-2] = Guid.Parse("a2222222-2222-2222-2222-222222222222"),
                    [-3] = Guid.Parse("a3333333-3333-3333-3333-333333333333"),
                    [-4] = Guid.Parse("a4444444-4444-4444-4444-444444444444"),
                    [-5] = Guid.Parse("a5555555-5555-5555-5555-555555555555"),
                    [-6] = Guid.Parse("a6666666-6666-6666-6666-666666666666"),
                    [-7] = Guid.Parse("a7777777-7777-7777-7777-777777777777"),
                    [-8] = Guid.Parse("a8888888-8888-8888-8888-888888888888")
                };

                context.DistancedJournalChallenges.AddRange(
                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Setite se nedavnog razgovora koji nije prošao onako kako ste želeli. Možda se nešto izgubilo u komunikaciji, ili ste otišli sa osećajem da niste bili shvaćeni.

Opišite šta se desilo u trećem licu, kao da pišete scenu iz filma gde ste vi i druga osoba likovi. Šta je koja osoba pokušala da kaže?",
                        @"Koji je jedan trenutak u tom razgovoru gde je moglo da krene drugačije? Šta bi glavni lik koji igra vas mogao da uradi ili kaže da bi ishod bio bolji?",
                        ChallengeLevel.Easy,
                        distancedJournalSkillMap[-6]
                    ),
                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Pomislite na nekoga ko vas je nedavno iznenadio (prijatno ili neprijatno) nečim što je rekao ili uradio.

Opišite šta se desilo u trećem licu, kao da pišete scenu iz filma gde ste vi i druga osoba likovi. Šta je osoba koja igra vas očekivala pre tog trenutka, i kako se to promenilo?",
                        @"Šta mislite da je tu drugu osobu navelo da postupi baš tako? Koje okolnosti ili brige, nevidljive u tom trenutku, bi mogle objasniti njeno ponašanje?",
                        ChallengeLevel.Easy,
                        distancedJournalSkillMap[-7]
                    ),
                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Zamislite se nad zadatkom, aktivnostima ili ponašanjem koji rutinski vršite većinu dana u nedelji.

Opišite tu rutinu u trećem licu, kao da posmatrate sebe iz drugog kraja sobe. Šta osoba koja igra vas radi tokom te rutine? Šta oseća tokom tog procesa?",
                        @"U toj rutini, koji u kom trenutnku bi osoba koja gleda sa strane primetila osmeh ili nervozu na licu osobe koja igra vas?",
                        ChallengeLevel.Easy,
                        distancedJournalSkillMap[-3]
                    ),
                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Setite se trenutka iz poslednje nedelje kada ste pomogli nekome, čak i na mali način.

Opišite tu situaciju u trećem licu, kao da pričate o nekome koga poznajete. Šta je tu osobu motivisalo da pomogne? Kako se osećala pre, tokom i posle?",
                        @"Kako je, po vašem mišljenju, osoba koja je primila pomoć doživela taj trenutak? Šta je možda primetila, a šta joj je promaklo?",
                        ChallengeLevel.Easy,
                        distancedJournalSkillMap[-7]
                    ),
                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Setite se situacije kada ste nedavno osetili ponos ili zadovoljstvo sobom.

Opišite tu situaciju u trećem licu, kao da pišete kratku priču o nekome. Šta je ta osoba postigla? Šta joj je to značilo?",
                        @"Da li ta osoba dovoljno prepoznaje ono što je uradila, ili postoji nešto što umanjuje u sopstvenim očima? Šta bi nepristrasni posmatrač dodao?",
                        ChallengeLevel.Easy,
                        distancedJournalSkillMap[-5]
                    ),
                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Pomislite na nešto što ste nedavno čuli ili pročitali (vest, komentar, ili nečiju priču) što vas je emotivno pogodilo, makar i malo.

Opišite svoje iskustvo u trećem licu. Šta je tu osobu tačno pogodilo? Šta takva reakcija govori o toj osobi?",
                        @"Da li je neko drugi mogao čuti istu stvar i reagovati sasvim drugačije? Šta bi oblikovalo tu drugačiju reakciju?",
                        ChallengeLevel.Easy,
                        distancedJournalSkillMap[-7]
                    ),
                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Setite se situacije u kojoj ste nedavno morali da se prilagodite, da li zbog novog okruženja, novih ljudi, novih pravila ili bilo koje sitne promene.

Opišite to iskustvo u trećem licu. Šta je ta osoba osećala tokom prilagođavanja? Šta je bilo najteže i zbog čega?",
                        @"Kako su drugi ljudi u toj situaciji verovatno videli tu osobu dok se prilagođavala? Da li bi se njihov utisak razlikovao od onog kako se ona osećala iznutra?",
                        ChallengeLevel.Easy,
                        distancedJournalSkillMap[-3]
                    ),
                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Setite se nedavnog trenutka kada ste osetili frustraciju, razočaranje ili neslaganje sa nekim.

Opišite šta se desilo u trećem licu, kao da pišete scenu iz filma gde ste vi i druga osoba likovi. Opišite šta se desilo, šta su likovi mislili i osećali i kako su reagovali.",
                        @"Postavite se u položaj osobe koja je razlog frustracije ili razočarenja. Navedite dobar razlog zašto je ta osoba postupila kako je postupila, takav da se otkloni deo negativne emocije koju glavni lik oseća.",
                        ChallengeLevel.Medium,
                        distancedJournalSkillMap[-8]
                    ),
                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Pomislite na neku odluku koju trenutno odlažete ili oko koje se dvoumite.

Opišite tu situaciju u trećem licu, kao da posmatrate prijatelja koji se nalazi na istom raskršću. Šta ta osoba želi? Čega se plaši? Šta je drži na mestu?",
                        @"Da ta osoba za pet godina gleda unazad na ovaj trenutak, šta bi joj bilo važno, a šta bi joj delovalo beznačajno?",
                        ChallengeLevel.Medium,
                        distancedJournalSkillMap[-2]
                    ),
                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Setite se nedavne situacije u kojoj ste bili sigurni da ste u pravu, a druga osoba se nije slagala.

Opišite tu situaciju u trećem licu, kao da ne pišete o sebi već o nekome koga poznajete. Kako je ta osoba videla stvari? Zašto je bila ubeđena da je u pravu?",
                        @"Šta bi druga osoba rekla da je neko pita da ispriča svoju stranu priče? U čemu bi se njena verzija najviše razlikovala?",
                        ChallengeLevel.Medium,
                        distancedJournalSkillMap[-7]
                    ),
                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Pomislite na grupu ljudi kojoj pripadate (kolege, porodica, društvo) i na neku nedavnu dinamiku unutar te grupe koja vam je bila neprijatna ili zbunjujuća.

Opišite šta se dešavalo u trećem licu, kao da opisujete scenu iz filma. Šta je radio i osećao lik koji igra vas? Kakvu ulogu je ta osoba imala u toj dinamici?",
                        @"Kako bi neko ko tek upoznaje ovu grupu opisao tu situaciju? Šta bi mu prvo upalo u oči, a šta bi mu ostalo nevidljivo?",
                        ChallengeLevel.Medium,
                        distancedJournalSkillMap[-3]
                    ),
                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Pomislite na nekoga koga ste nedavno sreli ili sa kim ste razgovarali, a ko vas je iritirao ili vam bio nesimpatičan.

Opišite tu interakciju u trećem licu, kao neutralan izveštaj bez optužbe. Šta je druga osoba radila? Šta je osoba koja vas predstavlja mislila i zbog čega?",
                        @"Kako bi prijatelj od nesimpatične osobe, koji je dobro poznaje, objasnio njeno ponašanje?",
                        ChallengeLevel.Medium,
                        distancedJournalSkillMap[-8]
                    ),
                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Pomislite na nešto što vas u poslednje vreme opterećuje, poput brige koja se stalno vraća u misli.

Opišite vas i vašu brigu u trećem licu, kao da pratite nekoga kroz njegov dan. Kako ta briga utiče na njega? Kada je najglasnija, a kada se povlači?",
                        @"Ako bi vaš dobar prijatelj imao sličnu brigu koja ga toliko opterećuje, kako biste ga posavetovali?",
                        ChallengeLevel.Hard,
                        distancedJournalSkillMap[-4]
                    ),
                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Pomislite na osobu sa kojom ste u poslednje vreme u napetom ili hladnom odnosu.

Opišite situaciju u trećem licu, gde ste vi lik iz priče. Kako taj lik fizički doživljava tu napetost? Da li je tu više ljutnje, tuge, ili nečeg trećeg? Koja potreba glavnog lika nije ispunjena pa se tako oseća?",
                        @"Ako bi druga osoba iskreno opisala šta oseća povodom napetosti glavnog lika, šta mislite da bi rekla? Kako bi obrazložila svoju stranu priče?",
                        ChallengeLevel.Hard,
                        distancedJournalSkillMap[-7]
                    ),
                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Setite se nečega što izbegavate ili odlažete u poslednje vreme, poput razgovora, obaveze, ili odluke.

Opišite vaš slučaj u trećem licu, bez osuđivanja. Šta glavni lik izbegava? Šta misli da bi se desilo ako se suoči sa tim?",
                        @"Da li ta osoba izbegava sam ishod, ili osećanje koje bi taj ishod doneo? Koliki će uticaj na život dati ishod imati za šest meseci?",
                        ChallengeLevel.Hard,
                        distancedJournalSkillMap[-2]
                    )
                );
            }
            await context.SaveChangesAsync();

            if (!context.PerspectiveScenarioChallenges.Any())
            {
                var scenarioData = new[]
                {
                    new
                    {
                        Context = PerspectiveScenarioContext.Friends,
                        ActorCount = 4,
                        ChallengeLevel = ChallengeLevel.Hard,
                        Scenario = @"Četvoro prijatelja su na zajedničkom putovanju. Elena je organizovala detaljan plan: muzeji ujutru, ručak u rezervisanom restoranu, razgledanje popodne. Prvog dana, Katarina predlaže da preskoče muzej i odu na pijacu. Elena kaže: ""Ali već imam karte."" Dimitrije kaže: ""Možemo da se podelimo."" Elena odgovara: ""Onda nema smisla putovati zajedno."" Nada, koja je dotad ćutala, predlaže: ""Hajde u muzej do podneva pa onda na pijacu."" Elena pristaje ali je vidno napeta. Drugog dana, Katarina i Dimitrije odlaze sami na doručak bez da obaveste Elenu i Nadu. Elena je uznemirena. Nada pokušava da je smiri: ""Nisu mislili ništa loše."" Elena odgovara: ""Ti uvek braniš sve osim mene.""",
                        Questions = new[]
                        {
                            new
                            {
                                SkillId = Guid.Parse("a7777777-7777-7777-7777-777777777777"),
                                Order = 1,
                                Question = @"Šta se nalazi ispod Elenine potrebe za kontrolom plana i zašto je doživljava kao pitanje odnosa?",
                                Reveal = @"Elena je uložila dvadeset sati u planiranje. Za nju plan nije logistika, nego poklon grupi i izraz ljubavi. Kada Katarina predloži promenu, Elena ne čuje „hajde na pijacu“ već „tvoj trud ne vredi“. Ovo je duboko povezano sa njenim obrascem iz detinjstva: bila je dete koje je uvek organizovalo igre, a kada bi drugi prestali da se igraju po njenim pravilima, osećala bi se odbačena. Plan nije o muzeju, plan je o njenom mestu u grupi."
                            },
                            new
                            {
                                SkillId = Guid.Parse("a8888888-8888-8888-8888-888888888888"),
                                Order = 2,
                                Question = @"Koje potrebe Katarina i Dimitrije ispoljavaju koje Elenin plan narušava?",
                                Reveal = @"Katarina je spontana osoba koja doživljava detaljan plan kao zatvor; nije svesna koliko je rada Elena uložila, jer ona nikada ne bi planirala na taj način. „Preskoči muzej“ je za Katarinu trivijalan predlog, za Elenu egzistencijalan. Dimitrije je introvert kome je potrebno jutarnje vreme u tišini, a doručak sa Katarinom koja ne zahteva razgovor bio je njegov način da napuni bateriju, ne da isključi Elenu i Nadu. Nijedno od njih ne vidi da se njihovi trivijalni izbori kod Elene prevode u poruku „ti nisi bitna“."
                            },
                            new
                            {
                                SkillId = Guid.Parse("a5555555-5555-5555-5555-555555555555"),
                                Order = 3,
                                Question = @"Zašto je Nadina uloga mirotvorke zapravo problematična, iako izgleda konstruktivno?",
                                Reveal = @"Nada je hronični mirotvorac koji izbegava konflikt po svaku cenu. Predlog kompromisa u muzeju bio je funkcionalan, ali obrazac stalnog umirenja Elene čini da se Elena oseća kao da je njena ljutnja nelegitimna. Elenina rečenica „ti braniš sve osim mene“ otkriva godine nakupljene frustracije. Nada nikada ne validira Elenina osećanja pre nego što pokuša da ih reši. Brzo smirivanje nije podrška, to je gašenje."
                            }
                        }
                    },
                    new
                    {
                        Context = PerspectiveScenarioContext.Friends,
                        ActorCount = 3,
                        ChallengeLevel = ChallengeLevel.Hard,
                        Scenario = @"U grupnom četu troje prijatelja, Tamara šalje link na članak o mentalnom zdravlju sa komentarom: ""Ovo je baš pogodilo."" Pavle odgovara emoji-jem palca gore. Nataša piše: ""Preklinjem vas da ne pretvaramo ovo u terapijsku grupu 😂."" Tamara briše svoju poruku petnaest minuta kasnije. Sutradan, Pavle privatno piše Tamari: ""Jesi li dobro?"" Tamara odgovara: ""Da, sve super.""",
                        Questions = new[]
                        {
                            new
                            {
                                SkillId = Guid.Parse("a7777777-7777-7777-7777-777777777777"),
                                Order = 1,
                                Question = @"Šta je Tamara htela da postigne slanjem linka i kako je protumačila reakcije?",
                                Reveal = @"Tamara prolazi kroz period teškoće. Link je bio njen način da indirektno otvori temu. Nije bila spremna da kaže „loše mi je“, ali je htela da vidi da li će neko prepoznati signal. Natašinu šalu je čula kao: „Tvoja osećanja su pretežak teret za ovo društvo.“ Brisanjem poruke povukla se da ne ispadne ona koja pokušava da pretvori grupu u terapijsku."
                            },
                            new
                            {
                                SkillId = Guid.Parse("a7777777-7777-7777-7777-777777777777"),
                                Order = 2,
                                Question = @"Šta stoji iza Natašine šale i šta ona ne shvata o njenom efektu?",
                                Reveal = @"Nataša humor koristi kao vlastiti odbrambeni mehanizam. I sama se bori sa anksioznošću, ali je naučila da distancira emocije kroz šalu. Njen komentar je bio o njenoj sopstvenoj nelagodnosti sa temom, ne o Tamari, ali efekat je bio isti. Ne shvata da je u tom trenutku postavila grupnu normu: ovde se o tome ne priča."
                            },
                            new
                            {
                                SkillId = Guid.Parse("a8888888-8888-8888-8888-888888888888"),
                                Order = 3,
                                Question = @"Šta Pavlova privatna poruka otkriva o njegovom razumevanju situacije?",
                                Reveal = @"Pavle je jedini koji je pročitao situaciju ispravno i prepoznao je da brisanje poruke znači povlačenje. Međutim, njegov pristup je nepotpun: pitanjem u privatnoj poruci potvrdio je Tamari da je tema „za šapat“, umesto da u grupi normalizuje njen post i time pošalje signal da je takav razgovor dobrodošao. Dobra namera, pogrešan kanal."
                            }
                        }
                    },
                    new
                    {
                        Context = PerspectiveScenarioContext.Family,
                        ActorCount = 4,
                        ChallengeLevel = ChallengeLevel.Hard,
                        Scenario = @"Branko i Sanja ugošćuju Brankovog brata Gorana i njegovu suprugu Vesnu za večeru. Tokom razgovora, Goran brzo i bez pauze priča o skupom renoviranju kuće. Branko čestita, ali Sanja postaje tiha. Kasnije, Vesna pominje da su upisali decu u privatnu školu. Sanja odlazi u kuhinju ""da proveri desert."" Branko je prati i šapuće: ""Možeš li bar da se pretvaraš?"" Sanja odgovara: ""Ja se pretvaramo celo veče."" Vraćaju se za sto sa osmehom.",
                        Questions = new[]
                        {
                            new
                            {
                                SkillId = Guid.Parse("a5555555-5555-5555-5555-555555555555"),
                                Order = 1,
                                Question = @"Šta konkretno uzrokuje Sanjinu reakciju, da li je zavist, bol, ili nešto treće?",
                                Reveal = @"Sanja i Branko su prošle godine imali ozbiljnu finansijsku krizu. Brankov biznis je bio pred zatvaranjem, morali su da prodaju vikendicu i da se odreknu planiranog putovanja. Kada za stolom čuje o skupom renoviranju i privatnim školama, Sanja ne oseća zavist prema Goranu i Vesni. Rana od sopstvenog gubitka je još sveža, i ono što izgleda kao povučenost je zapravo bol koji se aktivira kad se slavi ono što su oni sami nedavno izgubili."
                            },
                            new
                            {
                                SkillId = Guid.Parse("a8888888-8888-8888-8888-888888888888"),
                                Order = 2,
                                Question = @"Šta Branko ne razume o Sanjinoj perspektivi kada joj kaže da se pretvara?",
                                Reveal = @"Branko misli da ga Sanja ne podržava pred bratom. Ne vidi da njena frustracija nije usmerena ka Goranu i Vesni nego ka njemu. On od nje traži da učestvuje u performansu koji ona smatra nedostojnim. Ne čuje da ona ne odbija da bude ljubazna gošća, nego odbija da pretvara njihov brak u scenu u kojoj se njihova stvarnost skriva."
                            },
                            new
                            {
                                SkillId = Guid.Parse("a7777777-7777-7777-7777-777777777777"),
                                Order = 3,
                                Question = @"Kako Goran i Vesna verovatno tumače atmosferu na večeri?",
                                Reveal = @"Goran o renoviranju priča jer je nervozan. Zna za bratovu finansijsku situaciju i popunjava tišinu temama na kojima se oseća sigurno, ne iz hvalisanja. Vesna pominje privatnu školu jer joj je to jedina tema o kojoj trenutno razmišlja, ne shvatajući kako zvuči u datom kontekstu. Oboje verovatno osećaju da je atmosfera nekako čudna, ali je pripisuju umoru ili pojedinačnom raspoloženju, ne shvatajući da cela večera počiva na nedorečenim stvarima."
                            }
                        }
                    },
                    new
                    {
                        Context = PerspectiveScenarioContext.Friends,
                        ActorCount = 3,
                        ChallengeLevel = ChallengeLevel.Hard,
                        Scenario = @"Dušan, Marija i Nemanja su zajedno počeli da treniraju u teretani pre šest meseci. Dušan je brzo napredovao i počeo da daje savete Mariji i Nemanji bez da su tražili. ""Trebalo bi da promeniš formu,"" ""Jedi više proteina."" Marija je počela da dolazi u drugom terminu kada ima manje ljudi. Nemanja dolazi u isto vreme ali stavlja slušalice čim uđe. Dušan je zbunjen i žali se zajedničkom prijatelju: ""Pomagao sam im a oni me izbegavaju.""",
                        Questions = new[]
                        {
                            new
                            {
                                SkillId = Guid.Parse("a7777777-7777-7777-7777-777777777777"),
                                Order = 1,
                                Question = @"Zašto Marija ima potrebu da skroz promeni termin?",
                                Reveal = @"Marija se ceo život bori sa telesnom slikom. Teretana je za nju bila veliki korak i mesto gde je htela da se oseća sigurno, ne procenjivano. Dušanovi saveti, koliko god dobronamerni, aktivirali su njen osećaj da je nešto pogrešno sa njom. Premeštanje u drugi termin nije poruka Dušanu, već zaštita teretane kao bezbednog prostora."
                            },
                            new
                            {
                                SkillId = Guid.Parse("a7777777-7777-7777-7777-777777777777"),
                                Order = 2,
                                Question = @"Šta je kod Nemanje drugačije da, bez promene termina, može da se izbori sa Dušanom?",
                                Reveal = @"Nemanja nema problem sa samopouzdanjem. Njemu smeta kontrola, ne sadržaj saveta. Ceni autonomiju i ne voli kada mu neko govori šta da radi, čak i kad je savet korektan. Slušalice su mu način da postavi granicu bez konflikta. Ne mora da beži, dovoljno je da ne pusti Dušana unutra."
                            },
                            new
                            {
                                SkillId = Guid.Parse("a7777777-7777-7777-7777-777777777777"),
                                Order = 3,
                                Question = @"Zašto Dušan ima potrebu da deli savete?",
                                Reveal = @"Dušan je čovek čiji je primarni jezik ljubavi davanje saveta. U njegovoj porodici briga se pokazivala kroz ispravljanje i usmeravanje i to je ono što on prepoznaje kao izraz bliskosti. Iskreno ne razume da njegovi saveti, iako tačni, nisu traženi i da se primaju kao kritika umesto kao podrška."
                            }
                        }
                    },
                    new
                    {
                        Context = PerspectiveScenarioContext.Work,
                        ActorCount = 3,
                        ChallengeLevel = ChallengeLevel.Medium,
                        Scenario = @"Goran objavljuje da je Viktor dobio unapređenje. Tijana, koja je u firmi dve godine duže od Viktora, čestita Viktoru kratko i odlazi na pauzu. Kolege primećuju da je Tijana ostatak dana bila šutljiva. Viktor prilazi Tijani i kaže: ""Nadam se da ti je okej, ti si me svemu naučila."" Tijana odgovara: ""Zaslužio si, svaka čast."" Sutradan Tijana šalje mejl HR odeljenju sa pitanjem o internim procedurama za unapređenje.",
                        Questions = new[]
                        {
                            new
                            {
                                SkillId = Guid.Parse("a7777777-7777-7777-7777-777777777777"),
                                Order = 1,
                                Question = @"Šta Tijana zapravo oseća uprkos onome što govori?",
                                Reveal = @"Tijana ne sumnja u Viktorove sposobnosti. Ono što je boli je sistemsko: dva puta je bila predložena za unapređenje i oba puta joj je rečeno ""sledeći ciklus."" Njen mejl HR-u nije osveta. To je pokušaj da razume pravila igre koja joj se čine netransparentna. Tiho sumnja da je Viktor napredovao brže jer je društveniji, češće igra basket sa Goranom i vidljiviji je na sastancima. Tijana je osoba koja radi dubinski posao koji je manje vidljiv."
                            },
                            new
                            {
                                SkillId = Guid.Parse("a8888888-8888-8888-8888-888888888888"),
                                Order = 2,
                                Question = @"Šta Viktor ne vidi u dinamici između sebe i ostalih učesnika?",
                                Reveal = @"Viktor iskreno misli da je Tijana srećna zbog njega jer ga je zaista mentorski podržavala. Ne vidi da je upravo ta mentorska dinamika ono što čini situaciju bolnom. Viktor koristi privilegiju bliskosti sa menadžerom a da toga nije svestan."
                            }
                        }
                    },
                    new
                    {
                        Context = PerspectiveScenarioContext.Family,
                        ActorCount = 2,
                        ChallengeLevel = ChallengeLevel.Medium,
                        Scenario = @"Bračni par Aleksa i Mina razgovaraju o planovima za Novu godinu. Mina predlaže da ove godine proslave sami kod kuće. Aleksa oklevajući kaže: ""Hajde da pitamo moju mamu da li da dođe kod nas, ona je sama otkad je tata umro."" Mina odgovara: ""Svake godine isto. Nikada nismo sami."" Aleksa povišenim tonom: ""Šta, da je ostavim samu na Novu godinu?"" Razgovor se završava ćutanjem.",
                        Questions = new[]
                        {
                            new
                            {
                                SkillId = Guid.Parse("a7777777-7777-7777-7777-777777777777"),
                                Order = 1,
                                Question = @"Šta Mina zapravo želi da kaže ali ne uspeva da artikuliše?",
                                Reveal = @"Mina ne mrzi Aleksinu mamu niti želi da je isključi. Problem je dublji: u četiri godine braka, Mina i Aleksa nijedan praznik nisu proveli kao par bez proširene porodice. Mina oseća da njihov brak nema prostora da izgradi sopstvene rituale i identitet. Njen zahtev ""da budemo sami"" je zapravo zahtev: ""Da li mi kao par postojimo nezavisno?"""
                            },
                            new
                            {
                                SkillId = Guid.Parse("a7777777-7777-7777-7777-777777777777"),
                                Order = 2,
                                Question = @"Šta sprečava Aleksu da uvaži Minin stav i pomogne joj da artikuliše potrebu koju ima?",
                                Reveal = @"Aleksa je zarobljen u ulozi sina koji brine o majci. Ovu ulogu je preuzeo kad mu je otac umro pre tri godine. On interpretira Minin zahtev za odvojenošću kao napad na majku i povezana emotivna reakcija ga sprečava da uvaži Minu."
                            }
                        }
                    },
                    new
                    {
                        Context = PerspectiveScenarioContext.Friends,
                        ActorCount = 2,
                        ChallengeLevel = ChallengeLevel.Medium,
                        Scenario = @"Ivana je organizovala proslavu rođendana u restoranu i pozvala dvadeset prijatelja. Milica, njena najbliža prijateljica, koja je u poslednje vreme sve ređe izlazila, potvrdila je dolazak ali se nije pojavila. Nije poslala poruku tokom večeri. Sutradan je napisala: ""Izvini, nisam mogla."" Ivana joj nije odgovorila.",
                        Questions = new[]
                        {
                            new
                            {
                                SkillId = Guid.Parse("a7777777-7777-7777-7777-777777777777"),
                                Order = 1,
                                Question = @"Šta Ivana verovatno pretpostavlja o Milicinom izostanku?",
                                Reveal = @"Ivana verovatno tumači izostanak kao znak da joj Milica ne poklanja važnost koju je očekivala od najbolje prijateljice. Poruku „nisam mogla“ bez objašnjenja čita kao odbacivanje bez poštovanja. Pretpostavlja da je u pitanju bio izbor, a ne nemogućnost."
                            },
                            new
                            {
                                SkillId = Guid.Parse("a5555555-5555-5555-5555-555555555555"),
                                Order = 2,
                                Question = @"Šta bi moglo da stoji iza reči „nisam mogla“, što bi opravdalo Miličin izostanak?",
                                Reveal = @"Milica pati od socijalne anksioznosti koja se pojačala poslednjih meseci. Ispred restorana je stajala petnaest minuta ali nije mogla da uđe, srce joj je lupalo, ruke su se tresle. Otišla je kući osećajući se poniženo. Poruku ""nisam mogla"" je napisala bukvalno. Nije bila u stanju, ne da nije htela. Ne priča o anksioznosti jer se plaši da će je prijatelji smatrati čudnom."
                            }
                        }
                    },
                    new
                    {
                        Context = PerspectiveScenarioContext.Friends,
                        ActorCount = 2,
                        ChallengeLevel = ChallengeLevel.Medium,
                        Scenario = @"Vuk i Sara su na večeri u restoranu. Sara priča o svom nedavnom putovanju u Grčku, opisuje plaže, hranu i ljude koje je upoznala. Vuk pokazuje iskreno interesovanje prema Sari i priči. On sluša i klima glavom, a posle par minuta mu pogled kreće lutati po restoranu. Vraća se u priču, pa zatim gleda jelovnik. Kad Sara završi, Vuk menja temu na fudbal. Sara kaže: ""Zanimljivo"" i otvara telefon.",
                        Questions = new[]
                        {
                            new
                            {
                                SkillId = Guid.Parse("a7777777-7777-7777-7777-777777777777"),
                                Order = 1,
                                Question = @"Šta Sara verovatno oseća posle Vukove reakcije?",
                                Reveal = @"Sara se oseća nevidljivo i odbijeno. Odlazak ka telefonu nije dosada nego tiho povlačenje. Vukovo ćutanje i nagla promena teme tumači kao signal da njen unutrašnji svet nije zanimljiv, ne samo putovanje nego ni ona sama. Njeno „zanimljivo“ je istovremeno učtiv zatvor i mala kazna."
                            },
                            new
                            {
                                SkillId = Guid.Parse("a7777777-7777-7777-7777-777777777777"),
                                Order = 2,
                                Question = @"Šta sprečava Vuka, iako se trudi da sluša, da se angažuje sa Sarinom pričom?",
                                Reveal = @"Vuk nije nezainteresovan za Sarin život. On pati od poremećaja pažnje (ADD) koji mu otežava praćenje dugih narativnih priča. Dok je Sara pričala, on se iskreno trudio da sluša, ali mu je pažnja stalno klizila. Promenio je temu ne zato što ga put nije zanimao, već jer je osećao anksioznost zbog toga što ne može da prati. Fudbal je tema koja zahteva kratke razmene, gde se oseća kompetentnim u razgovoru."
                            }
                        }
                    },
                    new
                    {
                        Context = PerspectiveScenarioContext.Work,
                        ActorCount = 1,
                        ChallengeLevel = ChallengeLevel.Easy,
                        Scenario = @"Tamara rukovodi timom od šest ljudi. Na prvom sastanku je bila kratka, govorila je isključivo o zadacima i rokovima, nije pitala nikoga kako se oseća niti se predstavila lično. Kada je jedan kolega pokušao da napravi šalu, Tamara se nasmešila ali je odmah nastavila dalje. Posle sastanka, članovi tima su komentarisali da je ""hladna"" i ""roboti imaju više emocija.""",
                        Questions = new[]
                        {
                            new
                            {
                                SkillId = Guid.Parse("a7777777-7777-7777-7777-777777777777"),
                                Order = 1,
                                Question = @"Zašto je Tamara tako pristupila prvom sastanku?",
                                Reveal = @"Tamara je introvert koja je dobila jasnu povratnu informaciju od svog prethodnog šefa: ""Previše se opuštaš sa timom i gubiš autoritet."" Odlučila je da na novom mestu postavi jasne profesionalne granice od početka. Kod kuće je bila nervozna celu noć pre sastanka i vežbala šta će reći. Njena krutost nije ravnodušnost. To je pažljivo isplanirana strategija osobe koja se plaši da ne pogreši."
                            }
                        }
                    },
                    new
                    {
                        Context = PerspectiveScenarioContext.Family,
                        ActorCount = 2,
                        ChallengeLevel = ChallengeLevel.Easy,
                        Scenario = @"Za Gocin rođendan, njena ćerka Maja je kupila skupu kremu za lice koju je videla u reklami. Kada je Goca otvorila poklon, kratko je rekla ""Hvala"" i odložila kutiju u stranu. Nastavila je da razgovara sa drugim gostima. Maja je primetila da Goca danima kasnije nije otvorila kutiju.",
                        Questions = new[]
                        {
                            new
                            {
                                SkillId = Guid.Parse("a7777777-7777-7777-7777-777777777777"),
                                Order = 1,
                                Question = @"Šta Gordana verovatno oseća u vezi sa poklonom?",
                                Reveal = @"Goca pripada generaciji koja smatra da su skupe kozmetičke kreme nepotrebna rasipnost. Ceo život koristi domaće preparate i ponosna je na to. Poklon je protumačila kao suptilnu kritiku njenog izgleda, kao da joj ćerka poručuje da joj je potrebna pomoć da izgleda mlađe. Nije bila nezahvalna, već povređena."
                            }
                        }
                    },
                    new
                    {
                        Context = PerspectiveScenarioContext.Work,
                        ActorCount = 2,
                        ChallengeLevel = ChallengeLevel.Easy,
                        Scenario = @"Dejan razgovara na telefon u zajedničkom radnom prostoru. Po običaju govori glasno i živo, smeje se i gestikulira. Ana, koja sedi dva stola dalje, stavlja slušalice i pomera se bliže prozoru. Kada je Dejan završio poziv i pitao Anu nešto u vezi projekta, odgovorila mu je hladno u jednoj rečenici, bez kontakta očima. Dejan je zbunjen ovom reakcijom.",
                        Questions = new[]
                        {
                            new
                            {
                                SkillId = Guid.Parse("a7777777-7777-7777-7777-777777777777"),
                                Order = 1,
                                Question = @"Zašto Dejan ne primećuje negativan uticaj ovog ponašanja na okolinu?",
                                Reveal = @"Dejan je odrastao u velikoj i glasnoj porodici gde je visok nivo buke bio znak topline i bliskosti. Njegova glasnoća nije svesna odluka. To je njegov podrazumevani način komunikacije koji doživljava kao prirodan. Nikada mu niko na poslu nije rekao da je preglasan jer svi pretpostavljaju da on to zna i da mu je svejedno."
                            }
                        }
                    },
                    new
                    {
                        Context = PerspectiveScenarioContext.Family,
                        ActorCount = 2,
                        ChallengeLevel = ChallengeLevel.Easy,
                        Scenario = @"Petar dolazi kod roditelja na nedeljni ručak. Za stolom priča o tome kako razmišlja da promeni posao i upiše kurs programiranja. Otac Dragan ga prekida: ""Ti imaš stabilan posao, nemoj da budeš dete."" Petar ućuti, a ostatak ručka prolazi u tišini.",
                        Questions = new[]
                        {
                            new
                            {
                                SkillId = Guid.Parse("a7777777-7777-7777-7777-777777777777"),
                                Order = 1,
                                Question = @"Šta Dragan zapravo oseća ispod svoje oštre reakcije?",
                                Reveal = @"Kada čuje da Petar želi da napusti siguran posao, kod Dragana se aktivira duboki strah za sina. Njegov oštar ton nije prezir prema Petrovim ambicijama, već panika prerušena u autoritet."
                            },
                            new
                            {
                                SkillId = Guid.Parse("a7777777-7777-7777-7777-777777777777"),
                                Order = 2,
                                Question = @"Šta je Dragan proživeo da mu ovakva ideja izaziva to osećanje?",
                                Reveal = @"Dragan je ceo radni vek proveo na istom radnom mestu jer nikada nije imao mogućnost izbora. Preživeo je dva talasa otpuštanja i od tada živi u stalnom strahu od finansijske nesigurnosti. Za njega, stabilan posao nije kompromis, nego preživljavanje."
                            }
                        }
                    }
                };


                var challenges = scenarioData.Select(scenario =>
                {
                    var questions = scenario.Questions
                        .Select(q => (Guid.NewGuid(), q.SkillId, q.Order, q.Question, q.Reveal))
                        .ToList();

                    return new PerspectiveScenarioChallenge(
                        Guid.NewGuid(),
                        scenario.Context,
                        scenario.ActorCount,
                        scenario.Scenario,
                        scenario.ChallengeLevel,
                        questions);
                }).ToList();

                context.PerspectiveScenarioChallenges.AddRange(challenges);
            }

            await context.SaveChangesAsync();
        }
    }
}



