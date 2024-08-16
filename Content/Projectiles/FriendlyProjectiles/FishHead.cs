using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FrancisMod.Content.Projectiles.FriendlyProjectiles
{
	public class FishHead : ModProjectile
	{
		private bool firstCall = true;
        public ref float mode => ref Projectile.ai[2];
        public ref float Timer => ref Projectile.ai[1];
		private const int DefaultWidthHeight = 31;
		public override void SetDefaults() {
			Projectile.width = DefaultWidthHeight; 
			Projectile.height = DefaultWidthHeight; 


			Projectile.DamageType = DamageClass.Ranged; // What type of damage does this projectile affect?

			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?

			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.light = 0.3f; // How much light emit around the projectile

			Projectile.tileCollide = true; // Can the projectile collide with tiles?
            Projectile.penetrate = 1;
            Projectile.maxPenetrate = 1;
			Projectile.timeLeft = 500; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)

            Projectile.aiStyle = 1;


            DrawOriginOffsetX = 4;
            DrawOriginOffsetY = -8;

		}

		// Additional hooks/methods here.
		public override void AI() {
            Projectile.spriteDirection = Projectile.direction;

            if (firstCall)
            {
                Projectile.velocity *= 1.6f;
                firstCall = false;
            }

            if (mode == 0)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + 0.06f;
                Projectile.scale = 0.6f;
                Projectile.penetrate = -1;
                Projectile.maxPenetrate = -1;

                if (Timer % 3 == 0)
                {
                    Dust dust1 = Dust.NewDustDirect(Projectile.Top, 20, 20, DustID.Water, 0f, 0f, 240, default, 2f);
                    dust1.noGravity = true;
                    dust1.velocity *= 0.2f;
                }
            }
            else if (mode == 1 && Timer % 2 == 0)
            {
                Projectile.light = 0.7f;
                Lighting.AddLight(Projectile.Center, 0.4f, 0.5f, 0.8f);
                Dust dust1 = Dust.NewDustDirect(Projectile.Center, 20, 20, DustID.BlueTorch, 0f, 0f, 250, default, 4f);
                dust1.noGravity = true;
            }

            Timer++;
		}


        public override void OnKill(int timeLeft)
        {
			if (Projectile.owner == Main.myPlayer && mode == 1)
            {
				for (int i = 0; i < 5; i++) {
					// Random upward vector.
					Vector2 launchVelocity = new Vector2(Main.rand.NextFloat(-6, 6), Main.rand.NextFloat(-6, -8));
					// Importantly, ai1 is set to 1 here. This is checked in OnTileCollide to prevent bouncing and here in Kill to prevent an infinite chain of splitting projectiles.
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + new Vector2(0, -10), launchVelocity, ModContent.ProjectileType<FishHead>(), Projectile.damage/5, Projectile.knockBack, Main.myPlayer);
				}
			}
            
            if (mode == 1)
            {
                SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
			    // Smoke Dust spawn
                for (int i = 0; i < 10; i++) {
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
                    dust.velocity *= 1.4f;
                }
                for (int i = 0; i < 14; i++) {
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.BlueTorch, 0f, 0f, 255, default, 5f);
                    dust.noGravity = true;
                    dust.velocity *= 8f;
                    dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Clentaminator_Cyan, 0f, 0f, 255, default, 1f);
                    dust.velocity *= 2f;
                }
            }
            else
            {
                SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
                for (int i = 0; i < 5; i++) {
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.BlueTorch, 0f, 0f, 100, default, 3f);
                    dust.noGravity = true;
                    dust.velocity *= 3f;
                }
            }

        }
    }
}