1.EXCELAPPWeb 为后端工程 数据库访问 采用了dapper+npoco 

前端页面 在asserts中 
2.ExcelAPP 为winform工程 主要采用了cefsharp内嵌浏览器  其中与客户端交互的功能都在 JSBridge/ JSMainObject .cs中 （excel数据处理，本地文件处理等本地交互）