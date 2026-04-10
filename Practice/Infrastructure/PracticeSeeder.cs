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
