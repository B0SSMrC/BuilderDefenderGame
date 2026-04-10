# 🏰 Builder Defender Game (塔防建造者)

欢迎来到 **Builder Defender Game**！这是一款结合了资源管理与防御塔建造的 2D 塔防游戏。在这个充满挑战的世界里，你需要收集资源、规划防线，抵御敌人的猛烈进攻！

---
## 🚀 最新更新：v1.3.0 动态地图资源与开发工具版本

本次版本引入了全新的动态资源生成系统，极大地提升了游戏的可玩性和每一局的随机体验；同时为开发者打造了专属的关卡编辑器面板，让关卡设计与数值调优变得前所未有的直观与高效！

### 🎉 新增游戏机制 (New Gameplay Features)
* **动态资源生成**：废弃了原本固定的地图资源摆放，引入了动态的资源生成器，资源（矿物、树木等）现在会以配置好的“矿脉簇（Cluster）”为单位随机分布在地图内。
* **大本营安全区**：加入了基地避让半径机制，确保资源绝对不会刷新在玩家大本营附近，保障了玩家开局的城墙与建筑规划空间。
* **物理防重叠检测**：底层集成了 2D 物理碰撞检测机制，确保系统自动生成的资源绝对不会与地图上的已有障碍物（或其它资源）发生穿模和重叠。

### 🛠️ 开发者专属工具 (Developer Tools)
* **可视化关卡编辑器**：为本项目专门定制了 Unity 扩展面板，开发者可以通过顶部菜单栏 `Tools/塔防关卡编辑器` 快速呼出配置中心。
* **极速关卡调优**：可以直接在面板中对核心的关卡配置文件（`LevelConfigSO`）进行无缝编辑，包括修改关卡名称和调整开局准备时间。
* **波次便捷管理**：告别繁琐的底层数据操作，现在可以通过显眼的增删按钮直观地管理怪物波次，精确控制单波怪物数量、生成间隔以及波次间的休息间隔。
- <img width="323" height="575" alt="Image" src="https://github.com/user-attachments/assets/2bf50f9e-5dad-4f26-8c96-40cbb9959974" />

## 🚀 最新更新：v1.2.0

本次版本为游戏引入了更强的竞技感与更自由的节奏控制：

### 🎉 新增功能 (New Features)
- **🏆 玩家排行榜系统 (Leaderboard)**
  - 全新排行榜功能上线！现在你可以在游戏中查看自己的高分排名，与全球/全服玩家一较高下，证明谁才是真正的“塔防大师”！
  - ![Image](https://github.com/user-attachments/assets/117843eb-d8b4-4e4a-8cf3-530cc865a612)
- **⏩ 游戏时间流速加倍 (Time Speed-up)**
  - 嫌前期资源收集太慢？只需点击新增的“加速”按钮，即可开启时间流速加倍！在双倍速下考验你的极限反应与布局能力。
  - ![Image](https://github.com/user-attachments/assets/f462cec1-8ed3-44ad-932b-54d6b6ff3701)

### 💾 历史核心更新 (Previous Updates)
- **v1.1.0**: 实现了数据持久化系统（记录木材、石头、金矿及关卡进度等），并修复了主界面 UI 遮挡与层级问题。

---

## 🎮 核心玩法

1. **资源采集**：开采周围的矿产和树木，积累你的原始资本。
- ![Image](https://github.com/user-attachments/assets/3772264c-e5fc-4eec-b5be-585a18b8cdaa)
2. **阵地建设**：合理规划你的炮塔位置，打造铜墙铁壁。
- ![Image](https://github.com/user-attachments/assets/030c3cad-affb-4052-b4ed-035e1f1dc853)
3. **迎击强敌**：随着波数增加，敌人会越来越强，及时扩展你的防御塔并修复破损的防御塔！
- ![Image](https://github.com/user-attachments/assets/694a0a64-c25d-4aa2-b9e2-59bb85967558)

---

## 📦 如何试玩

如果你只想体验游戏，无需下载庞大的源码，请直接前往 **Releases** 页面下载最新安装包：

1. 点击右侧的 [Releases 标签](https://github.com/B0SSMrC/BuilderDefenderGame/releases)
2. 下载最新的 `BuilderDefenderGame_v1.2.0.rar` 压缩包。
3. 解压后双击 `.exe` 文件即可开始游戏！

---

## 💻 开发者指南

如果你想克隆本项目并在 Unity 中查看源码：

```bash
# 1. 克隆项目到本地
git clone [https://github.com/B0SSMrC/BuilderDefenderGame.git](https://github.com/B0SSMrC/BuilderDefenderGame.git)

# 2. 使用 Unity Hub 打开