namespace Fashionhero.Portal.BusinessLogic
{
    public static class XmlTagConstants
    {
        #region Spartoo Export Tags

        public const string SPARTOO_ROOT = "root";

        public const string SPARTOO_PRODUCTS_ROOT = "products";

        public const string SPARTOO_PRODUCT = "product";

        public const string SPARTOO_REFERENCE_PARTNER = "reference_partenaire";
        public const string SPARTOO_MANUFACTURERS_NAME = "manufacturers_name";
        public const string SPARTOO_PRODUCT_SEX = "product_sex";
        public const string SPARTOO_PRODUCT_QUANTITY = "product_quantity";
        public const string SPARTOO_COLOUR_ID = "color_id";
        public const string SPARTOO_PRODUCT_STYLE = "product_style";
        public const string SPARTOO_HEEL_HEIGHT = "heel_height";
        public const string SPARTOO_COUNTRY_ORIGIN = "country_origin";
        public const string SPARTOO_CODE_HS = "code_hs";
        public const string SPARTOO_LANGUAGES = "languages";
        public const string SPARTOO_SIZE_LIST = "size_list";
        public const string SPARTOO_PRODUCT_COMPOSITION = "languages";
        public const string SPARTOO_LINING_COMPOSITION = "voering_composition";
        public const string SPARTOO_FIRST_COMPOSITION = "first_composition";
        public const string SPARTOO_HEEL_COMPOSITION = "zool_composition";
        public const string SPARTOO_PHOTOS = "photos";
        public const string SPARTOO_EXTRA_INFOS = "extra_infos";
        public const string SPARTOO_SELECTIONS = "selections";

        public const string SPARTOO_LANGUAGE = "language";
        public const string SPARTOO_SIZE = "size";
        public const string SPARTOO_INFO = "info";
        public const string SPARTOO_SELECTION = "selection";

        public const string SPARTOO_ID = "id";
        public const string SPARTOO_VALUE = "value";
        public const string SPARTOO_SIZE_NAME = "size_name";
        public const string SPARTOO_SIZE_QUANTITY = "size_quantity";
        public const string SPARTOO_SIZE_REFERENCE = "size_reference";
        public const string SPARTOO_EAN = "ean";

        public const string SPARTOO_CODE = "code";
        public const string SPARTOO_PRODUCT_NAME = "product_name";
        public const string SPARTOO_PRODUCT_DESCRIPTION = "product_description";
        public const string SPARTOO_CODE_IDU = "code_idu";
        public const string SPARTOO_PRODUCT_COLOR = "product_color";
        public const string SPARTOO_PRODUCT_PRICE = "product_price";
        public const string SPARTOO_DISCOUNT = "product_price";

        public const string SPARTOO_STARTDATE = "startdate";
        public const string SPARTOO_STOPDATE = "stopdate";
        public const string SPARTOO_PRICE_DISCOUNT = "price_discount";
        public const string SPARTOO_RATE = "rate";
        public const string SPARTOO_SALES = "sales";

        public static string SpartooPhotoUrl(int additionalPart)
        {
            return $"url{additionalPart}";
        }

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