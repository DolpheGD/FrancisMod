using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items;
using Terraria.Audio;
using Terraria.Graphics.Shaders;

namespace FrancisMod.Content.Projectiles.FriendlyProjectiles
{
    public class DiscOfFrancisProjectile2 : ModProjectile
    {
        public ref float Timer => ref Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;

            Projectile.penetrate = -1;
            Projectile.maxPenetrate = -1;

            Projectile.usesLocalNPCImmunity = true;

            Projectile.localNPCHitCooldown = 15;
            Projectile.timeLeft = 500;
            
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
        }
        // Taken from examplemod

        public override void AI()
        {
            LightingandDust();
            float maxDetectRadius = 2000f;
            float projSpeed = 17f;

            Projectile.rotation += 0.2f * Projectile.direction;

            if (Timer >= 60)
            {
                Projectile.penetrate = 1;
                Projectile.maxPenetrate = 1;

                NPC closestNPC = FindClosestNPC(maxDetectRadius);
                if (closestNPC == null)
                    return;

                float inertia = 20f;
                Vector2 direction = (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                
                Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction * projSpeed) / inertia;
            }
            else
            {
                Projectile.velocity *= 0.96f;
            }

            Timer++;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 3);
            return false;
        }
    

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCHit4, Projectile.Center);
        }


        public NPC FindClosestNPC(float maxDetectDistance) {
			NPC closestNPC = null;

			// Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
			float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

			// Loop through all NPCs(max always 200)
			for (int k = 0; k < Main.maxNPCs; k++) {
				NPC target = Main.npc[k];
				// Check if NPC able to be targeted. It means that NPC is
				// 1. active (alive)
				// 2. chaseable (e.g. not a cultist archer)
				// 3. max life bigger than 5 (e.g. not a critter)
				// 4. can take damage (e.g. moonlord core after all it's parts are downed)
				// 5. hostile (!friendly)
				// 6. not immortal (e.g. not a target dummy)
				if (target.CanBeChasedBy()) {
					// The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
					float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

					// Check if it is within the radius
					if (sqrDistanceToTarget < sqrMaxDetectDistance) {
						sqrMaxDetectDistance = sqrDistanceToTarget;
						closestNPC = target;
					}
				}
			}

			return closestNPC;
		}
        
        private void LightingandDust()
        {
            Lighting.AddLight(Projectile.Center, 0.4f, 0.4f, 0.4f);
            if (!Main.rand.NextBool(12))
                return;
            Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.TerraBlade, Projectile.velocity.X, Projectile.velocity.Y);
        }
    }
}