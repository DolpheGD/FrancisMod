using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Enums;
using Terraria.GameContent.Creative;
using Terraria.GameContent.UI;
using FrancisMod.Content.Items.Placeables;

namespace FrancisMod.Content.Items.Weapons
{
	public class FrancisSword : ModItem
	{
        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.FrancisMod.hjson file.

		public override void SetDefaults()
		{
			// HItbox
			Item.width = 40;
			Item.height = 40;

			// Animation
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.autoReuse = true;

			// Damage
			Item.damage = 35;
			Item.DamageType = DamageClass.Melee;
			Item.knockBack = 6;
			Item.crit = 10;

			// MIsc
			Item.SetShopValues(ItemRarityColor.Green2, 11050);
			Item.UseSound = SoundID.Item1;
			Item.useTurn = true;

			Item.shoot = ProjectileID.SwordBeam;
			Item.shootSpeed = 11;
		}
		
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<FrancisBar>(), 10);
			recipe.AddIngredient(ItemID.CopperBroadsword);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
		
	}
}