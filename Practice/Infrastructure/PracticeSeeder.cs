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

            var affirmationData = new (string Name, string[] Statements)[]
            {
                ("Empatija", new[]
                {
                    "Danas pokušavam da vidim svet tuđim očima.",
                }),
                ("Poniznost", new[]
                {
                    "Ne moram da budem u pravu da bih bio/la vredna.",
                }),
                ("Povezanost", new[]
                {
                    "Danas tražim ono što me povezuje s nekim ko je drugačiji od mene.",
                }),
                ("Radoznalost", new[]
                {
                    "Danas biram da pitam umesto da pretpostavim.",
                    "Svaka osoba ima perspektivu koju ja nisam razmatrao/la.",
                    "Radoznalost prema drugima počinje jednim iskrenim pitanjem.",
                    "Danas tražim priču iza mišljenja.",
                }),
                ("Autonomija", new[]
                {
                    "Biram da rastem zato što to želim, ne zato što moram.",
                    "Moj razvoj pripada meni i ja određujem ritam.",
                    "Danas radim ono što je u skladu s mojim vrednostima.",
                    "Ne moram da se složim da bih razumeo/la.",
                    "Biram da budem otvoren/a jer sam to odlučio/la, ne jer se to očekuje.",
                }),
                ("Pravičnost", new[]
                {
                    "Kontekst u kome neko živi oblikuje ono što može da postigne.",
                    "Danas se pitam šta bih ja uradio/la na tuđem mestu, s tuđim resursima i ograničenjima.",
                    "Razumevanje počinje kad uvažim da su startne pozicije različite.",
                    "Svačija realnost je oblikovana prilikama koje je imao.",
                    "Danas biram da ne sudim pre nego što razumem pozadinu.",
                    "Pravičnost je kad vidim celu sliku, ne samo svoj deo.",
                }),
                ("Velikodušnost", new[]
                {
                    "Danas biram da pretpostavim dobru nameru.",
                    "Danas tumačim tuđe reči blagonaklono dok ne saznam više.",
                    "Jedna velikodušna misao može da promeni ceo utisak o nekome.",
                    "Biram da vidim osobu iza postupka.",
                    "Nekad je najmudrije dati nekome drugu šansu.",
                    "Danas biram razumevanje pre reagovanja.",
                    "Lako je osuditi, a vredno je pokušati da razumem.",
                }),
                ("Hrabrost", new[]
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
                ("Mudrost", new[]
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
                ("Rast", new[]
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

            foreach (var entry in affirmationData)
            {
                var value = new AffirmationValue(Guid.NewGuid(), entry.Name);
                foreach (var statement in entry.Statements)
                {
                    value.AddStatement(Guid.NewGuid(), statement);
                }
                context.AffirmationValues.Add(value);
            }

            var beginMessages = new[]
            {
                "Iskoristite sledećih nekoliko minuta da vidite svet malo šire.",
                "Svaki put kad pokušate da razumete nekoga, vaš svet postaje veći.",
                "Danas imate priliku da vežbate nešto što većina ljudi nikad ne vežba svesno.",
                "Ono što vas čeka traži samo jednu stvar: iskrenu nameru da pokušate.",
                "Spremni ste. Ne morate biti savršeni, dovoljno je da ste prisutni.",
                "Nekoliko minuta fokusa danas gradi veštinu koja traje.",
                "Vaš trud danas menja način na koji ćete sutra videti ljude oko sebe.",
                "Ne postoji pogrešan odgovor, već samo vaša spremnost da razmislite.",
                "Danas vežbate nešto što vas čini boljim sagovornikom, kolegom i prijateljem.",
                "Čak i malo vežbanja daje velike rezultate na duže staze.",
                "Perspektiva se širi jednim pokušajem u trenutku. Ovo je vaš trenutak.",
                "Sve što vam treba za ovaj izazov već nosite sa sobom.",
                "Danas radite nešto retko: svesno birate da vidite šire.",
                "Nema žurbe. Uzmite vremena koliko vam treba i budite iskreni prema sebi.",
                "Svaki izazov koji rešite dodaje sloj razumevanja koji niste imali juče.",
                "Ovaj izazov je mali, ali ono što gradite njime nije.",
                "Danas birate da se potrudite oko nečega što je zaista važno.",
                "Vaša pažnja je najvredniji resurs koji imate. Uložite je u sledećih par minuta.",
                "Niko ne vidi rezultate ove vežbe odmah, ali svi ih osete vremenom.",
                "Počnite polako. Razumevanje nije trka, već praksa.",
            };

            var endMessages = new[]
            {
                "Upravo ste uradili nešto što većina ljudi danas nije: svesno ste pokušali da vidite šire.",
                "Ovo što ste upravo završili ne daje rezultate odmah, ali ih daje sigurno.",
                "Svaki put kad prođete kroz ovaj proces, malo lakše razumete ljude oko sebe.",
                "Možda ne osećate razliku danas, ali ona se upravo desila.",
                "Ono što ste upravo vežbali čini vas boljim u svakom razgovoru koji vas čeka.",
                "Malo ko danas svesno vežba razumevanje drugih. Vi jeste.",
                "Rezultati ove vežbe se ne mere brojevima, nego kvalitetom vaših budućih razgovora.",
                "Upravo ste uložili u veštinu koja tiho menja sve vaše odnose.",
                "Svaka sesija vas pomera napred, čak i kad to ne izgleda tako.",
                "Ovo nije bio lak zadatak i baš zato vredi što ste ga završili.",
                "Upravo ste trenirali mišić koji većina ljudi ne zna da ima.",
                "Vaš trud danas ima efekat koji ćete videti u nedeljama i mesecima koji dolaze.",
                "Ne morate savršeno da uradite svaki izazov. Dovoljno je da ste ga prošli iskreno.",
                "Ono što gradite ovim vežbama menja kako slušate, kako gledate i kako razumete.",
                "Danas ste izabrali da se potrudite oko nečeg teškog. To zaslužuje poštovanje.",
                "Perspektiva nije nešto što se stekne odjednom. Gradi se upravo ovako.",
                "Upravo ste završili nešto čemu se većina odraslih nikad ne vraća svesno.",
                "Razumevanje se razvija u tišini, ali se primećuje u svakom odnosu.",
                "Vi ste danas vežbali. To je jedini korak koji je bio potreban.",
                "Svaka završena sesija je dokaz da vam je stalo, a to je već mnogo.",
            };

            foreach (var message in beginMessages)
            {
                context.GrowthMessages.Add(new GrowthMessage(Guid.NewGuid(), message, GrowthMessageType.Begin));
            }

            foreach (var message in endMessages)
            {
                context.GrowthMessages.Add(new GrowthMessage(Guid.NewGuid(), message, GrowthMessageType.End));
            }

            if (!context.DistancedJournalChallenges.Any())
            {
                context.DistancedJournalChallenges.AddRange(

                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Setite se nedavnog razgovora koji nije prošao onako kako ste želeli. Možda se nešto izgubilo u komunikaciji, ili ste otišli sa osećajem da niste bili shvaćeni.

Opišite šta se desilo u trećem licu, kao da pišete scenu iz filma gde ste vi i druga osoba likovi. Šta je koja osoba pokušala da kaže?",
                        @"Koji je jedan trenutak u tom razgovoru gde je moglo da krene drugačije? Šta bi glavni lik mogao da uradi ili kaže da bi ishod bio bolji?",
                        ChallengeLevel.Easy
                    ),

                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Pomislite na nekoga ko vas je nedavno iznenadio (prijatno ili neprijatno) nečim što je rekao ili uradio.

Opišite šta se desilo u trećem licu, kao da pišete scenu iz filma gde ste vi i druga osoba likovi. Šta je osoba koja igra vas očekivala pre tog trenutka, i kako se to promenilo?",
                        @"Šta mislite da je tu drugu osobu navelo da postupi baš tako? Koje okolnosti ili brige, nevidljive u tom trenutku, bi mogle objasniti njeno ponašanje?",
                        ChallengeLevel.Easy
                    ),

                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Zamislite se nad zadatkom, aktivnostima ili ponašanjem koji rutinski vršite većinu dana u nedelji.

Opišite tu rutinu u trećem licu, kao da posmatrate sebe iz drugog kraja sobe. Šta osoba koja igra vas radi tokom te ruine? Šta oseća tokom tog procesa?",
                        @"U toj rutini, koji momenti izazivaju neku pozitivnu ili negativnu emociju i zbog čega?",
                        ChallengeLevel.Easy
                    ),

                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Setite se trenutka iz poslednje nedelje kada ste pomogli nekome, čak i na mali način.

Opišite tu situaciju u trećem licu, kao da pričate o nekome koga poznajete. Šta je tu osobu motivisalo da pomogne? Kako se osećala pre, tokom i posle?",
                        @"Kako je, po vašem mišljenju, ta osoba koja je primila pomoć doživela taj trenutak? Šta je možda primetila, a šta joj je promaklo?",
                        ChallengeLevel.Easy
                    ),

                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Setite se situacije kada ste nedavno osetili ponos ili zadovoljstvo sobom.

Opišite tu situaciju u trećem licu, kao da pišete kratku priču o nekome. Šta je ta osoba postigla? Šta joj je to značilo?",
                        @"Da li ta osoba dovoljno prepoznaje ono što je uradila, ili postoji nešto što umanjuje u sopstvenim očima? Šta bi nepristrasni posmatrač dodao?",
                        ChallengeLevel.Easy
                    ),

                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Pomislite na nešto što ste nedavno čuli ili pročitali (vest, komentar, ili nečiju priču) što vas je emotivno pogodilo, makar i malo.

Opišite svoje iskustvo u trećem licu. Šta je tu osobu tačno pogodilo? Šta takva reakcija govori o toj osobi?",
                        @"Da li je neko drugi mogao čuti istu stvar i reagovati sasvim drugačije? Šta bi oblikovalo tu drugačiju reakciju?",
                        ChallengeLevel.Easy
                    ),

                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Setite se situacije u kojoj ste nedavno morali da se prilagodite, da li zbog novog okruženja, novih ljudi, novih pravila ili bilo koje sitne promene.

