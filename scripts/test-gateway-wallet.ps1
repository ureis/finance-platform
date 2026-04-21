$ErrorActionPreference = 'Stop'

Write-Host '=== 1) POST /api/login ==='
$login = Invoke-RestMethod -Uri 'http://localhost:8080/api/login' -Method POST -ContentType 'application/json' -Body '{}'
if (-not $login.token) { throw 'Sem token' }
$token = $login.token
Write-Host "Token OK (len=$($token.Length))"

$hdr = @{ Authorization = "Bearer $token" }

Write-Host '=== 2) GET /api/wallet/assets ==='
$assets = Invoke-RestMethod -Uri 'http://localhost:8080/api/wallet/assets' -Headers $hdr -Method GET
Write-Host "Assets count: $($assets.Count)"

$t1 = 'TST' + [Guid]::NewGuid().ToString('N').Substring(0, 6).ToUpper()
$buy1 = @{ ticker = $t1; name = 'Test'; type = 'Stocks'; quantity = 10; price = 20 } | ConvertTo-Json -Compress
Write-Host "=== 3) POST buy NOVO ticker $t1 ==="
Invoke-RestMethod -Uri 'http://localhost:8080/api/wallet/assets/buy' -Method POST -Headers $hdr -ContentType 'application/json' -Body $buy1

$buy2 = @{ ticker = $t1; name = 'Test'; type = 'Stocks'; quantity = 5; price = 25 } | ConvertTo-Json -Compress
Write-Host '=== 4) POST buy MESMO ticker (segunda compra) ==='
Invoke-RestMethod -Uri 'http://localhost:8080/api/wallet/assets/buy' -Method POST -Headers $hdr -ContentType 'application/json' -Body $buy2
Write-Host 'Segunda compra OK'

Write-Host '=== 5) POST buy PETR4 (duas vezes) ==='
$p = 'PETR4'
Invoke-RestMethod -Uri 'http://localhost:8080/api/wallet/assets/buy' -Method POST -Headers $hdr -ContentType 'application/json' -Body (@{ ticker = $p; name = 'Petrobras'; type = 'Stocks'; quantity = 10; price = 20 } | ConvertTo-Json -Compress)
Invoke-RestMethod -Uri 'http://localhost:8080/api/wallet/assets/buy' -Method POST -Headers $hdr -ContentType 'application/json' -Body (@{ ticker = $p; name = 'Petrobras'; type = 'Stocks'; quantity = 2; price = 30 } | ConvertTo-Json -Compress)
Write-Host 'PETR4 duas compras OK'

Write-Host '=== TODOS OS TESTES PASSARAM ==='
