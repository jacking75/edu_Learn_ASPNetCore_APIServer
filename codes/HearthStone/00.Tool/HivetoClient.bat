@echo off
setlocal enabledelayedexpansion
echo ===== 모델 파일 복사 및 네임스페이스 수정 배치 파일 =====
echo.

:: 상위 폴더로 이동해서 작업
set SOURCE_DIR=..\HiveServer\Models
set TARGET_DIR=..\HearthStoneClient\Models\Hive

if not exist "%SOURCE_DIR%" (
    echo 소스 디렉토리가 없습니다: %SOURCE_DIR%
    echo 현재 경로: %CD%
    goto :error
)

if not exist "%TARGET_DIR%" (
    echo 대상 디렉토리를 생성합니다: %TARGET_DIR%
    mkdir "%TARGET_DIR%"
)

echo 소스: %SOURCE_DIR%
echo 대상: %TARGET_DIR%
echo.
echo 파일을 복사합니다...

:: 먼저 모든 파일 복사
xcopy /s /y /i "%SOURCE_DIR%\*.cs" "%TARGET_DIR%\"
if errorlevel 1 (
    echo 파일 복사 중 오류가 발생했습니다.
    goto :error
)
echo 복사 완료. 이제 네임스페이스 및 참조를 수정합니다...
:: 별도의 PowerShell 스크립트 파일 생성 및 실행
echo $files = Get-ChildItem -Path '%TARGET_DIR%' -Recurse -Filter *.cs > modify_namespace.ps1
echo foreach($file in $files) { >> modify_namespace.ps1
echo   Write-Host ('처리 중: ' + $file.Name) >> modify_namespace.ps1
echo   $content = Get-Content $file.FullName >> modify_namespace.ps1
echo   # 기존 HiveServer 네임스페이스 라인 제거 >> modify_namespace.ps1
echo   $filteredContent = $content ^| Where-Object { >> modify_namespace.ps1
echo     -not ($_ -match 'namespace\s+HiveServer\.Models(\.Dto)?;') -and >> modify_namespace.ps1
echo     -not ($_ -match 'using\s+HiveServer\.Models(\.Dto)?;') -and >> modify_namespace.ps1
echo     -not ($_ -match 'using\s+HiveServer\.Repository(\.Interface)?;') -and >> modify_namespace.ps1
echo	   -not ($_ -match 'namespace\s+HiveServer\.Models(\.DAO)?;') >> modify_namespace.ps1
echo   } >> modify_namespace.ps1
echo   # using 구문의 마지막 위치 찾기 >> modify_namespace.ps1
echo   $lastUsingIndex = -1 >> modify_namespace.ps1
echo   for ($i = 0; $i -lt $filteredContent.Count; $i++) { >> modify_namespace.ps1
echo     if ($filteredContent[$i] -match '^using\s+.*;\s*$') { >> modify_namespace.ps1
echo       $lastUsingIndex = $i >> modify_namespace.ps1
echo     } >> modify_namespace.ps1
echo   } >> modify_namespace.ps1
echo   # 새 네임스페이스 삽입 >> modify_namespace.ps1
echo   if ($lastUsingIndex -ge 0) { >> modify_namespace.ps1
echo     # using 구문 다음에 빈 줄과 함께 네임스페이스 추가 >> modify_namespace.ps1
echo     $newContent = $filteredContent[0..$lastUsingIndex] + @('', 'namespace HearthStoneWeb.Models.Hive;', '') + $filteredContent[($lastUsingIndex+1)..($filteredContent.Count-1)] >> modify_namespace.ps1
echo   } else { >> modify_namespace.ps1
echo     # using 구문이 없으면 파일 시작 부분에 네임스페이스 추가 >> modify_namespace.ps1
echo     $newContent = @('namespace HearthStoneWeb.Models.Hive;', '') + $filteredContent >> modify_namespace.ps1
echo   } >> modify_namespace.ps1
echo   Set-Content -Path $file.FullName -Value $newContent >> modify_namespace.ps1
echo } >> modify_namespace.ps1
powershell -ExecutionPolicy Bypass -File modify_namespace.ps1
del modify_namespace.ps1
if errorlevel 1 (
    echo 파일 수정 중 오류가 발생했습니다.
    goto :error
)
echo.
echo 파일이 성공적으로 복사되었고 네임스페이스 및 참조가 수정되었습니다.
echo   - 기존 'namespace HiveServer.Models;' 또는 'namespace HiveServer.Models.Dto;' 제거
echo   - 'using HiveServer.Models;' 및 'using HiveServer.Repository.Interface;' 참조 제거
echo   - 'using' 구문 이후에 'namespace HearthStoneWeb.Models.Hive;' 추가
echo.
goto :end
:error
echo.
echo 오류가 발생했습니다. 작업이 완료되지 않았습니다.
:end
pause