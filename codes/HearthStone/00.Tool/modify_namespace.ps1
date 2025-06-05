$targetDir = "..\HearthStoneClient\Models\Game"
$files = Get-ChildItem -Path $targetDir -Recurse -Filter *.cs

foreach ($file in $files) {
    Write-Host "처리 중: $($file.FullName)"
    $content = Get-Content $file.FullName

    # 불필요한 using, namespace, [FromHeader] 특성 제거
    $filteredContent = @()
    foreach ($line in $content) {
        if (
            -not ($line -match '^\s*using\s+GameServer\b') -and
            -not ($line -match '^\s*namespace\s+GameServer\.Models(\.Dto)?;\s*$') -and
            -not ($line -match '^\s*\[FromHeader\]\s*$')
        ) {
            $filteredContent += $line
        }
    }

    # using 구문의 마지막 위치 찾기
    $lastUsingIndex = -1
    for ($i = 0; $i -lt $filteredContent.Count; $i++) {
        if ($filteredContent[$i] -match '^\s*using\s+.*;\s*$') {
            $lastUsingIndex = $i
        }
    }

    # 새 네임스페이스 삽입
    if ($lastUsingIndex -ge 0) {
        $newContent = $filteredContent[0..$lastUsingIndex] + @('', 'namespace HearthStoneWeb.Models.Game;', '') + $filteredContent[($lastUsingIndex+1)..($filteredContent.Count-1)]
    } else {
        $newContent = @('namespace HearthStoneWeb.Models.Game;', '') + $filteredContent
    }

    Set-Content -Path $file.FullName -Value $newContent
}