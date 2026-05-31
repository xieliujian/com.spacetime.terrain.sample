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

### TODO: WebGL LOD0 不显示

**问题描述：**
WebGL 打包后 LOD0 不显示，浏览器 Console 报错：
```
Shader SpaceTime/Scene/TerrainMesh/Splatmap
GLSL link error: FRAGMENT shader texture image units count exceeds MAX_TEXTURE_IMAGE_UNITS(16)
```

**原因分析：**
WebGL 最多支持 16 个纹理单元，Splatmap shader 支持最多 8 层地形，每层包含 Diffuse、Normal、Mask 共 3 张贴图，加上 SplatMap 贴图，总数超出 WebGL 限制。

**解决方案：**
需要在 WebGL 平台限制 Splatmap shader 的最大层数为 4 层（`_T2M_LAYER_COUNT_4`），或针对 WebGL 平台单独处理纹理数量限制。

**相关文件：**
- `com.spacetime.terrain/Shaders/TerrainMesh/TerrainSplatmap.shader`

**优先级：** 高

---

## 参考文档

- [com.spacetime.terrain 包文档](https://github.com/xieliujian/com.spacetime.terrain)

---

## 快速使用

1. Unity 菜单 `SpaceTime → Terrain Viewer Setup` → 点击 **Setup** 按钮
2. 运行场景，左上角按钮切换显示模式（原始地形 / LOD0 / LOD1 / LOD2）
3. 摄像机控制：`WASD` 移动，`QE` 升降，鼠标右键旋转，滚轮调速

