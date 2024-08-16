using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Enums;
using FrancisMod.Content.Items.Placeables;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Weapons.Melee;
using FrancisMod.Content.Items.NonPlaceables;

namespace FrancisMod.Content.Items.Weapons
{
	public class JoshsFrostBlade : ModItem
	{
		public override void SetDefaults() {
			Item.width = 100; 
			Item.height = 184; 

			Item.useStyle = ItemUseStyleID.Swing; // The useStyle of the Item.
			Item.useTime = 15; // The time span of using the weapon. Remember in terraria, 60 frames is a second.
			Item.useAnimation = 15; // The time span of the using animation of the weapon, suggest setting it the same as useTime.
			Item.autoReuse = true; // Whether the weapon can be used more than once automatically by holding the use button.

			Item.DamageType = DamageClass.Melee; // Whether your item is part of the melee class.
			Item.damage = 5602000; // The damage your item deals.
			Item.knockBack = 10; // The force of knockback of the weapon. Maximum is 20
			Item.crit = 50; // The critical strike chance the weapon has. The player, by default, has a 4% critical strike chance.
            
            Item.shoot = ProjectileID.FrostBoltSword;
            Item.shootSpeed = 20f;

			Item.SetShopValues(ItemRarityColor.Cyan9, 10101010);

			Item.UseSound = SoundID.Item1; // The sound when the weapon is being used.
		}

		public override void MeleeEffects(Player player, Rectangle hitbox) {
			if (Main.rand.NextBool(3)) {
				// Emit dusts when the sword is swung
                Lighting.AddLight(new Vector2(hitbox.X, hitbox.Y), 0.5f, 0.6f, 0.8f);

				Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.Frost);
			}
		}

		public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) {
			target.AddBuff(BuffID.Frostburn, 60);
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<RefinedFrancisBar>(), 9999);
			recipe.AddIngredient(ModContent.ItemType<FrancisBar>(), 500);
			recipe.AddIngredient(ModContent.ItemType<FrancisDust>(), 1000);
			recipe.AddIngredient(ModContent.ItemType<FrostBarrier>(), 2);
			recipe.AddIngredient(ModContent.ItemType<TrueBiomeBlade>(), 1);
			recipe.AddIngredient(ModContent.ItemType<TheFrancisSlayer>(), 20);
			recipe.AddIngredient(ModContent.ItemType<FrancisSword>(), 20);
			recipe.AddIngredient(ItemID.CopperShortsword, 150);
			recipe.AddIngredient(ItemID.Frostbrand, 5);
			recipe.AddIngredient(ItemID.FrostCore, 50);
			recipe.AddIngredient(ItemID.TerraBlade, 1);
			recipe.AddIngredient(ItemID.ToiletFrozen, 1);
			recipe.AddIngredient(ItemID.FrozenSofa, 999);
			recipe.AddIngredient(ItemID.Snowball, 9999);
			recipe.AddIngredient(ItemID.FlowerofFrost, 1);
			recipe.AddIngredient(ItemID.TerraToilet, 100);
			recipe.AddIngredient(ItemID.IceBlock, 9999);

			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}


	}
}
