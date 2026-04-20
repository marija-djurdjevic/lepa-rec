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

            var affirmationData = new (int LegacyId, string Name, (string Sr, string En)[] Statements)[]
            {
                (-1, "Empatija", new[]
                {
                    ("Danas pokušavam da vidim svet tuđim očima.", "Today I try to see the world through someone else's eyes."),
                }),
                (-2, "Poniznost", new[]
                {
                    ("Ne moram da budem u pravu da bih bio/la vredna.", "I do not have to be right to be worthy."),
                }),
                (-3, "Povezanost", new[]
                {
                    ("Danas tražim ono što me povezuje s nekim ko je drugačiji od mene.", "Today I look for what connects me with someone who is different from me."),
                }),
                (-4, "Radoznalost", new[]
                {
                    ("Danas biram da pitam umesto da pretpostavim.", "Today I choose to ask instead of assume."),
                    ("Svaka osoba ima perspektivu koju ja nisam razmatrao/la.", "Every person has a perspective I have not considered."),
                    ("Radoznalost prema drugima počinje jednim iskrenim pitanjem.", "Curiosity about others starts with one sincere question."),
                    ("Danas tražim priču iza mišljenja.", "Today I look for the story behind the opinion."),
                }),
                (-5, "Autonomija", new[]
                {
                    ("Biram da rastem zato što to želim, ne zato što moram.", "I choose to grow because I want to, not because I must."),
                    ("Moj razvoj pripada meni i ja određujem ritam.", "My development belongs to me, and I set the pace."),
                    ("Danas radim ono što je u skladu s mojim vrednostima.", "Today I do what aligns with my values."),
                    ("Ne moram da se složim da bih razumeo/la.", "I do not have to agree in order to understand."),
                    ("Biram da budem otvoren/a jer sam to odlučio/la, ne jer se to očekuje.", "I choose to stay open because I decided to, not because it is expected."),
                }),
                (-6, "Pravičnost", new[]
                {
                    ("Kontekst u kome neko živi oblikuje ono što može da postigne.", "The context someone lives in shapes what they can achieve."),
                    ("Danas se pitam šta bih ja uradio/la na tuđem mestu, s tuđim resursima i ograničenjima.", "Today I ask what I would do in someone else's position, with their resources and limits."),
                    ("Razumevanje počinje kad uvažim da su startne pozicije različite.", "Understanding begins when I acknowledge that starting points are different."),
                    ("Svačija realnost je oblikovana prilikama koje je imao.", "Everyone's reality is shaped by the opportunities they had."),
                    ("Danas biram da ne sudim pre nego što razumem pozadinu.", "Today I choose not to judge before I understand the background."),
                    ("Pravičnost je kad vidim celu sliku, ne samo svoj deo.", "Fairness is seeing the whole picture, not only my part."),
                }),
                (-7, "Velikodušnost", new[]
                {
                    ("Danas biram da pretpostavim dobru nameru.", "Today I choose to assume good intent."),
                    ("Danas tumačim tuđe reči blagonaklono dok ne saznam više.", "Today I interpret others' words kindly until I know more."),
                    ("Jedna velikodušna misao može da promeni ceo utisak o nekome.", "One generous thought can change my whole impression of someone."),
                    ("Biram da vidim osobu iza postupka.", "I choose to see the person behind the action."),
                    ("Nekad je najmudrije dati nekome drugu šansu.", "Sometimes the wisest choice is to give someone a second chance."),
                    ("Danas biram razumevanje pre reagovanja.", "Today I choose understanding before reacting."),
                    ("Lako je osuditi, a vredno je pokušati da razumem.", "It is easy to judge, but worth trying to understand."),
                }),
                (-8, "Hrabrost", new[]
                {
                    ("Danas biram da ostanem otvoren/a čak i kad je nelagodno.", "Today I choose to stay open even when it feels uncomfortable."),
                    ("Danas ne bežim od mišljenja koje me uznemirava.", "Today I do not run from opinions that unsettle me."),
                    ("Rast počinje tamo gde prestaje zona komfora.", "Growth begins where the comfort zone ends."),
                    ("Biram da saslušam ono što je teško čuti.", "I choose to hear what is hard to hear."),
                    ("Snaga je u tome da izdržim nesigurnost bez da odustanem.", "Strength is tolerating uncertainty without giving up."),
                    ("Danas se ne branim, već slušam.", "Today I do not defend myself; I listen."),
                    ("Hrabrost je priznati sebi da sam možda pogrešio/la.", "Courage is admitting to myself that I might be wrong."),
                    ("Jedan neugodan razgovor danas može otvoriti nova vrata sutra.", "One uncomfortable conversation today can open new doors tomorrow."),
                }),
                (-9, "Mudrost", new[]
                {
                    ("Danas biram da sagledam celu sliku pre nego što zaključim.", "Today I choose to see the whole picture before I conclude."),
                    ("Pre nego što odlučim, pitam se šta bih savetovao/la prijatelju.", "Before I decide, I ask what advice I would give a friend."),
                    ("Danas biram da razmišljam sporije i šire.", "Today I choose to think more slowly and more broadly."),
                    ("Situacija izgleda drugačije kad je pogledam iz daljine.", "A situation looks different when I view it from a distance."),
                    ("Danas tražim ono što mi nije očigledno na prvi pogled.", "Today I look for what is not obvious at first glance."),
                    ("Žuran odgovor retko je mudar odgovor.", "A rushed answer is rarely a wise answer."),
                    ("Biram da zadržim više mogućnosti otvorenim pre nego što zaključim.", "I choose to keep multiple possibilities open before I conclude."),
                    ("Mudrost je znati da moja perspektiva nije jedina koja važi.", "Wisdom is knowing my perspective is not the only valid one."),
                    ("Danas gledam svoje probleme kao da pomažem nekom drugom.", "Today I look at my problems as if I were helping someone else."),
                }),
                (-10, "Rast", new[]
                {
                    ("Mali koraci danas donose velike rezultate sutra.", "Small steps today bring big results tomorrow."),
                    ("Svaka greška je prilika da naučim nešto novo.", "Every mistake is a chance to learn something new."),
                    ("Danas vežbam veštinu koju juče nisam imao/la.", "Today I practice a skill I did not have yesterday."),
                    ("Trud danas gradi sposobnost za sutra.", "Effort today builds ability for tomorrow."),
                    ("Danas biram napredak, ne savršenstvo.", "Today I choose progress, not perfection."),
                    ("Ono što mi je teško sada, biće lakše s vežbom.", "What is hard for me now will become easier with practice."),
                    ("Fokusiram se na proces, ne na rezultat.", "I focus on the process, not the result."),
                    ("Svaki pokušaj me čini boljim/om nego juče.", "Every attempt makes me better than yesterday."),
                    ("Razvoj je niz malih odluka, ne jedan veliki skok.", "Growth is a series of small decisions, not one giant leap."),
                    ("Danas biram da pokušam, čak i kad nisam siguran/na.", "Today I choose to try, even when I am not sure."),
                }),
            };

            var affirmationIdsByLegacy = new Dictionary<int, Guid>();
            foreach (var entry in affirmationData)
            {
                var affirmationId = Guid.NewGuid();
                var value = new AffirmationValue(affirmationId, entry.Name);
                foreach (var statement in entry.Statements)
                {
                    value.AddStatement(Guid.NewGuid(), statement.Sr, textEn: statement.En);
                }
                context.AffirmationValues.Add(value);
                affirmationIdsByLegacy[entry.LegacyId] = affirmationId;
            }

            var beginMessages = new (string Text, string TextEn, int? LegacyAffirmationId)[]
            {
                ("Vaša pažnja je najvredniji resurs koji imate. Uložite je u sledećih par minuta.", "Your attention is the most valuable resource you have. Invest it in the next few minutes.", null),
                ("Danas imate priliku da vežbate nešto što većina ljudi nikad ne vežba svesno.", "Today you have the chance to practice something most people never practice consciously.", null),
                ("Sve što vam treba za ovaj izazov već nosite sa sobom.", "You already carry everything you need for this challenge.", null),
                ("Ono što vas čeka traži samo jednu stvar: iskrenu nameru da pokušate.", "What awaits you asks only one thing: a sincere intention to try.", null),
                ("Svaki put kad pokušate da razumete nekoga, vaš svet postaje veći.", "Every time you try to understand someone, your world gets bigger.", -1),
                ("Pre nego što počnete, pitajte se kako se druga osoba oseća.", "Before you begin, ask yourself how the other person feels.", -1),
                ("Fokusirajte se na ono što osoba oseća, ne samo na ono što je rekla.", "Focus on what the person feels, not only on what they said.", -1),
                ("Vaš trud danas menja način na koji ćete sutra videti ljude oko sebe.", "Your effort today changes how you will see people around you tomorrow.", -1),
                ("Ne postoji pogrešan odgovor, već samo vaša spremnost da razmislite.", "There is no wrong answer, only your willingness to reflect.", -2),
                ("Dopustite sebi da ne znate sve. Tu nastaje novo razumevanje.", "Allow yourself not to know everything. That is where new understanding begins.", -2),
                ("Jedna suprotna misao može vam dati više nego deset potvrda vaše.", "One opposing thought can give you more than ten confirmations of your own.", -2),
                ("Spremni ste. Ne morate biti savršeni, dovoljno je da ste prisutni.", "You are ready. You do not have to be perfect; being present is enough.", -2),
                ("Razumevanje koje sada gradite čini vaše odnose dubljim, jedan po jedan.", "The understanding you are building now makes your relationships deeper, one by one.", -3),
                ("Povezanost počinje kada pokušate da razumete drugog. To je ono što upravo radite.", "Connection begins when you try to understand someone else. That is exactly what you are doing now.", -3),
                ("Predstojeća vežba tiho gradi mostove ka ljudima oko vas.", "The upcoming exercise quietly builds bridges to the people around you.", -3),
                ("Danas vežbate nešto što vas čini boljim sagovornikom, kolegom i prijateljem.", "Today you are practicing something that makes you a better conversation partner, colleague, and friend.", -3),
                ("Svaki pogled na svet krije nešto što niste videli. Ovo su minuti da to potražite.", "Every worldview hides something you have not seen. These are the minutes to look for it.", -4),
                ("Jedno iskreno pitanje danas vredi više od deset pretpostavki.", "One sincere question today is worth more than ten assumptions.", -4),
                ("Ono što vam nije jasno kod drugih je mesto gde učenje počinje.", "What is unclear to you about others is where learning begins.", -4),
                ("Ova vežba počinje interesovanjem za ono što ne vidite na prvi pogled.", "This exercise begins with curiosity about what you do not see at first glance.", -4),
                ("Nema žurbe. Uzmite vremena koliko vam treba i budite iskreni prema sebi.", "There is no rush. Take as much time as you need and be honest with yourself.", -5),
                ("Danas birate da se potrudite oko nečega što je zaista važno.", "Today you are choosing to put effort into something that truly matters.", -5),
                ("Danas radite nešto retko: svesno birate da vidite šire.", "Today you are doing something rare: consciously choosing to see more broadly.", -5),
                ("Iza svakog ponašanja stoji kontekst koji još ne poznajete.", "Behind every behavior there is context you do not yet know.", -6),
                ("Ne kreću svi ljudi sa istog mesta. U toj spoznaji počinje razumevanje.", "Not everyone starts from the same place. Understanding begins with that realization.", -6),
                ("Ova vežba vas poziva da vidite okolnosti, ne samo postupke.", "This exercise invites you to see circumstances, not just actions.", -6),
                ("Pitajte se šta biste uradili u tuđim okolnostima, sa tuđim ograničenjima.", "Ask yourself what you would do in someone else's circumstances, with someone else's limitations.", -6),
                ("Danas probajte da pretpostavite dobru nameru pre nego što procenite.", "Today, try to assume good intent before you judge.", -7),
                ("Iza nesporazuma je češće umor ili nespretnost, a ne zloba. Krenite odatle.", "Behind misunderstandings there is more often fatigue or awkwardness than malice. Start from there.", -7),
                ("Svako zaslužuje šansu da bude shvaćen.", "Everyone deserves a chance to be understood.", -7),
                ("Pokušaj da razumete je velikodušniji od žurbe da osudite.", "Trying to understand is more generous than rushing to judge.", -7),
                ("Nelagodnost koju osetite tokom vežbe je znak da učite.", "The discomfort you feel during the exercise is a sign that you are learning.", -8),
                ("Hrabrost je ostati prisutan kad bi bilo lakše napustiti razgovor.", "Courage is staying present when it would be easier to leave the conversation.", -8),
                ("Možda ćete primetiti nešto neprijatno o sebi. To je znak da vežba radi.", "You may notice something uncomfortable about yourself. That is a sign the exercise is working.", -8),
                ("Iskoristite sledećih nekoliko minuta da vidite svet malo šire.", "Use the next few minutes to see the world a little wider.", -9),
                ("Perspektiva se širi jednim pokušajem u trenutku. Ovo je vaš trenutak.", "Perspective expands one attempt at a time. This is your moment.", -9),
                ("Počnite polako. Razumevanje nije trka, već praksa.", "Start slowly. Understanding is not a race, but a practice.", -9),
                ("Nekoliko minuta fokusa danas gradi veštinu koja traje.", "A few minutes of focus today build a skill that lasts.", -10),
                ("Čak i malo vežbanja daje velike rezultate na duže staze.", "Even a little practice leads to big long-term results.", -10),
                ("Ovaj izazov je mali, ali ono što gradite njime nije.", "This challenge is small, but what you build through it is not.", -10),
                ("Svaki izazov koji rešite dodaje sloj razumevanja koji niste imali juče.", "Every challenge you complete adds a layer of understanding you did not have yesterday.", -10),
                ("Niko ne vidi rezultate ove vežbe odmah, ali svi ih osete vremenom.", "No one sees the results of this exercise immediately, but everyone feels them over time.", -10),
                ("Nekoliko minuta pažnje danas vredi više nego sat rasejanog truda.", "A few minutes of attention today are worth more than an hour of scattered effort.", null),
                ("Ne morate imati odgovore. Dovoljno je da ste spremni da razmislite.", "You do not have to have all the answers. It is enough to be willing to reflect.", null),
                ("Ovo što vas čeka ne zahteva pripremu, samo iskrenost prema sebi.", "What awaits you does not require preparation, only honesty with yourself.", null),
                ("Započnite bez očekivanja. Ono što treba da dođe, doći će kroz sam pokušaj.", "Begin without expectations. What needs to come will come through the attempt itself.", null),
            };
            var endMessages = new (string Text, string TextEn)[]
            {
                ("Upravo ste uradili nešto što većina ljudi danas nije: svesno ste pokušali da vidite šire.", "You just did something most people did not do today: you consciously tried to see more broadly."),
                ("Ovo što ste upravo završili ne daje rezultate odmah, ali ih daje sigurno.", "What you just completed does not deliver results immediately, but it delivers them reliably."),
                ("Svaki put kad prođete kroz ovaj proces, malo lakše razumete ljude oko sebe.", "Each time you go through this process, understanding people around you becomes a little easier."),
                ("Možda ne osećate razliku danas, ali ona se upravo desila.", "You may not feel the difference today, but it just happened."),
                ("Ono što ste upravo vežbali čini vas boljim u svakom razgovoru koji vas čeka.", "What you just practiced makes you better in every conversation ahead of you."),
                ("Malo ko danas svesno vežba razumevanje drugih. Vi jeste.", "Few people consciously practice understanding others today. You did."),
                ("Rezultati ove vežbe se ne mere brojevima, nego kvalitetom vaših budućih razgovora.", "The results of this exercise are not measured in numbers, but in the quality of your future conversations."),
                ("Upravo ste uložili u veštinu koja tiho menja sve vaše odnose.", "You just invested in a skill that quietly changes all your relationships."),
                ("Svaka sesija vas pomera napred, čak i kad to ne izgleda tako.", "Every session moves you forward, even when it does not look that way."),
                ("Ovo nije bio lak zadatak i baš zato vredi što ste ga završili.", "This was not an easy task, and that is exactly why completing it matters."),
                ("Upravo ste trenirali mišić koji većina ljudi ne zna da ima.", "You just trained a muscle most people do not even know they have."),
                ("Vaš trud danas ima efekat koji ćete videti u nedeljama i mesecima koji dolaze.", "Your effort today has an effect you will see in the weeks and months ahead."),
                ("Ne morate savršeno da uradite svaki izazov. Dovoljno je da ste ga prošli iskreno.", "You do not have to do every challenge perfectly. It is enough that you went through it honestly."),
                ("Ono što gradite ovim vežbama menja kako slušate, kako gledate i kako razumete.", "What you are building through these exercises changes how you listen, how you see, and how you understand."),
                ("Danas ste izabrali da se potrudite oko nečeg teškog. To zaslužuje poštovanje.", "Today you chose to put effort into something difficult. That deserves respect."),
                ("Perspektiva nije nešto što se stekne odjednom. Gradi se upravo ovako.", "Perspective is not something you gain all at once. It is built exactly like this."),
                ("Upravo ste završili nešto čemu se većina odraslih nikad ne vraća svesno.", "You just completed something most adults never consciously return to."),
                ("Razumevanje se razvija u tišini, ali se primećuje u svakom odnosu.", "Understanding develops quietly, but it is felt in every relationship."),
                ("Vi ste danas vežbali. To je jedini korak koji je bio potreban.", "You practiced today. That was the only step required."),
                ("Svaka završena sesija je dokaz da vam je stalo, a to je već mnogo.", "Every completed session is proof that you care, and that is already a lot."),
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
                    affirmationValueId,
                    textEn: message.TextEn));
            }
            foreach (var message in endMessages)
            {
                context.GrowthMessages.Add(new GrowthMessage(
                    Guid.NewGuid(),
                    message.Text,
                    GrowthMessageType.End,
                    textEn: message.TextEn));
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
                        distancedJournalSkillMap[-6],
                        @"Think of a recent conversation that did not go the way you wanted. Maybe something got lost in communication, or you left feeling misunderstood.

Describe what happened in the third person, as if you were writing a movie scene where you and the other person are characters. What was each person trying to say?",
                        @"What is one moment in that conversation where things could have gone differently? What could the main character who represents you have done or said to produce a better outcome?"
                    ),
                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Pomislite na nekoga ko vas je nedavno iznenadio (prijatno ili neprijatno) nečim što je rekao ili uradio.

