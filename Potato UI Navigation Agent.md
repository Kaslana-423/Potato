# Potato UI Navigation Agent

你正在维护一个 Unity 2022.3 项目，其核心玩法已经可以完整进行一局游戏。

当前目标不是重新制作游戏系统，而是按照《The Binding of Isaac》主菜单的导航思想，重新整理整个游戏的 UI 流程与界面层级。

重点学习《以撒》的**UI 逻辑与信息架构**，不要直接模仿它的纸张、手绘字体、胶带、涂鸦等视觉表现。

---

# 一、核心目标

将项目 UI 改造成一套：

**存档上下文明确、树状导航清晰、高频操作层级浅、返回行为稳定、状态驱动菜单变化**

的 UI 系统。

整体目标流程：

```text
启动游戏
↓
标题界面
↓
存档选择
↓
主菜单
├─ 继续游戏
├─ 开始游戏
│  └─ 角色选择
│     └─ 开始 Run
├─ 图鉴
│  ├─ 道具
│  ├─ 武器
│  ├─ 角色
│  └─ 敌人
├─ 设置
└─ 退出游戏
```

如果当前存档存在尚未完成的 Run：

```text
主菜单
├─ 继续游戏
├─ 新游戏
├─ 放弃当前游戏
├─ 图鉴
├─ 设置
└─ 退出游戏
```

如果不存在未完成 Run：

```text
主菜单
├─ 开始游戏
├─ 图鉴
├─ 设置
└─ 退出游戏
```

菜单必须根据当前状态动态生成或动态决定可见项，而不是永远展示全部按钮再大量置灰。

---

# 二、UI 设计原则

## 1. 先确定存档，再进入游戏主菜单

游戏启动后不要直接进入当前主菜单。

流程应该变成：

```text
Boot
↓
TitleScreen
↓
SaveSelect
↓
MainMenu
```

玩家选择存档之后，将该存档设为：

```text
CurrentSave
```

之后所有依赖永久进度的数据，例如：

```text
角色解锁
武器解锁
道具发现
图鉴
最高波次
游戏设置以外的永久进度
当前未完成 Run
```

默认从 CurrentSave 获取。

不要让每一个 UI Panel 自己保存一个：

```text
saveSlot
```

也不要在页面之间层层传递存档编号。

应该存在一个明确的当前存档上下文。

---

## 2. UI 使用严格树状导航

所有主要页面应该具有明确父节点。

例如：

```text
SaveSelect
└─ MainMenu
   ├─ NewRun
   │  └─ CharacterSelect
   ├─ Collection
   │  ├─ ItemCollection
   │  ├─ WeaponCollection
   │  └─ EnemyCollection
   └─ Settings
```

进入页面：

```text
Push
```

返回页面：

```text
Pop
```

Back / ESC / 手柄 B 应优先执行：

```text
返回当前页面的父节点
```

禁止出现：

```text
CharacterSelect
按返回
→ 直接跳 TitleScreen
```

或者：

```text
Collection
按返回
→ 重新加载 MainMenu Scene
```

这种非连续导航。

正常情况下连续按 Back 应该可以：

```text
CharacterSelect
→ NewRun
→ MainMenu
→ SaveSelect
→ TitleScreen
```

逐层退出。

---

# 三、不要让 UI Panel 互相强耦合

禁止逐渐形成：

```csharp
MainMenuPanel.OpenCharacterPanel();
CharacterPanel.OpenCollectionPanel();
CollectionPanel.OpenSettingsPanel();
SettingsPanel.OpenMainMenuPanel();
```

这种 Panel 互相知道彼此存在的结构。

UI 页面应该只表达：

```text
我要进入某个 Route
我要返回
```

具体页面显示与隐藏由统一导航系统处理。

建议建立类似：

```text
UIRouter
UINavigationStack
UIScreen
UIRoute
```

的结构。

例如逻辑概念：

```text
Navigate(MainMenu)
Navigate(CharacterSelect)
Back()
```

页面本身不直接寻找和控制其他页面 GameObject。

---

# 四、不要制作万能主页

主菜单不是所有功能的中央控制大厅。

禁止把未来所有系统直接堆成：

```text
开始
继续
角色
装备
图鉴
武器
敌人
道具
成就
商店
设置
存档
制作人员
退出
```

一级入口数量应该严格控制。

相关功能必须形成子树。

例如：

```text
图鉴
├─ 道具
├─ 武器
├─ 角色
└─ 敌人
```

而不是：

