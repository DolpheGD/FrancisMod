using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FrancisMod.Content.Projectiles.EnemyProjectiles
{
	public class SkinnyPopBomb : ModProjectile
	{

        private const int DefaultWidthHeight = 44;


		public override void SetDefaults() {
			Projectile.width = DefaultWidthHeight; // The width of projectile hitbox
			Projectile.height = DefaultWidthHeight; // The height of projectile hitbox

			Projectile.aiStyle = 0; // The ai style of the projectile (0 means custom AI). For more please reference the source code of Terraria
			Projectile.DamageType = DamageClass.Ranged; // What type of damage does this projectile affect?
			Projectile.damage /= 3;

			Projectile.friendly = false; // Can the projectile deal damage to enemies?
			Projectile.hostile = true; // Can the projectile deal damage to the player?

			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.light = 0.8f; // How much light emit around the projectile

			Projectile.tileCollide = true; // Can the projectile collide with tiles?
            Projectile.maxPenetrate = 0;
			Projectile.timeLeft = 180; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)

            DrawOriginOffsetY = -4;

		}

		public override void AI() {

            Lighting.AddLight(Projectile.Center, 0.9f, 0.1f, 0.3f);

			if (Projectile.ai[0] % 2 == 0)
			{
				Dust dust2 = Dust.NewDustDirect(Projectile.Center, 40, 40, DustID.Smoke, 0f, 0f, 100, default, 3f);
				Dust dust1 = Dust.NewDustDirect(Projectile.Center, 40, 40, DustID.Torch, 0f, 0f, 100, default, 2f);
				dust1.noGravity = true;
				dust1.velocity *= 0.1f;
				dust2.noGravity = true;
				dust2.velocity *= 0.1f;
			}

            if (Projectile.ai[0] > 20)
            {
				if (Projectile.velocity.Y < 14f)
                	Projectile.velocity.Y += 0.4f;

				if (Math.Abs(Projectile.velocity.X) > 1f)
					Projectile.velocity.X *= 0.98f;
            }
            

			if (Projectile.ai[0] > 180)
			{
				Projectile.Kill();
			}

            Projectile.ai[0] += 1f;

		}

		// Finding the closest NPC to attack within maxDetectDistance range
		// If not found then returns null

        public override void OnKill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Top, Vector2.Zero, ProjectileID.DD2ExplosiveTrapT3Explosion, 0, 0f, Main.myPlayer);

                Vector2 direction;

                            
                for(int i = 210; i <= 330; i += 60){
					Projectile.ai[1] = 24f - Main.rand.Next(8);
                    direction = MathHelper.ToRadians(i).ToRotationVector2();
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Top, direction * Projectile.ai[1], ProjectileID.GreekFire1, Projectile.damage, 0f, Main.myPlayer);
                }
            }

            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
        }


    }
}