    using Microsoft.Xna.Framework;
    using System;
    using Terraria;
    using Terraria.Audio;
    using Terraria.ModLoader;
    using Terraria.GameContent.Bestiary;
    using Terraria.GameContent.ItemDropRules;
    using Terraria.ID;
    using Terraria.ModLoader.Utilities;
    using FrancisMod.Content.Items.Weapons;
using FrancisMod.Content.Items.Placeables;
using FrancisMod.Content.Items.Consumables;

namespace FrancisMod.Content.NPCs.Enemies
    {
        internal class FrancisSlime : ModNPC
        {
            public int Timer;
            public override void SetStaticDefaults()
            {
                Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.RainbowSlime];

                NPCID.Sets.ShimmerTransformToNPC[NPC.type] = NPCID.ShimmerSlime;

                NPCID.Sets.NPCBestiaryDrawModifiers value = new()
                {
                    Velocity = 1f
                };
                NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            }

            public override void SetDefaults()
            {
                NPC.CloneDefaults(NPCID.RainbowSlime);

                NPC.lifeMax = 500;
                NPC.damage = 30;
                NPC.value = 10000;
                NPC.alpha = 20;

                NPC.npcSlots = 1f;

                AIType = NPCID.DungeonSlime;
                AnimationType = NPCID.RainbowSlime;

                Banner = Item.NPCtoBanner(NPCID.RainbowSlime);
                BannerItem = Item.NPCtoBanner(Banner);

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
                if (Timer > 120) {
                    if (NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        var source = NPC.GetSource_FromAI();

                        Vector2 position = NPC.Center;
                        Vector2 targetPosition = Main.player[NPC.target].Center;
                        Vector2 direction = targetPosition - position;

                        direction.Normalize();
                        float speed = 8f;
                        
                        int type = ProjectileID.EyeLaser;
                        int damage = (int)((double)NPC.damage / 4f); //If the projectile is hostile, the damage passed into NewProjectile will be applied doubled, and quadrupled if expert mode, so keep that in mind when balancing projectiles if you scale it off NPC.damage (which also increases for expert/master)
                        Projectile.NewProjectile(source, position, direction * speed, type, damage, 0f, Main.myPlayer);
                    }
                    Timer = 0;
                }

            }

            public override void ModifyNPCLoot(NPCLoot npcLoot)
            {
                var slimeDropRule = Main.ItemDropsDB.GetRulesForNPCID(NPCID.MotherSlime, false);
                foreach(var rule in slimeDropRule)
                {
                    npcLoot.Add(rule);
                }

                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FrancisOre>(), 1, 3, 8));
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FrancisSword>(), 30));
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SkinnyPop>(), 15));
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MechanicalSkinnyPop>(), 50));
            }

            public override float SpawnChance(NPCSpawnInfo spawninfo)
            {
                return SpawnCondition.OverworldDaySlime.Chance * 0.07f;
            } 

            public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
            {
                bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
                {
                    BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,
                    BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                    new FlavorTextBestiaryInfoElement("Francis became a slime.")
                });
            }
        }
    }