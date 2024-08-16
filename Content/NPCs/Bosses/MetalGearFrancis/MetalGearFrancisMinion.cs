using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using FrancisMod.Content.Projectiles;

namespace FrancisMod.Content.NPCs.Bosses.MetalGearFrancis
{
	// The minions spawned when the body spawns
	// Please read MinionBossBody.cs first for important comments, they won't be explained here again
	public class MetalGearFrancisMinion : ModNPC
	{
		// This is a neat trick that uses the fact that NPCs have all NPC.ai[] values set to 0f on spawn (if not otherwise changed).
		// We set ParentIndex to a number in the body after spawning it. If we set ParentIndex to 3, NPC.ai[0] will be 4. If NPC.ai[0] is 0, ParentIndex will be -1.
		// Now combine both facts, and the conclusion is that if this NPC spawns by other means (not from the body), ParentIndex will be -1, allowing us to distinguish
		// between a proper spawn and an invalid/"cheated" spawn
		public int ParentIndex {
			get => (int)NPC.ai[0] - 1;
			set => NPC.ai[0] = value + 1;
		}

		public bool HasParent => ParentIndex > -1;

		public float PositionOffset {
			get => NPC.ai[1];
			set => NPC.ai[1] = value;
		}

		public const float RotationTimerMax = 360;
		public ref float RotationTimer => ref NPC.ai[2];

        private bool firstSpawn = true;
        
        public float AI_Timer = 0;

        public ref float AI_Choice => ref NPC.ai[3];

        private enum ActionState
		{
			Defense,
			Attack,
			Vortex,
			Fireball
        }

        public override void FindFrame(int frameHeight) {
            switch (AI_Choice)
            {
                case (float)ActionState.Defense:
                    NPC.frame.Y = (int)ActionState.Defense * frameHeight;
                break;

                case (float)ActionState.Attack:
                    NPC.frame.Y = (int)ActionState.Attack * frameHeight;
                break;

                case (float)ActionState.Vortex:
                    NPC.frame.Y = (int)ActionState.Vortex * frameHeight;
                break;

                case (float)ActionState.Fireball:
                    NPC.frame.Y = (int)ActionState.Fireball * frameHeight;
                break;
            }
            
        }

