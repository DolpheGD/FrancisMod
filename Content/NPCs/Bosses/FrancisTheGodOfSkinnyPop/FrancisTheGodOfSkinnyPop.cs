using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;
using FrancisMod.Content.Projectiles.EnemyProjectiles;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using FrancisMod.Content.BossBars;
using FrancisMod.Content.NPCs.Enemies;
using FrancisMod.Common.Players;
using FrancisMod.Content.Items.Consumables;
using FrancisMod.Common.Systems;
using System.Threading;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Sounds;

namespace FrancisMod.Content.NPCs.Bosses.FrancisTheGodOfSkinnyPop
{
    [AutoloadBossHead]
	public class FrancisTheGodOfSkinnyPop : ModNPC
	{
		// Damages
		private int contactDamage = 0;
		private int nukeDamage = 250;
		private int laserDamage = 200;

		// Hitboxes
        private int hitboxWidth = 244;
	    private int hitboxHeight = 244;

		// Bools
		private bool canRandom = false;
		private bool firstSpawn = true;

		// AI sync
		private ref float AI_Timer => ref NPC.ai[0];
		private ref float AI_Choice => ref NPC.ai[3];
		public Vector2 TargetDestination {
			get => new Vector2(NPC.ai[1], NPC.ai[2]);
			set {
				NPC.ai[1] = value.X;
				NPC.ai[2] = value.Y;
			}
		}

		// battle timer
		private int TotalBattleTimer = 0;

		private Vector2 initDirection = Vector2.Zero;

		private enum ActionState
		{
			Intro,
			Teleport,
			BSshooting,
			BarackObama,
			Dash,
			SurroundPlayer
		}

        public override void AI()
        {
			if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active) {
				NPC.TargetClosest();
			}
			Player player = Main.player[NPC.target];
			if (firstSpawn){
				SoundEngine.PlaySound(new SoundStyle($"{nameof(FrancisMod)}/Assets/Sounds/FrancisPhaseTransition"){Volume = 1.5f}, NPC.Center);
				SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, NPC.Center);

				NPC.Center = player.Center + new Vector2(0, -320);
				firstSpawn = false;

				PunchCameraModifier modifier = new PunchCameraModifier(NPC.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 30f, 6f, 120, default, FullName);
				Main.instance.CameraModifiers.Add(modifier);
			}
			if (player.dead) {
				NPC.velocity.Y -= 0.08f;
				// This method makes it so when the boss is in "despawn range" (outside of the screen), it despawns in 10 ticks
				NPC.EncourageDespawn(10);
				return;
			}



			if (AI_Timer == 0 && canRandom)
			{
				NPC.netUpdate = true;
				Lighting.AddLight(NPC.Center, 0.9f, 0.9f, 0.9f);

				if (AI_Choice == 1)
				{
					AI_Choice = Main.rand.Next(4) + 2;
				}
				else
				{
					AI_Choice = 1;
				}
			}


			switch (AI_Choice)
			{
				case (float)ActionState.Intro:
					Intro(player);
				break;
				
				case (float)ActionState.Teleport:
					Teleport(player);
				break;

				case (float)ActionState.BSshooting:
					BSshooting(player);
				break;

				case (float)ActionState.BarackObama:
					BarackObama(player);
				break;

				case (float)ActionState.Dash:
					Dash(player);
				break;

				case (float)ActionState.SurroundPlayer:
					SurroundPlayer(player);
				break;
			}






