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

namespace FrancisMod.Content.NPCs.Bosses.MetalGearFrancis
{
	// The main part of the boss, usually referred to as "body"
	[AutoloadBossHead] // This attribute looks for a texture called "ClassName_Head_Boss" and automatically registers it as the NPC boss head icon

	public class MetalGearFrancis : ModNPC
	{
		private enum ActionState
		{
			Hover,
			Shoot,
			Dash,
			SkinnyPop,
			Ring,
			PhaseTransition,
			DefaultMove2,
			FillerAttack,
			Dash2,
			Filler2,
			FrancisBallLaunch,
			FrancisMechMinionSummon,
			HalfHealthTransition,
			Desperation
		}

		private enum Frame
		{
			Default1,
            Default2,
            Default3,
            Default4,
			Transition1,
			Transition2,
			Transition3,
			Transition4,
			Phase2Default1,
			Phase2Default2,
			Phase2Default3,
			Phase2Default4
		}

	
		// This boss has a second phase and we want to give it a second boss head icon, this variable keeps track of the registered texture from Load().
		// It is applied in the BossHeadSlot hook when the boss is in its second stage
		private float speed = 8f;

		private float baseSpeed = 9f;
		private float timeToExecuteRandom = 200f;

		private int phase = 1;
		
		private bool canRandom = true;

		public int MinionHealthTotal { get; set; }

		public int MinionMaxHealthTotal = 0;
		
		public bool firstCall = true;
		private bool isFirstDesperation = false;
		private bool isDesperationDone = false;

		private int AttackRange = 4;
		private int AttackMin = 1;
		private float finalAttackAngle = 0;

		
		public Vector2 TargetDestination {
			get => new Vector2(NPC.ai[1], NPC.ai[2]);
			set {
				NPC.ai[1] = value.X;
				NPC.ai[2] = value.Y;
			}
		}

		// This property uses NPC.localAI[] instead which doesn't get synced, but because SpawnedMinions is only used on spawn as a flag, this will get set by all parties to true.
		// Knowing what side (client, server, all) is in charge of a variable is important as NPC.ai[] only has four entries, so choose wisely which things you need synced and not synced
		public bool SpawnedMinions {
			get => NPC.localAI[0] == 1f;
			set => NPC.localAI[0] = value ? 1f : 0f;
		}
		
		
		public ref float AI_Timer => ref NPC.ai[0];

		public ref float AI_Choice => ref NPC.ai[3];
		
		// Auto-implemented property, acts exactly like a variable by using a hidden backing field
		public Vector2 LastFirstStageDestination { get; set; } = Vector2.Zero;

		// Do NOT try to use NPC.ai[4]/NPC.localAI[4] or higher indexes, it only accepts 0, 1, 2 and 3!
		// If you choose to go the route of "wrapping properties" for NPC.ai[], make sure they don't overlap (two properties using the same variable in different ways), and that you don't accidently use NPC.ai[] directly

/* 		public override void BossHeadSlot(ref int index) {
			int slot = secondStageHeadSlot;
			if (SecondStage && slot != -1) {
				// If the boss is in its second stage, display the other head icon instead
				index = slot;
			}
		} */

		public override void AI() {
			// This should almost always be the first code in AI() as it is responsible for finding the proper player target
			if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active) {
				NPC.TargetClosest();
			}

			Player player = Main.player[NPC.target];

			// If the targeted player is dead, flee
			if (player.dead) {
				NPC.velocity.Y -= 0.04f;
				// This method makes it so when the boss is in "despawn range" (outside of the screen), it despawns in 10 ticks
				NPC.EncourageDespawn(10);
				return;
			}


			if (AI_Timer == 0)
			{
				NPC.TargetClosest();
				firstCall = true;
				NPC.netUpdate = true;

				if (isFirstDesperation)
				{
					AI_Choice = 13;
				}
				else if (phase < 4)
					AI_Choice = 0;
				else 
					AI_Choice = 6;
			}

			// Randomize the AI choice
			if (Main.netMode != NetmodeID.MultiplayerClient && AI_Timer == timeToExecuteRandom && canRandom)
			{
				Lighting.AddLight(NPC.Center, 0.9f, 0.9f, 0.9f);
				AI_Choice = Main.rand.Next(AttackRange) + AttackMin;
				NPC.netUpdate = true;
			}
			
			// AI attack branch
			switch (AI_Choice)
			{
				case (float)ActionState.Hover:
					Hover(player);
				break;

				case (float)ActionState.Shoot:
					NPC.velocity *= 0.98f;
					Shoot(player);
				break;

				case (float)ActionState.Dash:
					Lighting.AddLight(NPC.Center, 0.9f, 0.4f, 0.4f);
					if (NPC.Center.X > player.Center.X)
						Dash(player, 500);
					else
						Dash(player, -500);
				break;

				case (float)ActionState.SkinnyPop:
					Hover(player); // Hover increments for skinnypop
					SkinnyPop(player);
				break;

				case (float)ActionState.Ring:
					NPC.velocity *= 0.98f;
					Ring(player);
				break;

				case (float)ActionState.PhaseTransition:
					NPC.velocity *= 0.95f;
					phaseTransition();
				break;

				case (float)ActionState.DefaultMove2:
					DefaultMove2(player);
				break;

				case (float)ActionState.FillerAttack:
					softFollow(player, 8f);
					FillerAttack(player);
				break;

				case (float)ActionState.Dash2:
					if (AI_Timer < timeToExecuteRandom + 60)
						DashPrep(player);
					else 
						Dash2(player);
				break;

				case (float)ActionState.Filler2:
					softFollow(player, 8f);
					Filler2(player);
				break;

				case (float)ActionState.FrancisBallLaunch:
					NPC.velocity *= 0.99f;
					FrancisBallLaunch(player);
				break;

				case (float)ActionState.FrancisMechMinionSummon:
					NPC.velocity *= 0.99f;
					FrancisMechMinionSummon(player);
				break;

				case (float)ActionState.HalfHealthTransition:
					NPC.velocity *= 0.95f;
					HalfHealthTransition();
				break;

				case (float)ActionState.Desperation:
					NPC.velocity *= 0.9f;
					Desperation();
				break;
			}

