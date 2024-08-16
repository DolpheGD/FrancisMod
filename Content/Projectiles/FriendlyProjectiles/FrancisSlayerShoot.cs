using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FrancisMod.Content.Projectiles.FriendlyProjectiles
{
	public class FrancisSlayerShoot : ModProjectile
	{
        public override string Texture => "FrancisMod/Content/Items/Weapons/TheFrancisSlayer"; // Use texture of item as projectile texture

        public float isHit { // 0 for no, 1 for yes
			get => Projectile.ai[0];
			set => Projectile.ai[0] = (float)value;
		}

		public override void SetDefaults() {
			Projectile.width = 90; // The width of projectile hitbox
			Projectile.height = 100; // The height of projectile hitbox

			Projectile.aiStyle = 0; // The ai style of the projectile (0 means custom AI). For more please reference the source code of Terraria
			Projectile.DamageType = DamageClass.Melee; // What type of damage does this projectile affect?

			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?

			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.light = 0.8f; // How much light emit around the projectile

			Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.penetrate = -1;
			Projectile.timeLeft = 120; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)


		}

		// Additional hooks/methods here.

		public override void AI() {
        
            Lighting.AddLight(Projectile.Center, 0.9f, 0.1f, 0.3f);

            if (Projectile.ai[0] % 3 == 0)
            {
                Dust dust1 = Dust.NewDustDirect(Projectile.position, 60, 60, DustID.Torch, 0f, 0f, 100, default, 3f);
                dust1.noGravity = true;
            }

            Projectile.rotation += MathHelper.ToRadians(15);
            Projectile.velocity *= 0.995f;
		}


        // Finding the closest NPC to attack within maxDetectDistance range
        // If not found then returns null
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item89, Projectile.Center);

			for (int g = 0; g < 2; g++) {
				var goreSpawnPosition = new Vector2(Projectile.position.X + Projectile.width / 2 - 24f, Projectile.position.Y + Projectile.height / 2 - 24f);
				Gore gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, 1225, 1f);
				gore.scale = 1.5f;
				gore.velocity.X += 1.5f;
				gore.velocity.Y += 1.5f;
				gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, 1225, 1f);
				gore.scale = 1.5f;
				gore.velocity.X -= 1.5f;
				gore.velocity.Y += 1.5f;
				gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, 1225, 1f);
				gore.scale = 1.5f;
				gore.velocity.X += 1.5f;
				gore.velocity.Y -= 1.5f;
				gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, 1225, 1f);
				gore.scale = 1.5f;
				gore.velocity.X -= 1.5f;
				gore.velocity.Y -= 1.5f;

                Dust dust1 = Dust.NewDustDirect(Projectile.position, 60, 60, DustID.Torch, 0f, 0f, 100, default, 3f);
                dust1.noGravity = true;
			}
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            if (isHit == 0)
            {
                isHit = 1;
                SoundEngine.PlaySound(SoundID.Item4, Projectile.Center);
                AdvancedPopupRequest request = new AdvancedPopupRequest
                {
                    Text = "Hit!",
                    Color = new Color(186, 241, 255),
                    DurationInFrames = 70,
                    Velocity = new Vector2(0, -5)
                };
                
                int index = PopupText.NewText(request, Projectile.Center);
                Main.popupText[index].scale += 10f;

                Player targetPlayer = Main.player[Projectile.owner];
                Vector2 speed = Main.rand.NextVector2Unit();

                for (int i = 0; i < 360; i += 5) {
                    Dust dust = Dust.NewDustPerfect(targetPlayer.Center, DustID.IceTorch, speed.RotatedBy(MathHelper.ToRadians(i)) * 4f, 255, Color.White, 4f);
                        
                    dust.noGravity = true;
                }

            }
        }

    }
}