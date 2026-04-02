# 🏰 Builder Defender Game (塔防建造者)
欢迎来到 **Builder Defender Game**！这是一款结合了资源管理与防御塔建造的 2D 塔防游戏。在这个充满挑战的世界里，你需要收集资源、规划防线，抵御敌人的猛烈进攻！

---

## 🚀 最新更新：v1.1.0 里程碑版本

本次版本重点解决了玩家反馈的进度丢失问题，并对整体视觉体验进行了打磨：

### 🎉 新增功能 (New Features)
- **💾 数据持久化系统 (Save & Load)**
  - 千呼万唤的存档功能上线！游戏现在会自动记录你的木材、石头、金矿、防御塔等级和关卡进度。
  - 核心逻辑基于本地 JSON 序列化实现，退出游戏再进，心血不再白费！

### 🛠️ 优化与修复 (Tweaks & Bug Fixes)
- **UI 显示优化**
  - 修复了主界面部分 UI 元素在不同分辨率下被遮挡的问题。
  - 调整了防御塔升级面板的层级 (Sorting Layer)，不再会被高大的建筑物挡住。
  - 优化了主菜单的字体渲染清晰度。

---

## 🎮 核心玩法

1. **资源采集**：开采周围的矿产和树木，积累你的原始资本。
2. **阵地建设**：合理规划你的炮塔位置，打造铜墙铁壁。
3. **迎击强敌**：随着波数增加，敌人会越来越强，及时扩展你的防御塔并修复破损的防御塔！

---

## 📦 如何试玩

如果你只想体验游戏，无需下载庞大的源码，请直接前往 **Releases** 页面下载最新安装包：

1. 点击右侧的 [Releases 标签](https://github.com/B0SSMrC/BuilderDefenderGame/releases)
2. 下载最新的 `BuilderDefenderGame_v1.1.0.rar` 压缩包。
3. 解压后双击 `.exe` 文件即可开始游戏！

---

## 💻 开发者指南

如果你想克隆本项目并在 Unity 中查看源码：

```bash
# 1. 克隆项目到本地
git clone [https://github.com/B0SSMrC/BuilderDefenderGame.git](https://github.com/B0SSMrC/BuilderDefenderGame.git)

# 2. 使用 Unity Hub 打开