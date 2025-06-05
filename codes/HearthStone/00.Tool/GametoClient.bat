@echo off
setlocal enabledelayedexpansion
echo ===== 모델 파일 복사 및 네임스페이스/특성 정리 =====
echo.

:: 경로 설정
set "SOURCE_DIR=..\GameServer\Models"
set "TARGET_DIR=..\HearthStoneClient\Models\Game"

if not exist "%SOURCE_DIR%" (
    echo [오류] 소스 디렉토리가 없습니다: %SOURCE_DIR%
    goto :error
)

if not exist "%TARGET_DIR%" (
    echo [안내] 대상 디렉토리를 생성합니다: %TARGET_DIR%
    mkdir "%TARGET_DIR%"
)

echo [안내] 파일 복사 중...
xcopy /s /y /i "%SOURCE_DIR%\*.cs" "%TARGET_DIR%\"
if errorlevel 1 (
    echo [오류] 파일 복사 중 오류가 발생했습니다.
    goto :error
)

echo [안내] 네임스페이스 및 특성 정리 중...
powershell -ExecutionPolicy Bypass -File modify_namespace.ps1

if errorlevel 1 (
    echo [오류] 파일 수정 중 오류가 발생했습니다.
    goto :error
)

echo.
echo [완료] 파일 복사 및 네임스페이스/특성 정리가 성공적으로 완료되었습니다.
goto :end

:error
echo.
echo [실패] 작업이 완료되지 않았습니다.

:end
pause