			// Check Phase 2 -- Interlude with minions
			CheckForPhase2();
			
			// Check Phase 3 -- All minions dead, increase attacks
			CheckForPhase3();

			// Half health, change attack pattern
			CheckForPhase4();

			// Check Phase 5 -- Interlude with minions
			CheckForPhase5();

			// Check Phase 6 -- All minions dead, increase attacks
			CheckForPhase6();


			// Bug moment
			if (phase == 2 || phase == 5)
				NPC.dontTakeDamage = true;
		}

		// Hover -- default movement
		//---------------------------------------------------------------
		private void Hover(Player player){
			float offsetX = 300f;
			NPC.position += NPC.netOffset;

			Vector2 abovePlayer = player.Top + new Vector2(NPC.direction * offsetX, - NPC.height - 200);

			Vector2 toAbovePlayer = abovePlayer - NPC.Center;
			Vector2 toAbovePlayerNormalized = toAbovePlayer.SafeNormalize(Vector2.UnitY);

			// The NPC tries to go towards the offsetX position, but most likely it will never get there exactly, or close to if the player is moving
			// This checks if the npc is "70% there", and then changes direction
			float changeDirOffset = offsetX * 0.7f;

			if (NPC.direction == -1 && NPC.Center.X - changeDirOffset < abovePlayer.X ||
				NPC.direction == 1 && NPC.Center.X + changeDirOffset > abovePlayer.X) {
				NPC.direction *= -1;
			}

			speed = baseSpeed + 4f;
			float inertia = 25f;

			// If the boss is somehow below the player, move faster to catch up
			if (NPC.Top.Y > player.Bottom.Y) {
				speed = baseSpeed + 12;
			}

			TargetDestination = toAbovePlayerNormalized * speed;
			NPC.velocity = (NPC.velocity * (inertia - 1) + TargetDestination) / inertia; // Interia equation?

			NPC.position -= NPC.netOffset;

			AI_Timer++;
		}

		// Dash attack
		//---------------------------------------------------------------
		private void Dash(Player player, float distance)
		{
			NPC.position += NPC.netOffset;

			speed = baseSpeed + 12;
			if (firstCall)
			{
				DustWarning();
				SoundEngine.PlaySound(new SoundStyle($"{nameof(FrancisMod)}/Assets/Sounds/FrancisFloaterSummon"){
                PitchVariance = 0.3f,
                MaxInstances = 3,
			    }, NPC.Center);
				firstCall = false;
			}

			// Position calculations
			if (AI_Timer >= timeToExecuteRandom && AI_Timer < timeToExecuteRandom + 80)
			{
				TargetDestination = new Vector2(player.Center.X + distance, player.Center.Y);

				// Check if it is within a box of distance/2 to distance * 2. If not, increase speed.
				if ((distance > 0 && (NPC.Center.X < player.Center.X + distance/2 || NPC.Center.X > player.Center.X + 2 * distance)) ||
				    (distance < 0 && (NPC.Center.X > player.Center.X + distance/2 || NPC.Center.X < player.Center.X + 2 * distance)))
					speed = baseSpeed + 30;
			}
			else if (AI_Timer == timeToExecuteRandom + 80)
			{
				TargetDestination = new Vector2(player.Center.X - (distance * 3.5f), player.Center.Y);
				speed = baseSpeed + 100f;
				SoundEngine.PlaySound(SoundID.Roar, player.position);
			}
			else if (AI_Timer > timeToExecuteRandom + 120)
			{
				speed *= 0.95f;
			}


			var actualVelocity = (TargetDestination - NPC.Center).SafeNormalize(Vector2.UnitY) * speed;
			

			// Velocity based on charge or wind up
			if (AI_Timer >= timeToExecuteRandom + 80)
			{
				NPC.velocity = actualVelocity;
				spawnChargeDust();
			}
			else
			{
				NPC.velocity = (NPC.velocity * (15f - 1) + actualVelocity) / 15f;

				// Spawn Rockets during charge
				if (AI_Timer % (timeToExecuteRandom / 10) == 0)
				{
					SoundEngine.PlaySound(SoundID.Item11, NPC.Center);
				
					if (NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
						Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0,-8), ModContent.ProjectileType<FrancisHoming>(), NPC.damage / 2, 0f, Main.myPlayer);
				}
			} 

			NPC.position -= NPC.netOffset;

			AI_Timer++;
			
