# 项目规则 (CLAUDE.md)

## 项目结构说明

本项目 (`com.spacetime.terrain.sample`) 是一个 Unity 示例工程，通过 Git Submodule 引用了以下两个子模块：

| 子模块 | 本项目路径 | 源码仓库路径 |
|---|---|---|
| `com.spacetime.core` | `Packages/com.spacetime.core` | `D:\xieliujian\com.spacetime.core\Packages\com.spacetime.core` |
| `com.spacetime.terrain` | `Packages/com.spacetime.terrain` | `D:\xieliujian\com.spacetime.core\Packages\com.spacetime.terrain` |

---

## 强制规则：子模块禁止在本项目中修改和提交

**严禁**在本项目目录下对以下子模块路径进行任何修改或 Git 提交操作：

- `Packages/com.spacetime.core`
- `Packages/com.spacetime.terrain`

这两个目录是只读引用，任何对其内容的改动都不应在本仓库中提交。

---

## 子模块修改规范

如需修改 `com.spacetime.core` 或 `com.spacetime.terrain` 的代码，**必须**在以下路径进行修改和提交：

```
D:\xieliujian\com.spacetime.core\Packages\
```

具体包目录：
- `D:\xieliujian\com.spacetime.core\Packages\com.spacetime.core`   — 修改 core 包
- `D:\xieliujian\com.spacetime.core\Packages\com.spacetime.terrain` — 修改 terrain 包

在上述路径完成修改并推送到远端后，再回到本项目更新子模块引用（`git submodule update`），然后仅提交子模块指针变更。

---

## 提交检查清单

- [ ] 未在 `Packages/com.spacetime.core` 下直接修改任何文件
- [ ] 未在 `Packages/com.spacetime.terrain` 下直接修改任何文件
- [ ] 所有包代码修改均在 `D:\xieliujian\com.spacetime.core\Packages\` 完成并提交
