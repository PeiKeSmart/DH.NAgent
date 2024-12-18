﻿using NewLife.Agent.Models;

namespace NewLife.Agent;

/// <summary>服务主机</summary>
public interface IHost
{
    /// <summary>名称</summary>
    String Name { get; }

    /// <summary>服务是否已安装</summary>
    /// <param name="serviceName">服务名</param>
    /// <returns></returns>
    Boolean IsInstalled(String serviceName);

    /// <summary>服务是否已启动</summary>
    /// <param name="serviceName">服务名</param>
    /// <returns></returns>
    Boolean IsRunning(String serviceName);

    /// <summary>安装服务</summary>
    /// <param name="service">服务</param>
    /// <returns></returns>
    Boolean Install(ServiceModel service);

    /// <summary>卸载服务</summary>
    /// <param name="serviceName">服务名</param>
    /// <returns></returns>
    Boolean Remove(String serviceName);

    /// <summary>启动服务</summary>
    /// <param name="serviceName">服务名</param>
    /// <returns></returns>
    Boolean Start(String serviceName);

    /// <summary>停止服务</summary>
    /// <param name="serviceName">服务名</param>
    /// <returns></returns>
    Boolean Stop(String serviceName);

    /// <summary>重启服务</summary>
    /// <param name="serviceName">服务名</param>
    Boolean Restart(String serviceName);

    /// <summary>开始执行服务</summary>
    /// <param name="service"></param>
    void Run(ServiceBase service);

    /// <summary>查询服务配置</summary>
    /// <param name="serviceName">服务名</param>
    ServiceConfig QueryConfig(String serviceName);
}