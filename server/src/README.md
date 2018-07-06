# CAT

## 常用函数命名说明
- Pack() interface{}
- Unpack(interface{})  
打包数据用于存放数据库，和把数据库的包解压


- PackMsg() *proro.Message  
打包数据用于网络传输


- recv***(Session, interface{})
- send***(ISender, interface{})  
***待每个协议消息去掉头两个字节剩下的部分，recv表示收到的，send发出的
每个CS消息对应一个send，SC消息对应recv


## 接口
### ISender
- Send(interface{})  
发送数据，通过Session或Role发送网络消息

### ISql
- Exec(*mgo.Session) (interface{}, error)  
执行数据库查询

### IService
- Init()
- Start()
- Stop()

    


