# Fluentd

https://github.com/thatslifebro/MiniGameHeavenAPIServer 이 프로젝트를 기반으로 한다.

## fluentd.conf

1. json structured 로그 파일을 읽어온다.
1. Error 로그와 Info 로그로 분리한다.
1. message 필드의 내용에서 api 필드를 생성한다.
1. Error 로그를 mysql에 저장한다.
1. Info 로그에서 api가 Login, MiniGameSave, 둘다 아닌 경우 3가지로 태그를 붙인다.
1. 태그에 따라 로그를 분리하여 mysql에 저장한다.

---
```conf
<source>
	@type tail
	path /App/log/*
	pos_file /tmp/fluent/temp.pos
	tag docker.log
	<parse>
		@type json
	</parse>
</source>
```
=> /App/log/ 경로에 있는 모든 파일을 읽어와 docker.log 태그를 붙여준다.
```conf
<match docker.log>
	@type rewrite_tag_filter
	<rule>
		key LogLevel
		pattern Error
		tag error.${tag}
	</rule>
	<rule>
		key LogLevel
		pattern Information
		tag info.${tag}
	</rule>
</match>
```
=> docker.log 태그를 가진 로그를 읽어와 LogLevel이 Error인 경우 error.docker.log 태그를 붙여주고, Information인 경우 info.docker.log 태그를 붙여준다.
```conf
<filter info.docker.log>
	@type record_transformer
	enable_ruby
	<record>
		api ${record["Message"].split(']')[0].delete('[')}
	</record>
</filter>
```
=> info.docker.log 태그를 가진 로그를 읽어와 Message 필드를 기준으로 api 필드를 생성해준다.
```conf
<match error.docker.log>
	@type sql
	host db
	port 3306
	database log_db
	adapter mysql2
	username root
	password Rlatjd095980!
	<table>
		table error_log
		column_mapping 'LogLevel:level, Message:msg, Timestamp:timestamp'
	</table>
</match>
```
=> error.docker.log 태그를 가진 로그를 log_db 데이터베이스에 error_log 테이블에 저장한다.
```conf
<match info.docker.log>
	@type rewrite_tag_filter
	<rule>
		key api
		pattern Login
		tag login.${tag}
	</rule>
	<rule>
		key api
		pattern MiniGameSave
		tag game.${tag}
	</rule>
	<rule>
		key api
		pattern MiniGameLoad|Login
		invert true
		tag api.${tag}
	</rule>
</match>
```
=> info.docker.log 태그를 가진 로그를 api가 Login, MiniGameSave, 둘 다 아닌 겨우 3가지를 나누어 태그를 붙여준다.
```conf
<match api.**>
	@type sql
	host db
	port 3306
	database log_db
	adapter mysql2
	username root
	password Rlatjd095980!
	<table>
		table info_api_log
		column_mapping 'LogLevel:level, Timestamp:timestamp, api:api, uid:uid, result:result'
	</table>
</match>
```
=> api.** 태그를 가진 로그를 log_db 데이터베이스에 info_api_log 테이블에 저장한다.

LogLevel, Timestamp, uid, result는 json_structred 로그에 포함되어 있기 때문에 바로 사용할 수 있다.

서버에서 로그를 json 형태로 저장해야 가능하다.
```conf
<match login.**>
	@type copy

	<store>
		@type sql
		host db
		port 3306
		database log_db
		adapter mysql2
		username root
		password Rlatjd095980!
		<table>
			table info_login_log
			column_mapping 'LogLevel:level, Timestamp:timestamp, api:api, uid:uid, result:result, playerId:player_id'
		</table>
	</store>

	<store>
		@type sql
		host db
		port 3306
		database log_db
		adapter mysql2
		username root
		password Rlatjd095980!
		<table>
			table info_api_log
			column_mapping 'LogLevel:level, Timestamp:timestamp, api:api, uid:uid, result:result'
		</table>
	</store>

</match>
```
=> login.** 태그를 가진 로그를 info_login_log 테이블과 info_api_log 테이블에 저장한다.
```conf
``` conf
<match game.**>
	@type copy

	<store>
		@type sql
		host db
		port 3306
		database log_db
		adapter mysql2
		username root
		password Rlatjd095980!
		<table>
			table info_game_log
			column_mapping 'LogLevel:level, Timestamp:timestamp, api:api, uid:uid, result:result, gameKey:game_key, score:score'
		</table>
	</store>

	<store>
		@type sql
		host db
		port 3306
		database log_db
		adapter mysql2
		username root
		password Rlatjd095980!
		<table>
			table info_api_log
			column_mapping 'LogLevel:level, Timestamp:timestamp, api:api, uid:uid, result:result'
		</table>
	</store>

</match>
```
=> game.** 태그를 가진 로그를 info_game_log 테이블과 info_api_log 테이블에 저장한다.

---
위의 파일을 도커 컨테이너에서 활용하기 위해서는 Dockerfile을 통해 fluentd 뿐만 아니라 다른 플러그인과 어댑터를 설치해야한다.

Dockerfile과 docker-compose.yml이 프로젝트에 포함되어 있다.

## docker-compose.yml

```yml
---
version: "1.0"

services:
  db:
    image: mydbimage
    container_name: mydb
    build:
      context: ./DB
      dockerfile: Dockerfile
    restart:
      always
    ports:
      - "3306:3306"

  hive:
    image : hiveimage
    container_name: hive
    build:
      context: ./FakeHiveServer/aspnetapp
      dockerfile: Dockerfile
    restart:
      always
    ports:
      - "11501:11501"
    depends_on:
      - db
      - redis

  server:
    image : serverimage
    container_name: server
    build:
      context: ./APIServer/aspnetapp
      dockerfile: Dockerfile
    restart:
      always
    ports:
      - "11500:11500"
    depends_on:
      - db
    volumes:
      - log-volume:/App/log
  
  redis:
    image: redis
    container_name: redis
    restart:
      always
    ports: 
      - "6379:6379"

  fluentd:
    image: fluentd
    container_name: fluentd
    build:
      context: ./fluentd
      dockerfile: Dockerfile
    ports:
      - "24224:24224"
    volumes:
      - log-volume:/App/log

volumes:
  log-volume:    
```

=> https://github.com/thatslifebro/Docker 에서 fluentd를 추가하였다.

volume을 통해 fluentd 컨테이너와 server 컨테이너가 /App/log/ 경로를 공유하도록 하였다.