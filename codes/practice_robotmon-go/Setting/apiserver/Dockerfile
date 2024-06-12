FROM rockylinux

RUN dnf update -y
RUN dnf install vim -y
RUN dnf install dotnet -y

EXPOSE 5000

COPY ./APIServer/bin/Release ./home