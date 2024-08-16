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
        internal class FrancisMechMinion : ModNPC
        {
            private bool firstSpawn = true;
            private Vector2 direction = new Vector2 (0,0);
		public ref float AI_Timer => ref NPC.ai[0];
        
        public ref float AI_Choice => ref NPC.ai[1];
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
                NPC.width = 70;
                NPC.height = 70;

                NPC.lifeMax = 20000;
                NPC.defDefense = 70;
                NPC.damage = 55;

                NPC.value = 1;

                NPC.noTileCollide = true;
                NPC.noGravity = true;

                NPC.npcSlots = 1f;
                NPC.knockBackResist = 0f;


                NPC.HitSound = SoundID.NPCHit4;
                NPC.DeathSound = new SoundStyle($"{nameof(FrancisMod)}/Assets/Sounds/FrancisDeath"){
                    Volume = 0.9f,
                    PitchVariance = 0.3f,
                    MaxInstances = 3,
			    };

                NPC.rarity = 0;
            }
            public override void AI ()
            {
                NPC.TargetClosest();
                Player player = Main.player[NPC.target];

                if (firstSpawn)
                {
                    Vector2 initdirection = Main.rand.NextVector2Unit();
                    NPC.velocity = initdirection.SafeNormalize(Vector2.UnitY) * 8f;
                     
                    SoundEngine.PlaySound(new SoundStyle($"{nameof(FrancisMod)}/Assets/Sounds/FrancisLaugh1"){
                    PitchVariance = 0.3f,
                    MaxInstances = 3,
                    Volume = 1.1f
                    }, NPC.Center);
                    firstSpawn = false;
                }
                
                if (AI_Timer% 2 == 0)
                {
                    Lighting.AddLight(NPC.Center, 0.9f, 0.1f, 0.3f);
                    Dust dust2 = Dust.NewDustDirect(NPC.Center, 40, 40, DustID.Smoke, 0f, 0f, 100, default, 3f);
                    Dust dust1 = Dust.NewDustDirect(NPC.Center, 40, 40, DustID.Torch, 0f, 0f, 100, default, 2f);
                    dust1.noGravity = true;
                    dust1.velocity *= 0.1f;
                    dust2.noGravity = true;
                    dust2.velocity *= 0.1f;
                }

                if (AI_Timer>= 60)
                {
                    if (AI_Timer== 60)
                    {
                        spawnHitDust();
                        SoundEngine.PlaySound(new SoundStyle($"{nameof(FrancisMod)}/Assets/Sounds/FrancisOhNo"){
                        PitchVariance = 0.3f,
                        MaxInstances = 3,
                        Volume = 1.3f
                        }, NPC.Center);

                        AI_Choice = Main.rand.Next(3);
                    }
                        
                    
                    if (AI_Choice == 0)
                    {
                        float inertia = 55f;

                        direction = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
                        
                        NPC.velocity = (NPC.velocity * (inertia - 1) + direction * 9f) / inertia;

                        if (Main.netMode != NetmodeID.MultiplayerClient && AI_Timer% 10 == 0 && AI_Timer>= 100 && AI_Timer<= 140)
                        {
                            SoundEngine.PlaySound(SoundID.Item12);
                            var source = NPC.GetSource_FromAI();

                            Vector2 peturbedSpeed = (player.Center - NPC.Center).RotatedByRandom(MathHelper.ToRadians(30));			
                            peturbedSpeed.Normalize();
                            
                            Projectile.NewProjectile(source, NPC.Center, peturbedSpeed * 12f, ProjectileID.SaucerLaser, NPC.damage / 4, 0f, Main.myPlayer);
                        }
                    }
                    else if (AI_Choice == 1)
                    {
                        if (AI_Timer >= 60 && AI_Timer % 60 == 0)
                        {
                            direction = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
                            SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                        }

                        NPC.velocity = direction * 13f;
                    }
                    else if (AI_Choice == 2)
                    {
                        float inertia = 55f;
                        direction = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
                        
                        NPC.velocity = (NPC.velocity * (inertia - 1) + direction * 5f) / inertia;
                        if (Main.netMode != NetmodeID.MultiplayerClient && AI_Timer % 80 == 0 )
                        {
                            SoundEngine.PlaySound(SoundID.Item12);
                            var source = NPC.GetSource_FromAI();

                            Vector2 direction = player.Center - NPC.Center;			
                            direction.Normalize();

                            Projectile.NewProjectile(source, NPC.Center, direction.RotatedBy(MathHelper.ToRadians(30)) * 9f, ProjectileID.SaucerLaser, NPC.damage / 5, 0f, Main.myPlayer);
                            Projectile.NewProjectile(source, NPC.Center, direction * 9f, ProjectileID.SaucerLaser, NPC.damage / 5, 0f, Main.myPlayer);
                            Projectile.NewProjectile(source, NPC.Center, direction.RotatedBy(MathHelper.ToRadians(-30)) * 9f, ProjectileID.SaucerLaser, NPC.damage / 5, 0f, Main.myPlayer);
                        }
                    }
                }
                else
                {
                    NPC.velocity *= 0.99f;
                }

                AI_Timer++;

                if (AI_Timer > 220)
                {
                    AI_Timer= 60;
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


		private void spawnHitDust()
		{
			Vector2 speed = Main.rand.NextVector2Unit();
            for (int i = 0; i < 360; i += 10) {
                Dust dust = Dust.NewDustPerfect(NPC.Center, DustID.Torch, speed.RotatedBy(MathHelper.ToRadians(i)) * 8f, 250, Color.White, 4f);
                    
                dust.noGravity = true;
            }
		}

            public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
            {
                bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
                {
                    BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,
                    new FlavorTextBestiaryInfoElement("Francids")
                });
            }
        }
    }