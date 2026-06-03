public sealed class RelicBag : ShopBagBase
{
    protected override string MissingBagMessage => "道具背包没有找到 Content。";

    protected override bool CanAdd(ShopContentDefinition content, out string failureReason)
    {
        failureReason = string.Empty;
        if (content.Kind != ShopContentKind.Item)
        {
            failureReason = "这个商品不是道具，不能放入道具背包。";
            return false;
        }

        return true;
    }
}
