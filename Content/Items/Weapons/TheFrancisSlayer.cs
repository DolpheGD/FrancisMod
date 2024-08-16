using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Enums;
using Terraria.Audio;
using FrancisMod.Content.Items.Placeables;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using FrancisMod.Content.Projectiles.FriendlyProjectiles;
using CalamityMod.Items.Materials;

namespace FrancisMod.Content.Items.Weapons
{
	public class TheFrancisSlayer : ModItem
	{
        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.FrancisMod.hjson file.

        private Projectile proj = new Projectile();
        private int count = 0;
		public override void SetDefaults()
		{
			// HItbox
			Item.width = 90;
			Item.height = 100;

			// Animation
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 32;
			Item.useAnimation = 32;
			Item.autoReuse = true;

			// Damage
			Item.damage = 600;
			Item.DamageType = DamageClass.Melee;
			Item.knockBack = 6;
			Item.crit = 10;

			// MIsc
			Item.SetShopValues(ItemRarityColor.Cyan9, 91550);

			Item.noMelee = true;  // This makes sure the item does not deal damage from the swinging animation
			Item.noUseGraphic = true;


			// Projectile Properties
			Item.shoot = ModContent.ProjectileType<FrancisSlayerSwing>(); // The sword as a projectile

		}


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			if (player.altFunctionUse == 0)
			{
                count++;

                if (proj.ai[0] == 1)
                {
                    Projectile.NewProjectile(source, position, velocity, type, 3 * damage, knockback, Main.myPlayer, 1);
                    proj.ai[0] = 0;
                }
                else{
                    Projectile.NewProjectile(source, position, velocity, type, damage, knockback, Main.myPlayer, 0);
                }
                
                return false;
            }
			else if (player.altFunctionUse == 2 && count >= 3)
			{
                Vector2 direction = Main.MouseWorld - player.Center;
                direction.Normalize();

                proj = Projectile.NewProjectileDirect(source, position, direction * 9f, ModContent.ProjectileType<FrancisSlayerShoot>(), damage, knockback, player.whoAmI, 0);
                
                SoundEngine.PlaySound(SoundID.Item60, player.Center);
                count -= 5;
            }

            if (count > 9)
                count = 9;

            return false;
        }

		public override bool CanUseItem(Player player){
			
			if ((player.altFunctionUse == 2 && count >= 5) || player.altFunctionUse == 0)
			{
				return true;
			}
			return false;
		}

		public override bool AltFunctionUse(Player player) {
			return true;
		}

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) {
			// Inflict the OnFire debuff for 1 second onto any NPC/Monster that this hits.
			// 60 frames = 1 second
			target.AddBuff(BuffID.OnFire, 60);
		}


        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<FrancisBar>(), 25);
			recipe.AddIngredient(ModContent.ItemType<RefinedFrancisBar>(), 25);
            recipe.AddIngredient(ModContent.ItemType<FrancisSword>(), 2);
			recipe.AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 5);
			
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
		
	}
}