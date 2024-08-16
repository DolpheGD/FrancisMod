using Terraria;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using FrancisMod.Content.Items.Placeables;
using Terraria.Audio;
using FrancisMod.Content.Projectiles.EnemyProjectiles;
using FrancisMod.Content.Items.NonPlaceables;
using FrancisMod.Content.Items.Consumables;

namespace FrancisMod.Content.NPCs.Enemies
{
    
	public class FrancisBot : ModNPC
	{
        public const double PI = 3.14159;
        public int AI_Timer;
        public int AI_Choice;

		private enum ActionState
		{
            LaserShoot,
            RocketLaunch
		}

		private enum Frame
		{
			Walk1,
            Walk2,
            Walk3,
            Walk4,
            RocketLaunch
		}

        public override void AI(){
            // Set random attack 
            Lighting.AddLight(NPC.Center, 0.3f, 0.23f, 0.1f);

            if (AI_Timer == 140 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                SoundEngine.PlaySound(SoundID.Item11);
                AI_Choice = Main.rand.Next(2);
                NPC.netUpdate = true;
            }
            
            NPC.TargetClosest(true);

            // Laser attack 
            if (AI_Choice == (float)ActionState.LaserShoot && AI_Timer >= 140 && AI_Timer <= 200 && AI_Timer % 10 == 0) {
                if (NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    LaserShoot();
                }
            }
            // Rocket launch
            else if (AI_Choice == (float)ActionState.RocketLaunch && AI_Timer == 140)
            {
                if (NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    RocketLaunch();
                }
            }
            AI_Timer++;

            if (AI_Timer > 200)
            {
                AI_Timer = 0;
            }

        }

        public override void FindFrame(int frameHeight) {
            NPC.spriteDirection = NPC.direction;

            // Walks when timer is below 140
            if (AI_Timer < 140 || AI_Choice == (float)ActionState.LaserShoot)
            {
                if (AI_Timer % 20 < 5)
                {
                    NPC.frame.Y = (int)Frame.Walk1 * frameHeight;
                }
                else if (AI_Timer % 20 >= 5 && AI_Timer % 20 < 10)
                {
                    NPC.frame.Y = (int)Frame.Walk2 * frameHeight;
                }
                else if (AI_Timer % 20 >= 10 && AI_Timer % 20 < 15)
                {
                    NPC.frame.Y = (int)Frame.Walk3 * frameHeight;
                }
                else if (AI_Timer % 20 >= 15 && AI_Timer % 20 < 20)
                {
                    NPC.frame.Y = (int)Frame.Walk4 * frameHeight;
                }
            }
            // Rocket launch
            else if (AI_Timer >= 140 && AI_Choice == (float)ActionState.RocketLaunch)
            {
                NPC.velocity.X *= 0;
                NPC.frame.Y = (int)Frame.RocketLaunch * frameHeight;
            }


        }

        private void LaserShoot(){
            var source = NPC.GetSource_FromAI();

            Vector2 position = NPC.Center;
            Vector2 targetPosition = Main.player[NPC.target].Center;
            Vector2 direction = targetPosition - position;

            Vector2 peturbedSpeed = direction.RotatedByRandom( 5 * (PI/180));
        
            peturbedSpeed.Normalize();
            float speed = 6f;
                        
            int type = ProjectileID.MartianWalkerLaser;
            int damage = (int)((double)NPC.damage / 4f); //If the projectile is hostile, the damage passed into NewProjectile will be applied doubled, and quadrupled if expert mode, so keep that in mind when balancing projectiles if you scale it off NPC.damage (which also increases for expert/master)
            Projectile.NewProjectile(source, position, peturbedSpeed * speed, type, damage, 0f, Main.myPlayer);
        }

        private void RocketLaunch(){
            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -8), ModContent.ProjectileType<FrancisHoming>(), NPC.damage, 0f, Main.myPlayer);
        }

		public override void SetStaticDefaults() {
            
			Main.npcFrameCount[Type] = 5;

			NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers() { // Influences how the NPC looks in the Bestiary
				Velocity = 1f // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
		}

		public override void SetDefaults() {
			NPC.width = 46;
			NPC.height = 70;

            NPC.npcSlots = 3f;

            // Stats
			NPC.damage = 60;
			NPC.defense = 65;
			NPC.lifeMax = 2000;

			NPC.value = 1600f;
			NPC.knockBackResist = 0.1f;

            // Sounds
			NPC.HitSound = SoundID.NPCHit4;

            NPC.DeathSound = new SoundStyle($"{nameof(FrancisMod)}/Assets/Sounds/FrancisDeath"){
                Volume = 0.9f,
                PitchVariance = 0.3f,
                MaxInstances = 3,
		    };

            NPC.aiStyle = 3;

            // Banner
			Banner = Item.NPCtoBanner(NPCID.Zombie);
            BannerItem = Item.NPCtoBanner(Banner);

            // Immunities
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;

            NPC.rarity = 1;
		}

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FrancisBar>(), 1, 2, 4));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FrancisDust>(), 4, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SkinnyPop>(), 15));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MechanicalSkinnyPop>(), 50));
        }

        public override float SpawnChance(NPCSpawnInfo spawninfo)
        {
            return SpawnCondition.OverworldNightMonster.Chance * 0.04f;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("Francis Bot description.")
            });
        }
    }
}