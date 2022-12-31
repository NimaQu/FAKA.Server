# FAKA
### Made with ❤️ by NimaQu & [ChatGPT](https://openai.com/blog/chatgpt/)
## WIP
ASP.NET Core 学习项目，用来重建[先辈杂货铺](https://shop.114514.cloud), 之前的是用的 whmcs + 插件，重+闭源+屎山

纯 WEB API 后端项目，前端 [sun00108](https://github.com/sun00108) 会拿 React 帮我写，如果你们有兴趣的话我可能会开一个 org 之类的放上单独前端的仓库，API reference 后面等快完成了会完善 swagger

为什么用 ASP.NET Core 重建呢？因为我没用静态语言写后端，而且我想尝试一下 Microsoft 的生态，ASP.NET 又是企业级的，性能也不错，而且脚本胶水语言写多了我想换换口味

为什么不用 Rails? 最近写的太多了换换口味，而且我不喜欢 Ruby, Rails 的生态也不及 .NET

为什么不用 Laravel? php 那语法我还不如写 C#，性能还不如 ASP.NET Core

至于 Django，虽然我喜欢用 Python，但是这个框架还是不如 rails 和 ASP.NET 成熟，很多功能都得自己写，而且我不喜欢 Django 的 ORM
## 使用的组件

用户身份和权限管理使用的是 ASP.NET Identity, 未来可能会增加更多的身份验证方式，例如 Azure AD 和 OAuth2，以及 MFA (10 刀买的 yubikey 总得用上)

ORM 使用的是 Entity Framework Core，数据库开发中暂时使用的是 SQLServer LocalDB，鉴于这个框架的特性，我觉得应该可以很方便的迁移到其他数据库，最后生产环境应该会用 postgresql
## TODO
- [x] 用户系统
- [x] 订单系统
- [x] 商品系统
- [ ] 购买流程
- [ ] 支付接口 （还没想好怎么设计能让第三方能快速集成，应该会接 stripe 和支付宝官方 sdk）
- [ ] recaptcha 等验证码集成
- [ ] 邮件系统
- [ ] Rate Limit 等安全相关

## Special Thanks
- [ChatGPT](https://openai.com/blog/chatgpt/) 帮我 debug, 帮我实现功能，帮我设计数据库，你们都应该试试这个