			TotalBattleTimer++;
        }

		//---------------------------------------------------------------
		// Draw
		//---------------------------------------------------------------
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			Vector2 origin;
			SpriteEffects effects;
			Vector2 scale = new Vector2(1f, 1f);

			if (NPC.spriteDirection > 0) {
				origin = new Vector2(NPC.width/2, NPC.height);
				effects = SpriteEffects.None;
			}
			else {
				origin = new Vector2(NPC.width/2, NPC.height);
				effects = SpriteEffects.FlipHorizontally;
			}

			switch (AI_Choice)
			{
				case (float)ActionState.Intro:
					if (AI_Timer <= 20)
					{
						scale.X = normalLerp(0, 1, AI_Timer / 20);
					}
					break;

				case (float)ActionState.Teleport:
					if (AI_Timer <= 10)
					{
						scale.X = normalLerp(1, 0, AI_Timer / 10);
					}
					else if (AI_Timer >= 40)
					{
						scale.X = normalLerp(0, 1, (AI_Timer - 40) / 10);
					}
					else
					{
						scale.X = 0;
					}
					break;
			}

			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
			Main.spriteBatch.Draw(texture, (NPC.position + origin) - screenPos, default, drawColor, NPC.rotation, origin, scale, effects, 0);
			return false;
        }
		//---------------------------------------------------------------
		//---------------------------------------------------------------



		// Functions
		//---------------------------------------------------------------
		private void Intro(Player player)
		{
			AI_Timer++;
			if (AI_Timer >= 120)
			{
				AI_Timer = 0;
				canRandom = true;
			}
		}
		// Teleport
		//---------------------------------------------------------------
		private void Teleport(Player player)
		{
			if (AI_Timer == 5)
			{
				NPC.dontTakeDamage = true;
				SoundEngine.PlaySound(SoundID.Item114, NPC.Center);
			}

			if (AI_Timer == 11)
			{
				NPC.Center = player.Center + new Vector2((float)(600 * Math.Sin(TotalBattleTimer / 10)), -380);
				NPC.netUpdate = true;
			}
			
			if (AI_Timer >= 11 && AI_Timer <= 50)
			{
				spawnSuckDust();
			} 

			if (AI_Timer == 42)
			{
				NPC.dontTakeDamage = false;
				SoundEngine.PlaySound(SoundID.Item115, NPC.Center);
			}

			AI_Timer++;
			if (AI_Timer >= 50)
			{
				AI_Timer = 0;
			}
		}

		// BS shooting
		//---------------------------------------------------------------
		private void BSshooting(Player player)
		{
			if (AI_Timer >= 50 && AI_Timer <= 120 && AI_Timer % 2 == 0)
            {
				SoundEngine.PlaySound(CommonCalamitySounds.ExoLaserShootSound with { Volume = 0.2f * CommonCalamitySounds.ExoLaserShootSound.Volume }, NPC.Center);
				if (NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
				{
					Vector2 direction = player.Center - NPC.Center;
					Vector2 peturbedSpeed = direction.RotatedByRandom(MathHelper.ToRadians(90));			
					peturbedSpeed.Normalize();

					Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, peturbedSpeed * 9, ModContent.ProjectileType<ThanatosLaser>(), laserDamage, 0f, Main.myPlayer, 0f, -1f);
				}
			}

			if (AI_Timer == 150)
            {
				SoundEngine.PlaySound(new SoundStyle($"{nameof(FrancisMod)}/Assets/Sounds/Fart"){Volume = 1.5f}, NPC.Center);
				if (NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
				{
					Vector2 direction = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
					for (int i = -75; i <= 75; i += 5)
					{
						Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, direction.RotatedBy(MathHelper.ToRadians(i)) * 8, ModContent.ProjectileType<ThanatosLaser>(), laserDamage, 0f, Main.myPlayer, 0f, -1f);
					}
				}
			}

			AI_Timer++;
			
			if (AI_Timer >= 180)
			{
				AI_Timer = 0;
			}
		}

		// Bomb lmao
		//---------------------------------------------------------------
		private void BarackObama(Player player)
		{
			if (AI_Timer == 40)
            {
				SoundEngine.PlaySound(new SoundStyle($"{nameof(FrancisMod)}/Assets/Sounds/FrancisLaugh1"), NPC.Center);

				if (NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
				{
					Vector2 direction = player.Center - NPC.Center;	
					direction.Normalize();

					Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, direction * 12f, ModContent.ProjectileType<AresGaussNukeProjectile>(), nukeDamage, 0f, Main.myPlayer);
				}
			}

			AI_Timer++;
			
			if (AI_Timer >= 60)
			{
				AI_Timer = 0;
			}
		}

		// Dash lmao
		//---------------------------------------------------------------
		private void Dash(Player player)
		{
			Lighting.AddLight(NPC.Center, 0.9f, 0.9f, 0.9f);
			if (AI_Timer == 0)
				contactDamage = 1000;

				if (Main.netMode != NetmodeID.Server && AI_Timer % 2 == 0 && AI_Timer < 30) {
					// For visuals regarding NPC position, netOffset has to be concidered to make visuals align properly
					NPC.position += NPC.netOffset;
					Dust.QuickDustLine(NPC.Center, player.Center, (player.Center - NPC.Center).Length() / 15f, Color.White);
					NPC.position -= NPC.netOffset;
				}

			if (AI_Timer == 30)
			{
				SoundEngine.PlaySound(new SoundStyle($"{nameof(FrancisMod)}/Assets/Sounds/VineBoom"){Volume = 1.5f, PitchVariance = 0.3f}, NPC.Center);
				TargetDestination = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
				NPC.velocity = TargetDestination * 60;
			}

			if (AI_Timer >= 30 && AI_Timer < 70)
			{
				dashDust();
				if (AI_Timer % 3 == 0)
				{
					SoundEngine.PlaySound(SoundID.Item11, NPC.Center);
					if (Main.netMode != NetmodeID.Server)
					{
						Vector2 direction = TargetDestination.SafeNormalize(Vector2.One);

						Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, direction.RotatedBy(MathHelper.ToRadians(90)), ModContent.ProjectileType<AdiBullet>(), nukeDamage, 0f, Main.myPlayer);
						Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, direction.RotatedBy(MathHelper.ToRadians(-90)), ModContent.ProjectileType<AdiBullet>(), nukeDamage, 0f, Main.myPlayer);
					}
				}
			}


			if (AI_Timer >= 70)
				NPC.velocity *= 0.92f;
			
			
			AI_Timer++;
			
			if (AI_Timer >= 90)
			{
				contactDamage = 0;
				NPC.velocity *= 0f;
				AI_Timer = 0;
			}
		}

		// Surrounded all players with the same attack
		//---------------------------------------------------------------
		private void SurroundPlayer(Player player)
		{
			if (AI_Timer == 0)
			{
				SoundEngine.PlaySound(SoundID.Item94, NPC.Center);
				SoundEngine.PlaySound(new SoundStyle($"{nameof(FrancisMod)}/Assets/Sounds/FrancisLaugh2"){PitchVariance = 0.3f}, NPC.Center);

				for (int i = 0; i < Main.maxPlayers; i++)
				{
					Player target = Main.player[i];
					if (target.active)
					{
						Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<FrancisMeal>(), laserDamage, 0f, Main.myPlayer, i, 300);
						Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<FrancisMeal>(), laserDamage, 0f, Main.myPlayer, i, -300f);
					}
				}
			}


			AI_Timer++;
			
			if (AI_Timer >= 300)
			{
				AI_Timer = 0;
			}
		}






		private void spawnSuckDust()
		{
			Vector2 randVect = Main.rand.NextVector2Unit().SafeNormalize(Vector2.UnitY);
			randVect *= 160;
			Vector2 Direction = -1 * (randVect).SafeNormalize(Vector2.UnitY);

            Dust dust = Dust.NewDustPerfect(NPC.Center + randVect, 175, Direction * 12, 100, Color.White, 5f);
			dust.rotation *= 0.5f;
			dust.noGravity = true;
		}

		private void dashDust()
		{
			Dust dust1 = Dust.NewDustDirect(NPC.Center, 150, 150, DustID.Smoke, 0, 0, 0, Color.White, 5f);
			Dust dust2 = Dust.NewDustDirect(NPC.Center, 150, 150, DustID.Electric, 0, 0, 0, Color.White, 3f);
			dust1.noGravity = true;
			dust2.noGravity = true;
			dust1.rotation *= 0.4f; 
			dust2.rotation *= 0.4f; 
		}















        public override void SetStaticDefaults() {
			Main.npcFrameCount[Type] = 1;

			// Add this in for bosses that have a summon item, requires corresponding code in the item (See MinionBossSummonItem.cs)
			NPCID.Sets.MPAllowedEnemies[Type] = true;
			// Automatically group with other bosses
			NPCID.Sets.BossBestiaryPriority.Add(Type);

			// Specify the debuffs it is immune to. Most NPCs are immune to Confused.
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Ichor] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

			// Influences how the NPC looks in the Bestiary

			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers() {
				// CustomTexturePath = "ExampleMod/Assets/Textures/Bestiary/MinionBoss_Preview",
				PortraitScale = 0.6f, // Portrait refers to the full picture when clicking on the icon in the bestiary
				PortraitPositionYOverride = 0f,
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

		}
		public override void SetDefaults() {
			NPC.BossBar = ModContent.GetInstance<MetalGearFrancisBossBar>();

			NPC.width = hitboxWidth;
			NPC.height = hitboxHeight;

			NPC.damage = contactDamage;

			NPC.defense = 9500;
			NPC.lifeMax = 99100000;


			NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = new SoundStyle($"{nameof(FrancisMod)}/Assets/Sounds/FrancisDeath"){
                Volume = 0.9f,
                PitchVariance = 0.3f,
                MaxInstances = 3,
		    };
	
			NPC.knockBackResist = 0f;

			NPC.noGravity = true;
			NPC.noTileCollide = true;

			NPC.value = Item.buyPrice(platinum: 1125);

			NPC.SpawnWithHigherTime(60);

			NPC.boss = true;
		
			NPC.npcSlots = 10f; 
            NPC.aiStyle = -1;


			// The following code assigns a music track to the boss in a simple way.
			if (!Main.dedServ) {
				Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/TwistedGardenKuudray");
			} 
		}
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
			// Sets the description of this NPC that is listed in the bestiary
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
				new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), // Plain black background
				new FlavorTextBestiaryInfoElement("Francis The God of Skinny Pop")
			});
		}

		private float normalLerp(float a, float b, float t)
		{
			return a + (t*(b-a));
		}

    }
}