			if (AI_Timer > timeToExecuteRandom + 160)
			{
				AI_Timer = 0;
				firstCall = true;
			}
		}

		// Stand still shoot attack
		//---------------------------------------------------------------
		private void Shoot(Player player)
		{
			if (firstCall)
			{
				SoundEngine.PlaySound(new SoundStyle($"{nameof(FrancisMod)}/Assets/Sounds/FrancisLaugh2"){
                PitchVariance = 0.3f,
                MaxInstances = 3,
			    }, NPC.Center);
				firstCall = false;
			}


			if (NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient && AI_Timer % 10 == 0)
            {
				NPC.position += NPC.netOffset;

				var source = NPC.GetSource_FromAI();

				Vector2 direction = player.Center - NPC.Center;
				Vector2 peturbedSpeed = direction.RotatedByRandom(MathHelper.ToRadians(25));			
				peturbedSpeed.Normalize();

				Projectile.NewProjectile(source, NPC.Center, peturbedSpeed * 10f, ProjectileID.EyeLaser, NPC.damage / 4, 0f, Main.myPlayer);
				// Check phase
				if (phase >= 3 && AI_Timer % 20 == 0)
				{
					direction.Normalize();
					Projectile.NewProjectile(source, NPC.Center, peturbedSpeed * -4f, ModContent.ProjectileType<FrancisBall>(), NPC.damage / 3, 0f, Main.myPlayer);
				}
				NPC.position -= NPC.netOffset;
			}

			AI_Timer++;

			if (AI_Timer > timeToExecuteRandom + 120)
			{
				AI_Timer = 0;
				firstCall = true;
			}
		}

		// Skinnypop attack
		//---------------------------------------------------------------
		private void SkinnyPop(Player player)
		{
			if (firstCall)
			{
				SoundEngine.PlaySound(new SoundStyle($"{nameof(FrancisMod)}/Assets/Sounds/FrancisLaugh1"){
                PitchVariance = 0.3f,
                MaxInstances = 3,
			    }, NPC.Center);
				firstCall = false;
			}
	
			if (AI_Timer % (timeToExecuteRandom/5) == 0)
			{
				SoundEngine.PlaySound(SoundID.Item71, NPC.Center);

				if (NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
				{
					NPC.position += NPC.netOffset;

					var source = NPC.GetSource_FromAI();

					Projectile.NewProjectile(source, NPC.Center, new Vector2(11, -4), ModContent.ProjectileType<SkinnyPopBomb>(), NPC.damage / 5, 0f, Main.myPlayer);
					Projectile.NewProjectile(source, NPC.Center, new Vector2(-11, -4), ModContent.ProjectileType<SkinnyPopBomb>(), NPC.damage / 5, 0f, Main.myPlayer);

					NPC.position -= NPC.netOffset;
				}
			}
			if (AI_Timer > timeToExecuteRandom + 120)
			{
				AI_Timer = 0;
				firstCall = true;
			}
		}

		// Ring attack
		//---------------------------------------------------------------
		private void Ring(Player player){
			if (firstCall)
			{
				SoundEngine.PlaySound(new SoundStyle($"{nameof(FrancisMod)}/Assets/Sounds/FrancisLaugh2"){
                PitchVariance = 0.3f,
                MaxInstances = 3,
			    }, NPC.Center);
				firstCall = false;
			}

			if (AI_Timer == timeToExecuteRandom + 60)
			{
				SoundEngine.PlaySound(SoundID.Item12);

				if (NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
				{
					NPC.position += NPC.netOffset;

					var source = NPC.GetSource_FromAI();

					Vector2 direction = player.Center - NPC.Center;
					direction.Normalize();
					float projspeed = 11f;

					for(int i = 0; i < 360; i += 20){
						direction = MathHelper.ToRadians(i).ToRotationVector2();
						Projectile.NewProjectile(source, NPC.Center, direction * projspeed, ProjectileID.SaucerLaser, NPC.damage / 5, 0f, Main.myPlayer);
					}

					if (phase >= 3)
					{
						projspeed = 9f;
						for(int i = 0; i < 360; i += 8){
							direction = MathHelper.ToRadians(i).ToRotationVector2();
							Projectile.NewProjectile(source, NPC.Center, direction * projspeed, ProjectileID.SaucerLaser, NPC.damage / 5, 0f, Main.myPlayer);
						}
					}
					
					NPC.position -= NPC.netOffset;
				}
			}

			AI_Timer++;

			if (AI_Timer > timeToExecuteRandom + 70)
			{
				AI_Timer = timeToExecuteRandom - 70;
				AI_Choice = 0;
				firstCall = true;
			}
		}

		// Spawn Minions
		//---------------------------------------------------------------
		private void SpawnMinions(int count) {
			if (SpawnedMinions) {
				// No point executing the code in this method again
				return;
			}

			SpawnedMinions = true;
			if (Main.netMode == NetmodeID.MultiplayerClient) {
				// Because we want to spawn minions, and minions are NPCs, we have to do this on the server (or singleplayer, "!= NetmodeID.MultiplayerClient" covers both)
				// This means we also have to sync it after we spawned and set up the minion
				return;
			}

			var entitySource = NPC.GetSource_FromAI();
			
			MinionMaxHealthTotal = 0;
			for (int i = 0; i < count; i++) {
				NPC minionNPC = NPC.NewNPCDirect(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<MetalGearFrancisMinion>(), NPC.whoAmI);
				if (minionNPC.whoAmI == Main.maxNPCs)
					continue; // spawn failed due to spawn cap

				// Now that the minion is spawned, we need to prepare it with data that is necessary for it to work
				// This is not required usually if you simply spawn NPCs, but because the minion is tied to the body, we need to pass this information to it
				MetalGearFrancisMinion minion = (MetalGearFrancisMinion)minionNPC.ModNPC;
				minion.ParentIndex = NPC.whoAmI; // Let the minion know who the "parent" is
				minion.PositionOffset = i / (float)count; // Give it a separate position offset

				MinionMaxHealthTotal += minionNPC.lifeMax; // add the total minion life for boss bar shield text

				// Finally, syncing, only sync on server and if the NPC actually exists (Main.maxNPCs is the index of a dummy NPC, there is no point syncing it)
				if (Main.netMode == NetmodeID.Server) {
					NetMessage.SendData(MessageID.SyncNPC, number: minionNPC.whoAmI);
				}
			}

			// sync MinionMaxHealthTotal
			if (Main.netMode == NetmodeID.Server) {
				NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
			}
		}

		// Spins for transition
		//---------------------------------------------------------------
		private void phaseTransition(){
			if (AI_Timer <= 0)
			{
				spawnHitDust();
				phaseHit();
				PunchCameraModifier modifier = new PunchCameraModifier(NPC.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 20f, 6f, 100, 15000f, FullName);
				Main.instance.CameraModifiers.Add(modifier);
				SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath, NPC.Center);
			}

			if (AI_Timer >= 0 && AI_Timer < 60)
				NPC.rotation += MathHelper.ToRadians(AI_Timer * AI_Timer/60);
			else if (AI_Timer >= 60 && AI_Timer < 120)
				NPC.rotation += MathHelper.ToRadians((AI_Timer - 118) * (AI_Timer - 118)/60);

			if (AI_Timer == 60)
				NPC.rotation -= MathHelper.ToRadians(125f);

			if (AI_Timer >= 120)
				NPC.rotation = 0;

			AI_Timer++;

			if (AI_Timer >= 140)
			{
				AI_Timer = 0;
				if (phase < 4)
					AI_Choice = 0;
				else
					AI_Choice = 6;
			}
		}

		// Default move for under half health
		//---------------------------------------------------------------
		private void DefaultMove2(Player player){

			speed = baseSpeed - 8f;
			float inertia = 25f;

			Vector2 toPlayer = player.Center - NPC.Center;

			if (player.Center.X <= NPC.Center.X)
				TargetDestination = new Vector2(toPlayer.X + 650, toPlayer.Y);
			else
				TargetDestination = new Vector2(toPlayer.X - 650, toPlayer.Y);

			TargetDestination = TargetDestination.SafeNormalize(Vector2.Zero) * baseSpeed;

			NPC.velocity = (NPC.velocity * (inertia - 1) + TargetDestination) / inertia; // Interia equation?

			AI_Timer++;
		}

		// Helper move for phase 2
		//---------------------------------------------------------------
		private void softFollow(Player player, float theSpeed){
			Vector2 direction = player.Center - NPC.Center;
			direction.Normalize();
			NPC.velocity = direction * theSpeed;
		}
		
		// Filler for phase 2
		//---------------------------------------------------------------
		private void FillerAttack(Player player){
			if (firstCall)
			{
				DustWarning();
				SoundEngine.PlaySound(new SoundStyle($"{nameof(FrancisMod)}/Assets/Sounds/FrancisOmg"){
                PitchVariance = 0.3f,
                MaxInstances = 3,
			    }, NPC.Center);
				firstCall = false;
			}


			if (AI_Timer == timeToExecuteRandom + 60)
            {
				SoundEngine.PlaySound(SoundID.Item71, NPC.Center);
				if (NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
				{
					var source = NPC.GetSource_FromAI();

					Projectile.NewProjectile(source, NPC.Center, new Vector2(-20, -8), ModContent.ProjectileType<SkinnyPopBomb>(), NPC.damage/4, 0f, Main.myPlayer);
					Projectile.NewProjectile(source, NPC.Center, new Vector2(-10, -9), ModContent.ProjectileType<SkinnyPopBomb>(), NPC.damage/4, 0f, Main.myPlayer);
					Projectile.NewProjectile(source, NPC.Center, new Vector2(0, -10), ModContent.ProjectileType<SkinnyPopBomb>(), NPC.damage/4, 0f, Main.myPlayer);
					Projectile.NewProjectile(source, NPC.Center, new Vector2(10, -9), ModContent.ProjectileType<SkinnyPopBomb>(), NPC.damage/4, 0f, Main.myPlayer);
					Projectile.NewProjectile(source, NPC.Center, new Vector2(20, -8), ModContent.ProjectileType<SkinnyPopBomb>(), NPC.damage/4, 0f, Main.myPlayer);

					if (phase >= 6)
					{
						Projectile.NewProjectile(source, NPC.Center, new Vector2(-30, -7), ModContent.ProjectileType<SkinnyPopBomb>(), NPC.damage/4, 0f, Main.myPlayer);
						Projectile.NewProjectile(source, NPC.Center, new Vector2(30, -7), ModContent.ProjectileType<SkinnyPopBomb>(), NPC.damage/4, 0f, Main.myPlayer);
					}
				}
            }


			AI_Timer++;

			if (AI_Timer > timeToExecuteRandom + 70)
			{
				AI_Timer = timeToExecuteRandom - 100;
				AI_Choice = 6;
				firstCall = true;
			}
		}

		// Dash for Phase 2
		//---------------------------------------------------------------
		private void DashPrep(Player player){
			if (firstCall)
			{
				SoundEngine.PlaySound(new SoundStyle($"{nameof(FrancisMod)}/Assets/Sounds/FrancisLaugh2"){
                PitchVariance = 0.3f,
                MaxInstances = 3,
			    }, NPC.Center);
				firstCall = false;
			}
			
			Vector2 direction = player.Center - NPC.Center;
			direction.Normalize();
			NPC.velocity = direction * 5f;

			if (AI_Timer % 20 == 0)
			{
				DustWarning();
			}
			AI_Timer++;
		}
		private void Dash2(Player player){

			spawnChargeDust();

			// taken from minion boss
			float distance = 500; // Distance in pixels behind the player
			if (AI_Timer % 100 <= 70) {
				if (AI_Timer % 100 == 0) {

					Vector2 relativeDestination = (player.Center - NPC.Center) * distance;
					SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
					TargetDestination = player.Center + relativeDestination;
					NPC.netUpdate = true;
				}

				// Move along the vector
				Vector2 toDestination = TargetDestination - NPC.Center;
				Vector2 toDestinationNormalized = toDestination.SafeNormalize(Vector2.UnitY);
				float speed = Math.Min(distance, toDestination.Length());
				NPC.velocity = toDestinationNormalized * speed / 16;

				if (TargetDestination != LastFirstStageDestination) {
					// If destination changed
					NPC.TargetClosest(); // Pick the closest player target again
				}
				LastFirstStageDestination = TargetDestination;
				

				if (phase >= 6 && AI_Timer % 20 == 0)
				{	
					if (NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
						Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0,-1), ModContent.ProjectileType<FrancisHoming>(), NPC.damage / 3, 0f, Main.myPlayer);
				}
			}
			else
			{
				softFollow(player, 15f);
			}

			AI_Timer++;
			if (AI_Timer > timeToExecuteRandom + 300)
			{
				AI_Timer = 0;
				AI_Choice = 6;
				firstCall = true;
			}
		}

		// Other filler attack
		//---------------------------------------------------------------
		private void Filler2(Player player)
		{
			if (firstCall)
			{
				SoundEngine.PlaySound(new SoundStyle($"{nameof(FrancisMod)}/Assets/Sounds/FrancisLaugh2"){
                PitchVariance = 0.3f,
                MaxInstances = 3,
			    }, NPC.Center);
				firstCall = false;
			}

			if (NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient && AI_Timer % 8 == 0 && AI_Timer <= timeToExecuteRandom + 120)
            {
				var source = NPC.GetSource_FromAI();

				Vector2 direction = player.Center - NPC.Center;
				Vector2 peturbedSpeed = direction.RotatedByRandom(MathHelper.ToRadians(30));			
				peturbedSpeed.Normalize();
				
				Projectile.NewProjectile(source, NPC.Center, peturbedSpeed * 10f, ProjectileID.EyeLaser, NPC.damage / 6, 0f, Main.myPlayer);

				if (phase >= 6 && AI_Timer % 16 == 0)
				{
					peturbedSpeed = direction.RotatedByRandom(MathHelper.ToRadians(30));			
					peturbedSpeed.Normalize();
					
					Projectile.NewProjectile(source, NPC.Center, peturbedSpeed * 15f, ProjectileID.SaucerLaser, NPC.damage / 4, 0f, Main.myPlayer);
				}
			}

			// Shotgun blast
			if (AI_Timer == timeToExecuteRandom + 170)
            {
				if (NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
				{
					var source = NPC.GetSource_FromAI();
					
					for (int i = 0; i < 20; i++) {
						Vector2 peturbedSpeed = (player.Center - NPC.Center).RotatedByRandom(MathHelper.ToRadians(80));			
						peturbedSpeed.Normalize();
						peturbedSpeed *= 1f - Main.rand.NextFloat(0.3f);
						
						Projectile.NewProjectile(source, NPC.Center, peturbedSpeed * 10f, ProjectileID.DeathLaser, NPC.damage / 4, 0f, Main.myPlayer);

						if (phase >= 6)
							Projectile.NewProjectile(source, NPC.Center, peturbedSpeed * 10f, ProjectileID.SaucerLaser, NPC.damage / 5, 0f, Main.myPlayer);
					}
				}
				PunchCameraModifier modifier = new PunchCameraModifier(NPC.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 10f, 6f, 30, default, FullName);
				Main.instance.CameraModifiers.Add(modifier);
			}

			AI_Timer++;

			if (AI_Timer > timeToExecuteRandom + 170)
			{
				AI_Timer = 0;
				firstCall = true;
				AI_Choice = 6;
			}
		}
		
		// summon FrancisBall
		//---------------------------------------------------------------
		private void FrancisBallLaunch(Player player)
		{
			if (firstCall)
			{
				SoundEngine.PlaySound(new SoundStyle($"{nameof(FrancisMod)}/Assets/Sounds/FrancisLaugh2"){
                PitchVariance = 0.3f,
                MaxInstances = 3,
			    }, NPC.Center);
				firstCall = false;
			}

			if (AI_Timer == timeToExecuteRandom + 50)
			{
				SoundEngine.PlaySound(SoundID.Item45);

				if (NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
				{
					NPC.position += NPC.netOffset;

					var source = NPC.GetSource_FromAI();

					Vector2 direction = player.Center - NPC.Center;
					direction.Normalize();
					float projspeed = 12f;

					float mode = 0;
					if (phase >= 6)
						mode = 69;

					for(int i = 0; i < 360; i += 36){
						direction = MathHelper.ToRadians(i).ToRotationVector2();
						Projectile.NewProjectile(source, NPC.Center, direction * projspeed, ModContent.ProjectileType<FrancisBall>(), NPC.damage / 3, 0f, Main.myPlayer, 0 , mode);
					}

					NPC.position -= NPC.netOffset;
				}
			}

			AI_Timer++;
			if (AI_Timer > timeToExecuteRandom + 100)
			{
				AI_Timer = 0;
				AI_Choice = 6;
				firstCall = true;
			}
		}

		// summon Mech minion
		//---------------------------------------------------------------
		private void FrancisMechMinionSummon(Player player){
			if (firstCall)
			{
				SoundEngine.PlaySound(new SoundStyle($"{nameof(FrancisMod)}/Assets/Sounds/FrancisLaugh2"){
                PitchVariance = 0.3f,
                MaxInstances = 3,
			    }, NPC.Center);
				firstCall = false;
			}

			if (AI_Timer == timeToExecuteRandom + 60)
			{
				spawnHitDust();
                var entitySource = NPC.GetSource_FromAI();
                // Spawn minions
                NPC minionNPC = NPC.NewNPCDirect(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<FrancisMechMinion>(), NPC.whoAmI);
			}


			AI_Timer++;
			if (AI_Timer > timeToExecuteRandom + 110)
			{
				AI_Timer = 0;
				AI_Choice = 6;
				firstCall = true;
			}
		}

		// HalfHealthTransition
		//---------------------------------------------------------------
		private void HalfHealthTransition(){
			if (AI_Timer <= 0)
			{
				NPC.dontTakeDamage = true;
				spawnHitDust();
				phaseHit();
				PunchCameraModifier modifier = new PunchCameraModifier(NPC.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 20f, 6f, 100, 15000f, FullName);
				Main.instance.CameraModifiers.Add(modifier);
				SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath, NPC.Center);
				PlayerCamera.setNPCPos = true;

				if (!Main.dedServ) {
					Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/RedSunSecondLoop");
				} 
				canRandom = false;

				Main.windSpeedCurrent = 1.2f;
				Main.cloudAlpha = 1f;
			}
			PlayerCamera.destination = NPC.Center;

			if (AI_Timer <= 60)
				PlayerCamera.step = AI_Timer / 60; 
			else if (AI_Timer >= 180)
				PlayerCamera.step = 1 - ((AI_Timer - 180) / 60);
			else
				PlayerCamera.step = 1f; 

			AI_Timer++;

			if (AI_Timer >= 240)
			{
				NPC.dontTakeDamage = false;
				canRandom = true;
				PlayerCamera.setNPCPos = false;
				AI_Timer = 0;
				AI_Choice = 6;
			}
		}
		// Desperation Attack
		//---------------------------------------------------------------
		private void Desperation()
		{
			if (AI_Timer >= 60)
			{
				for (int i = 0; i < Main.maxPlayers; i++) {
					Player target = Main.player[i];
					if (target.active) {
						Vector2 toNPC = target.DirectionTo(NPC.Center) * normalLerp(0f, 1.1f, AI_Timer/710);
						target.velocity += toNPC;
					}
				}

					Vector2 speed = Main.rand.NextVector2Unit((float)MathHelper.Pi / 4, 3* (float)MathHelper.Pi / 4) * Main.rand.NextFloat() * 5f;	
					Dust dust1 = Dust.NewDustPerfect(NPC.Center, 174, speed, 250, Color.White, 4f);
					dust1.noGravity = true;

				if (AI_Timer % 20 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
				{
					Vector2 randVect = Main.rand.NextVector2Unit();
					randVect.Normalize();
					Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, randVect * 10f, ProjectileID.EyeLaser, NPC.damage / 2, 0f, Main.myPlayer);
				}


				if (AI_Timer % 40 == 0 && AI_Timer >= 250 && Main.netMode != NetmodeID.MultiplayerClient)
				{
					for(int i = 0; i < 360; i += 30){
						Vector2 direction = MathHelper.ToRadians(i + finalAttackAngle).ToRotationVector2();
						Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, direction * 7f, ProjectileID.SaucerLaser, NPC.damage / 5, 0f, Main.myPlayer);
					}
					finalAttackAngle += 18;
				}

				if (AI_Timer % 20 == 0 && AI_Timer >= 500 && Main.netMode != NetmodeID.MultiplayerClient )
				{
					Vector2 angle = MathHelper.ToRadians(finalAttackAngle).ToRotationVector2();
					Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, angle * 10f, ModContent.ProjectileType<FrancisBall>(), NPC.damage / 5, 0f, Main.myPlayer);
					
					finalAttackAngle += 47;
				}

				if (AI_Timer % 10 == 0 && AI_Timer >= 650 && Main.netMode != NetmodeID.MultiplayerClient )
				{
					Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Main.rand.NextVector2Unit(5 * MathHelper.Pi / 4, 7 * MathHelper.Pi / 4) * 10f, ModContent.ProjectileType<SkinnyPopBomb>(), NPC.damage / 5, 0f, Main.myPlayer);
				}

				if (AI_Timer % 200 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
				{
					SoundEngine.PlaySound(new SoundStyle($"{nameof(FrancisMod)}/Assets/Sounds/FrancisLaugh2"){
					PitchVariance = 0.3f,
					MaxInstances = 3,
					}, NPC.Center);
				}
			}

			AI_Timer++;
			AI_Choice = 13;
			
			if (AI_Timer == 800)
			{
				FrancisDeath();		
			}
			if (AI_Timer >= 800)
			{
				NPC.defense = 0;
			}
		}



		//---------------------------------------------------------------
		// Phase Checks
		//
		// Phase 2 check -- currently phase 1
		//---------------------------------------------------------------
		private void CheckForPhase2(){
			if (phase >= 2)
				return;

			if (NPC.life < 3 * NPC.lifeMax / 4)
			{
				spawnHitDust();
				phase = 2;
				NPC.dontTakeDamage = true;
				canRandom = false;

				AI_Timer = 0;
				AI_Choice = 5;
				baseSpeed = 10f;
				phaseTransition();
				SpawnMinions(6);
				
				SoundEngine.PlaySound(new SoundStyle($"{nameof(FrancisMod)}/Assets/Sounds/FrancisPhaseTransition"), NPC.Center);

				if (Main.netMode != NetmodeID.MultiplayerClient)
					NPC.netUpdate = true;
			}
		}
		
		// Phase 3 check -- currently phase 2
		//---------------------------------------------------------------
		private void CheckForPhase3() {
			MinionHealthTotal = 0;
			if (phase >= 3 || phase <= 1) 
				return;

			// Updates minionn health total
			for (int i = 0; i < Main.maxNPCs; i++) {
				NPC otherNPC = Main.npc[i];
				if (otherNPC.active && otherNPC.type == MinionType() && otherNPC.ModNPC is MetalGearFrancisMinion minion) {
					if (minion.ParentIndex == NPC.whoAmI) {
						MinionHealthTotal += otherNPC.life;
					}
				}
			}
			
			if (MinionHealthTotal <= 0) {
				// If we have no shields (aka "no minions alive"), we initiate the second stage, and notify other players that this NPC has reached its second stage
				// by setting NPC.netUpdate to true in this tick. It will send important data like position, velocity and the NPC.ai[] array to all connected clients

				// Because SecondStage is a property using NPC.ai[], it will get synced this way
				phase = 3;

				spawnHitDust();
				NPC.dontTakeDamage = false;

				timeToExecuteRandom = 130f;
				baseSpeed = 24f;
				AI_Timer = 0;
				canRandom = true;

				SoundEngine.PlaySound(new SoundStyle($"{nameof(FrancisMod)}/Assets/Sounds/FrancisPhaseTransition"), NPC.Center);
			}
		}

		// Phase 4 check -- currently phase 3
		//---------------------------------------------------------------
		private void CheckForPhase4(){
			if (phase >= 4)
				return;

			if (NPC.life < NPC.lifeMax / 2)
			{
				spawnHitDust();
				phase = 4;

				AI_Timer = 0;
				timeToExecuteRandom = 140f;
				AI_Choice = 12;
				HalfHealthTransition();
				baseSpeed = 36f;
				
				AttackMin = 7;
				AttackRange = 5;
				
				SoundEngine.PlaySound(new SoundStyle($"{nameof(FrancisMod)}/Assets/Sounds/FrancisPhaseTransition"), NPC.Center);

				if (Main.netMode != NetmodeID.MultiplayerClient)
					NPC.netUpdate = true;
			}
		}

		// Phase 5 check -- currently phase 4
		//---------------------------------------------------------------
		private void CheckForPhase5(){
			if (phase >= 5)
				return;

			if (NPC.life < NPC.lifeMax / 4)
			{
				spawnHitDust();
				phase = 5;
				
				NPC.dontTakeDamage = true;
				canRandom = false;

				SpawnedMinions = false;
				AI_Timer = 0;
				AI_Choice = 5;
				baseSpeed = 18f;
				phaseTransition();
				SpawnMinions(12);
				SoundEngine.PlaySound(new SoundStyle($"{nameof(FrancisMod)}/Assets/Sounds/FrancisPhaseTransition"), NPC.Center);

				if (Main.netMode != NetmodeID.MultiplayerClient)
					NPC.netUpdate = true;
			}
		}

		// Phase 6 check -- currently phase 5
		private void CheckForPhase6() {
			MinionHealthTotal = 0;
			if (phase >= 6 || phase <= 4) 
				return;

			// Updates minionn health total
			for (int i = 0; i < Main.maxNPCs; i++) {
				NPC otherNPC = Main.npc[i];
				if (otherNPC.active && otherNPC.type == MinionType() && otherNPC.ModNPC is MetalGearFrancisMinion minion) {
					if (minion.ParentIndex == NPC.whoAmI) {
						MinionHealthTotal += otherNPC.life;
					}
				}
			}
			
			if (MinionHealthTotal <= 0) {
				phase = 6;

				spawnHitDust();
				timeToExecuteRandom = 40f;
				NPC.dontTakeDamage = false;
				baseSpeed = 50f;
				AI_Timer = 0;
				AI_Choice = 6;
				canRandom = true;
				NPC.defense = 150;

				SoundEngine.PlaySound(new SoundStyle($"{nameof(FrancisMod)}/Assets/Sounds/FrancisPhaseTransition"), NPC.Center);
			}
		}



		//---------------------------------------------------------------
        // AI HELPERS
		//---------------------------------------------------------------
        private void spawnChargeDust()
		{
            Dust dust2 = Dust.NewDustDirect(NPC.Center, 40, 40, DustID.Smoke, 0f, 0f, 50, default, 3f);
            dust2.noGravity = true;
            dust2.velocity *= 0.1f;
		}
		private void spawnHitDust()
		{
			Vector2 speed = Main.rand.NextVector2Unit();
            for (int i = 0; i < 360; i += 5) {
                Dust dust = Dust.NewDustPerfect(NPC.Center, DustID.SnowflakeIce, speed.RotatedBy(MathHelper.ToRadians(i)) * 9f, 50, Color.White, 4f);
                    
                dust.noGravity = true;
            }
		}
		private void DustWarning()
		{
			Vector2 speed = Main.rand.NextVector2Unit();
            for (int i = 0; i < 360; i += 5) {
                Dust dust = Dust.NewDustPerfect(NPC.Center, 174, speed.RotatedBy(MathHelper.ToRadians(i)) * 11f, 50, Color.White, 4f);
                    
                dust.noGravity = true;
            }
		}
		private void phaseHit(){
			for (int i = 0; i < 20; i++)
			{
            	Dust.NewDustDirect(NPC.Center, 40, 40, DustID.Smoke, Main.rand.Next(4) -8, Main.rand.Next(4) - 12, 50, default, 4f);
				Dust.NewDustDirect(NPC.Center, 40, 40, DustID.CursedTorch, Main.rand.Next(4) -8, Main.rand.Next(4) - 12, 50, default, 2f);
			}
		}
		public static int MinionType() {
			return ModContent.NPCType<MetalGearFrancisMinion>();
		}








		public override void SetStaticDefaults() {
			Main.npcFrameCount[Type] = 12;

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

		private int hitbox = 120;
		public override void SetDefaults() {
			NPC.BossBar = ModContent.GetInstance<MetalGearFrancisBossBar>();

			NPC.width = hitbox;
			NPC.height = hitbox;
			NPC.damage = 115;
			NPC.defense = 100;
			NPC.lifeMax = 450000;


			NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = new SoundStyle($"{nameof(FrancisMod)}/Assets/Sounds/FrancisDeath"){
                Volume = 0.9f,
                PitchVariance = 0.3f,
                MaxInstances = 3,
		    };
	
			NPC.knockBackResist = 0f;

			NPC.noGravity = true;
			NPC.noTileCollide = true;

			NPC.value = Item.buyPrice(gold: 15);

			NPC.SpawnWithHigherTime(30);

			NPC.boss = true;

			NPC.npcSlots = 10f; 

			NPC.aiStyle = -1;

			AI_Timer = 0;

			// The following code assigns a music track to the boss in a simple way.
			if (!Main.dedServ) {
				Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/RedSunFirstLoop");
			} 
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
			// Sets the description of this NPC that is listed in the bestiary
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
				new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), // Plain black background
				new FlavorTextBestiaryInfoElement("Metal Gear Francis")
			});
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot) {
			// Do NOT misuse the ModifyNPCLoot and OnKill hooks: the former is only used for registering drops, the latter for everything else
			
			npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeables.MGFrancisRelic>()));
			
			npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<MGFrancisBossBag>()));

			LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());


			npcLoot.Add(notExpertRule);

		}

		public override void BossLoot(ref string name, ref int potionType) {
			potionType = ItemID.SuperHealingPotion;
		}
		public override bool CanHitPlayer(Player target, ref int cooldownSlot) {
			cooldownSlot = ImmunityCooldownID.Bosses; // use the boss immunity cooldown counter, to prevent ignoring boss attacks by taking damage from other sources
			return true;
		}




        public override void FindFrame(int frameHeight) {
            NPC.spriteDirection = NPC.direction;

			if (AI_Timer % 3 == 0)
				Lighting.AddLight(NPC.Center, 0.7f, 0.4f, 0.3f);


            if (AI_Choice <= (float)ActionState.Ring || (AI_Choice == (float)ActionState.PhaseTransition && phase < 4))
			{

                if (AI_Timer % 40 < 10)
                {
                    NPC.frame.Y = (int)Frame.Default1 * frameHeight;
                }
                else if (AI_Timer % 40 >= 10 && AI_Timer % 40 < 20)
                {
                    NPC.frame.Y = (int)Frame.Default2 * frameHeight;
                }
                else if (AI_Timer % 40 >= 20 && AI_Timer % 40 < 30)
                {
                    NPC.frame.Y = (int)Frame.Default3 * frameHeight;
                }
                else if (AI_Timer % 40 >= 30 && AI_Timer % 40 < 40)
                {
                    NPC.frame.Y = (int)Frame.Default4 * frameHeight;
                }
			}
            else if (AI_Choice > (float)ActionState.PhaseTransition && AI_Choice <= (float)ActionState.FrancisMechMinionSummon)
			{

                if (AI_Timer % 40 < 10)
                {
                    NPC.frame.Y = (int)Frame.Phase2Default1 * frameHeight;
                }
                else if (AI_Timer % 40 >= 10 && AI_Timer % 40 < 20)
                {
                    NPC.frame.Y = (int)Frame.Phase2Default2 * frameHeight;
                }
                else if (AI_Timer % 40 >= 20 && AI_Timer % 40 < 30)
                {
                    NPC.frame.Y = (int)Frame.Phase2Default3 * frameHeight;
                }
                else if (AI_Timer % 40 >= 30 && AI_Timer % 40 < 40)
                {
                    NPC.frame.Y = (int)Frame.Phase2Default4 * frameHeight;
                }
			}
			else if (AI_Choice == (float)ActionState.HalfHealthTransition)
			{
				Lighting.AddLight(NPC.Center, 0.9f, 0.9f, 0.9f);

                if (AI_Timer >= 70 && AI_Timer < 85)
                {
                    NPC.frame.Y = (int)Frame.Transition1 * frameHeight;
                }
                else if (AI_Timer >= 85 && AI_Timer < 100)
                {
                    NPC.frame.Y = (int)Frame.Transition2 * frameHeight;
                }
                else if (AI_Timer >= 100 && AI_Timer < 115)
                {
                    NPC.frame.Y = (int)Frame.Transition3 * frameHeight;
                }
                else if (AI_Timer >= 115 && AI_Timer < 130)
                {
                    NPC.frame.Y = (int)Frame.Transition4 * frameHeight;
                }
				else if (AI_Timer == 150)
				{
					NPC.damage = 130;
					SoundEngine.PlaySound(new SoundStyle($"{nameof(FrancisMod)}/Assets/Sounds/FrancisPhaseTransition"), NPC.Center);

					PunchCameraModifier modifier = new PunchCameraModifier(NPC.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 20f, 6f, 60, default, FullName);
					Main.instance.CameraModifiers.Add(modifier);
					spawnChargeDust();
					phaseHit();
					DustWarning();
				}
			}

		}




		public override void HitEffect(NPC.HitInfo hit) {
			
			// Triggers once On below 1 HP
			if (NPC.life <= 1 && !isFirstDesperation) {
				// These gores work by simply existing as a texture inside any folder which path contains "Gores/"
				NPC.life = 1;
				phaseHit();
				DustWarning();
				SoundEngine.PlaySound(new SoundStyle($"{nameof(FrancisMod)}/Assets/Sounds/FrancisPhaseTransition"), NPC.Center);
				PunchCameraModifier modifier = new PunchCameraModifier(NPC.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 35f, 6f, 60, default, FullName);
				Main.instance.CameraModifiers.Add(modifier);
				isFirstDesperation = true;
				AI_Choice = 13;
				AI_Timer = 0;
				canRandom = false;
				NPC.defense = 100000000;
				if (!Main.dedServ) {
					Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/RedSunFinale");
				} 
			}

			if (NPC.life <= 1 && !isDesperationDone)
			{
				NPC.life = 1;
			}

		}

		private void FrancisDeath()
		{
			phaseHit();
			DustWarning();

			isDesperationDone = true;
			PunchCameraModifier modifier = new PunchCameraModifier(NPC.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 40f, 6f, 120, default, FullName);
			Main.instance.CameraModifiers.Add(modifier);

			Vector2 speed = Main.rand.NextVector2Unit();
            for (int i = 0; i < 360; i += 3) {
                Dust dust = Dust.NewDustPerfect(NPC.Center, DustID.Lava, speed.RotatedBy(MathHelper.ToRadians(i)) * 15f, 250, Color.White, 5f);
                dust.velocity *= 0.99f;
                dust.noGravity = true;
            }

					for(int i = 0; i < 360; i += 90){
						Vector2 direction = MathHelper.ToRadians(i).ToRotationVector2();
						Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, direction, ProjectileID.DD2ExplosiveTrapT3Explosion, 0, 0f, Main.myPlayer);
					}
		}

		private float normalLerp(float a, float b, float t)
		{
			return a + (t*(b-a));
		}

		public override void OnKill() {
			NPC.SetEventFlagCleared(ref BossSystem.downedMGFrancis, -1);
		}


	}
}
