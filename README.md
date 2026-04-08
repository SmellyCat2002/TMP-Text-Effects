# TMP Text Effects

Unity TextMeshPro 文字特效系统，类似 SNKRX 的标签式文本动画。

## 功能特性

- 🎨 **标签化语法** - `[wavy,rainbow]Hello World[/wavy]`
- 🌊 **内置特效** - 波浪、震动、脉冲、彩虹、弹跳、旋转
- 🎯 **高性能** - 直接操作 TMP 顶点，单 GameObject 实现
- 🔧 **易扩展** - 几行代码即可添加自定义特效

## 快速开始

### 1. 安装

将 `TextEffects` 文件夹复制到你的 Unity 项目 `Assets` 目录下。

### 2. 使用

1. 在 GameObject 上添加 **TextMeshPro - Text (UI)** 组件
2. 再添加 **TMPEffectText** 组件（会自动绑定 TMP）
3. 通过代码或 Inspector 设置文本

```csharp
var text = GetComponent<TMPEffectText>();
text.SetText("[wavy,rainbow,pulse]Hello World[/wavy]");
```

或在 Inspector 中直接编辑 `Raw Text` 字段：
```
[wavy,rainbow,pulse]Hello World[/wavy]
```

### 3. 内置标签

| 标签 | 效果 | 示例 |
|------|------|------|
| `wavy` | 波浪跳动 | `[wavy]Hello[/wavy]` |
| `wavy_mid` | 中等波浪 | `[wavy_mid]Hello[/wavy_mid]` |
| `shake` | 随机震动 | `[shake]Alert![/shake]` |
| `pulse` | 脉冲缩放 | `[pulse]Important[/pulse]` |
| `rainbow` | 彩虹色 | `[rainbow]Colorful[/rainbow]` |
| `bounce` | 弹跳 | `[bounce]Jump[/bounce]` |
| `spin` | 旋转 | `[spin]Rotate[/spin]` |

### 4. 组合使用

```csharp
// 多个标签同时作用
"[wavy,rainbow,pulse]Hello World[/wavy]"

// 颜色 + 特效
"[wavy,yellow]Gold[/wavy] - [pulse,red]HP: 100[/pulse]"
```

## 自定义特效

在 `TextEffectSystem.cs` 中添加：

```csharp
RegisterTag(new TextTag("myeffect", (c, dt, i, text) => {
    c.oy = Mathf.Sin(Time.time * 5f + i) * 10f;  // Y轴波浪
    c.scale = 1.2f;                              // 放大
    c.color = Color.red;                         // 变红
}));
```

使用：
```csharp
"[myeffect]Custom Effect[/myeffect]"
```

## 参数说明

`CharData` 包含以下可修改属性：

| 属性 | 类型 | 说明 | 默认值 |
|------|------|------|--------|
| `ox` | float | X轴偏移 | 0 |
| `oy` | float | Y轴偏移 | 0 |
| `scale` | float | 缩放比例 | 1 |
| `rotation` | float | 旋转角度（度） | 0 |
| `color` | Color | 颜色 | Color.white |

## 文件结构

```
TextEffects/
├── README.md                 # 本文件
├── README_TextEffect.md      # 详细技术文档
├── TMPEffectText.cs          # 主组件
├── TMPEffectTextDemo.cs      # 使用示例
├── TextEffectSystem.cs       # 特效标签系统
└── TextTag.cs                # 数据结构
```

## 技术细节

- 每帧重置 CharData → 应用标签 → 更新顶点
- 直接操作 TMP 的顶点数组，性能高效
- 支持多标签叠加（修改不同属性时）

详见 [README_TextEffect.md](./README_TextEffect.md)

## 依赖

- Unity 2019.4+
- TextMeshPro (com.unity.textmeshpro)

## 参考

灵感来自 [SNKRX](https://github.com/a327ex/SNKRX) 的文字特效系统。

SNKRX 是 MIT 许可证开源的游戏，本系统参考了其 `global_text_tags` 的设计思路，针对 Unity TextMeshPro 进行了重新实现。

## License

MIT License