Opišite šta se desilo u trećem licu, kao da pišete scenu iz filma gde ste vi i druga osoba likovi. Šta je osoba koja igra vas očekivala pre tog trenutka, i kako se to promenilo?",
                        @"Šta mislite da je tu drugu osobu navelo da postupi baš tako? Koje okolnosti ili brige, nevidljive u tom trenutku, bi mogle objasniti njeno ponašanje?",
                        ChallengeLevel.Easy,
                        distancedJournalSkillMap[-7],
                        @"Think of someone who recently surprised you (pleasantly or unpleasantly) with something they said or did.

Describe what happened in the third person, as if you were writing a movie scene where you and the other person are characters. What did the person representing you expect before that moment, and how did that change?",
                        @"What do you think led the other person to act that way? What circumstances or concerns, invisible at that moment, might explain their behavior?"
                    ),
                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Zamislite se nad zadatkom, aktivnostima ili ponašanjem koji rutinski vršite većinu dana u nedelji.

Opišite tu rutinu u trećem licu, kao da posmatrate sebe iz drugog kraja sobe. Šta osoba koja igra vas radi tokom te rutine? Šta oseća tokom tog procesa?",
                        @"U toj rutini, koji u kom trenutnku bi osoba koja gleda sa strane primetila osmeh ili nervozu na licu osobe koja igra vas?",
                        ChallengeLevel.Easy,
                        distancedJournalSkillMap[-3],
                        @"Think about a task, activity, or behavior you routinely do on most days of the week.