Opišite to iskustvo u trećem licu. Šta je ta osoba osećala tokom prilagođavanja? Šta je bilo najteže i zbog čega?",
                        @"Kako su drugi ljudi u toj situaciji verovatno videli tu osobu dok se prilagođavala? Da li bi se njihov utisak razlikovao od onog kako se ona osećala iznutra?",
                        ChallengeLevel.Easy
                    ),

                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Setite se nedavnog trenutka kada ste osetili frustraciju, razočaranje ili neslaganje sa nekim.

Opišite šta se desilo u trećem licu, kao da pišete scenu iz filma gde ste vi i druga osoba likovi. Opišite šta se desilo, šta su likovi mislili i osećali i kako su reagovali.",
                        @"Postavite se u položaj osobe koja je razlog frustracije ili razočarenja. Navedite dobar razlog zašto je ta osoba postupila kako je postupila, takav da se otkloni deo negativne emocije koju glavni lik oseća.",
                        ChallengeLevel.Medium
                    ),

                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Pomislite na neku odluku koju trenutno odlažete ili oko koje se dvoumite.

Opišite tu situaciju u trećem licu, kao da posmatrate prijatelja koji se nalazi na istom raskršću. Šta ta osoba želi? Čega se plaši? Šta je drži na mestu?",
                        @"Da ta osoba za pet godina gleda unazad na ovaj trenutak, šta bi joj bilo važno, a šta bi joj delovalo sitno?",
                        ChallengeLevel.Medium
                    ),

                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Setite se nedavne situacije u kojoj ste bili sigurni da ste u pravu, a druga osoba se nije slagala.

