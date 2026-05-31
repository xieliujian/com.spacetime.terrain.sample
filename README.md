# com.spacetime.terrain.sample

Unity Terrain 模型地形查看器示例项目，用于对比原始 Terrain 和导出的模型地形（LOD0/LOD1/LOD2）的显示效果。

---

## 已知问题与 TODO

### TODO: LOD0 显示效果过亮

**问题描述：**
LOD0 模型地形的显示效果比原始 Terrain 和 LOD1/LOD2 更亮，光照不一致。

**原因分析：**
LOD0 使用 Splatmap 材质，Shader 代码中的光照计算可能与 Unity Terrain 的光照模型不一致。

**解决方案：**
需要修改 LOD0 的 Shader 代码，调整光照计算逻辑，使其与原始 Terrain 的显示效果一致。

**相关文件：**
- LOD0 材质使用的 Shader（位于 `com.spacetime.terrain` 包的 Shaders 目录）
- 需要对比 Unity Terrain 的内置 Shader 光照计算

**优先级：** 中等

---

## 参考文档

- [com.spacetime.terrain 包文档](https://github.com/xieliujian/com.spacetime.terrain)

---

## 快速使用

1. Unity 菜单 `SpaceTime → Terrain Viewer Setup` → 点击 **Setup** 按钮
2. 运行场景，左上角按钮切换显示模式（原始地形 / LOD0 / LOD1 / LOD2）
3. 摄像机控制：`WASD` 移动，`QE` 升降，鼠标右键旋转，滚轮调速

