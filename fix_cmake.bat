@echo off
echo Installing CMake 3.22.1 for Unity Android SDK...
cd /d "C:\Program Files\Unity\Hub\Editor\6000.0.70f1\Editor\Data\PlaybackEngines\AndroidPlayer\SDK\cmdline-tools\16.0\bin"
call sdkmanager.bat --install "cmake;3.22.1"
echo Done!
pause
