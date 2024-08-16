using System;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities.Terraria.Utilities;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.CameraModifiers;


namespace FrancisMod.Content.Projectiles.EnemyProjectiles
{
	public class FrancisMeal : ModProjectile
	{
		private float RotationTimer = 0;
		private float RotationTimerMax = 360;

		public ref float playerIndex => ref Projectile.ai[0];
		public ref float PositionOffset => ref Projectile.ai[1];
		public ref float Timer => ref Projectile.ai[2];
		private bool firstCall = true;

		public override void SetDefaults() {
			Projectile.width = 78; // The width of projectile hitbox
			Projectile.height = 52; // The height of projectile hitbox

			Projectile.aiStyle = 0; // The ai style of the projectile (0 means custom AI). For more please reference the source code of Terraria

			Projectile.friendly = false; // Can the projectile deal damage to enemies?
			Projectile.hostile = true; // Can the projectile deal damage to the player?

			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.light = 0.8f; // How much light emit around the projectile

			Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.maxPenetrate = -1;

			Projectile.timeLeft = 300; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
		}


        public override void AI()
        {
			dashDust();
			Player target = Main.player[(int)playerIndex];

			if (Timer < 240) // Circle and shoot
			{
			float rad = (float)PositionOffset * MathHelper.TwoPi;

			// Add some slight uniform rotation to make the eyes move, giving a chance to touch the player and thus helping melee players
			RotationTimer += 2f;
			if (RotationTimer > RotationTimerMax) {
				RotationTimer = 0;
			}

			// Since RotationTimer is in degrees (0..360) we can convert it to radians (0..TwoPi) easily
			rad += MathHelper.ToRadians(RotationTimer);
			if (rad > MathHelper.TwoPi) {
				rad -= MathHelper.TwoPi;
			}
			else if (rad < 0) {
				rad += MathHelper.TwoPi;
			}


			if (Timer >= 50 && Timer <= 210 && Timer % 8 == 0)
			{
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					Vector2 direction = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitY);
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, direction * 8f, ProjectileID.EyeBeam, Projectile.damage, 0f, Main.myPlayer);
				}
			}


			float distanceFromBody = PositionOffset;

			// offset is now a vector that will determine the position of the NPC based on its index
			Vector2 offset = Vector2.One.RotatedBy(rad) * distanceFromBody;
            Projectile.Center = target.Center + offset;
			}
			else if (Timer >= 240 && Timer <= 270)
			{
				Lighting.AddLight(Projectile.Center, 0.9f, 0.9f, 0.9f);
				if (Timer == 240)
				{
					SoundEngine.PlaySound(SoundID.Item71, Projectile.Center);
					Projectile.velocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitY);
				}

				if (Projectile.velocity.Length() < 30)
				{
					Projectile.velocity *= 1.16f;
				}
			}
			else if (Timer >= 271)
			{
				Vector2 direction = Projectile.velocity;
				direction.Normalize();
				if (Timer == 271)
				{
					PunchCameraModifier modifier = new PunchCameraModifier(Projectile.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 20f, 4f, 40, default, FullName);
					Main.instance.CameraModifiers.Add(modifier);

					Projectile.velocity *= 0;
					
					SoundEngine.PlaySound(new SoundStyle($"{nameof(FrancisMod)}/Assets/Sounds/Fart"){Volume = 1.5f}, Projectile.Center);
					for (int i = 95; i <= 145; i += 5)
					{
						Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, direction.RotatedBy(MathHelper.ToRadians(i)) * 10, ProjectileID.EyeBeam, Projectile.damage, 0f, Main.myPlayer);
					}

					for (int i = -95; i >= -145; i -= 5)
					{
						Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, direction.RotatedBy(MathHelper.ToRadians(i)) * 10, ProjectileID.EyeBeam, Projectile.damage, 0f, Main.myPlayer);
					}
				}
			}


			Timer++;
        }


        public override bool PreDraw(ref Color lightColor)
        {
			Vector2 origin;
			SpriteEffects effects;
			Vector2 scale = new Vector2(1f, 1f);

			if (Projectile.spriteDirection > 0) {
				origin = new Vector2(Projectile.width/2, Projectile.height);
				effects = SpriteEffects.None;
			}
			else {
				origin = new Vector2(Projectile.width/2, Projectile.height);
				effects = SpriteEffects.FlipHorizontally;
			}

			if (Timer <= 30)
			{
				scale.X = normalLerp(0, 1, Timer / 30);
			}
			else if (Timer >= 270)
			{
				scale.X = normalLerp(1, 0, (Timer - 270) / 30);
				scale.Y = normalLerp(1, 0, (Timer - 270) / 30);
			}

			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
			Main.spriteBatch.Draw(texture, (Projectile.position + origin) - Main.screenPosition, default, lightColor * Projectile.Opacity, Projectile.rotation, origin, scale, effects, 0);
			return false;
        }

		private float normalLerp(float a, float b, float t)
		{
			return a + (t*(b-a));
		}


		private void dashDust()
		{
			Dust dust1 = Dust.NewDustDirect(Projectile.Center, 30, 30, DustID.FoodPiece, 0, 0, 0, Color.White, 1f);
			Dust dust2 = Dust.NewDustDirect(Projectile.Center, 30, 30, DustID.ShimmerSplash, 0, 0, 0, Color.White, 1f);
			dust1.noGravity = true;
			dust2.noGravity = true;
			dust1.rotation *= 0.4f; 
			dust2.rotation *= 0.4f; 
		}


    }
}