```text
主菜单
├─ 道具
├─ 武器
├─ 角色
└─ 敌人
```

玩家进入主菜单时最重要的问题永远应该是：

```text
我要继续上一局？

还是开一局新的？
```

而不是面对十几个系统入口。

---

# 五、高频操作层级必须最浅

按照使用频率安排 UI 深度。

最高频：

```text
继续游戏
开始游戏
```

必须处于 MainMenu 一级。

中频：

```text
角色选择
图鉴
```

允许增加一层。

低频：

```text
具体图鉴分类
详细统计
制作人员
高级设置
```

可以继续向下。

不要为了视觉整齐，让：

```text
开始游戏
→ 游戏模式
→ Run 类型
→ 难度
→ 角色
→ 确认
→ 开始
```

形成六层审批流程。

如果当前版本只有一种普通 Run，就直接：

```text
开始游戏
→ 角色选择
→ 开始
```

不要为“未来可能会加模式”提前制造不存在的 UI 层级。

---

# 六、Continue 应当是状态驱动的快捷入口

存在有效进行中 Run 时：

```text
Continue
```

必须成为主菜单最直接的入口之一。

点击 Continue：

```text
加载 CurrentSave 中的 Run 状态
↓
直接进入游戏
```

不应该重新经过：

```text
角色选择
难度选择
新游戏确认
```

如果没有有效 Run：

```text
Continue
```

不要显示。

不要长期保留一个灰色 Continue 按钮。

---

# 七、新游戏与放弃当前 Run 必须分离

如果已经存在当前 Run，而玩家选择 New Run：

不要静默覆盖。

应该进入一个明确确认流程，例如：

```text
当前游戏尚未结束。

开始新游戏会放弃当前进度。

[放弃并开始新游戏]
[返回]
```

危险操作必须：

```text
显式
低频
需要确认
```

普通操作不要滥用确认窗口。

不要出现：

```text
是否进入角色选择？
是否返回主菜单？
是否打开设置？
```

这种弹窗污染。

---

# 八、暂停菜单属于游戏内独立 UI 子树

Run 中：

```text
Gameplay
└─ Pause
   ├─ Resume
   ├─ Settings
   └─ ReturnToMainMenu
```

打开 Settings：

```text
Gameplay
→ Pause
→ Settings
```

从 Settings 返回：

```text
Settings
→ Pause
```

而不是：

```text
Settings
→ MainMenu
```

同一个 Settings 页面可以被不同上下文调用，但 Back 的目标由导航栈决定。

例如：

```text
MainMenu → Settings → Back → MainMenu
```

以及：

```text
Pause → Settings → Back → Pause
```

不允许 Settings 自己硬编码：

```text
Back = MainMenu
```

---

# 九、场景切换和 UI 导航必须分离

不要为了切一个菜单页面就加载 Scene。

建议：

```text
MainMenu Scene
```

内部承担：

```text
Title
SaveSelect
MainMenu
CharacterSelect
Collection
Settings
```

这些界面之间只进行 UI Route 切换。

只有真正进入 Run 时：

```text
MainMenu Scene
→ Gameplay Scene
```

游戏结束或者主动返回时：

```text
Gameplay Scene
→ MainMenu Scene
```

然后根据导航上下文进入正确页面。

不要：

```text
MainMenu.unity
CharacterSelect.unity
Settings.unity
Collection.unity
```

一页一个 Scene。

---

# 十、导航状态与游戏数据分离

明确区分：

```text
SaveData
RunData
UIState
SettingsData
```

它们不是一回事。

SaveData：

```text
永久解锁
图鉴
统计
当前 Run 引用
```

RunData：

```text
当前波次
生命
属性
经验
材料
武器
道具
商店状态
```

UIState：

```text
当前 Route
导航栈
当前焦点按钮
临时选择
```

SettingsData：

```text
音量
全屏
分辨率
控制设置
```

不要把：

```text
当前打开哪个 Panel
```

写入正式游戏存档。

---

# 十一、输入系统必须统一

以下输入应该共享同一导航逻辑：

```text
鼠标点击 Back
ESC
手柄 B / Cancel
```

全部最终调用：

```text
UIRouter.Back()
```

不要分别实现三套返回行为。

确认操作同理：

```text
鼠标点击
Enter
Space
手柄 A
```

最终作用于当前 Selected UI Element。

UI 必须从结构上支持手柄导航，不要只保证鼠标能点。

---

# 十二、焦点恢复

进入子页面时，应记录父页面当前选择项。

例如：

