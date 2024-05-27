namespace Fashionhero.Portal.BusinessLogic
{
    public static class XmlTagConstants
    {
        #region Spartoo Export Tags

        public const string SPARTOO_ROOT = "root";
        public const string SPARTOO_PRODUCTS = "products";

        #endregion

        #region Inventory Tags

        public const string INVENTORY_ROOT = "products";
        public const string INVENTORY_SINGLE_PRODUCT = "product";
        public const string INVENTORY_ID = "id";
        public const string INVENTORY_LINK = "link";
        public const string INVENTORY_IMAGE_LINK = "image_link";

        public const string
            INVENTORY_AVAILABILITY = "availability"; // Unused as 'Availability' is calculated from stock

        public const string INVENTORY_REGULAR_PRICE = "regular_price";
        public const string INVENTORY_SALE_PRICE = "sale_price";
        public const string INVENTORY_PRICE_EUR = "price_eur";
        public const string INVENTORY_SALE_PRICE_EUR = "sale_price_eur";
        public const string INVENTORY_PRICE_SEK = "price_sek";
        public const string INVENTORY_SALE_PRICE_SEK = "sale_price_sek";
        public const string INVENTORY_PRICE_PLN = "price_pln";
        public const string INVENTORY_SALE_PRICE_PLN = "sale_price_pln";
        public const string INVENTORY_MODEL_PRODUCT_NUMBER = "mpn";
        public const string INVENTORY_BRAND = "brand";
        public const string INVENTORY_EAN = "gtin";
        public const string INVENTORY_STOCK = "stock";
        public const string INVENTORY_SIZE_A = "size-a";
        public const string INVENTORY_SIZE_B = "size-b";
        public const string INVENTORY_SPARTOO_KODE = "spartoo-kode";
        public const string INVENTORY_PRODUCT_CATEGORY = "product_category";

        #endregion

        #region Localized Products Tags

        public const string LOCALE_ROOT = "products";
        public const string LOCALE_SINGLE_PRODUCT = "product";
        public const string LOCALE_ID = "id";
        public const string LOCALE_TITLE = "title";
        public const string LOCALE_DESCRIPTION = "description";
        public const string LOCALE_ITEM_GROUP_ID = "item_group_id";
        public const string LOCALE_PRODUCT_TYPE = "product_type";
        public const string LOCALE_PRODUCT_TYPE_LOCAL = "product_type_local";
        public const string LOCALE_COLOR = "color";
        public const string LOCALE_GENDER = "gender";
        public const string LOCALE_MATERIALE = "materiale";
        public const string LOCALE_COUNTRY_OF_ORIGIN = "country_of_origin";

        #endregion
    }
}