# docker 테스트 환경
![](./images/fluentd_test.PNG) 

# docker 설치, 켜기
sudo wget -qO- http://get.docker.com/ | sh  
sudo systemctl start docker  

# build된 container 다운 받기
sudo docker pull richard0326/robotmon-go-apiserver  
sudo docker pull richard0326/fluentd  

# 사전 작업
1. OS 사전 작업  
log가 있는 경로에 접근 권한이 있도록 허용해준다.  
sudo chmod -R  00777 "richard0326"  
  
2. mysql 사전 작업  
mysql의 dump 파일을 미리 세팅해둔다.  
인게임 정보에 대한 덤프 파일  
(https://github.com/richard0326/RobotMon-Go/blob/main/Setting/mysql/DumpInGame_my5_6_36.sql)  
로그에 대한 덤프 파일  
(https://github.com/richard0326/RobotMon-Go/blob/main/Setting/mysql/DumpLog_my5_6_36.sql)  
  
3. fluentd 사전 작업  
fluentd conf 파일을 미리 volume될 경로에 세팅해둔다.  
apiserver 컨테이너의 fluentd.conf 파일  
(https://github.com/richard0326/RobotMon-Go/tree/main/Setting/fluentdSettings/fluent.conf)  
forward 기능만 있는 fluentd 컨테이너의 fluentd.conf 파일  
(https://github.com/richard0326/RobotMon-Go/blob/main/Setting/fluentdSettings/fluentForward.conf)  

# mysql 실행하기
sudo docker run --name mysql-container -e MYSQL_ROOT_PASSWORD=root1234 -d -p 3306:3306 -v /home/richard0326:/var/lib/mysql mysql:5.6.36    
sudo docker exec -it mysql-container bash   
mysql -u root -p  
root1234  

# redis 실행하기
sudo docker run -p 6379:6379 --name myredis -d redis  

# foward fluentd 실행하기  
sudo docker run -u root -p 24224:24224 -v /home/richard0326/fluentForward.conf:/fluentd/etc/fluent.conf --name fluentd_forward richard0326/fluentd  
  
# apiserver 실행하기
sudo docker run -u root -v /home/richard0326/fluent.conf:/fluentd/etc/fluent.conf -v /home/richard0326:/fluentd/logs/ --name fluentd richard0326/fluentd  
  
fluentd container는 돌아가게 냅두고, 서버에 다시 새로 접속한다.    
sudo docker run -d --privileged -p 5000:5000 -v /home/richard0326:/home/fluentd --name apiserver richard0326/robotmon-go-apiserver /sbin/init  
sudo docker exec -it apiserver /bin/bash  
cd home/net6.0  
vi MyConfig.json    
아래와 같이 변경해준다.  
{  
  "Environment": "Production",  
  "urls": "http://*:5000",  
  "logdir": "/home/fluentd/"  
}   
dotnet ApiServer.dll   

# 테스트  
테스트는 메인 화면의 README.md를 참고하여 진행하면 좋을 것 같다.  
(https://github.com/richard0326/RobotMon-Go#readme)  
