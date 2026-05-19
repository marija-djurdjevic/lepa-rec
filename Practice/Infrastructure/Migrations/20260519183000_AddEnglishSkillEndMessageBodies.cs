using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AngularNetBase.Practice.Infrastructure.Migrations
{
    [DbContext(typeof(PracticeContext))]
    [Migration("20260519183000_AddEnglishSkillEndMessageBodies")]
    public partial class AddEnglishSkillEndMessageBodies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE practice."GrowthMessages" gm
                SET "TextEn" = v."TextEn"
                FROM (VALUES
                    ('90000000-0000-0000-0000-000000000001'::uuid, 'When emotion overwhelms us, it becomes hard to hear the other person, which leads to misunderstanding and conflict. Distancing restores attention and patience. We forgive more easily, hurt people less in anger, and better understand those we care about.'),
                    ('90000000-0000-0000-0000-000000000002'::uuid, 'Impulsive decisions are rarely good. This technique restores clarity exactly when it is hardest to find: in conflict, in negotiation, and before important choices. Most people react from emotion then. If we stay cool-headed in those moments, we gain more from the situation.'),
                    ('90000000-0000-0000-0000-000000000003'::uuid, 'It is common to repeat the same mistakes under emotional pressure, reacting to a partner or colleague in ways we do not want. With this technique, we can process emotion before it drives an unwanted reaction and choose our response instead of letting the situation choose for us.'),
                    ('90000000-0000-0000-0000-000000000004'::uuid, 'Under stress, we can easily damage relationships through words spoken in affect and rushed decisions. This technique lowers the chance of those mistakes and protects what we have built: relationships, reputation, and everyday peace.'),

                    ('90000000-0000-0000-0000-000000000005'::uuid, 'This perspective helps us avoid sacrificing long-term relationships for a momentary emotion.'),
                    ('90000000-0000-0000-0000-000000000006'::uuid, 'Our time and energy are limited resources. If urgent problems constantly press on us, little remains for long-term goals. Temporal distancing helps separate what truly matters from the noise that feels important in the moment but only drains us.'),
                    ('90000000-0000-0000-0000-000000000007'::uuid, 'When we become trapped in immediate concerns, we lose control over our thoughts.'),
                    ('90000000-0000-0000-0000-000000000008'::uuid, 'In emotional states, we can make decisions that cost us. We may miss future opportunities or harm relationships with people we care about. Time distance puts things in perspective and helps us avoid these scenarios.'),

                    ('90000000-0000-0000-0000-000000000009'::uuid, 'When we look at ourselves from the outside, we see how our behavior affects people around us. It is not always comfortable, but it helps us become kinder to ourselves and others. People we care about will feel that shift in our energy.'),
                    ('90000000-0000-0000-0000-000000000010'::uuid, 'Only when we step outside ourselves do we see the real possibilities in front of us. Without that, we play below our potential.'),
                    ('90000000-0000-0000-0000-000000000011'::uuid, 'When we are stuck in our own head, the world feels constrained. We may believe we cannot repair a relationship or have a difficult conversation. From another perspective, those steps feel possible and less frightening, giving us the power to act.'),
                    ('90000000-0000-0000-0000-000000000012'::uuid, 'To preserve healthy relationships, we need to protect our boundaries and respect others'' boundaries. Observer perspective helps us see situations more clearly and preserve both personal and social peace.'),

                    ('90000000-0000-0000-0000-000000000013'::uuid, 'It may be easier for us to be caring and patient with others because serving them feels meaningful. In the same spirit, caring better for ourselves gives us more to offer others.'),
                    ('90000000-0000-0000-0000-000000000014'::uuid, 'Our ego can be a major obstacle to good judgment, especially under pressure. The what-would-I-tell-a-friend approach bypasses ego, restores the quality of thinking we already have, and helps us make better moves.'),
                    ('90000000-0000-0000-0000-000000000015'::uuid, 'We often impose stricter standards on ourselves than on friends. This exercise trains us to become a good friend to ourselves and release unnecessary self-imposed pressure.'),
                    ('90000000-0000-0000-0000-000000000016'::uuid, 'The voice we use to advise someone we care about is often calm and reasonable. Applied to ourselves, that same voice protects us and guides choices we will not regret.'),

                    ('90000000-0000-0000-0000-000000000017'::uuid, 'If we assume we already know everything about a person or topic, connection is often blocked. Calibrating our knowledge opens space for real conversation, especially with people whose experience differs from ours.'),
                    ('90000000-0000-0000-0000-000000000018'::uuid, 'Overconfidence in our own judgments limits us. Without accurate information, we make poor decisions. If we defend a wrong view too strongly, others trust us less. Reaching truth with help from others serves our goals.'),
                    ('90000000-0000-0000-0000-000000000019'::uuid, 'The belief that I already know shuts down curiosity. Recognizing the limits of our knowledge opens questions, and answers to those questions expand our worldview and reveal possibilities we did not know existed.'),
                    ('90000000-0000-0000-0000-000000000020'::uuid, 'Stability does not come from believing we know everything we need, but from having a realistic picture of what we know and what we do not. When we know our limits, we are less likely to be caught off guard in front of others.'),

                    ('90000000-0000-0000-0000-000000000021'::uuid, 'We want to understand friends and family, and we want them to understand us. Sometimes our or their behavior leads to empty conversations and unnecessary arguments that create distance. This skill helps us notice those patterns, change the flow, and deepen relationships.'),
                    ('90000000-0000-0000-0000-000000000022'::uuid, 'The ability to anticipate multiple outcomes gives us power. Instead of reacting blindly to what is in front of us, we analyze how the situation may unfold and take actions that increase the chance of an attractive outcome.'),
                    ('90000000-0000-0000-0000-000000000023'::uuid, 'When we accept that much can change based on our moves, a wide space for action opens. We are not trapped in one outcome; we gain freedom to shape what a conversation, family meal, or team meeting will look like.'),
                    ('90000000-0000-0000-0000-000000000024'::uuid, 'When a demanding conversation awaits us, we may imagine frightening outcomes. Thinking through multiple possibilities helps us prepare for bad scenarios, plan responses, and increase the chance of a good result.'),

                    ('90000000-0000-0000-0000-000000000025'::uuid, 'Good intentions are not enough. For support to be useful, we must understand where the other person truly is, what they see, and what they fear. Without that, we respond to our assumptions, not their real needs.'),
                    ('90000000-0000-0000-0000-000000000026'::uuid, 'In negotiation, leadership, and any high-stakes conversation, seeing the other side''s angle is a prerequisite for success. Without understanding what they want and fear, it is hard to reach solutions that work for everyone.'),
                    ('90000000-0000-0000-0000-000000000027'::uuid, 'We live one life, but by intentionally imagining how the world looks to someone else, we gain access to insights otherwise unavailable. Seeing through others'' eyes is a powerful way to expand our own world.'),
                    ('90000000-0000-0000-0000-000000000028'::uuid, 'Most unnecessary conflicts start from wrong assumptions about what someone meant or intended. When we imagine the other person''s angle before reacting, those errors decrease and daily life becomes calmer.'),

                    ('90000000-0000-0000-0000-000000000029'::uuid, 'The ability to find solutions everyone can live with brings short-term relief and long-term flourishing for all involved. Families thrive, teams perform exceptionally, and communities become more cohesive.'),
                    ('90000000-0000-0000-0000-000000000030'::uuid, 'Compromise divides the pie. Integration grows the pie. The ability to produce a better solution than two conflicting proposals is a rare skill that separates merely managing conflict from truly resolving it.'),
                    ('90000000-0000-0000-0000-000000000031'::uuid, 'Combining ideas that seem incompatible requires creativity because we are building something new instead of choosing from existing options. This practice develops that capacity and opens new possibilities.'),
                    ('90000000-0000-0000-0000-000000000032'::uuid, 'Solutions where one side yields without real resolution are short-lived and conflict returns. Integrating interests and needs of opposing sides creates agreements that last and reduces repeated conflict patterns.'),

                    ('90000000-0000-0000-0000-000000000033'::uuid, 'If we treat our own view as reality itself, every different opinion looks like error or bad intent. Once we see that we also look through a personal frame, genuine mutual understanding becomes possible.'),
                    ('90000000-0000-0000-0000-000000000034'::uuid, 'Blind spots are among the most common sources of costly mistakes. Those who can see beyond their own frame begin asking where that frame may not fit the situation and prevent avoidable errors.'),
                    ('90000000-0000-0000-0000-000000000035'::uuid, 'What seems obviously true is often only one perspective. Once we notice that, we are no longer trapped in it. We can choose another angle, test new interpretations, and see the world more freely.'),
                    ('90000000-0000-0000-0000-000000000036'::uuid, 'Many bad decisions are made because we are sure something is clear when we do not actually see well. The habit of questioning our own frame reduces surprises and relationship damage.')
                ) AS v("Id", "TextEn")
                WHERE gm."Id" = v."Id";
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