```text
MainMenu
当前选中：开始游戏

↓ Enter

CharacterSelect

↓ Back

MainMenu
当前仍选中：开始游戏
```

不要每次 Back 都把选中位置重置到第一个按钮。

这对键盘和手柄体验非常重要。

---

# 十三、UI 生命周期

UIScreen 建议至少区分：

```text
OnEnter
OnExit
OnPause
OnResume
```

不要让所有页面只依赖：

```csharp
gameObject.SetActive(true);
gameObject.SetActive(false);
```

然后把各种业务逻辑塞进：

```csharp
OnEnable()
```

避免重复打开页面时：

```text
重复注册事件
重复刷新数据
重复生成按钮
重复播放动画
```

页面的数据刷新应该有明确生命周期。

---

# 十四、禁止行为

不要：

```text
为了重构 UI 重写现有存档系统
```

不要：

```text
修改战斗、商店、升级、武器等已经正常工作的玩法逻辑
```

不要：

```text
一次删除现有 MainMenu 再从零重建
```

不要：

```text
Panel A 直接持有 Panel B、C、D、E 引用
```

不要：

```text
使用 FindObjectOfType / GameObject.Find 解决页面导航
```

不要：

```text
每增加一个按钮就增加一个新的全局 bool
```

不要：

```text
为了未来不存在的功能过度抽象
```

不要：

```text
照抄《以撒》的视觉素材
```

我们学习的是它：

```text
信息层级
导航树
状态驱动入口
低操作成本
稳定返回规则
```

而不是：

```text
纸
胶带
手写字
```

---

# 十五、执行顺序

不要一次性修改全部 UI。

按以下顺序推进。

## Phase 1：审计

先检查现有：

```text
MainMenu
SaveSystem
PauseMenu
Scene Loading
Settings
```

相关脚本、Prefab 和 Scene。

输出当前 UI 导航关系。

明确哪些逻辑可以直接复用。

此阶段不要修改代码。

---

## Phase 2：建立 Navigation Core

只建立最小导航核心：

```text
UIRoute
UIScreen
UIRouter
Back Stack
```

先让：

```text
MainMenu
→ Settings
→ Back
```

完整跑通。

验证无误以后再继续。

---

## Phase 3：接入 Save Context

建立明确的：

```text
CurrentSave
```

选择存档后设置上下文。

先完成：

```text
Title
→ SaveSelect
→ MainMenu
```

不要同时制作图鉴。

---

## Phase 4：重构 Run 入口

完成：

```text
MainMenu
├─ Continue
└─ NewRun
   └─ CharacterSelect
      └─ Gameplay
```

同时根据 CurrentSave 自动决定：

```text
Continue 是否存在
AbandonRun 是否存在
```

---

## Phase 5：暂停菜单

将：

```text
Pause
→ Settings
→ Back
→ Pause
```

纳入同一导航规则。

确认：

```text
Time.timeScale
```

不会因 UI 跳转异常残留为 0。

---

## Phase 6：扩展低频功能

导航骨架确认稳定以后，再加入：

```text
Collection
Statistics
Credits
其他永久系统
```

---

# 十六、每次修改规则

每次只处理一个阶段。

修改前：

1. 阅读相关现有代码。
2. 明确准备复用什么。
3. 明确准备修改什么。
4. 检查是否影响存档兼容。
5. 检查 Scene 和 Prefab 引用。

修改后：

1. 编译项目。
2. 确认 0 Error。
3. 测试鼠标导航。
4. 测试 ESC 返回。
5. 如果已有手柄支持，测试 Cancel。
6. 测试连续 Back。
7. 测试 Scene 切换。
8. 测试当前 Run 是否仍能 Continue。
9. 测试返回 MainMenu 后 Run 不会被意外删除。
10. 报告修改文件。

不要未经验证直接进入下一阶段。

---

# 十七、优先级原则

遇到设计选择时按照以下顺序判断：

```text
导航是否清楚
>
玩家操作是否少
>
返回行为是否稳定
>
代码耦合是否低
>
扩展是否方便
>
视觉是否漂亮
```

如果一个方案视觉更酷，但需要玩家多点两次：

优先简单方案。

如果一个方案抽象程度更高，但当前只有两个页面使用：

优先低复杂度方案。

如果一个页面不知道返回哪里：

说明导航架构有问题，不要给它硬编码一个 MainMenu。

最终目标不是制作一个“复杂 UI Framework”。

最终目标是：

**玩家永远知道自己在哪、下一步该点什么、按返回会去哪。**