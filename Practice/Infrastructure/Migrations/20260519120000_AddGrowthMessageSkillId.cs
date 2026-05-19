using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AngularNetBase.Practice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGrowthMessageSkillId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SkillId",
                schema: "practice",
                table: "GrowthMessages",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GrowthMessages_SkillId",
                schema: "practice",
                table: "GrowthMessages",
                column: "SkillId");

            migrationBuilder.AddForeignKey(
                name: "FK_GrowthMessages_Skills_SkillId",
                schema: "practice",
                table: "GrowthMessages",
                column: "SkillId",
                principalSchema: "practice",
                principalTable: "Skills",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.Sql(
                """
                INSERT INTO practice."Skills" ("Id", "Name", "Description")
                VALUES
                    ('a1111111-1111-1111-1111-111111111111', 'Narrative distancing', 'Actively shifting the linguistic frame when reflecting on one''s own experience using third-person pronouns and one''s own name rather than "I" and "me."'),
                    ('a2222222-2222-2222-2222-222222222222', 'Temporal distancing', 'Projecting a current situation forward in time and reasoning from the perspective of a future self (e.g., one or five years out).'),
                    ('a3333333-3333-3333-3333-333333333333', 'Observer framing', 'Mentally stepping outside one''s own body and viewing the current situation from an external visual vantage (e.g., the "fly on the wall" or "scene from across the room" perspective).'),
                    ('a4444444-4444-4444-4444-444444444444', 'Advisor simulation', 'Treating one''s own situation as though it were a close friend''s and generating the advice one would give to that friend.'),
                    ('a5555555-5555-5555-5555-555555555555', 'Epistemic calibration', 'The ability to epistemically map what one does and does not know, held without ego-threat.'),
                    ('a6666666-6666-6666-6666-666666666666', 'Prospective reasoning', 'The ability to consider multiple ways an interpersonal situation may unfold.'),
                    ('a7777777-7777-7777-7777-777777777777', 'Viewpoint simulation', 'Active consideration of how others involved in a situation see, feel, and reason through it.'),
                    ('a8888888-8888-8888-8888-888888888888', 'Perspective integration', 'Active effort to synthesize competing viewpoints into a coherent, higher-order understanding and a resolution that addresses the core interests behind each position.'),
                    ('a9999999-9999-9999-9999-999999999999', 'Frame recognition', 'Noticing that one''s read of a situation reflects a particular vantage point rather than reality as it is.')
                ON CONFLICT ("Id") DO NOTHING;
                """);

            migrationBuilder.Sql(
                """
                INSERT INTO practice."GrowthMessages" ("Id", "Text", "Type", "IsActive", "SkillId")
                SELECT v."Id", v."Text", 'End', TRUE, v."SkillId"
                FROM (VALUES
                    ('90000000-0000-0000-0000-000000000001'::uuid, 'a1111111-1111-1111-1111-111111111111'::uuid, 'U zadatku ste pisali o sopstvenom iskustvu u trećem licu. Istraživanja pokazuju da kada situaciju sagledavamo kao da nije naša, naš mozak je drugačije obrađuje i vidimo je objektivnije.'),
                    ('90000000-0000-0000-0000-000000000002'::uuid, 'a1111111-1111-1111-1111-111111111111'::uuid, 'Kad nas preplavi emocija, teško možemo čuti drugu osobu. Distanciranjem vraćamo pažnju i strpljenje i lakše biramo reakciju.'),
                    ('90000000-0000-0000-0000-000000000003'::uuid, 'a1111111-1111-1111-1111-111111111111'::uuid, 'Impulsivne odluke retko su dobre. Ova tehnika vraća jasnoću u momentima kada je najteže doći do nje.'),
                    ('90000000-0000-0000-0000-000000000004'::uuid, 'a1111111-1111-1111-1111-111111111111'::uuid, 'Ova veština čuva odnose, ugled i mir u svakodnevnom životu jer smanjuje reakcije iz afekta.'),

                    ('90000000-0000-0000-0000-000000000005'::uuid, 'a2222222-2222-2222-2222-222222222222'::uuid, 'U zadatku ste zamišljali kako će ova situacija izgledati za mesec ili godinu. Vremenska distanca pomaže da prepoznamo šta je zaista važno dugoročno.'),
                    ('90000000-0000-0000-0000-000000000006'::uuid, 'a2222222-2222-2222-2222-222222222222'::uuid, 'Vremensko distanciranje odvaja stvarno bitno od buke koja u trenutku deluje hitno, a samo nas troši.'),
                    ('90000000-0000-0000-0000-000000000007'::uuid, 'a2222222-2222-2222-2222-222222222222'::uuid, 'Kad pogledamo dalje od ovog trenutka, lakše izbegnemo odluke koje nas kasnije skupo koštaju.'),
                    ('90000000-0000-0000-0000-000000000008'::uuid, 'a2222222-2222-2222-2222-222222222222'::uuid, 'Ova veština smanjuje trenutni teret i povećava šansu da postupimo mudro.'),

                    ('90000000-0000-0000-0000-000000000009'::uuid, 'a3333333-3333-3333-3333-333333333333'::uuid, 'U zadatku ste sagledali situaciju iz perspektive neutralnog posmatrača. To smanjuje upliv ličnih emocija i daje jasniju sliku.'),
                    ('90000000-0000-0000-0000-000000000010'::uuid, 'a3333333-3333-3333-3333-333333333333'::uuid, 'Kad se pogledamo spolja, bolje vidimo kako naše ponašanje utiče na druge i gde možemo da korigujemo kurs.'),
                    ('90000000-0000-0000-0000-000000000011'::uuid, 'a3333333-3333-3333-3333-333333333333'::uuid, 'Posmatračka perspektiva često otkrije mogućnosti koje ne vidimo kada smo zaglavljeni u sopstvenoj glavi.'),
                    ('90000000-0000-0000-0000-000000000012'::uuid, 'a3333333-3333-3333-3333-333333333333'::uuid, 'Ova veština pomaže da očuvamo sopstveni i društveni mir kroz smirenije i jasnije odluke.'),

                    ('90000000-0000-0000-0000-000000000013'::uuid, 'a4444444-4444-4444-4444-444444444444'::uuid, 'U zadatku ste se postavili u ulogu savetnika nekoga do koga vam je stalo. Tako sebi dajete kvalitetnije savete koje je lakše prihvatiti.'),
                    ('90000000-0000-0000-0000-000000000014'::uuid, 'a4444444-4444-4444-4444-444444444444'::uuid, 'Pristup šta bih rekao prijatelju zaobilazi ego i vraća kvalitet razmišljanja koji već imate.'),
                    ('90000000-0000-0000-0000-000000000015'::uuid, 'a4444444-4444-4444-4444-444444444444'::uuid, 'Ovom vežbom trenirate da budete dobar prijatelj sebi i da smanjite nepotreban unutrašnji pritisak.'),
                    ('90000000-0000-0000-0000-000000000016'::uuid, 'a4444444-4444-4444-4444-444444444444'::uuid, 'Smiren i razuman glas koji imamo za druge, primenjen na sebe, pomaže da donesemo odluke bez kajanja.'),

                    ('90000000-0000-0000-0000-000000000017'::uuid, 'a5555555-5555-5555-5555-555555555555'::uuid, 'U zadatku ste ispitivali šta zaista znate, šta pretpostavljate i šta ne znate. To je osnova za bolje rasuđivanje.'),
                    ('90000000-0000-0000-0000-000000000018'::uuid, 'a5555555-5555-5555-5555-555555555555'::uuid, 'Kalibracija sopstvenog znanja otvara prostor za stvaran razgovor, posebno sa ljudima čije se iskustvo razlikuje od našeg.'),
                    ('90000000-0000-0000-0000-000000000019'::uuid, 'a5555555-5555-5555-5555-555555555555'::uuid, 'Prevelika sigurnost u sopstvene procene ograničava nas. Otvorenost za ispravku vodi boljim odlukama.'),
                    ('90000000-0000-0000-0000-000000000020'::uuid, 'a5555555-5555-5555-5555-555555555555'::uuid, 'Kada znamo granice svog znanja, radoznalost raste i vidimo mogućnosti koje ranije nisu bile vidljive.'),

                    ('90000000-0000-0000-0000-000000000021'::uuid, 'a6666666-6666-6666-6666-666666666666'::uuid, 'U zadatku ste razmatrali više načina na koje situacija može da se razvije. To gradi fleksibilnost i bolju pripremu.'),
                    ('90000000-0000-0000-0000-000000000022'::uuid, 'a6666666-6666-6666-6666-666666666666'::uuid, 'Sposobnost da vidimo više mogućih ishoda daje nam moć da biramo poteze koji vode boljem rezultatu.'),
                    ('90000000-0000-0000-0000-000000000023'::uuid, 'a6666666-6666-6666-6666-666666666666'::uuid, 'Kada prihvatimo da tok situacije nije fiksan, otvara se prostor za pametnije delovanje.'),
                    ('90000000-0000-0000-0000-000000000024'::uuid, 'a6666666-6666-6666-6666-666666666666'::uuid, 'Razmišljanje o alternativnim ishodima pomaže da zahtevne razgovore vodimo mirnije i sa više kontrole.'),

                    ('90000000-0000-0000-0000-000000000025'::uuid, 'a7777777-7777-7777-7777-777777777777'::uuid, 'U zadatku ste svesno zamišljali kako druga osoba vidi, oseća i razume situaciju. To smanjuje osuđivanje i približava ljude.'),
                    ('90000000-0000-0000-0000-000000000026'::uuid, 'a7777777-7777-7777-7777-777777777777'::uuid, 'Da bi podrška bila korisna, moramo razumeti gde se druga osoba zaista nalazi, a ne samo gde mi mislimo da jeste.'),
                    ('90000000-0000-0000-0000-000000000027'::uuid, 'a7777777-7777-7777-7777-777777777777'::uuid, 'Sagledavanje tuđeg ugla je preduslov za kvalitetne pregovore, vođenje i svakodnevnu saradnju.'),
                    ('90000000-0000-0000-0000-000000000028'::uuid, 'a7777777-7777-7777-7777-777777777777'::uuid, 'Mnoge svađe nastaju iz pogrešne pretpostavke o tuđoj nameri. Ova veština te greške značajno smanjuje.'),

                    ('90000000-0000-0000-0000-000000000029'::uuid, 'a8888888-8888-8888-8888-888888888888'::uuid, 'U zadatku ste pokušali da suprotstavljene tačke gledišta spojite u koherentno rešenje, ne samo kompromis.'),
                    ('90000000-0000-0000-0000-000000000030'::uuid, 'a8888888-8888-8888-8888-888888888888'::uuid, 'Integracija perspektiva ne deli postojeće opcije, već stvara bolje novo rešenje za uključene strane.'),
                    ('90000000-0000-0000-0000-000000000031'::uuid, 'a8888888-8888-8888-8888-888888888888'::uuid, 'Ova veština smanjuje vraćanje istih konflikata jer vodi dogovorima koji traju.'),
                    ('90000000-0000-0000-0000-000000000032'::uuid, 'a8888888-8888-8888-8888-888888888888'::uuid, 'Kada povezujemo naizgled nespojive ideje, razvijamo kreativnost i širimo prostor mogućih ishoda.'),

                    ('90000000-0000-0000-0000-000000000033'::uuid, 'a9999999-9999-9999-9999-999999999999'::uuid, 'U zadatku ste primetili da vaše viđenje nije objektivna stvarnost već interpretacija oblikovana iskustvom i emocijama.'),
                    ('90000000-0000-0000-0000-000000000034'::uuid, 'a9999999-9999-9999-9999-999999999999'::uuid, 'Prepoznavanje sopstvenog okvira smanjuje slepe tačke i otvara prostor za tačnije procene.'),
                    ('90000000-0000-0000-0000-000000000035'::uuid, 'a9999999-9999-9999-9999-999999999999'::uuid, 'Kad vidimo da i mi gledamo kroz okvir, lakše prihvatamo legitimno drugačije poglede drugih.'),
                    ('90000000-0000-0000-0000-000000000036'::uuid, 'a9999999-9999-9999-9999-999999999999'::uuid, 'Navika propitivanja sopstvenog okvira pomaže da izbegnemo brzoplete zaključke i skupe greške.')
                ) AS v("Id", "SkillId", "Text")
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM practice."GrowthMessages" gm
                    WHERE gm."Type" = 'End'
                      AND gm."SkillId" = v."SkillId"
                      AND gm."Text" = v."Text"
                );
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GrowthMessages_Skills_SkillId",
                schema: "practice",
                table: "GrowthMessages");

            migrationBuilder.DropIndex(
                name: "IX_GrowthMessages_SkillId",
                schema: "practice",
                table: "GrowthMessages");

            migrationBuilder.DropColumn(
                name: "SkillId",
                schema: "practice",
                table: "GrowthMessages");
        }
    }
}
