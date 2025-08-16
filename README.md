# IoT Registrium

## Login credentials

### Account 1
Username: admin

Password: admin
### Account 2
Username: user

Password: password

## Instructions for running the project

### Prerequisites
- Docker Desktop installed
- Windows Subsystem for Linux (WSL) is set up

  (Tested with Ubuntu-22.04)

### Launching
In the project root, start the system with on first launch:
```
docker compose up --build
```
Subsequent launches can be done with:
```
docker compose up
```
After all containers are launched successfully, the application can be accessed through: http://localhost:4200

To shutdown the application, run the following command:
```
docker compose down
```

### In case virtualization is not enabled
- To install WSL run the following command as administrator ```wsl --install``` or get the WSL directly from the repo https://github.com/microsoft/wsl/releases
- Run the following commands and restart your system if needed:
  - ```dism.exe /online /enable-feature /featurename:Microsoft-Windows-Subsystem-Linux /all /norestart```
  - ```dism.exe /online /enable-feature /featurename:VirtualMachinePlatform /all /norestart```
- Install the Ubuntu 22.04 image from Microsoft Store or get the image directly from the following link https://publicwsldistros.blob.core.windows.net/wsldistrostorage/Ubuntu2204LTS-230518_x64.appx
