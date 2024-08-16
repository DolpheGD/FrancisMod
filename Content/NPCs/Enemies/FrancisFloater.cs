using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using FrancisMod.Content.Items.NonPlaceables;
using Terraria.Audio;
using FrancisMod.Content.Items.Consumables;

namespace FrancisMod.Content.NPCs.Enemies
{
	public class FrancisFloater : ModNPC
	{
        public int Timer;
        public override void AI()
        {

            if (Timer % 3 == 0)
            {
                Lighting.AddLight(NPC.Center, 0.8f, 0.4f, 0.1f);
                

                Dust dust = Dust.NewDustDirect(NPC.position, 60, 60, DustID.Ichor , 0f, 0f, 100, default, 1f);
                dust.noGravity = true;
                dust.velocity *= 0f;
            }

            if (Timer == 0)
            {
                SoundEngine.PlaySound(new SoundStyle($"{nameof(FrancisMod)}/Assets/Sounds/FrancisFloaterSummon"){
                PitchVariance = 0.3f,
                MaxInstances = 3,
			    }, NPC.Center);

                for (int i = 0; i < 15; i++) {
                    int dustType = 175;
                    var dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, dustType);

                    dust.velocity.X += Main.rand.NextFloat(-0.2f, 0.2f);
                    dust.velocity.Y += Main.rand.NextFloat(-0.2f, 0.2f);

                    dust.scale *= 2f + Main.rand.NextFloat(-0.2f, 0.2f);
			    }


                var entitySource = NPC.GetSource_FromAI();
            
                // Spawn minions
			    for (int i = 0; i < 2; i++) {
                    NPC minionNPC = NPC.NewNPCDirect(entitySource, (int)NPC.Center.X + Main.rand.Next(22) - 11, (int)NPC.Center.Y + Main.rand.Next(22) - 11, ModContent.NPCType<FrancisMinion>(), NPC.whoAmI);
                    if (minionNPC.whoAmI == Main.maxNPCs)
                        continue; // spawn failed due to spawn cap
                }
            }


			if (Timer == 1100 || Timer == 550)
			{
				    if (NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        var source = NPC.GetSource_FromAI();

                        Vector2 position = NPC.Center;
                        Vector2 targetPosition = Main.player[NPC.target].Center;
                        Vector2 direction = targetPosition - position;

                        direction.Normalize();
                        float speed = 6f;
                        
						SoundEngine.PlaySound(SoundID.Item45);
                        int type = ProjectileID.FlamingWood;
                        int damage = (int)((double)NPC.damage / 5f); 

						for(int i = 0; i < 360; i += 36){
							direction = MathHelper.ToRadians(i).ToRotationVector2();
							Projectile.NewProjectile(source, NPC.Center, direction * speed, type, damage, 0f, Main.myPlayer);
						}
                    }
			}
            
            Timer++;

            if (Timer > 1200)
            {
                Timer = 0;
            }
        }
		public override void HitEffect(NPC.HitInfo hit) {
			// Spawn dust when hit
			for (int i = 0; i < 10; i++) {
				int dustType = DustID.Torch;
				var dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, dustType);

				dust.velocity.X += Main.rand.NextFloat(-0.05f, 0.05f);
				dust.velocity.Y += Main.rand.NextFloat(-0.05f, 0.05f);

				dust.scale *= 2f + Main.rand.NextFloat(-0.03f, 0.03f);
			}
		}

        public override void SetStaticDefaults() {
			Main.npcFrameCount[Type] = 1;


			NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers() { // Influences how the NPC looks in the Bestiary
				Velocity = 1f // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
		}

		public override void SetDefaults() {

			NPC.width = 56;
			NPC.height = 56;
			NPC.damage = 60;
			NPC.defense = 10;
			NPC.lifeMax = 3400;

			NPC.npcSlots = 5f;

			NPC.HitSound = SoundID.NPCHit1;

            NPC.DeathSound = new SoundStyle($"{nameof(FrancisMod)}/Assets/Sounds/FrancisDeath"){
                Volume = 0.9f,
                PitchVariance = 0.3f,
                MaxInstances = 3,
			};

			NPC.value = 60f;
			NPC.knockBackResist = 0.25f;
			NPC.aiStyle = 23; //

			AIType = NPCID.EnchantedSword; //

			Banner = Item.NPCtoBanner(NPCID.Zombie); // Makes this NPC get affected by the normal zombie banner.
			BannerItem = Item.BannerToItem(Banner); // Makes kills of this NPC go towards dropping the banner it's associated with.

			NPC.rarity = 2;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot) {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FrancisDust>(), 4, 1, 3));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SkinnyPop>(), 15));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MechanicalSkinnyPop>(), 50));
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo) {
			return SpawnCondition.Cavern.Chance * 0.005f; 
		}


		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
			// We can use AddRange instead of calling Add multiple times in order to add multiple items at once
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,
				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("Francis Floating????"),

			});
		}


		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo) {
			// Here we can make things happen if this NPC hits a player via its hitbox (not projectiles it shoots, this is handled in the projectile code usually)
			// Common use is applying buffs/debuffs:

			int buffType = BuffID.Slow;
			// Alternatively, you can use a vanilla buff: int buffType = BuffID.Slow;

			int timeToAdd = 5 * 60; //This makes it 5 seconds, one second is 60 ticks
			target.AddBuff(buffType, timeToAdd);
		}

	}
}
