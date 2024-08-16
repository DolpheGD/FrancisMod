using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FrancisMod.Content.Projectiles.FriendlyProjectiles
{
	public class FrancisOrb : ModProjectile
	{
        public float rotation_amount = 0.8f;
		public override void SetDefaults() {
			Projectile.width = 40; // The width of projectile hitbox
			Projectile.height = 40; // The height of projectile hitbox

			Projectile.aiStyle = 0; // The ai style of the projectile (0 means custom AI). For more please reference the source code of Terraria
			Projectile.DamageType = DamageClass.Magic; // What type of damage does this projectile affect?

			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?

			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.light = 0.8f; // How much light emit around the projectile

			Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.maxPenetrate = -1;
			Projectile.timeLeft = 540; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)

            DrawOriginOffsetX = -8;
            DrawOriginOffsetY = -16;

            Projectile.rotation = Main.rand.Next(360);
		}

		// Additional hooks/methods here.

		public override void AI() {
                
            Lighting.AddLight(Projectile.Center, 0.9f, 0.1f, 0.3f);

            if (Projectile.ai[0] % 3 == 0)
            {
                Dust dust1 = Dust.NewDustDirect(Projectile.position, 50, 50, DustID.Torch, 0f, 0f, 100, default, 3f);
                dust1.noGravity = true;
                dust1.velocity *= 0.2f;
            }

            Projectile.velocity *= 0.98f;

            if (rotation_amount >= 0.001)
            {
                rotation_amount *= 0.98f;
            }
            Projectile.rotation += rotation_amount;
            

            Projectile.ai[0] += 1f;
            
            if (Projectile.ai[0] > 180 && Projectile.ai[0] % 120 == 0)
            {
                for (int i = 0; i < 7; i++) {
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GemDiamond, 0f, 0f, 100, default, 2f);
                    dust.noGravity = true;
                    dust.velocity *= 2f;
                }
                
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

                if(Main.myPlayer == Projectile.owner)
                {
                    Vector2 direction;
                    float temp_rotation = Main.rand.Next(360);

                    for(int i = 0; i < 360; i += 36){
                        temp_rotation = MathHelper.ToRadians(i);
                        direction = temp_rotation.ToRotationVector2();
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, direction * 8f, ProjectileID.DiamondBolt, Projectile.damage * 2, 0f, Main.myPlayer);
                    }
                }
            }
            
		}

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item89, Projectile.Center);

			for (int g = 0; g < 2; g++) {
				var goreSpawnPosition = new Vector2(Projectile.position.X + Projectile.width / 2 - 24f, Projectile.position.Y + Projectile.height / 2 - 24f);
				Gore gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
				gore.scale = 1.5f;
				gore.velocity.X += 1.5f;
				gore.velocity.Y += 1.5f;
				gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
				gore.scale = 1.5f;
				gore.velocity.X -= 1.5f;
				gore.velocity.Y += 1.5f;
				gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
				gore.scale = 1.5f;
				gore.velocity.X += 1.5f;
				gore.velocity.Y -= 1.5f;
				gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
				gore.scale = 1.5f;
				gore.velocity.X -= 1.5f;
				gore.velocity.Y -= 1.5f;
			}
        }

        // Finding the closest NPC to attack within maxDetectDistance range
        // If not found then returns null


    }
}