# 说明

本次作业完成的项目如下

- 编写一个组件，提供常用窗口服务

  - 修改 Game Jam Menu Template 的脚本
    - 如 ShowPanels 脚本

  - 具体要求是实现一个方法
    - 支持某个面板窗口独立显示
    - 支持某个面板窗口模态，其他面板窗口显示在后面
    - 支持几个窗口同时显示，仅一个窗口是活动窗口

----

在Asset Store中搜索Game Jam Menu Template，并导入该资源，本次作业基于该资源完成。

- **某个窗口独立显示**

Game Jam Menu Template资源中有直接支持options窗口的显示，如下：
![image](https://github.com/Azure0Sky/Unity3d-learning/blob/master/HW8/UI%20System/options.gif)
Options Button 的 OnClick 事件绑定了`ShowPanels`脚本中的`ShowOptionsPanel`方法：
![image](https://github.com/Azure0Sky/Unity3d-learning/blob/master/HW8/UI%20System/optionsButton.png)
`ShowOptionsPanel`方法定义如下：
```c#
public void ShowOptionsPanel()
{
    optionsPanel.SetActive( true );
    optionsTint.SetActive( true );
    menuPanel.SetActive( false );
    SetSelection( optionsPanel );
}
```

该方法激活 options 窗口与一个背景，使使用者的焦点限制在 options 窗口上，同时将菜单menu窗口隐藏，这样就能达到单独显示某个窗口的效果。  
而 options 窗口的 Back Button 的 OnClick 事件与`HideOptionsPanel`方法绑定，该方法定义如下：
```c#
public void HideOptionsPanel()
{
    menuPanel.SetActive( true );
    optionsPanel.SetActive( false );
    optionsTint.SetActive( false );
}
```
与`ShowOptionsPanel`方法所完成的操作正好相反，激活 menu 窗口，隐藏 options 窗口，这样就能关闭某一窗口的显示。

- **某个面板窗口模态，其他面板窗口显示在后面**

实现效果如下：
![image](https://github.com/Azure0Sky/Unity3d-learning/blob/master/HW8/UI%20System/modal.gif)
Show Modal 按钮的 OnClick 事件与`ShowModalPanel`方法绑定，该方法定义如下：
```c#
public void ShowModalPanel()
{
    modalPanel.transform.SetAsLastSibling();
    modalPanel.SetActive( true );
    modalTint.SetActive( true );

    // Disable the main menu panel
    CanvasGroup menuCanvasGroup = menuPanel.GetComponent<CanvasGroup>();
    menuCanvasGroup.interactable = false;
    menuCanvasGroup.blocksRaycasts = false;
}
```
`ShowModalPanel`方法激活 modal 窗口，将其设为 Canvas 下最后一个子对象，并将此时显示的其他窗口（这里只有 menu 窗口）的 Canvas Group 设为不可交互和不接受射线检测。这样 modal 窗口就为模态窗口，其他窗口显示在后面。

- **几个窗口同时显示，仅一个窗口是活动窗口**

实现效果如下：
![image](https://github.com/Azure0Sky/Unity3d-learning/blob/master/HW8/UI%20System/multi.gif)
Add Timer Button 可以设置一个新窗口（最多四个），达到多个窗口同时显示的目的，新窗口在显示3s后消失。同时新窗口设置为不可互动和不接受射线检测，只有 menu 窗口是活动的。  
*（按照个人理解应该大概是类似这样的效果）*