Describe that routine in the third person, as if you were observing yourself from the other side of the room. What does the person who represents you do during that routine? What do they feel during the process?",
                        @"In that routine, at which moment would an outside observer notice a smile or nervousness on the face of the person representing you?"
                    ),
                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Setite se trenutka iz poslednje nedelje kada ste pomogli nekome, čak i na mali način.

Opišite tu situaciju u trećem licu, kao da pričate o nekome koga poznajete. Šta je tu osobu motivisalo da pomogne? Kako se osećala pre, tokom i posle?",
                        @"Kako je, po vašem mišljenju, osoba koja je primila pomoć doživela taj trenutak? Šta je možda primetila, a šta joj je promaklo?",
                        ChallengeLevel.Easy,
                        distancedJournalSkillMap[-7],
                        @"Recall a moment from the past week when you helped someone, even in a small way.

Describe that situation in the third person, as if you were talking about someone you know. What motivated that person to help? How did they feel before, during, and after?",
                        @"In your opinion, how did the person who received help experience that moment? What might they have noticed, and what might they have missed?"
                    ),
                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Setite se situacije kada ste nedavno osetili ponos ili zadovoljstvo sobom.

Opišite tu situaciju u trećem licu, kao da pišete kratku priču o nekome. Šta je ta osoba postigla? Šta joj je to značilo?",
                        @"Da li ta osoba dovoljno prepoznaje ono što je uradila, ili postoji nešto što umanjuje u sopstvenim očima? Šta bi nepristrasni posmatrač dodao?",
                        ChallengeLevel.Easy,
                        distancedJournalSkillMap[-5],
                        @"Recall a situation when you recently felt proud or satisfied with yourself.