Opišite tu situaciju u trećem licu, kao da ne pišete o sebi već o nekome koga poznajete. Kako je ta osoba videla stvari? Zašto je bila ubeđena da je u pravu?",
                        @"Šta bi druga osoba rekla da je neko pita da ispriča svoju stranu priče? U čemu bi se njena verzija najviše razlikovala?",
                        ChallengeLevel.Medium
                    ),

                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Pomislite na grupu ljudi kojoj pripadate (kolege, porodica, društvo) i na neku nedavnu dinamiku unutar te grupe koja vam je bila neprijatna ili zbunjujuća.

Opišite šta se dešavalo u trećem licu, kao da opisujete scenu iz filma. Šta je radio i osećao lik koji igra vas? Kakvu ulogu je ta osoba imala u toj dinamici?",
                        @"Kako bi neko ko tek upoznaje ovu grupu opisao tu situaciju? Šta bi mu prvo upalo u oči, a šta bi mu ostalo nevidljivo?",
                        ChallengeLevel.Medium
                    ),

                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Pomislite na nekoga koga ste nedavno sreli ili sa kim ste razgovarali, a ko vas je iritirao ili vam bio nesimpatičan.

Opišite tu interakciju u trećem licu, kao neutralan izveštaj bez optužbe. Šta je druga osoba radila? Šta je osoba koja zastupa vas mislila i zbog čega?",
                        @"Kako bi prijatelj od nesimpatična osobe, koji je dobro poznaje, objasnio njeno ponašanje?",
                        ChallengeLevel.Medium
                    ),

                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Pomislite na nešto što vas u poslednje vreme opterećuje, poput brige koja se stalno vraća u misli.

Opišite vas i vašu brigu u trećem licu, kao da pratite nekoga kroz njegov dan. Kako ta briga utiče na njega? Kada je najglasnija, a kada se povlači?",
                        @"Ako bi vaš dobar prijatelj imao sličnu brigu koja ga toliko opterećuje, kako biste ga posavetovali?",
                        ChallengeLevel.Hard
                    ),

                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Pomislite na osobu sa kojom ste u poslednje vreme u napetom ili hladnom odnosu.

