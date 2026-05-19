using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AngularNetBase.Practice.Infrastructure.Migrations
{
    [DbContext(typeof(PracticeContext))]
    [Migration("20260519170000_AlignSkillEndMessagesWithSheet")]
    public partial class AlignSkillEndMessagesWithSheet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE practice."GrowthMessages" gm
                SET "Text" = v."Text",
                    "IsActive" = TRUE
                FROM (VALUES
                    ('90000000-0000-0000-0000-000000000001'::uuid, 'Kad nas preplavi emocija, teško možemo čuti drugu osobu i to dovodi do nesporazuma i svađe. Distanciranjem vraćamo pažnju i strpljenje. Lakše oprostimo, ređe povredimo nekoga u besu i bolje razumemo ljude do kojih nam je stalo.'),
                    ('90000000-0000-0000-0000-000000000002'::uuid, 'Impulsivne odluke retko su dobre. Ova tehnika vraća jasnoću u momentima kad je najteže doći do nje: u sukobu, na pregovorima, pred važnim izborom. Većina ljudi tada reaguje iz afekta. Ako ostanemo hladne glave tada, više ćemo izvući iz situacije.'),
                    ('90000000-0000-0000-0000-000000000003'::uuid, 'Nije retko da ponavljamo iste greške u afektu, gde zbog nekog ponašanja partnera ili kolega reagujemo kako ne bismo želeli. Kroz ovu tehniku, možemo obraditi emociju pre nego što izazove reakciju koju ne želimo i odlučiti kako ćemo odgovoriti, umesto da situacija odluči za nas.'),
                    ('90000000-0000-0000-0000-000000000004'::uuid, 'Lako se zamerimo ljudima kada izgovaramo reči u afektu i donosimo odluke pod stresom. Ova tehnika smanjuje verovatnoću takvih grešaka i čuva ono što smo izgradili: odnose, ugled, mir u svakodnevnom životu.'),

                    ('90000000-0000-0000-0000-000000000005'::uuid, 'Ovaj pogled pomaže da ne žrtvujemo dugoročne odnose zbog trenutne emocije.'),
                    ('90000000-0000-0000-0000-000000000006'::uuid, 'Naše vreme i energija su ograničen resurs. Ako nas stalno pritiskaju gorući problemi, neće nam ostati snage da radimo na dugoročnim ciljevima. Vremensko distanciranje nam pomaže da razdvojimo ono što je stvarno bitno od buke koja u trenutku deluje važna, ali nas samo troši bez koristi.'),
                    ('90000000-0000-0000-0000-000000000007'::uuid, 'Kada smo zarobljeni u trenutnim stvarima, izgubimo kontrolu nad svojim mislima.'),
                    ('90000000-0000-0000-0000-000000000008'::uuid, 'U afektu možemo doneti odluke koje će nas koštati. Tako možemo uskratiti sebi buduće prilike ili se zameriti ljudima do kojih nam je stalo. Vremenska distanca stavlja stvari u perspektivu i pomaže nam da izbegnemo ove scenarije.'),

                    ('90000000-0000-0000-0000-000000000009'::uuid, 'Kad se pogledamo spolja, vidimo kako naše ponašanje deluje na ljude oko nas. To nije uvek ugodna stvar, ali je korisna kako bismo bili nežniji prema sebi i drugima. Oni do kojih nam je stalo će osetiti promenu naše energije.'),
                    ('90000000-0000-0000-0000-000000000010'::uuid, 'Tek kada izađemo iz sebe, vidimo kakve su stvarno mogućnosti pred nama. Bez toga igramo lošije nego što bismo mogli.'),
                    ('90000000-0000-0000-0000-000000000011'::uuid, 'Kada smo zaglavljeni "u svojoj glavi", svet deluje ograničeno. Ne deluje da možemo da izgladimo neki odnos ili da imamo taj težak razgovor sa kolegom ili partnerom. Iz tuđe perspektive vidimo da su ove stvari ipak moguće i da nisu toliko strašne, što nam daje moć da delujemo i prevaziđemo prepreku.'),
                    ('90000000-0000-0000-0000-000000000012'::uuid, 'Da bismo očuvali zdrave odnose, potrebno je da štitimo svoje granice i da uvažavamo tuđe. Posmatračka perspektiva nam pomaže da jasnije vidimo situaciju i time očuvamo sopstveni i društveni mir.'),

                    ('90000000-0000-0000-0000-000000000013'::uuid, 'Možda nam je lakše da budemo brižni i strpljivi sa drugima jer nas ispunjava da ih služimo na ovaj način. Na toj liniji je dobro da se bolje brinemo o sebi, jer ćemo imati više da damo drugima.'),
                    ('90000000-0000-0000-0000-000000000014'::uuid, 'Naš ego može biti velika prepreka dobrom rasuđivanju, naročito pod pritiskom. Pristup "šta bih rekao prijatelju" zaobilazi ego, vraća nam kvalitet razmišljanja koji već imamo i omogućava nam da povučemo bolje poteze.'),
                    ('90000000-0000-0000-0000-000000000015'::uuid, 'Često namećemo sebi strožije standarde koji nas ograničavaju, dok smo prema prijatelju nežniji i ostavljamo prostor za veću slobodu. Ovom vežbom treniramo da budemo dobar prijatelj prema sebi i da se oslobodimo pritiska koji sebi namećemo.'),
                    ('90000000-0000-0000-0000-000000000016'::uuid, 'Glas kojim savetujemo dragu osobu je često smiren i razuman. Primenjen na sebe, taj isti glas nas štiti i usmerava da donesemo odluke posle kojih se ne kajemo.'),

                    ('90000000-0000-0000-0000-000000000017'::uuid, 'Ako verujemo da već znamo sve o nekoj osobi ili temi, često ćemo blokirati konekciju sa drugim. Kalibracija sopstvenog znanja otvara prostor za stvaran razgovor, naročito sa onima čije se iskustvo razlikuje od našeg.'),
                    ('90000000-0000-0000-0000-000000000018'::uuid, 'Prevelika sigurnost u sopstvene procene nas ograničava. Ako ne raspolažemo sa tačnim podacima, pravićemo loše odluke. Ako se suviše zalažemo za pogled koji je pogrešan, ljudi će nam manje verovati. U interesu nam je da dođemo do istine uz pomoć drugih, kako bismo postigli naše ciljeve.'),
                    ('90000000-0000-0000-0000-000000000019'::uuid, 'Uverenje da "već znam" gasi radoznalost. Prepoznavanje granice svog znanja otvara pitanja, a odgovori na ta pitanja nam proširuju sliku sveta i otkrivaju nove mogućnosti koje nismo ni znali da postoje.'),
                    ('90000000-0000-0000-0000-000000000020'::uuid, 'Stabilnost ne dolazi sa uverenošću da sve znamo što treba, već sa dobrom slikom o tome šta znamo, a šta ne. Kada znamo svoje granice, nismo zatečeni ili dovedeni u neprijatnu situaciju pred drugima.'),

                    ('90000000-0000-0000-0000-000000000021'::uuid, 'Voleli bismo da razumemo prijatelje i članove naše porodice i želeli bismo da oni razumeju nas. Ponekad mi ili oni ispoljavamo ponašanje koje nas vodi u prazne razgovore ili nepotrebne rasprava koje nas udaljavaju. Sa ovom veštinom možemo prepoznati ove šablone, promeniti tok razgovora i produbiti naše odnose.'),
                    ('90000000-0000-0000-0000-000000000022'::uuid, 'Sposobnost predviđanja više mogućih ishoda nam daje moć. Umesto da slepo reagujemo na ono što je pred nama, analiziramo kako se situacija može razviti i povlačimo poteze koji će povećati šansu da postignemo ishod koji nam je privlačan.'),
                    ('90000000-0000-0000-0000-000000000023'::uuid, 'Kad prihvatimo da se puno toga može promeniti u zavisnosti od naših poteza, otvaramo velik prostor za delovanje. Nismo zarobljeni jednim ishodom, već imamo mnogo slobode da krojimo kako će sledeći nedeljni ručak, razgovor sa partnerom ili sastanak sa timom da izgleda.'),
                    ('90000000-0000-0000-0000-000000000024'::uuid, 'Kada nas čeka zahtevan razgovor sa šefom, partnerom ili roditeljem, možda zamišljamo situaciju koja nas plaši i opterećuje. Razmišljanje o više mogućih ishoda nam pomaže da sagledamo ružne ishode, promislimo kako ćemo delovati da ih prevaziđemo i povećamo šansu da ćemo dobiti dobar ishod.'),

                    ('90000000-0000-0000-0000-000000000025'::uuid, 'Nije dovoljno želeti dobro. Da bi naša podrška bila korisna, moramo razumeti gde se ta osoba zaista nalazi, šta vidi i čega se boji. Bez toga pomažemo prema sopstvenim pretpostavkama, a ne prema stvarnoj potrebi.'),
                    ('90000000-0000-0000-0000-000000000026'::uuid, 'U pregovorima, rukovođenju i svakom razgovoru gde postoji ulog, sposobnost sagledavanja tuđeg ugla je preduslov za uspeh. Ako ne razumemo šta druga strana zaista hoće i čega se plaši, teško ćemo doći do rešenja koje odgovara svima. Možemo nadjačati drugog i izvući kratkoročnu pobedu, ali je pravi izazov i dugoročni uspeh izvući obostranu dobit.'),
                    ('90000000-0000-0000-0000-000000000027'::uuid, 'Živimo jedan život, ali kroz svesno zamišljanje kako svet izgleda nekome drugom, dobijamo uvid koji inače nije dostupan. Duboko sagledavanje sveta kroz tuđe oči  je značajan način da proširimo sopstveni svet.'),
                    ('90000000-0000-0000-0000-000000000028'::uuid, 'Većina nepotrebnih svađa nastaje od pogrešne pretpostavke o tome šta je neko mislio ili nameravao. Kad zamislimo tuđi ugao pre nego što reagujemo, smanjujemo te greške. Svakodnevni život, u porodici, na poslu, u komšiluku, postaje mirniji.'),

                    ('90000000-0000-0000-0000-000000000029'::uuid, 'Sposobnost pronalaska rešenja sa kojim su svi zadovoljni donosi kratkoročno zadovoljstvo i dugoročno cvetanje svih koji su uključeni. Sa takvim pristupom porodice sijaju, timovi postižu neverovatne rezultate i zajednice su kohezivne i prijatne.'),
                    ('90000000-0000-0000-0000-000000000030'::uuid, 'Kompromis deli pitu. Integracija povećava pitu. Ko ume da iz dva sukobljena predloga izvuče rešenje koje je bolje od oba, poseduje retku veštinu. U poslu, u pregovorima, u timu, to je razlika između onoga ko gasi sukobe i onoga ko ih rešava.'),
                    ('90000000-0000-0000-0000-000000000031'::uuid, 'Spajanje ideja koje se čine nepomirljivima zahteva kreativnost, jer pravimo nešto novo umesto da biramo od postojećih rešenja. Kroz ove vežbe razvijamo tu sposobnost i otvaramo sebi mogućnosti koje ranije nismo imali.'),
                    ('90000000-0000-0000-0000-000000000032'::uuid, 'Rešenja u kojima jedna strana popušta bez stvarnog razrešenja imaju kratak vek i sukob se vraća. Sabiranje želja i potreba suprostavljenih strana dovodi do dogovora koji traju. To smanjuje ponavljanje istih konflikata u porodici, na poslu i u zajednici.'),

                    ('90000000-0000-0000-0000-000000000033'::uuid, 'Ako smatramo da je naše viđenje jednako stvarnosti, sve što neko drugačiji misli automatski postaje greška ili loša namera. Tek kad vidimo da i mi gledamo kroz sopstveni okvir, postaje moguće prihvatiti da je neko drugi legitimno drugačiji i tada nastaje međusobno razumevanje.'),
                    ('90000000-0000-0000-0000-000000000034'::uuid, 'Slepe tačke su jedan od najčešćih izvora skupih grešaka. Ko zna da vidi van sopstvenog okvira, počinje da pita gde taj okvir možda ne odgovara situaciji. To je način da sprečite propuste i više postignete.'),
                    ('90000000-0000-0000-0000-000000000035'::uuid, 'Ono što doživljavamo kao "očigledno kako stvari stoje" obično je samo jedan pogled. Kad to vidimo, nismo više zarobljeni u njemu. Možemo birati drugačiji ugao, eksperimentisati s tumačenjima i slobodnije gledati svet.'),
                    ('90000000-0000-0000-0000-000000000036'::uuid, 'Mnoge loše odluke donose se zato što smo sigurni da je nešto "jasno", a ne vidimo dobro. Navika propitivanja sopstvenog okvira smanjuje šansu da ćemo se zameriti drugima ili se neprijatno iznenaditi kada se realnost ispostavi drugačija od našeg okvira.')
                ) AS v("Id", "Text")
                WHERE gm."Id" = v."Id";
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