Describe that situation in the third person, as if writing a short story about someone. What did that person accomplish? What did it mean to them?",
                        @"Does that person fully recognize what they did, or is there something that minimizes it in their own eyes? What would an impartial observer add?"
                    ),
                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Pomislite na nešto što ste nedavno čuli ili pročitali (vest, komentar, ili nečiju priču) što vas je emotivno pogodilo, makar i malo.

Opišite svoje iskustvo u trećem licu. Šta je tu osobu tačno pogodilo? Šta takva reakcija govori o toj osobi?",
                        @"Da li je neko drugi mogao čuti istu stvar i reagovati sasvim drugačije? Šta bi oblikovalo tu drugačiju reakciju?",
                        ChallengeLevel.Easy,
                        distancedJournalSkillMap[-7],
                        @"Think of something you recently heard or read (news, a comment, or someone's story) that affected you emotionally, even a little.

Describe your experience in the third person. What exactly affected that person? What does such a reaction say about that person?",
                        @"Could someone else hear the same thing and react completely differently? What would shape that different reaction?"
                    ),
                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Setite se situacije u kojoj ste nedavno morali da se prilagodite, da li zbog novog okruženja, novih ljudi, novih pravila ili bilo koje sitne promene.

Opišite to iskustvo u trećem licu. Šta je ta osoba osećala tokom prilagođavanja? Šta je bilo najteže i zbog čega?",
                        @"Kako su drugi ljudi u toj situaciji verovatno videli tu osobu dok se prilagođavala? Da li bi se njihov utisak razlikovao od onog kako se ona osećala iznutra?",
                        ChallengeLevel.Easy,
                        distancedJournalSkillMap[-3],
                        @"Recall a situation where you recently had to adapt, whether because of a new environment, new people, new rules, or any small change.

Describe that experience in the third person. What did that person feel during adaptation? What was hardest, and why?",
                        @"How did other people in that situation likely see that person while they were adapting? Would their impression differ from how the person felt inside?"
                    ),
                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Setite se nedavnog trenutka kada ste osetili frustraciju, razočaranje ili neslaganje sa nekim.

Opišite šta se desilo u trećem licu, kao da pišete scenu iz filma gde ste vi i druga osoba likovi. Opišite šta se desilo, šta su likovi mislili i osećali i kako su reagovali.",
                        @"Postavite se u položaj osobe koja je razlog frustracije ili razočarenja. Navedite dobar razlog zašto je ta osoba postupila kako je postupila, takav da se otkloni deo negativne emocije koju glavni lik oseća.",
                        ChallengeLevel.Medium,
                        distancedJournalSkillMap[-8],
                        @"Recall a recent moment when you felt frustration, disappointment, or disagreement with someone.

Describe what happened in the third person, as if writing a movie scene where you and the other person are characters. Describe what happened, what the characters thought and felt, and how they reacted.",
                        @"Put yourself in the position of the person who caused the frustration or disappointment. Give a good reason why that person acted the way they did, in a way that softens part of the negative emotion the main character feels."
                    ),
                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Pomislite na neku odluku koju trenutno odlažete ili oko koje se dvoumite.

Opišite tu situaciju u trećem licu, kao da posmatrate prijatelja koji se nalazi na istom raskršću. Šta ta osoba želi? Čega se plaši? Šta je drži na mestu?",
                        @"Da ta osoba za pet godina gleda unazad na ovaj trenutak, šta bi joj bilo važno, a šta bi joj delovalo beznačajno?",
                        ChallengeLevel.Medium,
                        distancedJournalSkillMap[-2],
                        @"Think of a decision you are currently postponing or unsure about.

