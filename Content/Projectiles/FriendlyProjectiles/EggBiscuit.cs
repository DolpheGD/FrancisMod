using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FrancisMod.Content.Projectiles.FriendlyProjectiles
{
	public class EggBiscuit : ModProjectile
	{
        public ref float Timer => ref Projectile.ai[0];
		private const int DefaultWidthHeight = 40;

		public override void SetDefaults() {
			Projectile.width = DefaultWidthHeight; 
			Projectile.height = DefaultWidthHeight; 

			Projectile.DamageType = DamageClass.Magic; // What type of damage does this projectile affect?

			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?



			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.light = 0.7f; // How much light emit around the projectile

			Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.penetrate = -1;
            Projectile.maxPenetrate = -1;
			Projectile.timeLeft = 120; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)

            Projectile.scale = 1f;
            Projectile.aiStyle = 0;

            Projectile.alpha = 255;

		}

		// Additional hooks/methods here.
		public override void AI() {
            
            Lighting.AddLight(Projectile.Center, 0.85f, 0.8f, 0.4f);
            trailDust();

            if (Timer == 0)
            {
                spawnHitDust();
            }

            if (Timer <= 15)
            {
                Projectile.alpha -= 255 / 15;
            }
            
            if (Timer == 15)
            {
                SoundEngine.PlaySound(SoundID.Item71, Projectile.Center);
                Projectile.velocity *= 40;
            }

            if (Timer > 16 && Timer < 105)
            {
                 Projectile.velocity /= 1.05f;
            }


            if (Timer >= 105)
            {
                Projectile.alpha += 255 / 15;
            }

            Timer++;
		}
    
		private void spawnHitDust()
		{
			Vector2 speed = Main.rand.NextVector2Unit();
            for (int i = 0; i < 360; i += 36) {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Flare, speed.RotatedBy(MathHelper.ToRadians(i)) * 9f, 250, Color.White, 4f);
                    
                dust.noGravity = true;
            }
		}

        private void trailDust()
        {
            Dust dust2 = Dust.NewDustDirect(Projectile.Center, 20, 20, DustID.SolarFlare, 0f, 0f, 255, default, 1f);
            dust2.noGravity = true;
            dust2.velocity *= 0.1f;
            
        }
    
    }
}