		public override void AI() {
			if (Despawn()) {
				return;
			}

			FadeIn();

			MoveInFormation();

            // Pick random
			if (Main.netMode != NetmodeID.MultiplayerClient && AI_Timer == 0 && !firstSpawn)
			{
				AI_Choice = Main.rand.Next(4);
				NPC.netUpdate = true;
			}

            if (AI_Timer == 0){
                SpawnDustOnSpawn();
                firstSpawn = false;
				NPC.netUpdate = true;
            }


            switch (AI_Choice)
            {
                case (float)ActionState.Defense:
                    Lighting.AddLight(NPC.Center, 0.2f, 0.8f, 0.95f);
                    NPC.defense = 2000;
                break;

                case (float)ActionState.Attack:
                    NPC.defense = 50;
                    Attack();
                break;

                case (float)ActionState.Vortex:
                    NPC.defense = 50;
                    Vortex();
                break;

                case (float)ActionState.Fireball:
                    NPC.defense = 50;
                    Fireball();
                break;
            }


            AI_Timer++;
            if (AI_Timer >= 350)
                AI_Timer = 0;

		}
		// Laser Attack
		// ----------------------------------
        private void Attack(){
            Lighting.AddLight(NPC.Center, 0.95f, 0.3f, 0.2f);
			if (AI_Timer >= 60 && AI_Timer % 30 == 0 && AI_Timer <= 180)
			{
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					var source = NPC.GetSource_FromAI();

					NPC.TargetClosest();
					Player player = Main.player[NPC.target];

					Vector2 direction = player.Center - NPC.Center;
					direction.Normalize();

					Projectile.NewProjectile(source, NPC.Center, direction * 11f, ProjectileID.DeathLaser, NPC.damage / 3, 0f, Main.myPlayer);
				}
			}
        }
		// Vortex Attack
		// ----------------------------------
        private void Vortex(){
            Lighting.AddLight(NPC.Center, 0.5f, 0.2f, 0.95f);
			
			if (AI_Timer >= 200 && AI_Timer % 50 == 0 && AI_Timer < 350)
			{
				SoundEngine.PlaySound(SoundID.Item91, NPC.Center);

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					var source = NPC.GetSource_FromAI();

					NPC.TargetClosest();
					Player player = Main.player[NPC.target];

					Vector2 direction = player.Center - NPC.Center;
					direction.Normalize();
					float speed = 11f;
					Projectile.NewProjectile(source, NPC.Center, direction * speed, ProjectileID.DD2DarkMageBolt, NPC.damage / 5, 0f, Main.myPlayer);

					Projectile.NewProjectile(source, NPC.Center, direction.RotatedBy(MathHelper.ToRadians(70)) * speed, ProjectileID.DD2DarkMageBolt, NPC.damage / 5, 0f, Main.myPlayer);
					Projectile.NewProjectile(source, NPC.Center, direction.RotatedBy(MathHelper.ToRadians(35)) * speed, ProjectileID.DD2DarkMageBolt, NPC.damage / 5, 0f, Main.myPlayer);
					Projectile.NewProjectile(source, NPC.Center, direction.RotatedBy(MathHelper.ToRadians(-35)) * speed, ProjectileID.DD2DarkMageBolt, NPC.damage / 5, 0f, Main.myPlayer);
					Projectile.NewProjectile(source, NPC.Center, direction.RotatedBy(MathHelper.ToRadians(-70)) * speed, ProjectileID.DD2DarkMageBolt, NPC.damage / 5, 0f, Main.myPlayer);
				}
			}
        }
		// Fireball Attack
		// ----------------------------------
		private void Fireball(){
			Lighting.AddLight(NPC.Center, 0.75f, 0.54f, 0.2f);

			if (AI_Timer % 120 == 0 && AI_Timer >= 120)
			{

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					var source = NPC.GetSource_FromAI();

					NPC.TargetClosest();
					Player player = Main.player[NPC.target];

					Vector2 direction = player.Center - NPC.Center;
					direction.Normalize();

					Projectile.NewProjectile(source, NPC.Center, direction.RotatedByRandom(MathHelper.ToRadians(60)) * 9f, ProjectileID.CultistBossFireBall, NPC.damage / 5, 0f, Main.myPlayer);
				}
			}

		}







		private bool Despawn() {
			if (Main.netMode != NetmodeID.MultiplayerClient &&
				(!HasParent || !Main.npc[ParentIndex].active || Main.npc[ParentIndex].type != BodyType())) {
				// * Not spawned by the boss body (didn't assign a position and parent) or
				// * Parent isn't active or
				// * Parent isn't the body
				// => invalid, kill itself without dropping any items
				NPC.active = false;
				NPC.life = 0;
				NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
				return true;
			}
			return false;
		}

		private void FadeIn() {
			// Fade in (we have NPC.alpha = 255 in SetDefaults which means it spawns transparent)
			if (NPC.alpha > 0) {
				NPC.alpha -= 10;
				if (NPC.alpha < 0) {
					NPC.alpha = 0;
				}
			}
		}

		private void MoveInFormation() {
			NPC parentNPC = Main.npc[ParentIndex];

			// This basically turns the NPCs PositionIndex into a number between 0f and TwoPi to determine where around
			// the main body it is positioned at
			float rad = (float)PositionOffset * MathHelper.TwoPi;

			// Add some slight uniform rotation to make the eyes move, giving a chance to touch the player and thus helping melee players
			RotationTimer += 0.9f;
			if (RotationTimer > RotationTimerMax) {
				RotationTimer = 0;
			}

			// Since RotationTimer is in degrees (0..360) we can convert it to radians (0..TwoPi) easily
			float continuousRotation = MathHelper.ToRadians(RotationTimer);
			rad += continuousRotation;

			if (rad > MathHelper.TwoPi) {
				rad -= MathHelper.TwoPi;
			}
			else if (rad < 0) {
				rad += MathHelper.TwoPi;
			}

			float distanceFromBody = parentNPC.width + NPC.width + 50f;

			// offset is now a vector that will determine the position of the NPC based on its index
			Vector2 offset = Vector2.One.RotatedBy(rad) * distanceFromBody;

            NPC.Center = parentNPC.Center + offset;
		}

		// Helper method to determine the body type
		public static int BodyType() {
			return ModContent.NPCType<MetalGearFrancis>();
		}

		public override void SetStaticDefaults() {
			Main.npcFrameCount[Type] = 4;

			// By default enemies gain health and attack if hardmode is reached. this NPC should not be affected by that
			NPCID.Sets.DontDoHardmodeScaling[Type] = true;
			// Enemies can pick up coins, let's prevent it for this NPC
			NPCID.Sets.CantTakeLunchMoney[Type] = true;
			// Automatically group with other bosses
			NPCID.Sets.BossBestiaryPriority.Add(Type);

			// Specify the debuffs it is immune to. Most NPCs are immune to Confused.
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

			// Optional: If you don't want this NPC to show on the bestiary (if there is no reason to show a boss minion separately)
			// Make sure to remove SetBestiary code as well
			// NPCID.Sets.NPCBestiaryDrawModifiers bestiaryData = new NPCID.Sets.NPCBestiaryDrawModifiers() {
			//	Hide = true // Hides this NPC from the bestiary
			// };
			// NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, bestiaryData);
		}






		public override void SetDefaults() {
			NPC.width = 35;
			NPC.height = 35;
			NPC.damage = 60;
			NPC.defense = 2500;
			NPC.lifeMax = 16000;

			NPC.HitSound = SoundID.NPCHit4;
			NPC.DeathSound = SoundID.NPCDeath14;

			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.knockBackResist = 0f;
			NPC.alpha = 255; // This makes it transparent upon spawning, we have to manually fade it in in AI()
			NPC.netAlways = true;


			NPC.aiStyle = -1;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
			// Makes it so whenever you beat the boss associated with it, it will also get unlocked immediately
			int associatedNPCType = BodyType();
			bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);

			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
				new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), // Plain black background
				new FlavorTextBestiaryInfoElement("A minion protecting his boss from taking damage by sacrificing itself. If none are alive, the boss is exposed to damage.")
			});
		}

		public override Color? GetAlpha(Color drawColor) {
			if (NPC.IsABestiaryIconDummy) {
				// This is required because we have NPC.alpha = 255, in the bestiary it would look transparent
				return NPC.GetBestiaryEntryColor();
			}
			return Color.White * NPC.Opacity;
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot) {
			cooldownSlot = ImmunityCooldownID.Bosses; // use the boss immunity cooldown counter, to prevent ignoring boss attacks by taking damage from other sources
			return true;
		}



        private void SpawnDustOnSpawn(){
            SoundEngine.PlaySound(SoundID.Item71, NPC.Center);
            // Normal approach
            Vector2 speed = Main.rand.NextVector2Unit();

                switch (AI_Choice)
                {
                    case (float)ActionState.Defense:
                        for (int i = 0; i < 360; i += 5) {
                            Dust dust = Dust.NewDustPerfect(NPC.Center, DustID.IceTorch, (NPC.velocity + speed.RotatedBy(MathHelper.ToRadians(i))) * 3.5f, 250, Color.White, 3f);
                            
                            dust.noGravity = true;
                        }
                    break;

                    case (float)ActionState.Attack:
                         for (int i = 0; i < 360; i += 5) {
                            Dust dust = Dust.NewDustPerfect(NPC.Center, DustID.RedTorch, (NPC.velocity + speed.RotatedBy(MathHelper.ToRadians(i))) * 3.5f, 250, Color.White, 3f);
                            
                            dust.noGravity = true;
                        }
                    break;

                    case (float)ActionState.Vortex:
                        for (int i = 0; i < 360; i += 5) {
                            Dust dust = Dust.NewDustPerfect(NPC.Center, DustID.CrystalPulse2, (NPC.velocity + speed.RotatedBy(MathHelper.ToRadians(i))) * 3.5f, 250, Color.White, 3f);
                            
                            dust.noGravity = true;
                        }
                    break;

                    case (float)ActionState.Fireball:
                        for (int i = 0; i < 360; i += 5) {
                            Dust dust = Dust.NewDustPerfect(NPC.Center, DustID.Lava, (NPC.velocity + speed.RotatedBy(MathHelper.ToRadians(i))) * 3.5f, 250, Color.White, 3f);
                            
                            dust.noGravity = true;
                        }
                    break;
                }
        }

        public override void OnKill()
        {
			// Smoke Dust spawn
			for (int i = 0; i < 50; i++) {
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
				dust.velocity *= 1.4f;
			}

			for (int i = 0; i < 80; i++) {
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Torch, 0f, 0f, 100, default, 3f);
				dust.noGravity = true;
				dust.velocity *= 5f;
				dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Torch, 0f, 0f, 100, default, 2f);
				dust.velocity *= 3f;
			}

        }
	}
}