Describe that situation in the third person, as if observing a friend at the same crossroads. What does that person want? What are they afraid of? What keeps them stuck?",
                        @"If that person looked back at this moment in five years, what would matter to them and what would seem insignificant?"
                    ),
                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Setite se nedavne situacije u kojoj ste bili sigurni da ste u pravu, a druga osoba se nije slagala.

Opišite tu situaciju u trećem licu, kao da ne pišete o sebi već o nekome koga poznajete. Kako je ta osoba videla stvari? Zašto je bila ubeđena da je u pravu?",
                        @"Šta bi druga osoba rekla da je neko pita da ispriča svoju stranu priče? U čemu bi se njena verzija najviše razlikovala?",
                        ChallengeLevel.Medium,
                        distancedJournalSkillMap[-7],
                        @"Recall a recent situation where you were sure you were right, but the other person disagreed.

Describe that situation in the third person, as if you were not writing about yourself but about someone you know. How did that person see things? Why were they convinced they were right?",
                        @"What would the other person say if someone asked them to tell their side of the story? Where would their version differ the most?"
                    ),
                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Pomislite na grupu ljudi kojoj pripadate (kolege, porodica, društvo) i na neku nedavnu dinamiku unutar te grupe koja vam je bila neprijatna ili zbunjujuća.

Opišite šta se dešavalo u trećem licu, kao da opisujete scenu iz filma. Šta je radio i osećao lik koji igra vas? Kakvu ulogu je ta osoba imala u toj dinamici?",
                        @"Kako bi neko ko tek upoznaje ovu grupu opisao tu situaciju? Šta bi mu prvo upalo u oči, a šta bi mu ostalo nevidljivo?",
                        ChallengeLevel.Medium,
                        distancedJournalSkillMap[-3],
                        @"Think about a group you belong to (colleagues, family, friends) and a recent dynamic within that group that felt uncomfortable or confusing.

Describe what happened in the third person, as if describing a movie scene. What did the character who represents you do and feel? What role did that person have in that dynamic?",
                        @"How would someone who is just getting to know this group describe that situation? What would stand out to them first, and what would remain invisible?"
                    ),
                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Pomislite na nekoga koga ste nedavno sreli ili sa kim ste razgovarali, a ko vas je iritirao ili vam bio nesimpatičan.

Opišite tu interakciju u trećem licu, kao neutralan izveštaj bez optužbe. Šta je druga osoba radila? Šta je osoba koja vas predstavlja mislila i zbog čega?",
                        @"Kako bi prijatelj od nesimpatične osobe, koji je dobro poznaje, objasnio njeno ponašanje?",
                        ChallengeLevel.Medium,
                        distancedJournalSkillMap[-8],
                        @"Think of someone you recently met or spoke with who irritated you or felt unlikeable.

Describe that interaction in the third person, as a neutral report without blame. What was the other person doing? What was the person representing you thinking, and why?",
                        @"How would a friend of that unlikeable person, someone who knows them well, explain their behavior?"
                    ),
                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Pomislite na nešto što vas u poslednje vreme opterećuje, poput brige koja se stalno vraća u misli.

Opišite vas i vašu brigu u trećem licu, kao da pratite nekoga kroz njegov dan. Kako ta briga utiče na njega? Kada je najglasnija, a kada se povlači?",
                        @"Ako bi vaš dobar prijatelj imao sličnu brigu koja ga toliko opterećuje, kako biste ga posavetovali?",
                        ChallengeLevel.Hard,
                        distancedJournalSkillMap[-4],
                        @"Think of something that has been weighing on you lately, like a worry that keeps returning.

Describe yourself and your worry in the third person, as if following someone through their day. How does that worry affect them? When is it loudest, and when does it fade?",
                        @"If your close friend had a similar worry weighing on them that much, how would you advise them?"
                    ),
                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Pomislite na osobu sa kojom ste u poslednje vreme u napetom ili hladnom odnosu.

Opišite situaciju u trećem licu, gde ste vi lik iz priče. Kako taj lik fizički doživljava tu napetost? Da li je tu više ljutnje, tuge, ili nečeg trećeg? Koja potreba glavnog lika nije ispunjena pa se tako oseća?",
                        @"Ako bi druga osoba iskreno opisala šta oseća povodom napetosti glavnog lika, šta mislite da bi rekla? Kako bi obrazložila svoju stranu priče?",
                        ChallengeLevel.Hard,
                        distancedJournalSkillMap[-7],
                        @"Think of a person with whom you have recently had a tense or distant relationship.

Describe the situation in the third person, where you are the character in the story. How does that character physically experience the tension? Is there more anger, sadness, or something else? What need of the main character is unmet, causing them to feel this way?",
                        @"If the other person honestly described what they feel about the main character's tension, what do you think they would say? How would they explain their side of the story?"
                    ),
                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Setite se nečega što izbegavate ili odlažete u poslednje vreme, poput razgovora, obaveze, ili odluke.

Opišite vaš slučaj u trećem licu, bez osuđivanja. Šta glavni lik izbegava? Šta misli da bi se desilo ako se suoči sa tim?",
                        @"Da li ta osoba izbegava sam ishod, ili osećanje koje bi taj ishod doneo? Koliki će uticaj na život dati ishod imati za šest meseci?",
                        ChallengeLevel.Hard,
                        distancedJournalSkillMap[-2],
                        @"Recall something you have been avoiding or postponing lately, such as a conversation, obligation, or decision.

