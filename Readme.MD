# DH.NAgent - 守护服务管理组件

# 功能特点
DH.NAgent是一个服务管理框架，用于开发随系统自动启动的长时间运行后台应用程序，支持Windows/Linux。  
在Windows上注册为Windows服务，在Linux上注册为Systemd守护进程。  
Agent支持`netstandard2.0/net45/net40/mono`，旧版本还支持net20（已不再维护）。  

Agent常用于各种后台应用，如aspnetcore应用、RPC网络服务器、MQTT服务器、数据处理应用、数据消费应用（Redis/RocketMQ）等。同类软件有 NSSM、srvany，但并不一样，Agent是框架，而它们是具体软件，更像星尘代理 StarAgent。  

DH.NAgent主要功能：  
1. 注册应用为系统服务或守护进程，随系统自动启动
2. 支持控制台菜单控制安装、卸载、启动、停止，以及查看状态
3. 支持控制台调试应用，解决Windows服务难以调试的问题
4. 支持健康检测，限制内存、线程数、句柄数，超限时重启应用服务
5. 支持应用服务定时重启，通过配置指定
6. 支持看门狗WatchDog，通过配置指定要守护的目标应用服务，如果目标停止则启动
7. 支持配置文件修改服务名，一个应用程序可在多个目录上部署为不同的系统服务

# 服务控制
一个服务代理示例跑起来的样子  
![image.png](https://cdn.nlark.com/yuque/0/2020/png/1144030/1585307461258-ca6e7880-b2ef-4d87-aed7-35bace08a149.png#align=left&display=inline&height=202&name=image.png&originHeight=404&originWidth=926&size=60071&status=done&style=none&width=463)  
![image.png](https://cdn.nlark.com/yuque/0/2020/png/1144030/1585547897654-f6b570c5-cbfd-4f4e-86c2-a9d711a53c58.png#align=left&display=inline&height=207&name=image.png&originHeight=414&originWidth=1129&size=60665&status=done&style=none&width=564.5)  
这是Agent的标准控制台（Windows和Centos）。上面是该服务的状态信息，下面是控制菜单。  
示例分析：  
- 服务名 XAgent/StarAgent，可以命令启动停止，Windows是`net start XAgent/net stop XAgent`，Linux是`systemctl start StarAgent/systemctl stop StarAgent`。
- 显示名“新生命服务代理”是在windows服务控制板里面看到的名字
- 下一段信息给出了DH.NAgent和当前应用的版本信息和编译时间
- 黄色菜单可通过按键选择相应操作，内置012345，可自定义其它按键操作
- 菜单1，显示状态，按下1后刷新状态信息
- 菜单2，安装服务或卸载服务，安装成功后，显示信息变为卸载服务，反之亦然
- 菜单3，启动服务或停止服务，安装后才可以看见
- 菜单4，重启服务，安装且运行后可以看见
- 菜单5，模拟运行，在当前进程启动应用主逻辑，用于业务逻辑调试，等同于Windows服务调用
- 菜单0，退出应用服务

**！！！注意，服务安装、卸载、启动、停止，在Windows/Linux上需要管理员权限运行**  

服务应用在Windows上以本地用户权限运行，有最高权限；  
服务应用在Linux上以root权限运行，有最高权限；  
该设计尽管带来了一定安全风险，但能够避免绝大部分初级用户的简单问题，优先易用性。  

---
## 多种守护体系实现
下面列出各平台对应的守护实现及其行为特征，便于按需选择或排障：

- WindowsService（Windows 服务）
  - 通过 Win32 服务控制管理器（SCM）注册和运行，内部使用 `StartServiceCtrlDispatcher`/`RegisterServiceCtrlHandlerEx` 等 API。
  - 支持电源事件、会话变更、时间变更等系统事件回调；安装时自动配置“失败重启”动作。
  - 安装/启停：需要管理员；对应命令 `sc`/`net start|stop`。适合服务器长期托管。

- WindowsAutorun（Windows 登录自启动）
  - 将应用写入注册表 `HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Run`，随登录自动启动。
  - 使用 `.pid` 文件跟踪进程并实现停止；更适合桌面/开发环境或需要交互桌面的场景。
  - 安装/卸载：写入 HKLM 需管理员；启停不依赖 SCM。

- Systemd（Linux systemd）
  - 生成 `<ServiceName>.service` 单元文件到常见目录（优先于 `/etc/systemd/system` 等），并执行 `daemon-reload`/`enable`。
  - 支持设置 `WorkingDirectory`、`ExecStart`、`User/Group` 等；启动/停止/重启通过 `systemctl` 执行。
  - 适用大多数现代 Linux 发行版，是 Linux 优先选择的托管方式。

- RcInit（Linux SysV init 兼容）
  - 生成 `ServiceName.sh` 控制脚本（当前应用目录），并在 `/etc/rc*.d/` 或 `/etc/rc.d/` 创建 `S50/K50` 级别的软链接。
  - 以 `sh ServiceName.sh start|stop|restart` 方式管理；用 `.pid` 文件跟踪进程。
  - 适合仍使用 SysV init 或 BusyBox 的传统/裁剪系统。

- Procd（OpenWrt 专用）
  - 运行时探测 PID 1 是否为 `procd` 判定环境；生成 `ServiceName.sh` 并在 `/etc/rc.d/` 创建 `S50` 链接。
  - 以 `nohup` 启动，`.pid` 文件跟踪，`kill` 停止；贴合 OpenWrt 生态的守护方式。

- OSXLaunch（macOS launchd）
  - 在 `~/Library/LaunchAgents/` 写入 `ServiceName.plist`，设定 `ProgramArguments` 与 `RunAtLoad`，通过 `launchctl` 管理。
  - 适合用户级代理（LaunchAgents）；如需系统级可按需调整到 LaunchDaemons。

- DefaultHost（默认主机）
  - 无平台集成能力，仅作为兜底执行 `Run/Stop` 循环；通常只有在无法检测到任何平台守护体系时才会落到此实现。

> WatchDog 在 `Systemd` 主机中守护其它服务时，会智能探测被守护服务实际托管方式（Systemd/SysV），避免“已安装但未检测到”的误判。
