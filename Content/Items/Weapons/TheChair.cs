using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using FrancisMod.Content.Items.Placeables;

namespace FrancisMod.Content.Items.Weapons
{
	public class TheChair : ModItem
	{
		public override void SetDefaults() {
			Item.width = 68;
			Item.height = 30;

			Item.useTime = 8;
			Item.useAnimation = 8;

			Item.useStyle = ItemUseStyleID.Shoot;
			Item.autoReuse = true;

			Item.damage = 40;
			Item.DamageType = DamageClass.Ranged;

			Item.UseSound = SoundID.Item13;
			Item.shoot = ProjectileID.WaterStream;
			Item.useAmmo = AmmoID.Gel;
            Item.shootSpeed = 16;
			Item.useTurn = true;

			Item.SetShopValues(ItemRarityColor.Orange3, 19150);
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<FrancisBar>(), 15);
			recipe.AddIngredient(ItemID.HellstoneBar, 15);
			recipe.AddIngredient(ItemID.SlimeGun, 1);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}
