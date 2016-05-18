@echo off
set "file=led.log"
del %file% /q
led.exe "email@domain" "password" "pattern"
call :CheckEmpty "%file%"
goto :eof
:CheckEmpty
if %~z1 == 0 (blink1-tool.exe --off) else (blink1-tool.exe --red)