Opišite situaciju u trećem licu, gde ste vi lik iz priče. Kako taj lik fizički doživljava tu napetost? Da li je tu više ljutnje, tuge, ili nečeg trećeg? Koja potreba glavnog lika nije ispunjena da se tako oseća?",
                        @"Ako bi druga osoba iskreno opisala šta oseća povodom napetosti koju glavni lik oseća, šta mislite da bi rekla? Kako bi obrazložila svoju stranu priče?",
                        ChallengeLevel.Hard
                    ),

                    new DistancedJournalChallenge(
                        Guid.NewGuid(),
                        @"Setite se nečega što izbegavate ili odlažete u poslednje vreme, poput razgovora, obaveze, ili odluke.

Opišite vaš slučaj u trećem licu, bez osuđivanja. Šta glavni lik izbegava? Šta misli da bi se desilo ako se suoči sa tim?",
                        @"Da li ta osoba izbegava sam ishod, ili osećanje koje bi taj ishod doneo? Koliki će uticaj na život dati ishod imati za šest meseci?",
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

                PerspectiveScenarioContext MapContext(string value)
                {
                    return value.Trim().ToLowerInvariant() switch
                    {
                        "family" => PerspectiveScenarioContext.Family,
                        "work" => PerspectiveScenarioContext.Work,
                        "friends" => PerspectiveScenarioContext.Friends,
                        _ => PerspectiveScenarioContext.Unknown
                    };
                }

                Skill MapSkill(string questionType)
                {
                    return questionType.Trim().ToLowerInvariant() switch
                    {
                        "osećanje" => emotionRecognitionSkill,
                        "nevidljivi kontekst" => perspectiveTakingSkill,
                        "razmišljanje" => intentionReadingSkill,
                        _ => perspectiveTakingSkill
                    };
                }

                var scenarioData = new[]
                {
                    new
                    {
                        Context = "friends",
                        ActorCount = 4,
                        Difficulty = "hard",
                        Scenario = @"Četvoro prijatelja su na zajedničkom putovanju. Elena je organizovala detaljan plan: muzeji ujutru, ručak u rezervisanom restoranu, razgledanje popodne. Prvog dana, Katarina predlaže da preskoče muzej i odu na pijacu. Elena kaže: ""Ali već imam karte."" Dimitrije kaže: ""Možemo da se podelimo."" Elena odgovara: ""Onda nema smisla putovati zajedno."" Nada, koja je dotad ćutala, predlaže: ""Hajde u muzej do podneva pa onda na pijacu."" Elena pristaje ali je vidno napeta. Drugog dana, Katarina i Dimitrije odlaze sami na doručak bez da obaveste Elenu i Nadu. Elena je uznemirena. Nada pokušava da je smiri: ""Nisu mislili ništa loše."" Elena odgovara: ""Ti uvek braniš sve osim mene.""",
                        Reveal = @"Elena je uložila dvadeset sati u planiranje putovanja, čitala recenzije, pravila tabele, rezervisala karte. Za nju, plan nije logistika nego poklon grupi i izraz ljubavi. Kada Katarina predloži promenu, Elena ne čuje ""hajde na pijacu"" već ""tvoj trud ne vredi."" Ovo je duboko povezano sa njenim obrascem iz detinjstva: bila je dete koje je uvek organizovalo igre, a kada bi drugi prestali da se igraju po njenim pravilima, osećala bi se odbačena. Katarina je spontana osoba koja doživljava detaljan plan kao zatvor. Nije svesna koliko je rada Elena uložila jer ona nikada ne bi planirala na taj način, potkontekst ""preskoči muzej"" je za nju trivijalan, za Elenu egzistencijalan. Dimitrije je introvert kome je potrebno jutarnje vreme u tišini pre socijalizacije. Jutarnji doručak sa Katarinom (koja ne zahteva razgovor) bio je njegov način da napuni bateriju, ne da isključi Elenu i Nadu. Nada je hronični mirotvorac koji izbegava konflikt po svaku cenu. Njen predlog kompromisa u muzeju bio je funkcionalan, ali obrazac stalnog umirenja Elene čini da se Elena oseća kao da je njena ljutnja nelegitimna. Elenina rečenica ""ti braniš sve osim mene"" otkriva godina nakupljene frustracije jer Nada nikada ne validira Elenina osećanja pre nego što pokuša da ih reši. Konačno, svo četvoro su putovali noćnim letom i spavali manje od pet sati. Nedostatak sna smanjuje kapacitet za regulaciju emocija i za tumačenje tuđih namera u pozitivnom svetlu, svaka od ovih interakcija bi verovatno bila blaža da su svi odmorni.",
                        Questions = new[]
                        {
                            new { Question = @"Šta se nalazi ispod Elenine potrebe za kontrolom plana i zašto je doživljava kao pitanje odnosa?", QuestionType = "osećanje" },
                            new { Question = @"Šta Katarina i Dimitrije ne razumeju o tome kako Elena tumači njihovo ponašanje?", QuestionType = "nevidljivi kontekst" },
                            new { Question = @"Zašto je Nadina uloga mirotvorke zapravo problematična, iako izgleda konstruktivno?", QuestionType = "razmišljanje" },
                        }
                    },
                    new
                    {
                        Context = "friends",
                        ActorCount = 4,
                        Difficulty = "hard",
                        Scenario = @"U grupnom četu četvoro prijatelja, Tamara šalje link na članak o mentalnom zdravlju sa komentarom: ""Ovo je baš pogodilo."" Pavle odgovara emoji-jem palca gore. Nataša piše: ""Preklinjem vas da ne pretvaramo ovo u terapijsku grupu 😂."" Uroš ne odgovara uopšte. Tamara briše svoju poruku petnaest minuta kasnije. Sutradan, Pavle privatno piše Tamari: ""Jesi li dobro?"" Tamara odgovara: ""Da, sve super.""",
                        Reveal = @"Tamara prolazi kroz period teškoće i link je bio njen način da indirektno otvori temu. Nije bila spremna da kaže ""loše mi je"" ali je htela da vidi da li će neko prepoznati signal. Natašina šala je bila devastirajuća jer je Tamara to čula kao: ""Tvoja osećanja su pretežak teret za ovo društvo."" Nataša zapravo koristi humor kao vlastiti odbrambeni mehanizam. I sama se bori sa anksioznošću ali je naučila da distancira emocije kroz šalu. Njen komentar je bio o njenom sopstvenom nelagodnosti, ne o Tamari, ali efekat je bio isti. Uroš čita poruke ali nikada ne reaguje na emocionalno nabijene teme jer je odrastao u porodici gde se o emocijama nije govorilo. Bukvalno ne zna kako da odgovori i bira tišinu misleći da je to neutralno. Pavle je jedini koji je pročitao situaciju ispravno. Prepoznao je da brisanje poruke znači povlačenje, ali je i njegov pristup nepotpun: pitao je privatno umesto da u grupi normalizuje Tamarin post, čime bi poslao signal da je takav razgovor dobrodošao.",
                        Questions = new[]
                        {
                            new { Question = @"Šta je Tamara htela da postigne slanjem linka i kako je protumačila reakcije?", QuestionType = "osećanje" },
                            new { Question = @"Šta stoji iza Natašine šale i šta ona ne shvata o njenom efektu?", QuestionType = "nevidljivi kontekst" },
                            new { Question = @"Zašto Uroš ćuti i šta njegovo ćutanje znači u kontekstu grupne dinamike?", QuestionType = "razmišljanje" },
                            new { Question = @"Šta Pavlova privatna poruka otkriva o njegovom razumevanju situacije?", QuestionType = "nevidljivi kontekst" },
                        }
                    },
                    new
                    {
                        Context = "family",
                        ActorCount = 4,
                        Difficulty = "hard",
                        Scenario = @"Branko i Sanja ugošćuju Brankovog brata Gorana i njegovu suprugu Vesnu za večeru. Tokom razgovora, Goran priča o skupom renoviranju kuće. Branko čestita, ali Sanja postaje tiha. Kasnije, Vesna pominje da su upisali decu u privatnu školu. Sanja odlazi u kuhinju ""da proveri desert."" Branko je prati i šapuće: ""Možeš li bar da se pretvaraš?"" Sanja odgovara: ""Ja se pretvaramo celo veče."" Vraćaju se za sto sa osmehom.",
                        Reveal = @"Sanja i Branko su prošle godine imali ozbiljnu finansijsku krizu, Brankov biznis je bio pred zatvaranjem. Jedva su se izvukli ali su morali da prodaju vikendicu i da se odreknu planiranog putovanja. Sanja ne zavidi Goranu i Vesni na bogatstvu. Ono što je boli je što Branko ne priznaje koliko im je teško. Pred bratom, Branko glumi da je sve savršeno. Sanjina frustracija je usmerena ka Branku, ne ka gostima: on od nje traži da učestvuje u performansu koji ona smatra nedostojnim. Goran zapravo priča o renoviranju jer je nervozan. On zna za bratovu finansijsku situaciju i popunjava tišinu temama na kojima se oseća sigurno. Vesna pominje privatnu školu jer joj je to jedina tema o kojoj trenuto razmišlja, ne shvatajući kako to zvuči u datom kontekstu. Niko za stolom ne govori ono što zapravo misli.",
                        Questions = new[]
                        {
                            new { Question = @"Šta konkretno uzrokuje Sanjinu reakciju, da li je zavist, bol, ili nešto treće?", QuestionType = "osećanje" },
                            new { Question = @"Šta Branko ne razume o Sanjinoj perspektivi kada joj kaže da se pretvara?", QuestionType = "nevidljivi kontekst" },
                            new { Question = @"Kako Goran i Vesna verovatno tumače atmosferu na večeri?", QuestionType = "razmišljanje" },
                        }
                    },
                    new
                    {
                        Context = "friends",
                        ActorCount = 3,
                        Difficulty = "hard",
                        Scenario = @"Dušan, Marija i Nemanja su zajedno počeli da treniraju u teretani pre šest meseci. Dušan je brzo napredovao i počeo da daje savete Mariji i Nemanji bez da su tražili. ""Trebalo bi da promeniš formu,"" ""Jedi više proteina."" Marija je počela da dolazi u drugom terminu. Nemanja dolazi u isto vreme ali stavlja slušalice čim uđe. Dušan je zbunjen i žali se zajedničkom prijatelju: ""Pomagao sam im a oni me izbegavaju.""",
                        Reveal = @"Marija se ceo život bori sa telesnom slikom. Teretana je za nju bila veliki korak. Mesto gde je htela da se oseća sigurno, ne procenjivano. Dušanovi saveti, koliko god dobro namerni, aktivirali su njen osećaj da je nešto pogrešno sa njom. Prebacila se u drugi termin da bi sačuvala teretanu kao bezbedni prostor. Nemanja nema problem sa samopouzdanjem. Njemu smeta kontrola. On ceni autonomiju i ne voli kada mu neko govori šta da radi, čak i kad je savet korektan. Slušalice su mu način da postavi granicu bez konflikta. Dušan je čovek čiji je primarni jezik ljubavi davanje saveta. U njegovoj porodici, briga se pokazivala kroz ispravljanje i usmeravanje. On iskreno ne razume da njegovi saveti, iako tačni, nisu traženi i da se primaju kao kritika umesto kao podrška.",
                        Questions = new[]
                        {
                            new { Question = @"Zašto Marija ima potrebu da skroz promeni termin?", QuestionType = "nevidljivi kontekst" },
                            new { Question = @"Šta je kod Nemanje drugačije da, bez promene termina, može da se izbori sa Dušanom?", QuestionType = "nevidljivi kontekst" },
                            new { Question = @"Zašto Dušan ima potrebu da deli savete?", QuestionType = "nevidljivi kontekst" },
                        }
                    },
                    new
                    {
                        Context = "work",
                        ActorCount = 3,
                        Difficulty = "medium",
                        Scenario = @"Goran objavljuje da je Viktor dobio unapređenje. Tijana, koja je u firmi dve godine duže od Viktora, čestita Viktoru kratko i odlazi na pauzu. Kolege primećuju da je Tijana ostatak dana bila šutljiva. Viktor prilazi Tijani i kaže: ""Nadam se da ti je okej, ti si me svemu naučila."" Tijana odgovara: ""Zaslužio si, svaka čast."" Sutradan Tijana šalje mejl HR odeljenju sa pitanjem o internim procedurama za unapređenje.",
                        Reveal = @"Tijana ne sumnja u Viktorove sposobnosti. Ono što je boli je sistemsko: dva puta je bila predložena za unapređenje i oba puta joj je rečeno ""sledeći ciklus."" Njen mejl HR-u nije osveta. To je pokušaj da razume pravila igre koja joj se čine netransparentna. Tiho sumnja da je Viktor napredovao brže jer je društveniji, češće igra basket sa Goranom i vidljiviji je na sastancima. Tijana je osoba koja radi dubinski posao koji je manje vidljiv. Viktor iskreno misli da je Tijana srećna zbog njega jer ga je zaista mentorski podržavala. Ne vidi da je upravo ta mentorska dinamika ono što čini situaciju bolnom. Viktor koristi privilegiju bliskosti sa menadžerom a da toga nije svestan.",
                        Questions = new[]
                        {
                            new { Question = @"Šta Tijana zapravo oseća uprkos onome što govori?", QuestionType = "osećanje" },
                            new { Question = @"Šta Viktor ne vidi u dinamici između sebe i Tijane?", QuestionType = "nevidljivi kontekst" },
                        }
                    },
                    new
                    {
                        Context = "family",
                        ActorCount = 2,
                        Difficulty = "medium",
                        Scenario = @"Aleksa i Mina razgovaraju o planovima za Novu godinu. Mina predlaže da ove godine proslave sami kod kuće. Aleksa oklevajući kaže: ""Hajde da pitamo mamu, ona je sama otkad je tata umro."" Mina odgovara: ""Svake godine isto. Nikada nismo sami."" Aleksa povišenim tonom: ""Šta, da je ostavim samu na Novu godinu?"" Razgovor se završava ćutanjem.",
                        Reveal = @"Mina ne mrzi Aleksinu mamu niti želi da je isključi. Problem je dublji: u četiri godine braka, Mina i Aleksa nijedan praznik nisu proveli kao par bez proširene porodice. Mina oseća da njihov brak nema prostora da izgradi sopstvene rituale i identitet. Njen zahtev ""da budemo sami"" je zapravo zahtev: ""Da li mi kao par postojimo nezavisno?"" Aleksa to ne čuje jer je zarobljen u ulozi sina koji brine o majci, ulozi koju je preuzeo kad mu je otac umro pre tri godine. On interpretira svaki Minin zahtev za odvojenošću kao napad na majku, a zapravo je to Minin poziv za bliskošću u braku.",
                        Questions = new[]
                        {
                            new { Question = @"Šta Mina zapravo želi da kaže ali ne uspeva da artikuliše?", QuestionType = "osećanje" },
                            new { Question = @"Šta Aleksa ne vidi u Mininoj perspektivi jer je fokusiran na majku?", QuestionType = "nevidljivi kontekst" },
                        }
                    },
                    new
                    {
                        Context = "friends",
                        ActorCount = 2,
                        Difficulty = "medium",
                        Scenario = @"Ivana je organizovala proslavu rođendana u restoranu i pozvala dvadeset prijatelja. Milica, njena najbliža prijateljica, potvrdila je dolazak ali se nije pojavila. Nije poslala poruku tokom večeri. Sutradan je napisala: ""Izvini, nisam mogla."" Ivana joj nije odgovorila.",
                        Reveal = @"Milica pati od socijalne anksioznosti koja se pojačala poslednjih meseci. Ispred restorana je stajala petnaest minuta ali nije mogla da uđe, srce joj je lupalo, ruke su se tresle. Otišla je kući osećajući se poniženo. Poruku ""nisam mogla"" je napisala bukvalno. Nije bila u stanju, ne da nije htela. Ne priča o anksioznosti jer se plaši da će je prijatelji smatrati čudnom.",
                        Questions = new[]
                        {
                            new { Question = @"Šta Ivana verovatno pretpostavlja o Milicinom izostanku?", QuestionType = "razmišljanje" },
                            new { Question = @"Šta bi moglo da stoji iza reči ""nisam mogla"", što bi opravdalo Miličin izostanak?", QuestionType = "nevidljivi kontekst" },
                        }
                    },
                    new
                    {
                        Context = "friends",
                        ActorCount = 2,
                        Difficulty = "medium",
                        Scenario = @"Vuk i Sara su na večeri u restoranu. Sara priča o svom nedavnom putovanju u Grčku, opisuje plaže, hranu i ljude koje je upoznala. Vuk sluša, klimne glavom par puta, ali ne postavlja nijedno pitanje. Kad Sara završi, Vuk menja temu na fudbal. Sara kaže: ""Zanimljivo"" i otvara telefon.",
                        Reveal = @"Vuk nije nezainteresovan za Sarin život. On pati od poremećaja pažnje (ADD) koji mu otežava praćenje dugih narativnih priča. Dok je Sara pričala, on se iskreno trudio da sluša, ali mu je pažnja stalno klizila. Promenio je temu ne zato što ga put nije zanimao, već jer je osećao anksioznost zbog toga što ne može da prati. Fudbal je tema koja zahteva kratke razmene, gde se oseća kompetentnim u razgovoru.",
                        Questions = new[]
                        {
                            new { Question = @"Šta Sara verovatno oseća posle Vukove reakcije?", QuestionType = "osećanje" },
                            new { Question = @"Šta sprečava Vuka, koji stvarno prati razgovor, da se angažuje sa Sarinom pričom?", QuestionType = "nevidljivi kontekst" },
                        }
                    },
                    new
                    {
                        Context = "work",
                        ActorCount = 1,
                        Difficulty = "easy",
                        Scenario = @"Tamara rukovodi timom od šest ljudi. Na prvom sastanku je bila kratka, govorila je isključivo o zadacima i rokovima, nije pitala nikoga kako se oseća niti se predstavila lično. Kada je jedan kolega pokušao da napravi šalu, Tamara se nasmešila ali je odmah nastavila dalje. Posle sastanka, članovi tima su komentarisali da je ""hladna"" i ""roboti imaju više emocija.""",
                        Reveal = @"Tamara je introvert koja je dobila jasnu povratnu informaciju od svog prethodnog šefa: ""Previše se opuštaš sa timom i gubiš autoritet."" Odlučila je da na novom mestu postavi jasne profesionalne granice od početka. Kod kuće je bila nervozna celu noć pre sastanka i vežbala šta će reći. Njena krutost nije ravnodušnost. To je pažljivo isplanirana strategija osobe koja se plaši da ne pogreši.",
                        Questions = new[]
                        {
                            new { Question = @"Zašto je Tamara tako pristupila prvom sastanku?", QuestionType = "nevidljivi kontekst" },
                        }
                    },
                    new
                    {
                        Context = "family",
                        ActorCount = 2,
                        Difficulty = "easy",
                        Scenario = @"Za Gocin rođendan, njena ćerka Maja je kupila skupu kremu za lice koju je videla u reklami. Kada je Goca otvorila poklon, kratko je rekla ""Hvala"" i odložila kutiju u stranu. Nastavila je da razgovara sa drugim gostima. Maja je primetila da Goca danima kasnije nije otvorila kutiju.",
                        Reveal = @"Goca pripada generaciji koja smatra da su skupe kozmetičke kreme nepotrebna rasipnost. Ceo život koristi domaće preparate i ponosna je na to. Poklon je protumačila kao suptilnu kritiku njenog izgleda, kao da joj ćerka poručuje da joj je potrebna pomoć da izgleda mlađe. Nije bila nezahvalna, već povređena.",
                        Questions = new[]
                        {
                            new { Question = @"Šta Gordana verovatno oseća u vezi sa poklonom?", QuestionType = "osećanje" },
                        }
                    },
                    new
                    {
                        Context = "work",
                        ActorCount = 2,
                        Difficulty = "easy",
                        Scenario = @"Dejan razgovara na telefon u zajedničkom radnom prostoru. Govori glasno i živo, smeje se i gestikulira. Ana, koja sedi dva stola dalje, stavlja slušalice i pomera se bliže prozoru. Kada je Dejan završio poziv i pitao Anu nešto u vezi projekta, odgovorila mu je hladno u jednoj rečenici, bez kontakta očima. Dejan je zbunjen ovom reakcijom.",
                        Reveal = @"Dejan je odrastao u velikoj i glasnoj porodici gde je visok nivo buke bio znak topline i bliskosti. Njegova glasnoća nije svesna odluka. To je njegov podrazumevani način komunikacije koji doživljava kao prirodan. Nikada mu niko na poslu nije rekao da je preglasan jer svi pretpostavljaju da on to zna i da mu je svejedno.",
                        Questions = new[]
                        {
                            new { Question = @"Zašto Dejan ne primećuje negativan uticaj ovog ponašanja na okolinu?", QuestionType = "nevidljivi kontekst" },
                        }
                    },
                    new
                    {
                        Context = "family",
                        ActorCount = 2,
                        Difficulty = "easy",
                        Scenario = @"Petar dolazi kod roditelja na nedeljni ručak. Za stolom priča o tome kako razmišlja da promeni posao i upiše kurs programiranja. Otac Dragan ga prekida: ""Ti imaš stabilan posao, nemoj da budeš dete."" Petar ućuti, a ostatak ručka prolazi u tišini.",
                        Reveal = @"Dragan je ceo radni vek proveo na istom radnom mestu jer nikada nije imao mogućnost izbora. Preživeo je dva talasa otpuštanja i živi u stalnom strahu od finansijske nesigurnosti. Kada Petar kaže da želi da napusti siguran posao, kod Dragana se aktivira duboki strah. Ne za sebe, već za sina. Njegov oštar ton nije prezir, već panika prerušena u autoritet.",
                        Questions = new[]
                        {
                            new { Question = @"Šta Dragan zapravo oseća ispod svoje oštre reakcije?", QuestionType = "osećanje" },
                            new { Question = @"Šta je Dragan proživeo da mu ovakva ideja izaziva to osećanje?", QuestionType = "nevidljivi kontekst" },
                        }
                    },
                };

                var challenges = scenarioData.Select(scenario =>
                {
                    var challengeLevel = scenario.Difficulty.Trim().ToLowerInvariant() switch
                    {
                        "easy" => ChallengeLevel.Easy,
                        "medium" => ChallengeLevel.Medium,
                        "hard" => ChallengeLevel.Hard,
                        _ => ChallengeLevel.Medium
                    };

                    var questions = scenario.Questions
                        .Select(q =>
                        {
                            var skill = MapSkill(q.QuestionType);
                            return (Guid.NewGuid(), skill.Id, q.Question);
                        })
                        .ToList();

                    return new PerspectiveScenarioChallenge(
                        Guid.NewGuid(),
                        MapContext(scenario.Context),
                        scenario.ActorCount,
                        scenario.Scenario,
                        scenario.Reveal,
                        challengeLevel,
                        questions);
                }).ToList();

                context.PerspectiveScenarioChallenges.AddRange(challenges);
            }

            await context.SaveChangesAsync();
        }
    }
}
