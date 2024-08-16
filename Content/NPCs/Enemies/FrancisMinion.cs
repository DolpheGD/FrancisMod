    using Microsoft.Xna.Framework;

    using Terraria;
    using Terraria.Audio;
    using Terraria.ModLoader;
    using Terraria.GameContent.Bestiary;
    using Terraria.GameContent.ItemDropRules;
    using Terraria.ID;
using FrancisMod.Content.Items.NonPlaceables;

namespace FrancisMod.Content.NPCs.Enemies
    {
        internal class FrancisMinion : ModNPC
        {
            public int Timer;
            public override void SetStaticDefaults()
            {
                Main.npcFrameCount[Type] = 1;

                NPCID.Sets.NPCBestiaryDrawModifiers value = new()
                {
                    Velocity = 1f
                };
                NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            }

            public override void SetDefaults()
            {
                NPC.width = 34;
                NPC.height = 32;

                NPC.lifeMax = 600;
                NPC.defDefense = 20;
                NPC.damage = 40;

                NPC.value = 1500;

                NPC.noTileCollide = true;
                NPC.aiStyle = 44;
                
                NPC.npcSlots = 1f;
                NPC.knockBackResist = 0.4f;

                NPC.HitSound = new SoundStyle($"{nameof(FrancisMod)}/Assets/Sounds/FrancisHurt"){
                    Volume = 0.9f,
                    PitchVariance = 0.3f,
                    MaxInstances = 3,
			    };

                NPC.DeathSound = new SoundStyle($"{nameof(FrancisMod)}/Assets/Sounds/FrancisDeath"){
                    Volume = 0.9f,
                    PitchVariance = 0.3f,
                    MaxInstances = 3,
			    };

                NPC.rarity = 0;
            }
            public override void AI ()
            {
                Timer++;


                if (Timer % 3 == 0)
                {
                    Lighting.AddLight(NPC.Center, 0.3f, 0.45f, 0.8f);
                    

                    Dust dust = Dust.NewDustDirect(NPC.position, 60, 60, DustID.BlueTorch , 0f, 0f, 100, default, 1f);
                    dust.noGravity = true;
                    dust.velocity *= 0f;
                }


                if (Timer > 200) {
                    if (NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        var source = NPC.GetSource_FromAI();

                        Vector2 position = NPC.Center;
                        Vector2 targetPosition = Main.player[NPC.target].Center;
                        Vector2 direction = targetPosition - position;

                        direction.Normalize();
                        float speed = 6f;
                        
                
                        int type = ProjectileID.FlamingWood;
                        int damage = (int)((double)NPC.damage / 4f); //If the projectile is hostile, the damage passed into NewProjectile will be applied doubled, and quadrupled if expert mode, so keep that in mind when balancing projectiles if you scale it off NPC.damage (which also increases for expert/master)
                        Projectile.NewProjectile(source, position, direction * speed, type, damage, 0f, Main.myPlayer);
                    }
                    Timer = 0;
                }

            }

		    public override void HitEffect(NPC.HitInfo hit) {
			// Spawn dust when hit
                for (int i = 0; i < 10; i++) {
                    int dustType = DustID.BlueTorch;
                    var dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, dustType);

                    dust.velocity.X += Main.rand.NextFloat(-0.05f, 0.05f);
                    dust.velocity.Y += Main.rand.NextFloat(-0.05f, 0.05f);

                    dust.scale *= 2f + Main.rand.NextFloat(-0.03f, 0.03f);
                }
            }


            public override void ModifyNPCLoot(NPCLoot npcLoot)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FrancisDust>(), 15));
            }

            public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
            {
                bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
                {
                    BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,
                    new FlavorTextBestiaryInfoElement("Mini Franci the Francis Floater Summons")
                });
            }
        }
    }