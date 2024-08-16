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
using System.Threading;

namespace FrancisMod.Content.Projectiles.FriendlyProjectiles
{
    public class DiscOfFrancisProjectile : ModProjectile
    {
        public ref float Timer => ref Projectile.ai[1];
        public ref float Mode => ref Projectile.ai[2];

        private bool recall = false;
        private bool firstCall = true;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
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
            Projectile.timeLeft = 180;
            
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
        }

        public override void AI()
        {
            LightingandDust();
            if ((Main.player[Projectile.owner].position - Projectile.position).Length() > 660f)
            {
                recall = true;
                Projectile.tileCollide = false;
            }

            Projectile.rotation += 0.4f * Projectile.direction;

            Timer++;

            if (recall)
            {

                if (Mode == 2 && firstCall)
                {
                    SoundEngine.PlaySound(new SoundStyle($"{nameof(FrancisMod)}/Assets/Sounds/FrancisLaugh2"){
                    Volume = 0.9f,
                    PitchVariance = 0.3f,
                    MaxInstances = 3,
                    }, Projectile.Center);

                    Vector2 direction = Main.rand.NextVector2Unit();
                    direction.Normalize();

                    for (int i = 0; i < 360; i += 60)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, direction.RotatedBy(MathHelper.ToRadians(i)) * 10f, ModContent.ProjectileType<DiscOfFrancisProjectile2>(), Projectile.damage/2, 0f, Projectile.owner);
                    }
                    firstCall = false;
                }

                
                Vector2 posDiff = Main.player[Projectile.owner].position - Projectile.position;
                if (posDiff.Length() > 30f)
                {
                    posDiff.Normalize();
                    Projectile.velocity = posDiff * 38f;
                }
                else
                {
                    Projectile.timeLeft = 0;
                    OnKill(Projectile.timeLeft);
                }
                return;
            }
        }


        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 3);
            return false;
        }


        private void LightingandDust()
        {
            Lighting.AddLight(Projectile.Center, 0.4f, 0.4f, 0.4f);
            if (!Main.rand.NextBool(5))
                return;
            Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.TerraBlade, Projectile.velocity.X, Projectile.velocity.Y);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.NPCHit4, Projectile.Center);
        }
    }
}