# 土豆商店原型

## 运行时结构

- `ShopContentDefinition` 是武器和道具共用的展示模型。
- `ShopWeaponDefinition` 和 `ShopItemDefinition` 分别保存武器、道具字段。
- 每个具体武器或道具都有单独脚本，放在 `Content/Definitions` 下。
- `ShopContentCatalog` 注册手写内容和 XLSX 批量生成内容。
- `ShopManager` 随机刷新商品，并将数据绑定到 `ShopOfferView`。

图标不是必填项。定义脚本通过 `IconResourcePath` 绑定 `Assets/Resources` 下的 Sprite；
素材缺失时卡片会自动显示“武”或“道”占位符。

## 打开和关闭商店

早期测试用的临时界面生成器已经移除，现在商店 UI 只由场景里的 `ShopManager` 控制。
正式项目中把 `ShopManager` 挂到你的商店控制对象上，然后通过这些接口控制显隐：

```csharp
shopManager.OpenShop();
shopManager.CloseShop();
shopManager.ToggleShop();
shopManager.SetShopOpen(true);
```

推荐让 `ShopManager` 挂在一个常驻激活的父物体上，把真正需要隐藏的商店面板拖到
`Shop Window Root`。如果不拖，默认会控制 `ShopManager` 所在物体。

## 生成 ShopItem 预制体

运行：

`Tools > Potato Shop > Create ShopItem Prefab Template`

这会生成 `Assets/Prefebs/ShopItem.prefab`。模板尺寸固定为 `450 x 600`，武器与道具共用。
所有文本会自动绑定项目中的中文 TMP 字体。可以自由调整视觉效果，但请保留以下节点名称：

```text
ShopItem                 Image, Button, ShopOfferView
|- IconPanel             Image
|  `- Icon               Image
|     `- IconPlaceholder TextMeshProUGUI
|- NameText              TextMeshProUGUI
|- KindText              TextMeshProUGUI
|- LimitText             TextMeshProUGUI
|- DescriptionText       TextMeshProUGUI
|- StatsText             TextMeshProUGUI
`- PricePanel            Image
   `- PriceText          TextMeshProUGUI
```

`ShopOfferView` 会按节点名称自动绑定引用，不需要逐个拖拽字段。有限购数量的道具会显示
`限制 (0/N)`；无限制道具和武器会自动隐藏这一行。

然后配置商店界面：

1. 在商店根对象添加 `ShopManager`。
2. 创建空 UI 对象并命名为 `ShopItemContainer`，它是卡片生成位置。
3. 如果卡片需要横向排列，在容器上添加 `HorizontalLayoutGroup`。
4. 将 `ShopItem.prefab` 拖到 `ShopManager > Shop Item Prefab`。
5. 将刷新按钮命名为 `RefreshButton`，或拖到 `ShopManager > Refresh Button`。
6. `Start Open` 控制商店开局是否显示。

`ShopManager` 会根据 `Offer Count` 自动实例化并复用卡片。

具体武器和道具脚本是普通 C# 数据定义，不需要挂到 GameObject 上。

## XLSX 批量生成

将 `weapons.xlsx` 和 `items.xlsx` 放在项目根目录，然后运行：

`Tools > Potato Shop > Generate Scripts From XLSX`

编辑器导入器会为表格中的每一行生成一个脚本到 `Assets/Scripts/Shop/Generated`，
并重新构建 `GeneratedShopContentCatalog.generated.cs`。
