@echo off
cd /d "%~dp0"
node play-game.cjs
if errorlevel 1 pause
