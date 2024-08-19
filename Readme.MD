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