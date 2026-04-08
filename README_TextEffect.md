# 文字特效系统笔记

## Tag 委托的四个参数

```csharp
Action<CharData, float, int, string> update
```

| 参数 | 类型 | 作用 | 性质 |
|------|------|------|------|
| `c` | CharData | 字符数据（ox, oy, scale, rotation, color） | **核心数据结构** - 被修改的目标 |
| `dt` | float | 时间增量（Time.deltaTime） | 辅助变量 - 用于基于时间的计算 |
| `i` | int | 字符在文本中的索引 | 辅助变量 - 用于让相邻字符有差异 |
| `text` | string | 完整文本内容 | 辅助变量 - 用于整行效果（如中心扩散） |

### 核心理解

**CharData 是唯一的数据结构**，其他三个参数只是**辅助计算的工具**：

```csharp
// 例子：wavy 标签
(c, dt, i, text) => {
    // c 是要修改的目标
    c.oy = 2f * Mathf.Sin(4f * Time.time + i);  // 用 i 辅助计算，修改 c
}
```

- `c` → **被修改的对象**（输出目标）
- `i` → **辅助计算**（输入参数，让波浪有相位差）
- `dt`, `text` → **当前未使用**（预留的辅助参数）

### 使用频率

| 参数 | 当前标签使用情况 |
|------|----------------|
| `c` | **100%** - 所有标签都修改它 |
| `i` | **~70%** - wavy, pulse, rainbow, spin 等用它做相位差 |
| `dt` | **0%** - 预留，当前未使用 |
| `text` | **0%** - 预留，当前未使用 |

### 为什么需要辅助参数？

**`i`（索引）的作用**：
```csharp
// 没有 i：所有字符同步运动
"Hello" 的 H e l l o 一起上下

// 有 i：相邻字符有相位差，形成波浪
"Hello" 的 H↓ e↑ l↓ l↑ o↓ 逐个波动
```

**`dt` 的潜在作用**：
- 配合累加模式做平滑移动
- 当前设计是每帧重置，所以用不上

**`text` 的潜在作用**：
- 计算字符在整行中的位置（如中心扩散效果）
- 根据文本长度调整效果强度

---

## 其他笔记

### 重置机制
- 每帧重置 CharData 为基准值（ox=0, oy=0, scale=1, rotation=0）
- 目的：防止 `+=` 累加标签导致数值漂移
- 当前所有标签都是 `=` 直接赋值，重置看似多余但提供保险

### 标签冲突
- 修改**不同属性**的标签可以叠加（如 [wavy, pulse]）
- 修改**相同属性**的标签会覆盖（如 [wavy, bounce] 都改 oy）

---

## TMPEffectText.cs 代码分析

### 核心流程（开发者需要关心的）

```csharp
// UpdateMesh() - 主循环
for (int i = 0; i < charCount; i++)
{
    ResetCharData(charDataList[i]);      // ← 1. 重置 CharData
    ApplyTags(charDataList[i], i);        // ← 2. 标签修改 CharData（核心！）
    ApplyToVertices(i, textInfo);         // ← 3. CharData 应用到顶点
}
```

### CharData 数据流向

```
┌─────────────────────────────────────────────────────────┐
│  第1步：ResetCharData()                                  │
│  重置 CharData 的 4 个字段为基准值                        │
│  data.ox = 0; data.oy = 0;                               │
│  data.scale = 1f; data.rotation = 0f;                    │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│  第2步：ApplyTags() ← 【开发者关心的核心】                │
│  标签通过委托修改 CharData                                │
│  tag.update.Invoke(data, dt, index, text);               │
│                                                         │
│  例如 wavy 标签：                                        │
│  (c, dt, i, text) => c.oy = 2f * Sin(4f * time + i);     │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│  第3步：ApplyToVertices() + TransformRelativeToCenter()  │
│  将 CharData 应用到 TMP 的 4 个顶点                       │
│                                                         │
│  实际使用 CharData 的地方：                               │
│  • pos *= data.scale;           ← 缩放                  │
│  • Rotate2D(pos, data.rotation) ← 旋转                  │
│  • pos.x += data.ox;            ← X偏移                 │
│  • pos.y += data.oy;            ← Y偏移                 │
│  • colors[j] = data.color;      ← 颜色                  │
└─────────────────────────────────────────────────────────┘
```

### 代码相关性分类

#### ✅ 直接相关（使用 CharData）

| 代码位置 | 使用方式 | 说明 |
|---------|---------|------|
| `ResetCharData` | `data.ox = 0` 等 | 重置 4 个字段 |
| `ApplyTags` | `tag.update.Invoke(data, ...)` | 传给标签修改 |
| `TransformRelativeToCenter` | `pos *= data.scale` | 应用缩放 |
| `TransformRelativeToCenter` | `Rotate2D(pos, data.rotation)` | 应用旋转 |
| `TransformRelativeToCenter` | `pos.x += data.ox` | 应用 X 偏移 |
| `TransformRelativeToCenter` | `pos.y += data.oy` | 应用 Y 偏移 |
| `ApplyToVertices` | `colors[j] = data.color` | 应用颜色 |

**总计：约 10 行核心代码**

#### ❌ 无关代码（底层 TMP/数学操作）

| 代码位置 | 内容 | 说明 |
|---------|------|------|
| `ApplyToVertices` | `int matIndex = ...` | TMP 材质索引 |
| `ApplyToVertices` | `int vertIndex = ...` | 顶点索引 |
| `ApplyToVertices` | `Vector3[] verts = ...` | 获取顶点数组 |
| `ApplyToVertices` | `Vector3 center = ...` | 计算中心点 |
| `ApplyToVertices` | `for (int j = 0; j < 4; j++)` | 遍历 4 个顶点 |
| `Rotate2D` | `float cos = Cos(rad)` | 三角函数计算 |
| `Rotate2D` | 矩阵乘法 | 旋转公式 |

**总计：约 50+ 行底层代码**

### 结论

- **核心逻辑**：CharData 的 5 个字段（ox, oy, scale, rotation, color）
- **实际使用**：约 10 行代码直接操作 CharData
- **底层封装**：约 50 行 TMP 顶点操作（无需关心）
- **开发者只需关注**：`TextEffectSystem` 中标签如何修改 CharData
