# Project Fluentd Study

## Overview

이 프로젝트의 목적은 게임 서버의 운영 상태와 사용자 활동을 실시간으로 모니터링하고 이를 기반으로 통계가 가능한 로그 데이터를 생성 및 저장 하는것입니다. ZLogger와 Fluentd의 사용을 통해 로그 데이터를 효율적으로 수집하고 최소한의 프로세싱으로 데이터베이스에 저장하는것이 핵심입니다.

## Requirements

이전에 진행된 오목게임 API Server에서 발생하는 로그 데이터를 Fluentd를 통해 가공하고 데이터 베이스로 저장합니다.

설계되는 로그 데이터는 아래의 통계가 가능해야 합니다.

- Daily Active Users
- User-Specific Statistics
  - 로그인 횟수
  - 게임 플레이 횟수
- Time-Based Statistics
  - 기간 내 로그인 인구
  - 기간 내 매칭 요청 수
  - 기간 내 매칭 성사 수
  - 기간 내 게임 플레이 수

## 목차

- [프로젝트 로깅 시스템](/GameSolution/README.md)
- [Docker로 프로젝트 구성](/GameSolution/Docker.md)
- [서버 로그 출력 과정](/GameSolution/GameServer/README.md)
- [Fluentd로 로그 가공 및 저장](/GameSolution/FluentD/README.md)
- [로그 데이터 모델 설계](/GameSolution/Database/LogDb.md)
