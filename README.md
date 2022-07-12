<div id="top"></div>
<!--
*** Thanks for checking out the Best-README-Template. If you have a suggestion
*** that would make this better, please fork the repo and create a pull request
*** or simply open an issue with the tag "enhancement".
*** Don't forget to give the project a star!
*** Thanks again! Now go create something AMAZING! :D
-->



<!-- PROJECT SHIELDS -->
<!--
*** I'm using markdown "reference style" links for readability.
*** Reference links are enclosed in brackets [ ] instead of parentheses ( ).
*** See the bottom of this document for the declaration of the reference variables
*** for contributors-url, forks-url, etc. This is an optional, concise syntax you may use.
*** https://www.markdownguide.org/basic-syntax/#reference-style-links
-->




<!-- ABOUT THE PROJECT -->
## 简单概述

* 本库主要解决了在  线性工作流下，如何保持UI 相机还能够保持在 Gamma 颜色空间下工作
* UI 相机的渲染不应该受到 任何 postprocess 和 finalProcess( 例如 fxaa 和 fsr) 的影响
* 充分利用URP内置RenderTargetSwapBuff ,保持了原本代码一致性

<p align="right">(<a href="#top">back to top</a>)</p>




## 使用
UniversalRenderer.cs 有2个功能开关
public static bool sUISplitEnable = true; // 分辨率分离 ,场景相机可自由调节分辨率，ui相机为屏幕大小
public static bool sIsGammaCorrectEnable = true; // ui gamma校对

1 阅读本库对URP12的修改，最好用对比工具和源码进行对比，例如：Beyond Compare 
2 修改你的URP12 
3 讲本库shader/URP-UI-Default.shader 拖到  Project Setting/Graphic/BuildinShader 里面，#替换#掉原来的 UI/Default shader

<p align="right">(<a href="#top">back to top</a>)</p>



<!-- ROADMAP -->
## 路线图

- [ ]  gamma ui 在scen view 下的 和 game view 下的色差



<p align="right">(<a href="#top">back to top</a>)</p>





<!-- ACKNOWLEDGMENTS -->
## 参考
* [Choose an Open Source License](https://choosealicense.com)
* [GitHub Emoji Cheat Sheet](https://www.webpagefx.com/tools/emoji-cheat-sheet)
* [Malven's Flexbox Cheatsheet](https://flexbox.malven.co/)
* [Malven's Grid Cheatsheet](https://grid.malven.co/)
* [Img Shields](https://shields.io)
* [GitHub Pages](https://pages.github.com)
* [Font Awesome](https://fontawesome.com)
* [React Icons](https://react-icons.github.io/react-icons/search)

<p align="right">(<a href="#top">back to top</a>)</p>


