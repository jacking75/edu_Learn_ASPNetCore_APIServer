# Use the official MySQL image from Docker Hub
FROM mysql:8.4.2

COPY ./dump.sql /docker-entrypoint-initdb.d/

EXPOSE 3306
CMD ["mysqld", "--character-set-server=utf8", "--collation-server=utf8_general_ci"]
