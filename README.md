# YP.TimedJob

YP.TimedJob是一个轻量级的.NET定时任务框架，支持ASP.NET Core、WinForm、控制台等多种.NET项目类型。

## 功能特点

- 轻量级设计，易于集成
- 声明式配置，通过特性标记即可完成任务配置
- 灵活的时间配置，支持设置任务的开始时间和执行间隔
- 执行控制，可配置任务是否等待上一个执行完成
- 同步/异步支持，支持同步和异步定时任务
- 错误处理，内置错误处理和日志记录
- 依赖注入，支持依赖注入和自定义日志实现
- 跨平台支持，可在Windows、macOS、Linux上运行

## 使用方法

### 1. ASP.NET Core 项目

在`Program.cs`文件中注册定时任务服务：

```csharp
using YP.TimedJob.Extensions;

var builder = WebApplication.CreateBuilder(args);

// 添加定时任务服务
builder.Services.AddTimedJob();

var app = builder.Build();

// 使用定时任务中间件
app.UseTimedJob();

app.Run();
```

### 2. WinForm 项目

#### 2.1 安装依赖

在WinForm项目中安装以下包：
- Microsoft.Extensions.DependencyInjection

#### 2.2 注册服务和启动定时任务

在Form的构造函数中初始化依赖注入并获取JobScheduler实例：

```csharp
using Microsoft.Extensions.DependencyInjection;
using YP.TimedJob.Core;
using YP.TimedJob.Extensions;
using System.Reflection;

public partial class Form1 : Form
{
    private readonly JobScheduler _jobScheduler;

    public Form1()
    {
        InitializeComponent();

        // 创建依赖注入容器
        var services = new ServiceCollection();
        services.AddTimedJob();
        var serviceProvider = services.BuildServiceProvider();

        // 获取JobScheduler实例
        _jobScheduler = serviceProvider.GetRequiredService<JobScheduler>();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        // 启动定时任务
        _jobScheduler.RegisterJobs(Assembly.GetExecutingAssembly());
        _jobScheduler.Start();
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        // 停止定时任务
        _jobScheduler.Stop();
    }
}
```

### 3. 控制台项目

```csharp
using Microsoft.Extensions.DependencyInjection;
using YP.TimedJob.Core;
using YP.TimedJob.Extensions;
using System.Reflection;

class Program
{
    static void Main(string[] args)
    {
        // 创建依赖注入容器
        var services = new ServiceCollection();
        services.AddTimedJob();
        var serviceProvider = services.BuildServiceProvider();

        // 获取JobScheduler实例
        var jobScheduler = serviceProvider.GetRequiredService<JobScheduler>();

        // 注册和启动定时任务
        jobScheduler.RegisterJobs(Assembly.GetExecutingAssembly());
        jobScheduler.Start();

        Console.WriteLine("定时任务已启动，按任意键退出...");
        Console.ReadKey();

        // 停止定时任务
        jobScheduler.Stop();
    }
}
```

### 4. 创建定时任务

创建一个继承自`Job`的类，并使用`[Invoke]`特性标记要执行的方法：

```csharp
using YP.TimedJob.Core;

public class TestJob : Job
{
    [Invoke(Begin = "2024-01-01 00:00:00", Interval = 5000, SkipWhileExecuting = true)]
    public void RunEvery5Seconds()
    {
        Console.WriteLine($"Every 5 seconds job executed at: {DateTime.Now}");
    }

    [Invoke(Begin = "2024-01-01 00:00:00", Interval = 10000, SkipWhileExecuting = false)]
    public async Task RunEvery10SecondsAsync()
    {
        await Task.Delay(2000);
        Console.WriteLine($"Every 10 seconds async job executed at: {DateTime.Now}");
    }
}
```

### 5. 特性参数说明

`[Invoke]`特性支持以下参数：

| 参数 | 类型 | 说明 | 默认值 |
|------|------|------|--------|
| Begin | string | 任务开始执行的时间，格式为 "yyyy-MM-dd HH:mm:ss" | 当前时间 |
| Interval | int | 任务执行的时间间隔，单位为毫秒 | 1000 |
| SkipWhileExecuting | bool | 是否等待上一个任务执行完成，true 为等待 | true |

## 核心API

- `Job` - 定时任务基类
- `InvokeAttribute` - 任务配置特性
- `JobScheduler` - 任务调度器
- `IJobLogger` - 日志接口
- `ConsoleJobLogger` - 默认日志实现

## 项目结构

```
YP.TimedJob/
├── Core/
│   ├── Job.cs              - 定时任务基类
│   ├── InvokeAttribute.cs  - 任务配置特性
│   ├── JobScheduler.cs     - 任务调度器
│   ├── IJobLogger.cs        - 日志接口
│   └── ConsoleJobLogger.cs - 默认日志实现
├── Extensions/
│   ├── ServiceCollectionExtensions.cs  - 服务注册扩展
│   └── ApplicationBuilderExtensions.cs - 中间件扩展
└── Jobs/
    └── TestJob.cs          - 测试定时任务
```