Describe your case in the third person, without judgment. What is the main character avoiding? What do they think would happen if they faced it?",
                        @"Is that person avoiding the outcome itself, or the feeling that outcome would bring? How much impact will that outcome have on life in six months?"
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
                        .Select(q => (
                            Guid.NewGuid(),
                            q.SkillId,
                            q.Order,
                            q.Question,
                            q.Reveal,
                            (string?)TranslateToEnglish(q.Question),
                            (string?)TranslateToEnglish(q.Reveal)))
                        .ToList();

                    return new PerspectiveScenarioChallenge(
                        Guid.NewGuid(),
                        scenario.Context,
                        scenario.ActorCount,
                        scenario.Scenario,
                        scenario.ChallengeLevel,
                        questions,
                        TranslateToEnglish(scenario.Scenario));
                }).ToList();

                context.PerspectiveScenarioChallenges.AddRange(challenges);
            }

            await context.SaveChangesAsync();
        }

        private static string TranslateToEnglish(string source)
        {
            if (source.StartsWith("Četvoro prijatelja su na zajedničkom putovanju.", StringComparison.Ordinal))
                return @"Four friends are on a trip together. Elena organized a detailed plan: museums in the morning, lunch at a reserved restaurant, sightseeing in the afternoon. On day one, Katarina suggests skipping the museum and going to the market. Elena says, ""But I already have tickets."" Dimitrije says, ""We can split up."" Elena replies, ""Then traveling together makes no sense."" Nada, who had been quiet, suggests, ""Let's do the museum until noon, then the market."" Elena agrees but is visibly tense. On day two, Katarina and Dimitrije go to breakfast alone without informing Elena and Nada. Elena is upset. Nada tries to calm her: ""They didn't mean anything bad."" Elena replies: ""You always defend everyone except me.""";
            if (source.StartsWith("U grupnom četu troje prijatelja", StringComparison.Ordinal))
                return @"In a group chat of three friends, Tamara shares a link to a mental-health article with the comment: ""This really hit me."" Pavle responds with a thumbs-up emoji. Nataša writes: ""Please let's not turn this into a therapy group 😂."" Tamara deletes her message fifteen minutes later. The next day Pavle messages Tamara privately: ""Are you okay?"" Tamara replies: ""Yeah, all good.""";
            if (source.StartsWith("Branko i Sanja ugošćuju", StringComparison.Ordinal))
                return @"Branko and Sanja host Branko's brother Goran and his wife Vesna for dinner. During the conversation, Goran talks quickly and continuously about an expensive home renovation. Branko congratulates him, but Sanja becomes quiet. Later, Vesna mentions their children are enrolled in a private school. Sanja goes to the kitchen ""to check dessert."" Branko follows and whispers: ""Can you at least pretend?"" Sanja answers: ""I've been pretending all evening."" They return to the table smiling.";
            if (source.StartsWith("Dušan, Marija i Nemanja su zajedno počeli", StringComparison.Ordinal))
                return @"Dušan, Marija, and Nemanja started training together at the gym six months ago. Dušan progressed quickly and began giving Marija and Nemanja advice they did not ask for: ""You should change your form,"" ""Eat more protein."" Marija started coming at a different time when fewer people are there. Nemanja comes at the same time but puts on headphones as soon as he enters. Dušan is confused and complains to a mutual friend: ""I was helping them and now they avoid me.""";
            if (source.StartsWith("Goran objavljuje da je Viktor dobio unapređenje.", StringComparison.Ordinal))
                return @"Goran announces that Viktor got promoted. Tijana, who has worked at the company two years longer than Viktor, briefly congratulates him and leaves for a break. Colleagues notice she is quiet for the rest of the day. Viktor approaches Tijana and says: ""I hope you're okay, you taught me everything."" Tijana replies: ""You deserved it, congrats."" The next day Tijana sends an email to HR asking about internal promotion procedures.";
            if (source.StartsWith("Bračni par Aleksa i Mina razgovaraju", StringComparison.Ordinal))
                return @"Married couple Aleksa and Mina discuss New Year's plans. Mina suggests celebrating alone at home this year. Aleksa hesitantly says: ""Let's ask my mom if she wants to come, she's been alone since dad died."" Mina replies: ""Every year it's the same. We're never alone."" Aleksa raises his voice: ""What, should I leave her alone on New Year's Eve?"" The conversation ends in silence.";
            if (source.StartsWith("Ivana je organizovala proslavu rođendana", StringComparison.Ordinal))
                return @"Ivana organized a birthday celebration at a restaurant and invited twenty friends. Milica, her best friend, who had been going out less and less lately, confirmed she would come but did not show up. She sent no message during the evening. The next day she wrote: ""Sorry, I couldn't."" Ivana did not reply.";
            if (source.StartsWith("Vuk i Sara su na večeri u restoranu.", StringComparison.Ordinal))
                return @"Vuk and Sara are having dinner at a restaurant. Sara talks about her recent trip to Greece, describing beaches, food, and people she met. Vuk shows genuine interest, listens, and nods, but after a few minutes his gaze starts wandering around the restaurant. He returns to the story, then looks at the menu. When Sara finishes, Vuk changes the topic to football. Sara says, ""Interesting,"" and opens her phone.";
            if (source.StartsWith("Tamara rukovodi timom od šest ljudi.", StringComparison.Ordinal))
                return @"Tamara leads a team of six people. At the first meeting she was brief, spoke only about tasks and deadlines, asked no one how they felt, and did not introduce herself personally. When a colleague tried to make a joke, Tamara smiled but immediately continued. After the meeting, team members commented that she was ""cold"" and ""robots have more emotions.""";
            if (source.StartsWith("Za Gocin rođendan, njena ćerka Maja", StringComparison.Ordinal))
                return @"For Goca's birthday, her daughter Maja bought an expensive face cream she had seen in an ad. When Goca opened the gift, she briefly said ""Thank you"" and set the box aside. She continued talking with other guests. Days later, Maja noticed Goca still had not opened the box.";
            if (source.StartsWith("Dejan razgovara na telefon", StringComparison.Ordinal))
                return @"Dejan talks on the phone in a shared office space. As usual, he speaks loudly and energetically, laughs, and gestures. Ana, sitting two desks away, puts on headphones and moves closer to the window. When Dejan finishes and asks Ana something about a project, she answers coldly in one sentence without eye contact. Dejan is confused by her reaction.";
            if (source.StartsWith("Petar dolazi kod roditelja", StringComparison.Ordinal))
                return @"Petar comes to his parents for Sunday lunch. At the table he talks about considering a job change and enrolling in a programming course. His father Dragan interrupts: ""You have a stable job, don't be childish."" Petar falls silent, and the rest of lunch passes in silence.";

            if (source.StartsWith("Šta se nalazi ispod Elenine potrebe", StringComparison.Ordinal)) return "What lies beneath Elena's need to control the plan, and why does she experience it as a relationship issue?";
            if (source.StartsWith("Koje potrebe Katarina i Dimitrije", StringComparison.Ordinal)) return "What needs are Katarina and Dimitrije expressing that Elena's plan disrupts?";
            if (source.StartsWith("Zašto je Nadina uloga mirotvorke", StringComparison.Ordinal)) return "Why is Nada's peacemaker role actually problematic, even though it looks constructive?";
            if (source.StartsWith("Šta je Tamara htela da postigne", StringComparison.Ordinal)) return "What did Tamara want to achieve by sharing the link, and how did she interpret the reactions?";
            if (source.StartsWith("Šta stoji iza Natašine šale", StringComparison.Ordinal)) return "What is behind Nataša's joke, and what does she fail to understand about its effect?";
            if (source.StartsWith("Šta Pavlova privatna poruka", StringComparison.Ordinal)) return "What does Pavle's private message reveal about his understanding of the situation?";
            if (source.StartsWith("Šta konkretno uzrokuje Sanjinu reakciju", StringComparison.Ordinal)) return "What specifically causes Sanja's reaction: jealousy, pain, or something else?";
            if (source.StartsWith("Šta Branko ne razume o Sanjinoj perspektivi", StringComparison.Ordinal)) return "What does Branko fail to understand about Sanja's perspective when he tells her to pretend?";
            if (source.StartsWith("Kako Goran i Vesna verovatno tumače", StringComparison.Ordinal)) return "How do Goran and Vesna likely interpret the dinner atmosphere?";
            if (source.StartsWith("Zašto Marija ima potrebu", StringComparison.Ordinal)) return "Why does Marija feel the need to completely change her gym schedule?";
            if (source.StartsWith("Šta je kod Nemanje drugačije", StringComparison.Ordinal)) return "What is different about Nemanja that lets him cope with Dušan without changing his schedule?";
            if (source.StartsWith("Zašto Dušan ima potrebu", StringComparison.Ordinal)) return "Why does Dušan feel the need to give advice?";
            if (source.StartsWith("Šta Tijana zapravo oseća", StringComparison.Ordinal)) return "What does Tijana actually feel despite what she says?";
            if (source.StartsWith("Šta Viktor ne vidi u dinamici", StringComparison.Ordinal)) return "What does Viktor fail to see in the dynamics between himself and the others?";
            if (source.StartsWith("Šta Mina zapravo želi da kaže", StringComparison.Ordinal)) return "What does Mina actually want to say but fails to articulate?";
            if (source.StartsWith("Šta sprečava Aleksu", StringComparison.Ordinal)) return "What prevents Aleksa from acknowledging Mina's perspective and helping her express her need?";
            if (source.StartsWith("Šta Ivana verovatno pretpostavlja", StringComparison.Ordinal)) return "What does Ivana likely assume about Milica's absence?";
            if (source.StartsWith("Šta bi moglo da stoji iza reči", StringComparison.Ordinal)) return "What might be behind the words \"I couldn't\" that could explain Milica's absence?";
            if (source.StartsWith("Šta Sara verovatno oseća", StringComparison.Ordinal)) return "What does Sara likely feel after Vuk's reaction?";
            if (source.StartsWith("Šta sprečava Vuka", StringComparison.Ordinal)) return "What prevents Vuk, despite trying to listen, from engaging with Sara's story?";
            if (source.StartsWith("Zašto je Tamara tako pristupila", StringComparison.Ordinal)) return "Why did Tamara approach the first meeting that way?";
            if (source.StartsWith("Šta Gordana verovatno oseća", StringComparison.Ordinal)) return "What does Gordana likely feel about the gift?";
            if (source.StartsWith("Zašto Dejan ne primećuje", StringComparison.Ordinal)) return "Why does Dejan fail to notice the negative impact of this behavior on others?";
            if (source.StartsWith("Šta Dragan zapravo oseća", StringComparison.Ordinal)) return "What does Dragan actually feel beneath his harsh reaction?";
            if (source.StartsWith("Šta je Dragan proživeo", StringComparison.Ordinal)) return "What has Dragan lived through that causes this idea to trigger that emotion?";

            if (source.StartsWith("Elena je uložila dvadeset sati", StringComparison.Ordinal)) return "Elena invested twenty hours in planning. For her, the plan is not logistics, but a gift to the group and an expression of love. When Katarina suggests a change, Elena does not hear \"let's go to the market\" but \"your effort has no value.\" This is tied to her childhood pattern: she was the child who organized games, and when others stopped following her rules, she felt rejected. The plan is not about the museum, but about her place in the group.";
            if (source.StartsWith("Katarina je spontana osoba", StringComparison.Ordinal)) return "Katarina is spontaneous and experiences a detailed plan as confinement; she is unaware of how much work Elena invested because she would never plan that way. \"Skip the museum\" is trivial for Katarina, existential for Elena. Dimitrije is an introvert who needs quiet mornings, and breakfast with Katarina was his way to recharge, not to exclude Elena and Nada. Neither sees that their small choices translate for Elena into \"you are not important.\"";
            if (source.StartsWith("Nada je hronični mirotvorac", StringComparison.Ordinal)) return "Nada is a chronic peacemaker who avoids conflict at any cost. Her museum compromise was functional, but her pattern of constantly calming Elena makes Elena feel her anger is illegitimate. Elena's line \"you defend everyone except me\" reveals years of accumulated frustration. Nada never validates Elena's feelings before trying to solve them. Quick calming is not support; it is suppression.";
            if (source.StartsWith("Tamara prolazi kroz period teškoće", StringComparison.Ordinal)) return "Tamara is going through a difficult period. Sharing the link was her indirect way of opening the topic. She was not ready to say \"I'm not okay,\" but wanted to see whether someone would notice the signal. She heard Nataša's joke as: \"Your feelings are too heavy for this group.\" Deleting the message was a retreat so she would not seem like the person turning the group into therapy.";
            if (source.StartsWith("Nataša humor koristi", StringComparison.Ordinal)) return "Nataša uses humor as her own defense mechanism. She also struggles with anxiety but learned to distance emotions through jokes. Her comment was about her own discomfort, not about Tamara, but the effect was the same. In that moment she set a group norm: we do not talk about this here.";
            if (source.StartsWith("Pavle je jedini koji je pročitao", StringComparison.Ordinal)) return "Pavle was the only one who read the situation correctly and recognized that deleting the message meant withdrawal. However, his approach was incomplete: by messaging privately, he confirmed to Tamara that this topic belongs in whispers, instead of normalizing it in the group and signaling that such conversation is welcome. Good intention, wrong channel.";
            if (source.StartsWith("Sanja i Branko su prošle godine", StringComparison.Ordinal)) return "Sanja and Branko went through a serious financial crisis last year. Branko's business was near collapse, they had to sell their cottage and cancel planned travel. Hearing about expensive renovations and private schools does not trigger envy in Sanja, but pain from recent loss. What looks like withdrawal is an activated wound.";
            if (source.StartsWith("Branko misli da ga Sanja", StringComparison.Ordinal)) return "Branko thinks Sanja is not supporting him in front of his brother. He does not see that her frustration is not directed at Goran and Vesna, but at him. He asks her to participate in a performance she sees as undignified. She is not refusing to be a polite host; she is refusing to turn their marriage into a scene that hides their reality.";
            if (source.StartsWith("Goran o renoviranju priča", StringComparison.Ordinal)) return "Goran talks about renovation because he is nervous. He knows about his brother's finances and fills silence with topics that feel safe, not to brag. Vesna mentions private school because that is what occupies her mind, without realizing how it sounds in context. Both sense something is off, but attribute it to fatigue instead of unspoken tension.";
            if (source.StartsWith("Marija se ceo život bori", StringComparison.Ordinal)) return "Marija has struggled with body image all her life. The gym was a major step and a place she wanted to feel safe, not judged. Dušan's advice, however well-intended, activated her feeling that something is wrong with her. Changing schedules is not a message to Dušan, but protection of the gym as a safe space.";
            if (source.StartsWith("Nemanja nema problem sa samopouzdanjem", StringComparison.Ordinal)) return "Nemanja does not struggle with confidence. He is bothered by control, not by the content of advice. He values autonomy and dislikes being told what to do, even when the advice is correct. Headphones are his boundary-setting tool without open conflict.";
            if (source.StartsWith("Dušan je čovek čiji je primarni", StringComparison.Ordinal)) return "Dušan's primary language of care is giving advice. In his family, care was shown through correction and guidance, and he reads that as closeness. He sincerely does not understand that his advice, though accurate, is unasked-for and received as criticism rather than support.";
            if (source.StartsWith("Tijana ne sumnja u Viktorove sposobnosti", StringComparison.Ordinal)) return "Tijana does not doubt Viktor's abilities. What hurts is systemic: she was proposed for promotion twice and both times told \"next cycle.\" Her HR email is not revenge, but an attempt to understand rules that feel opaque. She quietly suspects Viktor advanced faster because he is more socially visible.";
            if (source.StartsWith("Viktor iskreno misli", StringComparison.Ordinal)) return "Viktor genuinely believes Tijana is happy for him because she truly mentored him. He does not see that the very mentoring dynamic is what makes this painful. He benefits from manager proximity without being aware of it.";
            if (source.StartsWith("Mina ne mrzi Aleksinu mamu", StringComparison.Ordinal)) return "Mina does not hate Aleksa's mother or want to exclude her. The deeper issue is that in four years of marriage, Mina and Aleksa have never spent a holiday as a couple alone. Mina feels their marriage has no room to build its own rituals and identity.";
            if (source.StartsWith("Aleksa je zarobljen u ulozi sina", StringComparison.Ordinal)) return "Aleksa is trapped in the role of a son caring for his mother, a role he took after his father died. He interprets Mina's request for separation as an attack on his mother, and that emotional reaction blocks him from hearing Mina.";
            if (source.StartsWith("Ivana verovatno tumači izostanak", StringComparison.Ordinal)) return "Ivana likely reads the absence as a sign that Milica does not value her as expected from a best friend. The message \"I couldn't\" without explanation feels like disrespectful rejection. She assumes it was a choice, not inability.";
            if (source.StartsWith("Milica pati od socijalne anksioznosti", StringComparison.Ordinal)) return "Milica suffers from social anxiety that has worsened in recent months. She stood in front of the restaurant for fifteen minutes but could not enter; her heart raced and her hands shook. She went home ashamed. \"I couldn't\" was literal: she was not able, not unwilling.";
            if (source.StartsWith("Sara se oseća nevidljivo", StringComparison.Ordinal)) return "Sara feels unseen and rejected. Reaching for her phone is not boredom but quiet withdrawal. Vuk's silence and sudden topic shift signal to her that her inner world is not interesting. Her \"interesting\" is both a polite closure and a small punishment.";
            if (source.StartsWith("Vuk nije nezainteresovan za Sarin život", StringComparison.Ordinal)) return "Vuk is not uninterested in Sara's life. He has attention difficulties (ADD) that make long narrative stories hard to follow. He genuinely tried, but his focus kept slipping. He changed the topic not because he did not care, but because he felt anxious about not being able to track the story.";
            if (source.StartsWith("Tamara je introvert koja je dobila jasnu", StringComparison.Ordinal)) return "Tamara is an introvert who got clear feedback from her previous boss: \"You relax too much with the team and lose authority.\" She decided to set professional boundaries from the start in the new role. Her rigidity is not indifference; it is a carefully planned strategy by someone afraid of making mistakes.";
            if (source.StartsWith("Goca pripada generaciji", StringComparison.Ordinal)) return "Goca belongs to a generation that sees expensive cosmetic creams as unnecessary waste. She has used homemade products all her life and is proud of that. She interpreted the gift as subtle criticism of her appearance, as if her daughter was telling her she needs help looking younger.";
            if (source.StartsWith("Dejan je odrastao u velikoj i glasnoj porodici", StringComparison.Ordinal)) return "Dejan grew up in a large, loud family where high noise signaled warmth and closeness. His loudness is not a deliberate choice; it is his default communication style. No one at work has told him directly he is too loud, so he assumes it is fine.";
            if (source.StartsWith("Kada čuje da Petar želi da napusti", StringComparison.Ordinal)) return "When Dragan hears Petar wants to leave a secure job, deep fear for his son is activated. His sharp tone is not contempt for Petar's ambitions, but panic disguised as authority.";
            if (source.StartsWith("Dragan je ceo radni vek proveo", StringComparison.Ordinal)) return "Dragan spent his entire career in one workplace because he never had the luxury of choice. He survived two waves of layoffs and has lived with fear of financial insecurity ever since. For him, a stable job is not compromise; it is survival.";

            return source;
        }
    }
}



